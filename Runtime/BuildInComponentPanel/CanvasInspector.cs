using UnityEngine;

namespace RuntimeInspector
{
    internal partial class RuntimeInspector
    {
        [ComponentInspector(typeof(Canvas))]
        public class CanvasInspector : ComponentInspector<Canvas>
        {
            public override void OnCompImu()
            {
                DrawMember("renderMode");
                DrawMember("pixelPerfect");
                DrawMember("sortingOrder");
                DrawMember("targetDisplay");
                DrawMember("additionalShaderChannels");
            }
        }
    }
}