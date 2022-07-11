using imugui.runtime;

namespace RuntimeInspector
{
    [EditPanel(typeof(int))]
    [ImuguiWindow(Imugui.EAnchorMode.Center, 0f, 0f, 512f, 100f, Imugui.ImuguiWindowDefaultScale,
        "ImuguiSkin/Inspector/EditPanel")]
    internal class IntPanel : BasePanel<int>
    {
        private NumericKeyboard<int> _numKeyboard;

        protected override void DataInitialized()
        {
            base.DataInitialized();
            _numKeyboard = new NumericKeyboard<int>
            {
                Value = Value,
                ValueChangeCallback = ValueChangeCallback
            };
            _numKeyboard.RefreshValueString();
        }

        protected override void BeforeSaving()
        {
            base.BeforeSaving();
            _numKeyboard.ChangeValueByInputString();
        }

        protected override void OnPanelImu()
        {
            Imu.Label($"<align=right>{_numKeyboard.RawString}");
            _numKeyboard.OnWidgetImu();
        }
    }
}