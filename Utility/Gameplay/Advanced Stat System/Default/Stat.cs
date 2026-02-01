using System;
using Astek.AdvancedStatSys.Core;

namespace Astek.AdvancedStatSys.Default
{
    public sealed class Stat : Stat<StatModifierData>
    {
        public Stat(float baseValue = 0) : base(baseValue, GetInnerStats()) { }

        public override void AddModifier(StatModifier<StatModifierData> modifier)
        {
            _stats[(int)modifier.Data.Type].AddModifier(modifier);
        }

        public override bool RemoveModifier(StatModifier<StatModifierData> modifier)
        {
            return _stats[(int)modifier.Data.Type].RemoveModifier(modifier);
        }

        public int RemoveModifiersFromSource(object source)
        {
            return RemoveAllModifiers(new StatModifierMatch(source: source));
        }

        protected override float CalculateFinalValue(float baseValue)
        {
            float finalValue = baseValue;
            finalValue += _stats[(int)StatModifierType.Add].FinalValue;
            finalValue *= _stats[(int)StatModifierType.Mult].FinalValue;
            finalValue *= _stats[(int)StatModifierType.MultTotal].FinalValue;
            finalValue = Math.Max(finalValue, _stats[(int)StatModifierType.Max].FinalValue);
            finalValue = Math.Min(finalValue, _stats[(int)StatModifierType.Min].FinalValue);
            return finalValue;
        }

        private static readonly StatModifierType[] modifierTypes = (StatModifierType[])Enum.GetValues(typeof(StatModifierType));

        private static IStat<StatModifierData>[] GetInnerStats()
        {
            IStat<StatModifierData>[] _stats = new IStat<StatModifierData>[modifierTypes.Length];

            for (int i = 0; i < _stats.Length; i++)
            {
                _stats[i] = modifierTypes[i] switch
                {
                    StatModifierType.Add       => new SimpleStatAdd<StatModifierData>(0),
                    StatModifierType.Mult      => new SimpleStatAdd<StatModifierData>(1),
                    StatModifierType.MultTotal => new SimpleStatMult<StatModifierData>(1),
                    StatModifierType.Max       => new SimpleStatMax<StatModifierData>(float.MinValue),
                    StatModifierType.Min       => new SimpleStatMin<StatModifierData>(float.MaxValue),
                    _                          => throw new NotImplementedException(),
                };
            }
            return _stats;
        }
    }
}