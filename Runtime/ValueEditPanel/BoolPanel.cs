using imugui.runtime;

namespace RuntimeInspector
{
    [EditPanel(typeof(bool))]
    [ImuguiWindow(Imugui.EAnchorMode.Center, 0f, 0f, 512f, 100f, Imugui.ImuguiWindowDefaultScale,
        "ImuguiSkin/Inspector/EditPanel")]
    internal class BoolPanel : BasePanel<bool>
    {
        protected override void OnPanelImu()
        {
            Imu.Button(string.Format("{0}True", Value ? "<color=orange>" : ""), () => ValueChangeCallback(true));
            Imu.Button(string.Format("{0}False", !Value ? "<color=orange>" : ""), () => ValueChangeCallback(false));
        }
    }
}