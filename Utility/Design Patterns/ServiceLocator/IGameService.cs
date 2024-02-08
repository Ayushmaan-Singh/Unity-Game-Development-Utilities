using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace AstekUtility.ServiceLocatorTool
{
    /// <summary>
    /// Base interface for our service locator to work with. Services implementing
    /// this interface will be retrievable using the locator.
    /// </summary> 
    public interface IGameService
    {
    }
}
