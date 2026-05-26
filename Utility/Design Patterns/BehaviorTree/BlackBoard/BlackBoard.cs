using System;
using UnityEngine;

namespace Astek.BehaviorTree
{
    [Serializable]
    public class BlackBoard
    {
        [SerializeField] private BlackBoardData data;

        public void Add(string key, object value)
        {
            ValueType valueType = AnyValue.ValueTypeOf(value.GetType());
            if (!Enum.IsDefined(typeof(ValueType), valueType))
                throw new Exception($"Value type {valueType} is not defined");

            if (data.TryAdd(key, default))
                data[key] = valueType switch
                {
                    ValueType.Int => new AnyValue { Type = ValueType.Int, IntValue = (int)value },
                    ValueType.Float => new AnyValue { Type = ValueType.Float, FloatValue = (float)value },
                    ValueType.Bool => new AnyValue { Type = ValueType.Bool, BoolValue = (bool)value },
                    ValueType.String => new AnyValue { Type = ValueType.String, StringValue = (string)value },
                    ValueType.Vector3 => new AnyValue { Type = ValueType.Vector3, Vector3Value = (Vector3)value },
                    ValueType.Quaternion => new AnyValue { Type = ValueType.Quaternion, QuaternionValue = (Quaternion)value },
                    _ => throw new ArgumentOutOfRangeException()
                };
            else
                SetValue(key, valueType switch
                {
                    ValueType.Int => (int)value,
                    ValueType.Float => (float)value,
                    ValueType.Bool => (bool)value,
                    ValueType.String => (string)value,
                    ValueType.Vector3 => (Vector3)value,
                    ValueType.Quaternion => (Quaternion)value,
                    _ => throw new ArgumentOutOfRangeException()
                });
        }

        public T GetValue<T>(string key)
        {
            if (!data.ContainsKey(key))
                return default;
            return data[key].ConvertValue<T>();
        }

        #region Set Value

        public void SetValue(string key, object value)
        {
            if (data.ContainsKey(key))
            {
                ValueType valueType = AnyValue.ValueTypeOf(value.GetType());
                if (!Enum.IsDefined(typeof(ValueType), valueType))
                    throw new Exception($"Value type {valueType} is not defined");

                data[key] = valueType switch
                {
                    ValueType.Int => new AnyValue { Type = ValueType.Int, IntValue = (int)value },
                    ValueType.Float => new AnyValue { Type = ValueType.Float, FloatValue = (float)value },
                    ValueType.Bool => new AnyValue { Type = ValueType.Bool, BoolValue = (bool)value },
                    ValueType.String => new AnyValue { Type = ValueType.String, StringValue = (string)value },
                    ValueType.Vector3 => new AnyValue { Type = ValueType.Vector3, Vector3Value = (Vector3)value },
                    ValueType.Quaternion => new AnyValue { Type = ValueType.Quaternion, QuaternionValue = (Quaternion)value },
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
        }

        private AnyValue SetValue(string key, int value) =>
            data.ContainsKey(key) ? data[key] = new AnyValue { Type = ValueType.Int, IntValue = value } : throw new Exception();

        private AnyValue SetValue(string key, float value) =>
            data.ContainsKey(key) ? data[key] = new AnyValue { Type = ValueType.Float, FloatValue = value } : throw new Exception();

        private AnyValue SetValue(string key, bool value) =>
            data.ContainsKey(key) ? data[key] = new AnyValue { Type = ValueType.Bool, BoolValue = value } : throw new Exception();

        private AnyValue SetValue(string key, string value) =>
            data.ContainsKey(key) ? data[key] = new AnyValue { Type = ValueType.String, StringValue = value } : throw new Exception();

        private AnyValue SetValue(string key, Vector3 value) =>
            data.ContainsKey(key) ? data[key] = new AnyValue { Type = ValueType.Vector3, Vector3Value = value } : throw new Exception();

        private AnyValue SetValue(string key, Quaternion value) =>
            data.ContainsKey(key) ? data[key] = new AnyValue { Type = ValueType.Quaternion, QuaternionValue = value } : throw new Exception();

        #endregion


        [Serializable]
        private class BlackBoardData : UnitySerializedDictionary<string, AnyValue> { }
    }
}