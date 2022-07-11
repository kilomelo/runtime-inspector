
using System;

namespace RuntimeInspector
{
    
    [AttributeUsage(AttributeTargets.Class)]
    public class EditPanelAttribute : Attribute
    {
        public Type TargetType
        {
            get;
            private set;
        }
        public EditPanelAttribute(Type targetType)
        {
            TargetType = targetType;
        }
    }
}
