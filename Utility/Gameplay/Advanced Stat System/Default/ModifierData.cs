using System;
using System.Runtime.CompilerServices;
using Astek.AdvancedStatSys.Core;

namespace Astek.AdvancedStatSys.Default
{
    public readonly struct ModifierData : IStatModifierData<ModifierData>
    {
        public readonly StatModifierType Type;
        public readonly object? Source;

        private readonly int _hashCode;

        public ModifierData(StatModifierType type, object? source = default)
        {
            Type = type;
            Source = source;
            _hashCode = HashCode.Combine(Type, Source);
        }

        //Equality
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int CompareTo(ModifierData other)
        {
            // Cast to int to avoid using CompareTo(object) and causing boxing
            return ((int)Type).CompareTo((int)other.Type);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(ModifierData other) => Type == other.Type && Source == other.Source;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj) => obj is ModifierData other && Equals(other);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode() => _hashCode;

        //Operator Overloaded
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(ModifierData a, ModifierData b) => a.Equals(b);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(ModifierData a, ModifierData b) => !(a == b);
    }
}