using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Astek.Stats_System
{
    [Serializable]
    public class CharacterStat
    {
        public float BaseValue;

        public virtual float Value
        {
            get
            {
                if (_isDirty || !BaseValue.Approximately(lastBaseValue))
                {
                    lastBaseValue = BaseValue;
                    _value = CalculateFinalValue();
                    _isDirty = false;
                }
                return _value;
            }
        }

        private float _value;
        private bool _isDirty = true;
        private float lastBaseValue = float.MinValue;

        private readonly List<StatModifier> _statModifiers;
        public readonly ReadOnlyCollection<StatModifier> StatModifiers;

        private readonly PredicateClosure _predicateClosure;
        private class PredicateClosure
        {
            public readonly Func<StatModifier, int> KeySelector;
            public readonly Predicate<StatModifier> Predicate;
            public object? SourceToRemove;

            public PredicateClosure()
            {
                KeySelector = modifier => modifier.Order;
                Predicate = modifier => modifier.Source == SourceToRemove;
            }
        }

        public CharacterStat()
        {
            _statModifiers = new List<StatModifier>();
            StatModifiers = _statModifiers.AsReadOnly();
            _predicateClosure = new PredicateClosure();
        }
        public CharacterStat(float baseValue) : this()
        {
            BaseValue = baseValue;
        }

        public virtual void AddModifier(StatModifier modifier)
        {
            _isDirty = true;
            _statModifiers.Add(modifier);
            _statModifiers.OrderBy(_predicateClosure.KeySelector);
        }
        public virtual bool RemoveStatModifier(StatModifier modifier)
        {
            if (_statModifiers.Remove(modifier))
            {
                _isDirty = true;
                return true;
            }
            return false;
        }
        public virtual bool RemoveAllModifierFromSource(object source)
        {
            _predicateClosure.SourceToRemove = source;
            int removedCount = _statModifiers.RemoveAll(_predicateClosure.Predicate);
            _predicateClosure.SourceToRemove = null;

            if (removedCount > 0)
                _isDirty = true;

            return removedCount > 0;
        }

        protected virtual float CalculateFinalValue()
        {
            float finalValue = BaseValue;
            float sumPercentAdd = 0;

            int count = _statModifiers.Count;
            for (int i = 0; i < count; i++)
            {
                StatModifier mod = _statModifiers[i];

                if (mod.Type == StatModType.Flat)
                {
                    finalValue += mod.Value;
                }
                else if (mod.Type == StatModType.PercentAdd)
                {
                    sumPercentAdd += mod.Value;
                    if (i + 1 >= count || _statModifiers[i + 1].Type != StatModType.PercentAdd)
                    {
                        finalValue *= 1 + sumPercentAdd;
                        sumPercentAdd = 0;
                    }
                }
                else if (mod.Type == StatModType.PercentMult)
                {
                    //Reason for 1-> modifier value = 0.1 i.e 10% so 1+0.1=1.1 i.1 110% of base value which is the same as baseValue + 10%
                    finalValue *= 1 + mod.Value;
                }
            }

            //TODO:IF you need more precision modify here
            //12.000158f->Rounded to 12.0002f 
            return (float)Math.Round(finalValue, 4);
        }
    }
}