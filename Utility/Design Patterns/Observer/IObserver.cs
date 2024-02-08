using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AstekUtility.Observer
{
    public interface IObserver
    {
        void OnNotify(ISubject subject);
    }
}
