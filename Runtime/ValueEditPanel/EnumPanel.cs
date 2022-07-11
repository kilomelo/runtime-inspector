using System;
using System.Reflection;
using imugui.runtime;

namespace RuntimeInspector
{
    internal class EnumPanel<T> : BasePanel<T>
    {
        private Type _realEnumType;
        private bool _isFlags;
        public override void InitPanel(string goName, string compType, string memberName, object defaultValue, Action<object> valueChangeCallback,
            Action closeCallback)
        {
            _realEnumType = defaultValue.GetType();
            _isFlags = null != _realEnumType.GetCustomAttribute(typeof(FlagsAttribute));
            base.InitPanel(goName, compType, memberName, defaultValue, valueChangeCallback, closeCallback);
        }

        protected override void OnPanelImu()
        {
            foreach (var enumValue in Enum.GetValues(_realEnumType))
            {
                var numericValue = (T) Enum.Parse(_realEnumType, enumValue.ToString());
                var longEnum = (long)Convert.ChangeType(numericValue, typeof(long));
                var longValue = (long)Convert.ChangeType(Value.ToString(), typeof(long));
                var selected = _isFlags ? 0 != (longValue & longEnum) : longEnum == longValue;
                var color = selected ? "orange" : "white";
                if (_isFlags)
                {
                    Imu.Button($"<color={color}>{enumValue} ({numericValue})", () =>
                    {
                        // recalculate
                        longValue = (long)Convert.ChangeType(Value.ToString(), typeof(long));
                        var targetValue = selected ? longValue & ~longEnum : longValue | longEnum;
                        ValueChangeCallback((T)Convert.ChangeType(targetValue, typeof(T)));
                    });
                }
                else
                {
                    Imu.Button($"<color={color}>{enumValue} ({numericValue})", () => ValueChangeCallback((T)enumValue));
                }
            }
        }
    }
    
    [EditPanel(typeof(EnumPanel<byte>))]
    [ImuguiWindow(Imugui.EAnchorMode.Center, 0f, 0f, 512f, 100f, Imugui.ImuguiWindowDefaultScale,
        "ImuguiSkin/Inspector/EditPanel")]
    internal class ByteEnumPanel : EnumPanel<byte> {}
    
    [EditPanel(typeof(EnumPanel<sbyte>))]
    [ImuguiWindow(Imugui.EAnchorMode.Center, 0f, 0f, 512f, 100f, Imugui.ImuguiWindowDefaultScale,
        "ImuguiSkin/Inspector/EditPanel")]
    internal class SByteEnumPanel : EnumPanel<sbyte> {}
    
    [EditPanel(typeof(EnumPanel<ushort>))]
    [ImuguiWindow(Imugui.EAnchorMode.Center, 0f, 0f, 512f, 100f, Imugui.ImuguiWindowDefaultScale,
        "ImuguiSkin/Inspector/EditPanel")]
    internal class UShortEnumPanel : EnumPanel<ushort> {}
    
    [EditPanel(typeof(EnumPanel<short>))]
    [ImuguiWindow(Imugui.EAnchorMode.Center, 0f, 0f, 512f, 100f, Imugui.ImuguiWindowDefaultScale,
        "ImuguiSkin/Inspector/EditPanel")]
    internal class ShortEnumPanel : EnumPanel<short> {}
    
    [EditPanel(typeof(EnumPanel<uint>))]
    [ImuguiWindow(Imugui.EAnchorMode.Center, 0f, 0f, 512f, 100f, Imugui.ImuguiWindowDefaultScale,
        "ImuguiSkin/Inspector/EditPanel")]
    internal class UIntEnumPanel : EnumPanel<uint> {}
    
    [EditPanel(typeof(EnumPanel<int>))]
    [ImuguiWindow(Imugui.EAnchorMode.Center, 0f, 0f, 512f, 100f, Imugui.ImuguiWindowDefaultScale,
        "ImuguiSkin/Inspector/EditPanel")]
    internal class IntEnumPanel : EnumPanel<int> {}
}