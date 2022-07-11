using UnityEngine.UI;

namespace RuntimeInspector
{
    internal partial class RuntimeInspector
    {
        [ComponentInspector(typeof(CanvasScaler))]
        public class CanvasScalerInspector : ComponentInspector<CanvasScaler>
        {
            public override void OnCompImu()
            {
                DrawMember("uiScaleMode");
                DrawMember("dynamicPixelsPerUnit");
                DrawMember("referencePixelsPerUnit");
            }
        }
    }
}