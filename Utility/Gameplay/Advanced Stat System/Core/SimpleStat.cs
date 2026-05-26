using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Astek.AdvancedStatSys.Core
{
    public abstract class SimpleStat<T> : IStat<T> where T : struct, IStatModifierData<T>
    {
        private readonly Dictionary<StatModifier<T>, int> _modifiers = new Dictionary<StatModifier<T>, int>();

        private bool _isDirty;
        private float _baseValue;
        private float _currentValue;
        private int _modifiersCount;

        public event Action? OnValueChanged;

        public float BaseValue { get => _baseValue; set => SetBaseValue(value); }
        public float FinalValue => GetFinalValue();
        public int ModifiersCount => _modifiersCount;

        IReadOnlyList<IStat<T>> IStat<T>.Stats => Array.Empty<IStat<T>>();
        IReadOnlyList<IStat> IStat.Stats => Array.Empty<IStat<T>>();
        IReadOnlyList<IReadOnlyStat<T>> IReadOnlyStat<T>.Stats => Array.Empty<IStat<T>>();
        IReadOnlyList<IReadOnlyStat> IReadOnlyStat.Stats => Array.Empty<IStat<T>>();

        protected SimpleStat(float baseValue = 0)
        {
            _baseValue = baseValue;
            _currentValue = baseValue;
        }

        public void AddModifier(StatModifier<T> modifier)
        {
            Add(modifier);
            bool isDirty = AddOperation(modifier, _baseValue, _currentValue, out float newValue);
            CheckValueChanged(isDirty, newValue);
        }

        public bool RemoveModifier(StatModifier<T> modifier)
        {
            if (Remove(modifier))
            {
                bool isDirty = RemoveOperation(modifier, _baseValue, _currentValue, out float newValue);
                CheckValueChanged(isDirty, newValue);
                return true;
            }
            return false;
        }

        public int RemoveAllModifiers<TEquatable>(TEquatable match) where TEquatable : IEquatable<StatModifier<T>>
        {
            int removedCount = 0;
            bool isDirty = false;
            float newValue = _currentValue;
            int keysCount = _modifiers.Count;

            StatModifier<T>[] keys = ArrayPool<StatModifier<T>>.Shared.Rent(keysCount);
            _modifiers.Keys.CopyTo(keys, 0);

            for (int i = 0; i < keysCount; i++)
            {
                StatModifier<T> modifier = keys[i];
                if (match.Equals(modifier) && _modifiers.Remove(modifier, out int count))
                {
                    for (int j = 0; j < count; j++)
                    {
                        isDirty |= RemoveOperation(modifier, _baseValue, newValue, out newValue);
                    }
                    removedCount += count;
                }
            }

            Array.Clear(keys, 0, keysCount); // Clear only the used portion of the array
            ArrayPool<StatModifier<T>>.Shared.Return(keys);

            _modifiersCount -= removedCount;
            CheckValueChanged(isDirty, newValue);
            return removedCount;
        }

        public void Clear()
        {
            _modifiers.Clear();
            _modifiersCount = 0;
            ClearCachedValues(_baseValue);
            CheckValueChanged(false, _baseValue);
        }

        public void GetModifiers(IList<StatModifier<T>> results)
        {
            foreach (KeyValuePair<StatModifier<T>, int> item in _modifiers)
            {
                for (int i = 0; i < item.Value; i++)
                    results.Add(item.Key);
            }
        }

        protected abstract bool AddOperation(StatModifier<T> modifier, float baseValue, float currentValue, out float newValue);
        protected abstract bool RemoveOperation(StatModifier<T> modifier, float baseValue, float currentValue, out float newValue);
        protected abstract bool SetBaseValue(float newBaseValue, float oldBaseValue, float currentValue, out float newValue);

        protected virtual void ClearCachedValues(float baseValue) { }

        private void Add(StatModifier<T> modifier)
        {
            _modifiers.TryGetValue(modifier, out int count);
            _modifiers[modifier] = count + 1;
            _modifiersCount++;
        }

        private bool Remove(StatModifier<T> modifier)
        {
            if (_modifiers.TryGetValue(modifier, out int count))
            {
                _modifiersCount--;

                if (count > 1)
                {
                    _modifiers[modifier] = count - 1;
                    return true;
                }
                return _modifiers.Remove(modifier);
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetBaseValue(float value)
        {
            if (!Mathf.Approximately(_baseValue, value))
            {
                bool isDirty = SetBaseValue(value, _baseValue, _currentValue, out float newValue);
                _baseValue = value;
                CheckValueChanged(isDirty, newValue);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private float GetFinalValue()
        {
            if (_isDirty)
            {
                _isDirty = false;
                _currentValue = _baseValue;
                ClearCachedValues(_baseValue);

                foreach (KeyValuePair<StatModifier<T>, int> item in _modifiers)
                {
                    for (int i = 0; i < item.Value; i++)
                    {
                        AddOperation(item.Key, _baseValue, _currentValue, out _currentValue);
                    }
                }
            }
            return _currentValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CheckValueChanged(bool newIsDirty, float newValue)
        {
            if (newIsDirty || !Mathf.Approximately(_currentValue, newValue))
            {
                _isDirty |= newIsDirty;
                _currentValue = newValue;
                OnValueChanged?.Invoke();
            }
        }
    }
}