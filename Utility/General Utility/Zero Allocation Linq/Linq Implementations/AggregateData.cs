using System.Collections;
using System.Collections.Generic;
namespace AstekUtility.ZeroAllocLinqInternal
{
    public struct AggregateData<TSource>:IZeroAllocEnumerable<TSource>
    {

        ZeroAllocEnumerator<TSource> IZeroAllocEnumerable<TSource>.GetEnumerator() => throw new System.NotImplementedException();
        IEnumerator<TSource> IEnumerable<TSource>.GetEnumerator() => throw new System.NotImplementedException();
        IEnumerator IEnumerable.GetEnumerator() => throw new System.NotImplementedException();
    }
}