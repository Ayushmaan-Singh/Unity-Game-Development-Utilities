using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
namespace Astek.AdvancedStatSys.Core
{
    public abstract class Stat<T> : IStat<T> where T : struct, IStatModifierData<T>
    {
        protected readonly IStat<T>[] _stats;

        private bool _isDirty;
        private float _baseValue;
        private float _currentValue;

        public event Action? OnValueChanged;

        public float BaseValue { get => _baseValue; set => SetBaseValue(value); }
        public float FinalValue => GetFinalValue();
        public int ModifiersCount => GetModifiersCount();

        public IReadOnlyList<IStat<T>> Stats => _stats;

        IReadOnlyList<IStat> IStat.Stats => _stats;
        IReadOnlyList<IReadOnlyStat<T>> IReadOnlyStat<T>.Stats => _stats;
        IReadOnlyList<IReadOnlyStat> IReadOnlyStat.Stats => _stats;

        protected Stat(float baseValue = 0, params IStat<T>[] stats)
        {
            _stats = stats;
            _baseValue = baseValue;
            _currentValue = CalculateFinalValue(baseValue);

            Action onChangedDelegate = OnChanged;

            for (int i = 0; i < stats.Length; i++)
            {
                stats[i].OnValueChanged += onChangedDelegate;
            }
        }

        public abstract void AddModifier(StatModifier<T> modifier);
        public abstract bool RemoveModifier(StatModifier<T> modifier);
        protected abstract float CalculateFinalValue(float baseValue);

        public int RemoveAllModifiers<TMatch>(TMatch match) where TMatch : IEquatable<StatModifier<T>>
        {
            int removedCount = 0;
            for (int i = 0; i < _stats.Length; i++)
            {
                removedCount += _stats[i].RemoveAllModifiers(match);
            }
            return removedCount;
        }

        public void Clear()
        {
            for (int i = 0; i < _stats.Length; i++)
            {
                _stats[i].Clear();
            }
        }

        public void GetModifiers(IList<StatModifier<T>> results)
        {
            for (int i = 0; i < _stats.Length; i++)
            {
                _stats[i].GetModifiers(results);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetBaseValue(float value)
        {
            if (!_baseValue.Approximately(value))
            {
                _isDirty = true;
                _baseValue = value;
                OnValueChanged?.Invoke();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private float GetFinalValue()
        {
            if (_isDirty)
            {
                _isDirty = false;
                _currentValue = CalculateFinalValue(_baseValue);
            }
            return _currentValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetModifiersCount()
        {
            int count = 0;
            for (int i = 0; i < _stats.Length; i++)
            {
                count += _stats[i].ModifiersCount;
            }
            return count;
        }

        private void OnChanged()
        {
            _isDirty = true;
            OnValueChanged?.Invoke();
        }
    }
}