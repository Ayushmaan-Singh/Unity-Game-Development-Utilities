using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Astek.AdvancedStatSys.Core
{
    public readonly struct StatModifier<T> : IComparable<StatModifier<T>>, IEquatable<StatModifier<T>> where T : struct, IStatModifierData<T>
    {
        public readonly float Value;
        public readonly T Data;

        private readonly int hashCode;
        public StatModifier(float value, T data)
        {
            Value = value;
            Data = data;
            hashCode = HashCode.Combine(Value, Data);
        }
        
        //Equality
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int CompareTo(StatModifier<T> other)
        {
            int result = Value.CompareTo(other.Value);
            if (result == 0)
                result = Data.CompareTo(other.Data);
            return result;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(StatModifier<T> other) 
            => Mathf.Approximately(Value, other.Value) && Data.Equals(other.Data);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj) 
            => obj is StatModifier<T> other && Equals(other);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode() => hashCode;

        //Operator Overloading
        public static bool operator ==(StatModifier<T> a, StatModifier<T> b) => a.Equals(b);
        public static bool operator !=(StatModifier<T> a, StatModifier<T> b) => !(a == b);
    }
}