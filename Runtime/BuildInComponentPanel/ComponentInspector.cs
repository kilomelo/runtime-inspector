using System;
using System.Collections.Generic;
using imugui.runtime;
using UnityEngine;

namespace RuntimeInspector
{

    internal partial class RuntimeInspector
    {
        private Dictionary<Type, Type> _buildInComponentInspector = new Dictionary<Type, Type>();
        private Dictionary<Type, IComponentInspector> _drawingComponentInspectors =
            new Dictionary<Type, IComponentInspector>();

        private void ClearComponentInspectors()
        {
            foreach (var componentInspector in _drawingComponentInspectors)
            {
                componentInspector.Value.Dispose();
            }
            _drawingComponentInspectors.Clear();
        }
        private void DrawComponent(Component instance)
        {
            DrawTitle(instance);
            if (!_expandComponents.Contains(instance.GetInstanceID())) return;
            var type = instance.GetType();
            if (_drawingComponentInspectors.TryGetValue(type, out var inspector))
            {
                inspector.OnCompImu();
                if (inspector.DrawDefaultProperties) DrawProperties(type, instance);
                if (inspector.DrawDefaultFields) DrawFields(type, instance);
                if (inspector.DrawDefaultMethods) DrawMethods(type, instance);
                return;
            }
            if (_buildInComponentInspector.TryGetValue(type, out var componentInspectorType))
            {
                var inspectorInstance = Activator.CreateInstance(componentInspectorType) as IComponentInspector;
                if (null == inspectorInstance) throw new ImuguiException();
                inspectorInstance.SetTargetComponent(instance);
                _drawingComponentInspectors.Add(type, inspectorInstance);
                return;
            }
            DrawProperties(type, instance);
            DrawFields(type, instance);
            DrawMethods(type, instance);
        }
        public class ComponentInspector<T> : IComponentInspector where T : Component
        {
            protected ImuguiComponent Imu => ImuguiComponent.Instance;
            public virtual bool DrawDefaultFields => false;
            public virtual bool DrawDefaultProperties => false;
            public virtual bool DrawDefaultMethods => false;
           
            protected T _target;

            public void SetTargetComponent(Component instance)
            {
                _target = instance as T;
                if (null == _target) throw new ImuguiException();
            }
            public virtual void OnCompImu() {}

            public virtual void Dispose() {}

            protected void DrawMember(string memberName)
            {
                DrawValueMember(_target.gameObject.name, _target.GetType().GetProperty(memberName), _target);
            }
        }
        
        internal interface IComponentInspector : IDisposable
        {
            bool DrawDefaultFields
            {
                get;
            }
            bool DrawDefaultProperties
            {
                get;
            }
            bool DrawDefaultMethods
            {
                get;
            }

            void SetTargetComponent(Component instance);

            void OnCompImu();
        }
    }
}
