using System.Collections;
using UnityEngine;

namespace RuntimeInspector
{

    public class SelectedGameObjectIndicator : MonoBehaviour
    {
        [SerializeField] private float _blinkDuration = 1f;
        [SerializeField] private int _blinkLoopCnt = 3;
        private void Awake()
        {
            var hierarchy = FindObjectOfType<RuntimeHierarchy>();
            if (null == hierarchy)
            {
                Debug.LogError("Can't find RuntimeHierarchy.");
                Destroy(gameObject);
            }
            hierarchy.AddSelectedGameObjectChangeCallback(SelectedGameObjectChange);
        }
        
        private void SelectedGameObjectChange(GameObject selected)
        {
            if (null != selected.GetComponent<RectTransform>())
            {
                var canvasGroup = selected.GetComponent<CanvasGroup>();
                var isNew = false;
                if (null == canvasGroup)
                {
                    isNew = true;
                    canvasGroup = selected.AddComponent<CanvasGroup>();
                }
                var origAlpha = canvasGroup.alpha;
                StartCoroutine(Blink(canvasGroup, isNew, origAlpha));
            }
        }

        private IEnumerator Blink(CanvasGroup canvasGroup, bool isNew, float origAlpha)
        {
            for (var i = 0; i < _blinkLoopCnt * 2; i++)
            {
                canvasGroup.alpha = i % 2 == 0 ? 0f : origAlpha;
                yield return new WaitForSeconds(0.5f * _blinkDuration / _blinkLoopCnt);
            }

            if (isNew) Destroy(canvasGroup);
            else canvasGroup.alpha = origAlpha;
        }

    }
}