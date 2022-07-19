using System.Collections.Generic;
using System.Linq;
using imugui.runtime;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RuntimeInspector
{
    [ImuguiWindow(Imugui.EAnchorMode.Center, 0f, 0f, 720f, 56f)]
    internal class Selector : ImuguiBehaviour
    {
        private GameObject _selected;
        private List<RaycastResult> _uiRaycastResults = new List<RaycastResult>();
        
        private RuntimeHierarchy _hierarchy;

        public override void OnImu()
        {
            base.OnImu();
            Imu.Label($"Seleted: {_selected}");
        }

        protected override void Init()
        {
            base.Init();
            _hierarchy = FindObjectOfType<RuntimeHierarchy>();
            if (null == _hierarchy)
            {
                Debug.LogError("Can't find RuntimeHierarchy.");
                Destroy(gameObject);
            }
        }

        protected override void Update()
        {
            base.Update();
// #if UNITY_EDITOR
            // if (Input.GetKeyDown(KeyCode.Space))
            // {
            //     var ray = Camera.main.ScreenPointToRay (Input.mousePosition);
            //     if (Physics.Raycast (ray, out var hit, 100f))
            //     {
            //         // Log.Debug($"hit scene obj {hit.transform.name}");
            //         ChangeSelected(hit.transform.gameObject);
            //         return;
            //     }
            //
            //     RaycastWorldUI();
            //     if (_uiRaycastResults.Any()) ChangeSelected(_uiRaycastResults[0].gameObject);
            // }
// #endif
        }

        private void ChangeSelected(GameObject go)
        {
            if (null == go) return;
            _hierarchy.SetSelectedGameObject(go);
        }
        
        private void RaycastWorldUI()
        {
            var pointerData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };
            _uiRaycastResults.Clear();
            EventSystem.current.RaycastAll(pointerData, _uiRaycastResults);

            if (!_uiRaycastResults.Any()) return;
            Debug.Log($"Cnt: {_uiRaycastResults.Count}, Root Element: {_uiRaycastResults[^1].gameObject.name}, GrandChild Element: {_uiRaycastResults[0].gameObject.name}");
            // dont select imugui
            if (_uiRaycastResults[^1].gameObject.scene != Imu.ImuguiRootTrans.gameObject.scene) return;
            var trans = _uiRaycastResults[^1].gameObject.transform;
            while (null != trans.parent) trans = trans.parent;
            if (Imu.ImuguiRootTrans == trans) _uiRaycastResults.Clear();
        }
    }
}