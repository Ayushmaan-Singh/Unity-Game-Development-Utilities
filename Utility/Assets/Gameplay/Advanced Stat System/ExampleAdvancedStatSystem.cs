#if UNITY_EDITOR
using Astek.AdvancedStatSys.Core;
using Astek.AdvancedStatSys.Default;
using UnityEngine;

namespace Astek.AdvancedStatSys.EditorOnly
{
    public class ExampleAdvancedStatSystem : MonoBehaviour
    {
        private Stat Strength = new(10);

        private void Awake()
        {
            MaxStat(200);
            MinStat(20);
            AddStat(10);
            MultStat(10);
            MultTotalStat(10);
        }

        private void AddStat(float amount)
        {
            float before = Strength.FinalValue;
            StatModifier<ModifierData> add = new StatModifier<ModifierData>(amount, new ModifierData(StatModifierType.Add, this));
            Strength.AddModifier(add);
            Debug.Log($"After adding {amount} from {before} to Strength: {Strength.FinalValue}");
        }
        private void MultStat(float amount)
        {
            StatModifier<ModifierData> mult = new StatModifier<ModifierData>(amount, new ModifierData(StatModifierType.Mult, this));
            Strength.AddModifier(mult);
            Debug.Log($"After multiplying {amount} to Strength: {Strength.FinalValue}");
        }
        private void MultTotalStat(float amount)
        {
            StatModifier<ModifierData> multTotal = new StatModifier<ModifierData>(amount, new ModifierData(StatModifierType.MultTotal, this));
            Strength.AddModifier(multTotal);
            Debug.Log($"After stacking {amount} to Strength: {Strength.FinalValue}");
        }
        private void MaxStat(float amount)
        {
            StatModifier<ModifierData> Max = new StatModifier<ModifierData>(amount, new ModifierData(StatModifierType.Max, this));
            Strength.AddModifier(Max);
            Debug.Log($"Max set to {amount} of Strength: {Strength.FinalValue}");
        }
        private void MinStat(float amount)
        {
            StatModifier<ModifierData> Min = new StatModifier<ModifierData>(amount, new ModifierData(StatModifierType.Min, this));
            Strength.AddModifier(Min);
            Debug.Log($"Min set to {amount} of Strength: {Strength.FinalValue}");
        }
        private void Reset() => Strength.RemoveModifiersFromSource(this);

    }
}

#endif