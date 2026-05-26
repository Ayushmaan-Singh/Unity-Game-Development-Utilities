#if UNITY_EDITOR
using UnityEngine;

namespace Astek.Stats_System
{
    public class Character : MonoBehaviour
    {
        public CharacterStat Strength;
        private Item item = new Item();

        [AddComponentMenu("Equip")]
        public void TestItemEquip()
        {
            item.Equip(this);
            Debug.Log($"Equipped\nStrength:{Strength.Value}");
        }
        [AddComponentMenu("Unequip")]
        public void TestItemUnequip()
        {
            item.Unequip(this);
            Debug.Log($"Unequipped\nStrength:{Strength.Value}");
        }

    }

    public class Item
    {
        public void Equip(Character c)
        {
            c.Strength.AddModifier(new StatModifier(10, StatModType.Flat));
            c.Strength.AddModifier(new StatModifier(0.1f, StatModType.PercentMult));
        }
        public void Unequip(Character c)
        {
            c.Strength.RemoveAllModifierFromSource(this);
        }
    }
}

#endif