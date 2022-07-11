using UnityEngine;

namespace RuntimeInspector
{
    internal partial class RuntimeInspector
    {
        [ComponentInspector(typeof(Transform))]
        public class TransformInspector : ComponentInspector<Transform>
        {
            public override bool DrawDefaultMethods => true;

            public override void OnCompImu()
            {
                DrawMember("localPosition");
                DrawMember("localEulerAngles");
                DrawMember("localScale");
            }
        }
    }
}