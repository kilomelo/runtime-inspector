using System;

namespace RuntimeInspector
{
    
    [AttributeUsage(AttributeTargets.Class)]
    public class ComponentInspectorAttribute : Attribute
    {
        public Type TargetType
        {
            get;
            private set;
        }
        public ComponentInspectorAttribute(Type targetType)
        {
            TargetType = targetType;
        }
    }
}
