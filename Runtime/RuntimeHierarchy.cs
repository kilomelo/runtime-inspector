using System.Collections.Generic;
using imugui.runtime;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using GUILayout = imugui.runtime.GUILayout;

namespace RuntimeInspector
{
    [ImuguiWindow(Imugui.EAnchorMode.Center, 0f, 0f, 720f, 1024f)]
    public class RuntimeHierarchy : ImuguiBehaviour
    {
        private const float ArrowWidth = 36f;
        private const string SceneToggleBtnStyle = "ImuguiSkin/Hierarchy/SceneToggleBtn";
        private const string GameObjectBtnStyle = "ImuguiSkin/Hierarchy/GameObjectBtn";
        private const string ExpandToggleStyle = "ImuguiSkin/Hierarchy/ExpandToggle";

        private Scene _dontDestroyOnLoadScene;
        private int _currentSelectedGameObjectInstanceId;
        private Dictionary<int, int> _expandGameObjects = new Dictionary<int, int>();
        private Imugui.ImuguiSkin _hierarchySkin;

        private UnityAction<GameObject> _selectedGameObjectChangeCallback;
        private GameObject _selectedGameObject;

        public void SetSelectedGameObject(GameObject selectedGameObject)
        {
            _selectedGameObject = selectedGameObject;
            var scene = _selectedGameObject.scene;
            SetExpand(scene, true);
            var trans = _selectedGameObject.transform.parent;
            while (null != trans)
            {
                SetExpand(trans.gameObject, true);
                trans = trans.parent;
            }
            _selectedGameObjectChangeCallback?.Invoke(_selectedGameObject);
        }
        public void AddSelectedGameObjectChangeCallback(UnityAction<GameObject> callback)
        {
            _selectedGameObjectChangeCallback += callback ?? throw new ImuguiException();
        }
        public void RemoveSelectedGameObjectChangeCallback(UnityAction<GameObject> callback)
        {
            _selectedGameObjectChangeCallback -= callback ?? throw new ImuguiException();
        }
        protected override void Init()
        {
            base.Init();
            _hierarchySkin = Imu.Skin;
            _hierarchySkin.Button = GameObjectBtnStyle;
            _hierarchySkin.Toggle = ExpandToggleStyle;
            _dontDestroyOnLoadScene = GetDontDestroyOnLoadScene();
        }

        public override void OnImu()
        {
            Imu.PushSkin(_hierarchySkin);
            var loadedSceneNum = SceneManager.sceneCount;
            Imu.BeginScrollView(Imu.WindowHeight - GUILayout.DefaultPadding * 2);
            for (var i = 0; i < loadedSceneNum; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                var sceneHashCode = scene.GetHashCode();
                Imu.Toggle(SceneToggleContent(scene), _expandGameObjects.ContainsKey(sceneHashCode),
                    value => { SetExpand(scene, value); }, SceneToggleBtnStyle);
                if (!_expandGameObjects.ContainsKey(sceneHashCode)) continue;
                foreach (var rootGameObject in scene.GetRootGameObjects())
                {
                    DrawGameObjectWithIndentation(rootGameObject, 1);
                }
            }
            // draw dontDestroyOnLoad objects
            var dontDestroyOnLoadSceneHashCode = _dontDestroyOnLoadScene.GetHashCode();
            Imu.Toggle("[DontDestroyOnLoad]", _expandGameObjects.ContainsKey(dontDestroyOnLoadSceneHashCode),
                value => { SetExpand(dontDestroyOnLoadSceneHashCode, value); }, SceneToggleBtnStyle);
            if (_expandGameObjects.ContainsKey(dontDestroyOnLoadSceneHashCode))
            {
                foreach (var rootGameObject in _dontDestroyOnLoadScene.GetRootGameObjects())
                {
                    DrawGameObjectWithIndentation(rootGameObject, 1);
                }
            }
            Imu.EndScrollView();
            Imu.PopSkin();
        }

        private void DrawGameObjectWithIndentation(GameObject go, int indentation)
        {
            // ignore self
            if (go.transform == Imu.ImuguiRootTrans) return;
            Imu.BeginHorizontalLayout();
            if (go.transform.childCount > 0)
            {
                Imu.Space(GUILayout.DefaultIndentationWidth * (indentation - 1));
                Imu.Toggle("", IsItemExpand(go), value =>
                {
                    SetExpand(go, value);
                }, GUILayout.Width(ArrowWidth));
            }
            else
            {
                Imu.Space(GUILayout.DefaultIndentationWidth * indentation);
            }
            Imu.Button(GameObjectBtnContent(go), () =>
                {
                    // set current select GameObject
                    _selectedGameObject = go;
                    _selectedGameObjectChangeCallback?.Invoke(go);
                }, GUILayout.Width(Imu.WindowWidth - GUILayout.DefaultIndentationWidth * indentation));
            Imu.EndHorizontalLayout();
            if (!IsItemExpand(go)) return;
            foreach (Transform child in go.transform)
            {
                DrawGameObjectWithIndentation(child.gameObject, indentation + 1);
            }

            bool IsItemExpand(GameObject thatGameObject)
            {
                return _expandGameObjects.ContainsKey(thatGameObject.GetInstanceID());
            }
        }
        private void SetExpand(GameObject go, bool expand)
        {
            if (null == go) throw new ImuguiException();
            SetExpand(go.GetInstanceID(), expand);
        }
            
        private void SetExpand(Scene scene, bool expand)
        {
            SetExpand(scene.GetHashCode(), expand);
        }
            
        private void SetExpand(int uid, bool expand)
        {
            switch (expand)
            {
                case true when !_expandGameObjects.ContainsKey(uid):
                    _expandGameObjects.Add(uid, uid);
                    break;
                case false when _expandGameObjects.ContainsKey(uid):
                    _expandGameObjects.Remove(uid);
                    break;
            }
        }

        private string GameObjectBtnContent(GameObject go)
        {
            var alpha = go.activeSelf ? "ff" : "44";
            var color = _selectedGameObject == go ? "ff8800" : "ffffff";
            return $"<color=#{color}{alpha}>{go.name}";
        }
        private string SceneToggleContent(Scene scene)
        {
            return scene.rootCount != 0 ? $"[{scene.name}]" : $"[{scene.name}] <color=#888888>EMPTY";
        }
        
        private static Scene GetDontDestroyOnLoadScene()
        {
            GameObject temp = null;
            try
            {
                temp = new GameObject();
                DontDestroyOnLoad( temp );
                Scene dontDestroyOnLoad = temp.scene;
                DestroyImmediate( temp );
                temp = null;
 
                return dontDestroyOnLoad;
            }
            finally
            {
                if( temp != null ) DestroyImmediate( temp );
            }
        }
    }
}