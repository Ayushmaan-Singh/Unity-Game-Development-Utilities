#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using AstekUtility;
using AstekUtility.ZeroAllocLinqInternal;
using UnityEngine;

namespace ZeroAllocLinqInternal
{
    #if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
    public class ZeroAllocLinqExample : MonoBehaviour
    {
        private List<int> first = new List<int>() { 1, 2, 3, 3, 4, 8, 9, 0 };
        private List<int> second = new List<int>() { 2, 9, 0 };

        private List<List<int>> selectManyTestList = new List<List<int>>()
        {
            new List<int>() { 0 },
            new List<int>() { 1, 2, 3 },
            new List<int>() { 4, 5, 6 },
            new List<int>() { 7, 8, 9 }
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

        [Button]
        private void TestWhere()
        {
            IEnumerable<int> result = first.Where(x => x % 2 == 0);
            string resultStr = "";
            result.ForEach(x => resultStr += $"{x}, ");
            Debug.Log(resultStr);
        }

        [Button]
        private void TestSelect()
        {
            IEnumerable<string> result = first.Select(x => x % 2 == 0 ? "Even" : "Odd");
            string resultStr = "";
            result.ForEach(x => resultStr += $"{x}, ");
            Debug.Log(resultStr);
        }

        [Button]
        private void TestSelectMany_Simple()
        {
            IEnumerable<int> result = selectManyTestList.SelectMany(x => x);
            string resultStr = "";
            result.ForEach(x => resultStr += $"{x}, ");
            Debug.Log(resultStr);
        }

        [Button]
        private void TestSelectMany_WithCollectionSelector()
        {
            SelectManyEnumerable<int, int, int> result =
                first.SelectMany(x => second, (x, y) => x + y);
            string resultStr = "";
            result.ForEach(x => resultStr += $"{x}, ");
            Debug.Log(resultStr);
        }

        [Button]
        private void TestReverse()
        {
            IEnumerable<int> result = first.AsZeroAlloc().Reverse();
            string resultStr = "";
            result.ForEach(x => resultStr += $"{x}, ");
            Debug.Log(resultStr);
        }

        [Button]
        private void TestIntersect()
        {
            IEnumerable<int> result = first.Intersect(second);
            string resultStr = "";
            result.ForEach(x => resultStr += $"{x}, ");
            Debug.Log(resultStr);
        }

        [Button]
        private void TestExcept()
        {
            IEnumerable<int> result = first.Except(second);
            string resultStr = "";
            result.ForEach(x => resultStr += $"{x}, ");
            Debug.Log(resultStr);
        }

        [Button]
        private void TestUnion()
        {
            IEnumerable<int> result = first.Union(second);
            string resultStr = "";
            result.ForEach(x => resultStr += $"{x}, ");
            Debug.Log(resultStr);
        }

        [Button]
        private void TestElementAt(int index)
        {
            Debug.Log($"{first.ElementAt(index)}");
        }
    }
    #else
    public class ZeroAllocLinqExample : MonoBehaviour
    {
        private List<int> first = new List<int>() { 1, 2, 3, 3, 4, 8, 9, 0 };
        private List<int> second = new List<int>() { 2, 9, 0 };
        
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
            IEnumerable<int> result = first.Where(x => x % 2 == 0);
            string resultStr = "";
            result.ForEach(x => resultStr += $"{x}, ");
            Debug.Log(resultStr);
        }

        private void TestSelect()
        {
            IEnumerable<string> result = first.Select(x => x % 2 == 0 ? "Even" : "Odd");
            string resultStr = "";
            result.ForEach(x => resultStr += $"{x}, ");
            Debug.Log(resultStr);
        }

        private void TestSelectMany_Simple()
        {
            IEnumerable<int> result = selectManyTestList.SelectMany(x => x);
            string resultStr = "";
            result.ForEach(x => resultStr += $"{x}, ");
            Debug.Log(resultStr);
        }

        private void TestSelectMany_WithCollectionSelector()
        {
            SelectManyEnumerable<List<int>, int, int> result =
                selectManyTestList.SelectMany(x => x, (x, y) => x.FirstOrDefault(z => z % y == 0));
            string resultStr = "";
            result.ForEach(x => resultStr += $"{x}, ");
            Debug.Log(resultStr);
        }

        private void TestReverse()
        {
            IEnumerable<int> result = first.AsZeroAlloc().Reverse();
            string resultStr = "";
            result.ForEach(x => resultStr += $"{x}, ");
            Debug.Log(resultStr);
        }

        private void TestIntersect()
        {
            IEnumerable<int> result = first.Intersect(second);
            string resultStr = "";
            result.ForEach(x => resultStr += $"{x}, ");
            Debug.Log(resultStr);
        }
        
        private void TestExcept()
        {
            IEnumerable<int> result = first.Except(second);
            string resultStr = "";
            result.ForEach(x => resultStr += $"{x}, ");
            Debug.Log(resultStr);
        }
        
        private void TestUnion()
        {
            IEnumerable<int> result = first.Union(second);
            string resultStr = "";
            result.ForEach(x => resultStr += $"{x}, ");
            Debug.Log(resultStr);
        }
    }
    #endif
}
#endif