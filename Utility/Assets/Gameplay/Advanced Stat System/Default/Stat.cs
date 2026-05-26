using System;
using Astek.AdvancedStatSys.Core;

namespace Astek.AdvancedStatSys.Default
{
    [Serializable]
    public sealed class Stat : Stat<ModifierData>
    {
        public event Action OnModifiersChanged = delegate { };

        public Stat(float baseValue = 0) : base(baseValue, GetInnerStats()) { }

        public override void AddModifier(StatModifier<ModifierData> modifier)
        {
            _stats[(int)modifier.Data.Type].AddModifier(modifier);
            OnModifiersChanged();
        }

        public override bool RemoveModifier(StatModifier<ModifierData> modifier)
        {
            OnModifiersChanged();
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
            finalValue =  Math.Max(finalValue, _stats[(int)StatModifierType.Max].FinalValue);
            finalValue =  Math.Min(finalValue, _stats[(int)StatModifierType.Min].FinalValue);
            return finalValue;
        }

        private static readonly StatModifierType[] modifierTypes = (StatModifierType[])Enum.GetValues(typeof(StatModifierType));

        private static IStat<ModifierData>[] GetInnerStats()
        {
            IStat<ModifierData>[] _stats = new IStat<ModifierData>[modifierTypes.Length];

            for (int i = 0; i < _stats.Length; i++)
            {
                _stats[i] = modifierTypes[i] switch
                {
                    StatModifierType.Add => new SimpleStatAdd<ModifierData>(0),
                    StatModifierType.Mult => new SimpleStatAdd<ModifierData>(1),
                    StatModifierType.MultTotal => new SimpleStatMult<ModifierData>(1),
                    StatModifierType.Max => new SimpleStatMax<ModifierData>(float.MinValue),
                    StatModifierType.Min => new SimpleStatMin<ModifierData>(float.MaxValue),
                    _ => throw new NotImplementedException(),
                };
            }

            return _stats;
        }
    }
}