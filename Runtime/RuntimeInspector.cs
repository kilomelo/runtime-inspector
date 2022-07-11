using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using imugui.runtime;
using UnityEngine;
using GUILayout = imugui.runtime.GUILayout;

namespace RuntimeInspector
{
    [ImuguiWindow(Imugui.EAnchorMode.Center, 0f, 0f, 720f, 1024f)]
    internal partial class RuntimeInspector : ImuguiBehaviour
    {
        private const string ValueBtnStyle = "ImuguiSkin/Inspector/ValueButton";
        private const string ComponentTitleBtnStyle = "ImuguiSkin/Inspector/ComponentTitleBtn";

        private static readonly string[] AutoExpandComponents = new[]
        {
            "Transform",
            "RectTransform"
        };
        
        private static readonly string[] IgnoredProperties = new[]
        {
            "useGUILayout",
            "runInEditMode",
            "enabled",
            "tag",
            "name",
            "hideFlags",
        };

        private static ValueEditPanel _valueEditPanel;
        
        private GameObject _selectedGameObject;
        private List<int> _expandComponents = new List<int>();
        private const int ExpandRecordLimit = 256;
        protected override void Init()
        {
            base.Init();
            var hierarchy = FindObjectOfType<RuntimeHierarchy>();
            if (null == hierarchy)
            {
                Debug.LogError("Can't find RuntimeHierarchy.");
                Destroy(gameObject);
            }
            hierarchy.AddSelectedGameObjectChangeCallback(SelectedGameObjectChange);
            _valueEditPanel = FindObjectOfType<ValueEditPanel>();
            if (null == _valueEditPanel)
            {
                Debug.LogError("Can't find ValueEditPanel.");
            }
            
            var buildInComponentInspectors = GetType().Assembly.GetTypes().Where(type =>
                type.GetCustomAttribute(typeof(ComponentInspectorAttribute), false) != null);
            foreach (var buildInComponentInspector in buildInComponentInspectors)
            {
                var info = buildInComponentInspector.GetCustomAttribute(typeof(ComponentInspectorAttribute), false) as ComponentInspectorAttribute;
                // Debug.Log($"buildInComponentInspector: [{info.TargetType}]");
                _buildInComponentInspector.Add(info.TargetType, buildInComponentInspector);
            }
        }

        public override void OnImu()
        {
            if (_selectedGameObject)
            {
                // gameObject title
                Imu.BeginHorizontalLayout();
                Imu.Toggle(
                    _selectedGameObject.activeSelf ? _selectedGameObject.name : $"<color=grey>{_selectedGameObject.name}",
                    _selectedGameObject.activeSelf, active =>
                {
                    _selectedGameObject.SetActive(active);
                }, GUILayout.Width(Imu.WindowWidth - GUILayout.DefaultIndentationWidth) );
                Imu.EndHorizontalLayout();
                
                Imu.BeginScrollView(Imu.WindowHeight - GUILayout.DefaultPadding * 2 - GUILayout.DefaultLineHeight - GUILayout.DefaultSpacing);
                foreach (var component in _selectedGameObject.GetComponents<Component>())
                {
                    Imu.Label("-------------------------------------------------------------");
                    if (component is MonoBehaviour behaviour) DrawMonoBehaviour(behaviour);
                    else DrawComponent(component);
                }
                Imu.EndScrollView();
            }
            else
            {
                Imu.Label("<color=grey><align=center><size=40>No GameObject Selected.", GUILayout.Height(100f));
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            var hierarchy = FindObjectOfType<RuntimeHierarchy>();
            if (null == hierarchy) return;
            hierarchy.RemoveSelectedGameObjectChangeCallback(SelectedGameObjectChange);
        }

        private void SelectedGameObjectChange(GameObject selected)
        {
            _selectedGameObject = selected;
            ClearComponentInspectors();
            foreach (var component in _selectedGameObject.GetComponents<Component>())
            {
                if (AutoExpandComponents.Contains(component.GetType().ToString().Split(".")[^1]))
                {
                    _expandComponents.Add(component.GetInstanceID());
                }
            }
        }

        private void DrawMonoBehaviour(MonoBehaviour instance)
        {
            DrawTitle(instance);
            if (!_expandComponents.Contains(instance.GetInstanceID())) return;
            var type = instance.GetType();
            DrawProperties(type, instance);
            DrawFields(type, instance);
            DrawMethods(type, instance);
        }

        private void DrawProperties(Type type, object instance)
        {
            Imu.Label("<color=orange>Properties:");
            var defaultFlags = BindingFlags.Public | BindingFlags.Instance;
            var drawAny = false;
            foreach (var property in type.GetProperties(defaultFlags).Where(
                         info => (info.SetMethod?.IsPublic ?? false) &&
                                 !IgnoredProperties.Contains(info.Name) &&
                                 info.CustomAttributes.All(attr => attr.AttributeType != typeof(ObsoleteAttribute))))
            {
                drawAny = true;
                Imu.VerticalSpace(0.25f * GUILayout.DefaultLineHeight);
                DrawValueMember(_selectedGameObject.name, property, instance);
            }
            if (!drawAny) Imu.Label("<color=grey>None");
        }

        private void DrawFields(Type type, object instance)
        {
            Imu.Label("<color=orange>Fields:");
            var drawAny = false;
            foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(
                         info => info.IsPublic || info.CustomAttributes.Any(attribute => attribute.AttributeType == typeof(SerializeField))))
            {
                drawAny = true;
                Imu.VerticalSpace(0.25f * GUILayout.DefaultLineHeight);
                DrawValueMember(_selectedGameObject.name, field, instance);
            }
            if (!drawAny) Imu.Label("<color=grey>None");
        }
        
        private void DrawMethods(Type type, object instance)
        {
            Imu.Label("<color=orange>Methods:");
            var drawAny = false;
            foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance).Where(
                         info => !info.IsSpecialName &&
                                 info.DeclaringType == type && !info.IsAbstract && !info.IsVirtual &&
                                 info.ReturnType == typeof(void) && !info.GetParameters().Any()))
            {
                drawAny = true;
                Imu.Button($"{method.Name}", () => { method.Invoke(instance, null); });
            }
            if (!drawAny) Imu.Label("<color=grey>None");
        }
        
        private void DrawTitle(Component instance)
        {
            var type = instance.GetType();
            
            var enabledProperty = type.GetProperty("enabled", BindingFlags.Public | BindingFlags.Instance);
            var typeName = type.ToString();
            var typeParts = typeName.Split(".");
            
            var componentInstanceId = instance.GetInstanceID();
            var expand = _expandComponents.Contains(componentInstanceId);
            var wrapLeftIcon = expand ? "<" : "--";
            var wrapRightIcon = expand ? ">" : "--";
            var title = $"<align=center> {wrapLeftIcon} {typeParts[^1]} {wrapRightIcon} [{componentInstanceId}]";
            var enabled = true;
            Imu.BeginHorizontalLayout();
            if (null != enabledProperty)
            {
                enabled = (bool)enabledProperty.GetValue(instance);
                Imu.Toggle("", enabled, active =>
                {
                    enabledProperty.SetValue(instance, active);
                }, GUILayout.Width(GUILayout.DefaultIndentationWidth));
            }
            else
            {
                Imu.Space(GUILayout.DefaultIndentationWidth);
            }
            Imu.Button(enabled ? title : $"<color=grey>{title}", () =>
            {
                if (expand)
                {
                    _expandComponents.Remove(componentInstanceId);
                }
                else
                {
                    _expandComponents.Add(componentInstanceId);
                    while (_expandComponents.Count > ExpandRecordLimit) _expandComponents.RemoveAt(0);
                }
            }, ComponentTitleBtnStyle, GUILayout.Width(Imu.WindowWidth - GUILayout.DefaultIndentationWidth - GUILayout.DefaultPadding * 4));
            Imu.EndHorizontalLayout();
            if (!expand) Imu.Label("...");
        }

        private static void DrawValueMember(string gameObjectName, MemberInfo member, object instance)
        {
            var fieldInfo = member as FieldInfo;
            var propertyInfo = member as PropertyInfo;
            object value = null;
            try
            {
                value = fieldInfo?.GetValue(instance) ?? propertyInfo?.GetValue(instance);
            }
            catch
            {
                // do nothing
            }
            var width = Imu.WindowWidth - GUILayout.DefaultIndentationWidth * 2 - GUILayout.DefaultPadding * 4;
            Imu.Label(member.Name, GUILayout.Width(width));
            Imu.Button(value?.ToString() ?? "value invalid", () =>
            {
                // open editor panel
                if (null != _valueEditPanel)
                {
                    _valueEditPanel.OpenPanel(
                        gameObjectName,instance.GetType().ToString().Split(".")[^1], member.Name,
                        value, o =>
                    {
                        fieldInfo?.SetValue(instance, o);
                        propertyInfo?.SetValue(instance, o);
                    });
                }
            }, ValueBtnStyle, GUILayout.Width(width));
        }
    }
}