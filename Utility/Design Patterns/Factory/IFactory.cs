using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AstekUtility
{
    public interface IFactory<T>
    {
        T CreateInstance(T type);
    }
}
