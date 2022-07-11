using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using imugui.runtime;
using UnityEngine;

namespace RuntimeInspector
{
    
    public class ValueEditPanel : MonoBehaviour
    {
        private Dictionary<Type, Type> _customEditPanels = new Dictionary<Type, Type>();
        private ImuguiBehaviour _openingPanel;
        private void Awake()
        {
            var editPanels = GetType().Assembly.GetTypes().Where(type =>
                type.GetCustomAttribute(typeof(EditPanelAttribute), false) != null);
            foreach (var editPanel in editPanels)
            {
                var info = editPanel.GetCustomAttribute(typeof(EditPanelAttribute), false) as EditPanelAttribute;
                // Debug.Log($"attr: [{info.TargetType}]");
                _customEditPanels.Add(info.TargetType, editPanel);
            }
        }
        public void OpenPanel(string goName, string compType, string memberName, object initialValue, Action<object> valueChangeCallback)
        {
            if (null != _openingPanel) return;
            if (null == initialValue) return;
            var type = initialValue.GetType();
            _customEditPanels.TryGetValue(type, out var editPanelType);
            if (type.IsEnum)
            {
                editPanelType = _customEditPanels[typeof(EnumPanel<>).MakeGenericType(Enum.GetUnderlyingType(initialValue.GetType()))];
            }
            // Log.Debug($"ValueEditPanel.OpenPanel, type: [{type}], defaultValue: [{initialValue}]");
            if (null ==  editPanelType)
            {
                Debug.LogWarning($"No suitable edit panel for type: [{type}]");
                return;
            }
            var panel = gameObject.AddComponent(editPanelType);
            editPanelType.GetMethod("InitPanel").Invoke(panel, 
                new object[]{goName, compType, memberName, initialValue, valueChangeCallback, (Action)ClosePanel});
            _openingPanel = panel as ImuguiBehaviour;
        }

        private void ClosePanel()
        {
            _openingPanel = null;
        }
    }
}
