using System;
using System.Text;

namespace RuntimeInspector
{

    internal class NumericKeyboard<T> : EditPanelWidget<T> where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable
    {
        internal string RawString => _valueStr.ToString();

        private static bool FloatValue => typeof(T) == typeof(float) || typeof(T) == typeof(double) || typeof(T) == typeof(decimal);

        private StringBuilder _valueStr = new StringBuilder();
        internal override void OnWidgetImu()
        {
            Imu.BeginHorizontalLayout();
            {
                Imu.BeginVerticalLayout(0f);
                {
                    for (var i = 0; i < 3; i++)
                    {
                        Imu.BeginHorizontalLayout();
                        for (var j = 2; j >= 0; j--)
                        {
                            var num = (3 - i) * 3 - j;
                            Imu.Button(num.ToString(), () => { InputNumber(num); });
                        }

                        Imu.EndHorizontalLayout();
                    }

                    Imu.BeginHorizontalLayout(0f);
                    {
                        if (FloatValue) Imu.Button(".", InputDot);
                        Imu.Button("0", () => { InputNumber(0); });
                        Imu.Button("-", InputNegative);
                    }
                    Imu.EndHorizontalLayout();
                }
                Imu.EndVerticalLayout();
                Imu.Button("←", Backspace);
            }
            Imu.EndHorizontalLayout();
        }

        internal void RefreshValueString()
        {
            _valueStr.Clear();
            _valueStr.Append(Value.ToString());
        }

        internal void ChangeValueByInputString()
        {
            if (typeof(T) == typeof(float))
            {
                if (float.TryParse(_valueStr.ToString(), out var modifiedValue)) ValueChangeCallback((T)Convert.ChangeType(modifiedValue, typeof(T)));
            }
            else if (typeof(T) == typeof(double))
            {
                if (double.TryParse(_valueStr.ToString(), out var modifiedValue)) ValueChangeCallback((T)Convert.ChangeType(modifiedValue, typeof(T)));
            }
            else if (typeof(T) == typeof(decimal))
            {
                if (decimal.TryParse(_valueStr.ToString(), out var modifiedValue)) ValueChangeCallback((T)Convert.ChangeType(modifiedValue, typeof(T)));
            }
            else if (typeof(T) == typeof(sbyte))
            {
                if (sbyte.TryParse(_valueStr.ToString(), out var modifiedValue)) ValueChangeCallback((T)Convert.ChangeType(modifiedValue, typeof(T)));
            }
            else if (typeof(T) == typeof(short))
            {
                if (short.TryParse(_valueStr.ToString(), out var modifiedValue)) ValueChangeCallback((T)Convert.ChangeType(modifiedValue, typeof(T)));
            }
            else if (typeof(T) == typeof(int))
            {
                if (int.TryParse(_valueStr.ToString(), out var modifiedValue)) ValueChangeCallback((T)Convert.ChangeType(modifiedValue, typeof(T)));
            }
            else if (typeof(T) == typeof(long))
            {
                if (long.TryParse(_valueStr.ToString(), out var modifiedValue)) ValueChangeCallback((T)Convert.ChangeType(modifiedValue, typeof(T)));
            }
        }
        
        private void InputNumber(int num)
        {
            if (_valueStr.ToString() == "0") _valueStr.Clear();
            _valueStr.Append(num.ToString());
            ChangeValueByInputString();
        }

        private void InputNegative()
        {
            if (typeof(T) == typeof(float))
            {
                if (!float.TryParse(_valueStr.ToString(), out var modifiedValue)) return;
                var negative = (T) Convert.ChangeType(-1 * modifiedValue, typeof(T));
                ValueChangeCallback(negative);
                Value = negative;
            }
            else if (typeof(T) == typeof(double))
            {
                if (!double.TryParse(_valueStr.ToString(), out var modifiedValue)) return;
                var negative = (T) Convert.ChangeType(-1 * modifiedValue, typeof(T));
                ValueChangeCallback(negative);
                Value = negative;
            }
            else if (typeof(T) == typeof(decimal))
            {
                if (!decimal.TryParse(_valueStr.ToString(), out var modifiedValue)) return;
                var negative = (T) Convert.ChangeType(-1 * modifiedValue, typeof(T));
                ValueChangeCallback(negative);
                Value = negative;
            }
            else if (typeof(T) == typeof(sbyte))
            {
                if (!sbyte.TryParse(_valueStr.ToString(), out var modifiedValue)) return;
                var negative = (T) Convert.ChangeType(-1 * modifiedValue, typeof(T));
                ValueChangeCallback(negative);
                Value = negative;
            }
            else if (typeof(T) == typeof(short))
            {
                if (!short.TryParse(_valueStr.ToString(), out var modifiedValue)) return;
                var negative = (T) Convert.ChangeType(-1 * modifiedValue, typeof(T));
                ValueChangeCallback(negative);
                Value = negative;
            }
            else if (typeof(T) == typeof(int))
            {
                if (!int.TryParse(_valueStr.ToString(), out var modifiedValue)) return;
                var negative = (T) Convert.ChangeType(-1 * modifiedValue, typeof(T));
                ValueChangeCallback(negative);
                Value = negative;
            }
            else if (typeof(T) == typeof(long))
            {
                if (!long.TryParse(_valueStr.ToString(), out var modifiedValue)) return;
                var negative = (T) Convert.ChangeType(-1 * modifiedValue, typeof(T));
                ValueChangeCallback(negative);
                Value = negative;
            }
            RefreshValueString();
        }

        private void Backspace()
        {
            _valueStr.Remove(_valueStr.Length - 1, 1);
            if (_valueStr.ToString() == "-") _valueStr.Clear();
            if (_valueStr.Length == 0) _valueStr.Append(0);
            ChangeValueByInputString();
        }

        private void InputDot()
        {
            if (_valueStr.ToString().Contains('.')) return;
            _valueStr.Append('.');
        }
    }
}
