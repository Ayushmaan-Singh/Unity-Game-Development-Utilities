#if UNITY_EDITOR
using System.Collections.Generic;
using AstekUtility;
using AstekUtility.ZeroAllocLinqInternal;
using UnityEngine;

namespace ZeroAllocLinqInternal
{
    using Sirenix.OdinInspector;
    public class ZeroAllocLinqExample : MonoBehaviour
    {
        #if ODIN_INSPECTOR
        [SerializeField] private bool _testMin = false;
        [SerializeField] private bool _testMax = false;
        #endif

        private readonly List<int> _first = new List<int>() { 1, 3, 2, 3, 8, 4, 9, 0, -1 };
        private readonly List<int> _second = new List<int>() { 2, 9, 0 };

        private readonly List<List<int>> _selectManyTestList = new List<List<int>>()
        {
            new List<int>() { 0 },
            new List<int>() { 1, 2, 3 },
            new List<int>() { 4, 5, 6 },
            new List<int>() { 7, 8, 9 }
        };

        private readonly Item[] _items = new Item[]
        {
            new Item { Name = "Sword", Category = "Weapon", Value = 100 },
            new Item { Name = "Shield", Category = "Armor", Value = 75 },
            new Item { Name = "Potion", Category = "Consumable", Value = 25 },
            new Item { Name = "Helmet", Category = "Armor", Value = 50 },
            new Item { Name = "Bow", Category = "Weapon", Value = 80 },
            new Item { Name = "Health Potion", Category = "Consumable", Value = 30 }
        };

        private void Awake()
        {
            Debug.Log("Where: ");
            TestWhere();
            Debug.Log("\nSelect: ");
            TestSelect();
            Debug.Log("\nSelectMany Simple: ");
            TestSelectMany_Simple();
            Debug.Log("\nSelectMany Collection selector and result selector: ");
            TestSelectMany_WithCollectionSelector();
            Debug.Log("\nReverse: ");
            TestReverse();
            Debug.Log("\nIntersect: ");
            TestIntersect();
            Debug.Log("\nExcept: ");
            TestExcept();
            Debug.Log("\nUnion: ");
            TestUnion();
        }

        private void TestWhere()
        {
            IEnumerable<int> result = _first.Where(x => x % 2 == 0);
            string resultStr = "";
            result.ForEach(x => resultStr += $"{x}, ");
            Debug.Log(resultStr);
        }
        private void TestSelect()
        {
            IEnumerable<string> result = _first.Select(x => x % 2 == 0 ? "Even" : "Odd");
            string resultStr = "";
            result.ForEach(x => resultStr += $"{x}, ");
            Debug.Log(resultStr);
        }
        private void TestSelectMany_Simple()
        {
            IEnumerable<int> result = _selectManyTestList.SelectMany(x => x);
            string resultStr = "";
            result.ForEach(x => resultStr += $"{x}, ");
            Debug.Log(resultStr);
        }
        private void TestSelectMany_WithCollectionSelector()
        {
            SelectManyEnumerable<int, int, int> result =
                _first.SelectMany(x => _second, (x, y) => x + y);
            string resultStr = "";
            result.ForEach(x => resultStr += $"{x}, ");
            Debug.Log(resultStr);
        }
        private void TestReverse()
        {
            IEnumerable<int> result = _first.AsZeroAlloc().Reverse();
            string resultStr = "";
            result.ForEach(x => resultStr += $"{x}, ");
            Debug.Log(resultStr);
        }
        private void TestIntersect()
        {
            IEnumerable<int> result = _first.Intersect(_second);
            string resultStr = "";
            result.ForEach(x => resultStr += $"{x}, ");
            Debug.Log(resultStr);
        }
        private void TestExcept()
        {
            IEnumerable<int> result = _first.Except(_second);
            string resultStr = "";
            result.ForEach(x => resultStr += $"{x}, ");
            Debug.Log(resultStr);
        }
        private void TestUnion()
        {
            IEnumerable<int> result = _first.Union(_second);
            string resultStr = "";
            result.ForEach(x => resultStr += $"{x}, ");
            Debug.Log(resultStr);
        }

        private void TestElementAt(int index) => Debug.Log($"{_first.ElementAt(index)}");

        #region Test Min Variation

        private void TestMin_int() => Debug.Log(_first.Min());
        private void TestMin_long() => Debug.Log(_first.Select(i => (long)i).Min());
        private void TestMin_float() => Debug.Log(_first.Select(i => (float)i).Min());
        private void TestMin_double() => Debug.Log(_first.Select(i => (double)i).Min());
        private void TestMin_Comparer() => Debug.Log(_items.Min((x, y) => x.Value < y.Value ? -1 : 0).Name);

        #endregion
        #region Test Max Variation

        private void TestMax_int() => Debug.Log(_first.Max());
        private void TestMax_long() => Debug.Log(_first.Select(i => (long)i).Max());
        private void TestMax_float() => Debug.Log(_first.Select(i => (float)i).Max());
        private void TestMax_double() => Debug.Log(_first.Select(i => (double)i).Max());
        private void TestMax_Comparer() => Debug.Log(_items.Max((x, y) => x.Value < y.Value ? -1 : 0).Name);

        #endregion

        private void TestGroupBy()
        {
            Debug.Log("\n1. Group items by category:");
            var groupsByCategory = _items.GroupBy(item => new { item.Category, item.Name });

            foreach (var group in groupsByCategory)
            {
                Debug.Log($"Category: {group.Key}");
                foreach (var item in group)
                {
                    Debug.Log($"  - {item.Name} (Value: {item.Value})");
                }
            }
        }

        private void TestOrderBy_Ascending()
        {
            IEnumerable<int> result = _first.OrderBy();
            string resultStr = "";
            result.ForEach(x => resultStr += $"{x}, ");
            Debug.Log(resultStr);
        }
        private void TestOrderBy_Descending()
        {
            IEnumerable<int> result = _first.OrderByDescending();
            string resultStr = "";
            result.ForEach(x => resultStr += $"{x}, ");
            Debug.Log(resultStr);
        }
        private void TestThenBy()
        {
            IOrderedEnumerable<Item> result = _items.OrderBy().ThenByMethod(i => i.Value);
            string resultStr = "";
            result.ForEach(x => resultStr += $"{x.Name}, ");
            Debug.Log(resultStr);
        }
        private void TestThenByDescending()
        {
            IOrderedEnumerable<Item> result = _items.OrderBy().ThenByDescendingMethod(i => i.Value);
            string resultStr = "";
            result.ForEach(x => resultStr += $"{x.Name}, ");
            Debug.Log(resultStr);
        }

        private void Aggregate()
        {
            Debug.Log(_first.Aggregate((x, y) => x + y));
        }
    }
    public struct Item
    {
        public string Name;
        public string Category;
        public int Value;
    }
}
#endif