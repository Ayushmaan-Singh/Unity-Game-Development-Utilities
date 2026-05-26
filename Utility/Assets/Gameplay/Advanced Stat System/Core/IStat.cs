using System;
using System.Collections.Generic;
namespace Astek.AdvancedStatSys.Core
{
    public interface IStat : IReadOnlyStat
    {
        new IReadOnlyList<IStat> Stats { get; }

        new float BaseValue { get; set; }
        float FinalValue { get; }
        void Clear();
    }
    public interface IStat<T> : IStat, IReadOnlyStat<T> where T : struct, IStatModifierData<T>
    {
        new IReadOnlyList<IStat<T>> Stats { get; }

        void AddModifier(StatModifier<T> modifier);
        bool RemoveModifier(StatModifier<T> modifier);
        int RemoveAllModifiers<TEquatable>(TEquatable match) where TEquatable : IEquatable<StatModifier<T>>;
    }
}