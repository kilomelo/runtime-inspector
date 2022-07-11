using imugui.runtime;

namespace RuntimeInspector
{
    [EditPanel(typeof(float))]
    [ImuguiWindow(Imugui.EAnchorMode.Center, 0f, 0f, 512f, 100f, Imugui.ImuguiWindowDefaultScale,
        "ImuguiSkin/Inspector/EditPanel")]
    internal class FloatPanel : BasePanel<float>
    {
        private NumericKeyboard<float> _numKeyboard;
        protected override void DataInitialized()
        {
            base.DataInitialized();
            _numKeyboard = new NumericKeyboard<float>
            {
                Value = Value,
                ValueChangeCallback = ValueChangeCallback
            };
            _numKeyboard.RefreshValueString();
        }

        

        protected override void OnPanelImu()
        {
            Imu.Label($"<align=right>{_numKeyboard.RawString}");
            _numKeyboard.OnWidgetImu();
        }
    }
}