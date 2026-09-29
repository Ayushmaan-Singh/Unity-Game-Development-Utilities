using System;
using UnityEngine;

namespace Astek.SerializableMethods
{
    public enum ValueType
    {
        Int,
        Float,
        Bool,
        String,
        Vector2,
        Vector2Int,
        Vector3,
        Vector3Int,
        Vector4,
        Color
    }

    [Serializable]
    public struct AnyValue
    {
        public ValueType Type;

        // Storage for different types of values
        public bool BoolValue;
        public int IntValue;
        public float FloatValue;
        public string StringValue;
        public Vector2 Vector2Value;
        public Vector2Int Vector2IntValue;
        public Vector3 Vector3Value;
        public Vector3Int Vector3IntValue;
        public Vector4 Vector4Value;
        public Color ColorValue;

        // Implicit conversion operators to convert AnyValue to different types
        public static implicit operator bool(AnyValue value) => value.ConvertValue<bool>();
        public static implicit operator int(AnyValue value) => value.ConvertValue<int>();
        public static implicit operator float(AnyValue value) => value.ConvertValue<float>();
        public static implicit operator string(AnyValue value) => value.ConvertValue<string>();
        public static implicit operator Vector2(AnyValue value) => value.ConvertValue<Vector2>();
        public static implicit operator Vector2Int(AnyValue value) => value.ConvertValue<Vector2Int>();
        public static implicit operator Vector3(AnyValue value) => value.ConvertValue<Vector3>();
        public static implicit operator Vector3Int(AnyValue value) => value.ConvertValue<Vector3Int>();
        public static implicit operator Vector4(AnyValue value) => value.ConvertValue<Vector4>();
        public static implicit operator Color(AnyValue value) => value.ConvertValue<Color>();
        public static implicit operator Color32(AnyValue value) => value.ConvertValue<Color32>();

        public T ConvertValue<T>()
        {
            if (typeof(T) == typeof(object)) return CastToObject<T>();
            return Type switch
            {
                ValueType.Int => AsInt<T>(IntValue),
                ValueType.Float => AsFloat<T>(FloatValue),
                ValueType.Bool => AsBool<T>(BoolValue),
                ValueType.String => (T)(object)StringValue,
                ValueType.Vector2 => AsVector2<T>(Vector2Value),
                ValueType.Vector2Int => AsVector2Int<T>(Vector2IntValue),
                ValueType.Vector3 => AsVector3<T>(Vector3Value),
                ValueType.Vector3Int => AsVector3Int<T>(Vector3IntValue),
                ValueType.Vector4 => AsVector4<T>(Vector4Value),
                ValueType.Color => AsColor<T>(ColorValue),
                _ => throw new InvalidCastException($"Cannot convert AnyValue of type {Type} to {typeof(T).Name}")
            };
        }

        // Helper methods for safe type conversions of the value types without the cost of boxing
        private T AsBool<T>(bool value) => typeof(T) == typeof(bool) && value is T correctType ? correctType : default;
        private T AsInt<T>(int value) => typeof(T) == typeof(int) && value is T correctType ? correctType : default;
        private T AsFloat<T>(float value) => typeof(T) == typeof(float) && value is T correctType ? correctType : default;
        private T AsVector2<T>(Vector2 value) => typeof(T) == typeof(Vector2) && value is T correctType ? correctType : default;
        private T AsVector2Int<T>(Vector2Int value) => typeof(T) == typeof(Vector2Int) && value is T correctType ? correctType : default;
        private T AsVector3<T>(Vector3 value) => typeof(T) == typeof(Vector3) && value is T correctType ? correctType : default;
        private T AsVector3Int<T>(Vector3Int value) => typeof(T) == typeof(Vector3Int) && value is T correctType ? correctType : default;
        private T AsVector4<T>(Vector4 value) => typeof(T) == typeof(Vector4) && value is T correctType ? correctType : default;
        private T AsColor<T>(Color value) => typeof(T) == typeof(Color) && value is T correctType ? correctType : default;

        public static Type TypeOf(ValueType valueType)
        {
            return valueType switch
            {
                ValueType.Bool => typeof(bool),
                ValueType.Int => typeof(int),
                ValueType.Float => typeof(float),
                ValueType.String => typeof(string),
                ValueType.Vector2 => typeof(Vector2),
                ValueType.Vector2Int => typeof(Vector2Int),
                ValueType.Vector3 => typeof(Vector3),
                ValueType.Vector3Int => typeof(Vector3Int),
                ValueType.Vector4 => typeof(Vector4),
                ValueType.Color => typeof(Color),
                _ => throw new NotSupportedException($"Unsupported ValueType: {valueType}")
            };
        }

        public static ValueType ValueTypeOf(Type type)
        {
            return type switch
            {
                _ when type == typeof(bool) => ValueType.Bool,
                _ when type == typeof(int) => ValueType.Int,
                _ when type == typeof(float) => ValueType.Float,
                _ when type == typeof(string) => ValueType.String,
                _ when type == typeof(Vector2) => ValueType.Vector2,
                _ when type == typeof(Vector2Int) => ValueType.Vector2Int,
                _ when type == typeof(Vector3) => ValueType.Vector3,
                _ when type == typeof(Vector3Int) => ValueType.Vector3Int,
                _ when type == typeof(Vector4) => ValueType.Vector4,
                _ when type == typeof(Color) => ValueType.Color,
                _ => throw new NotSupportedException($"Unsupported type: {type}")
            };
        }

        public T CastToObject<T>()
        {
            return Type switch
            {
                ValueType.Int => (T)(object)IntValue,
                ValueType.Float => (T)(object)FloatValue,
                ValueType.Bool => (T)(object)BoolValue,
                ValueType.String => (T)(object)StringValue,
                ValueType.Vector2 => (T)(object)Vector2Value,
                ValueType.Vector2Int => (T)(object)Vector2IntValue,
                ValueType.Vector3 => (T)(object)Vector3Value,
                ValueType.Vector3Int => (T)(object)Vector3IntValue,
                ValueType.Vector4 => (T)(object)Vector4Value,
                ValueType.Color => (T)(object)ColorValue,
                _ => throw new InvalidCastException($"Cannot convert AnyValue of type {Type} to {typeof(T).Name}")
            };
        }
    }
}