using imugui.runtime;
using UnityEngine;

namespace RuntimeInspector
{
    [EditPanel(typeof(Vector3Int))]
    [ImuguiWindow(Imugui.EAnchorMode.Center, 0f, 0f, 512f, 100f, Imugui.ImuguiWindowDefaultScale,
        "ImuguiSkin/Inspector/EditPanel")]
    internal class Vector3IntPanel : BasePanel<Vector3Int>
    {
        private NumericKeyboard<int> _keyboard;

        private readonly string[] _fieldNames = new[] {"x", "y", "z"};
        // x: 0; y: 1; z: 2
        private int _currentEditField = 0;
        protected override void DataInitialized()
        {
            base.DataInitialized();
            _currentEditField = 0;
            _keyboard = new NumericKeyboard<int>
            {
                Value = Value.x,
                ValueChangeCallback = value =>
                {
                    _keyboard.Value = value;
                    switch (_currentEditField)
                    {
                        case 0:
                            ValueChangeCallback(new Vector3Int(value, Value.y, Value.z));
                            break;
                        case 1:
                            ValueChangeCallback(new Vector3Int(Value.x, value, Value.z));
                            break;
                        case 2:
                            ValueChangeCallback(new Vector3Int(Value.x, Value.y, value));
                            break;
                    }
                },
            };
            _keyboard.RefreshValueString();
        }
        
        protected override void BeforeSaving()
        {
            base.BeforeSaving();
            _keyboard.ChangeValueByInputString();
        }
        protected override void OnPanelImu()
        {
            for (var i = 0; i < _fieldNames.Length; i++)
            {
                var color = _currentEditField == i ? "orange" : "white";
                Imu.BeginHorizontalLayout();
                {
                    var fieldValue = i == 0 ? Value.x : i == 1 ? Value.y : Value.z;
                    var displayString = _currentEditField == i ? _keyboard.RawString : fieldValue.ToString();
                    var idx = i;
                    Imu.Label($"<color={color}><align=left>{_fieldNames[i]}");
                    Imu.Button($"<color={color}><align=right>{displayString}", () =>
                    {
                        _currentEditField = idx;
                        _keyboard.Value = fieldValue;
                        _keyboard.RefreshValueString();
                    }, "ImuguiSkin/Hierarchy/GameObjectBtn");
                }
                Imu.EndHorizontalLayout();
            }
            _keyboard.OnWidgetImu();
        }
    }
}