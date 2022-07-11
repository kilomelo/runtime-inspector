using imugui.runtime;
using UnityEngine;

namespace RuntimeInspector
{
    [EditPanel(typeof(Vector4))]
    [ImuguiWindow(Imugui.EAnchorMode.Center, 0f, 0f, 512f, 100f, Imugui.ImuguiWindowDefaultScale,
        "ImuguiSkin/Inspector/EditPanel")]
    internal class Vector4Panel : BasePanel<Vector4>
    {
        private NumericKeyboard<float> _keyboard;

        private readonly string[] _fieldNames = new[] {"x", "y", "z", "w"};
        // x: 0; y: 1; z: 2; w: 3
        private int _currentEditField = 0;
        protected override void DataInitialized()
        {
            base.DataInitialized();
            _currentEditField = 0;
            _keyboard = new NumericKeyboard<float>
            {
                Value = Value.x,
                ValueChangeCallback = value =>
                {
                    _keyboard.Value = value;
                    switch (_currentEditField)
                    {
                        case 0:
                            ValueChangeCallback(new Vector4(value, Value.y, Value.z, Value.w));
                            break;
                        case 1:
                            ValueChangeCallback(new Vector4(Value.x, value, Value.z, Value.w));
                            break;
                        case 2:
                            ValueChangeCallback(new Vector4(Value.x, Value.y, value, Value.w));
                            break;
                        case 3:
                            ValueChangeCallback(new Vector4(Value.x, Value.y, Value.z, value));
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
                    var fieldValue = i switch
                    {
                        0 => Value.x,
                        1 => Value.y,
                        2 => Value.z,
                        _ => Value.w
                    };
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