using System;
using imugui.runtime;

namespace RuntimeInspector
{
    internal class BasePanel<T> : ImuguiBehaviour
    {
        private Action _closeCallback;
        private T _initialValue;
        protected T Value
        {
            get;
            private set;
        }

        protected Action<T> ValueChangeCallback
        {
            get;
            private set;
        }

        private bool Inited
        {
            get;
            set;
        }

        private string _goName;
        private string _compType;
        private string _memberName;

        protected virtual void OnPanelImu() {}
        protected virtual void DataInitialized() {}
        protected virtual void BeforeSaving() {}

        public virtual void InitPanel(string goName, string compType, string memberName,
            object defaultValue, Action<object> valueChangeCallback, Action closeCallback)
        {
            // Log.Debug($"{GetType()}.Init(defaultValue: {defaultValue}, valueChangeCallback: {valueChangeCallback})");
            _goName = goName;
            _compType = compType;
            _memberName = memberName;
            _initialValue = (T)defaultValue;
            Value = _initialValue;
            ValueChangeCallback = newValue =>
            {
                Value = newValue;
                valueChangeCallback(newValue);
            };
            _closeCallback = closeCallback;
            Inited = true;
            DataInitialized();
        }
        
        public override void OnImu()
        {
            if (!Inited) return;
            Imu.Label($"> {_goName}");
            Imu.Label($"  > {_compType}");
            Imu.Label($"    > {_memberName}");
            Imu.VerticalSpace(GUILayout.DefaultLineHeight * 0.5f);
            OnPanelImu();
            Imu.VerticalSpace(GUILayout.DefaultLineHeight * 0.5f);
            Imu.BeginHorizontalLayout();
            Imu.Button("Save", () =>
            {
                BeforeSaving();
                Destroy(this);
            });
            Imu.Button("Cancel", () =>
            {
                ValueChangeCallback(_initialValue);
                Destroy(this);
            });
            Imu.EndHorizontalLayout();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _closeCallback?.Invoke();
        }
    }
}