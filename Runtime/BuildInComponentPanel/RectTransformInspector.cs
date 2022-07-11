using UnityEngine;

namespace RuntimeInspector
{
    internal partial class RuntimeInspector
    {
        [ComponentInspector(typeof(RectTransform))]
        public class RectTransformInspector : ComponentInspector<RectTransform>
        {
            public override void OnCompImu()
            {
                DrawMember("localPosition");
                DrawMember("sizeDelta");
                DrawMember("localEulerAngles");
                DrawMember("localScale");
                Imu.VerticalSpace();
                Imu.Button("SetAsFirstSibling", _target.SetAsFirstSibling);
                Imu.Button("SetAsLastSibling", _target.SetAsLastSibling);
            }
        }
    }
}