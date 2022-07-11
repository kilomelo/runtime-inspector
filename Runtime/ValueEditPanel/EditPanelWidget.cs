using System;
using imugui.runtime;

namespace RuntimeInspector
{
    internal class EditPanelWidget<T>
    {
        protected ImuguiComponent Imu => ImuguiComponent.Instance;
        internal virtual void OnWidgetImu() {}

        internal T Value
        {
            get;
            set;
        }

        internal Action<T> ValueChangeCallback
        {
            get;
            set;
        }
    }
}