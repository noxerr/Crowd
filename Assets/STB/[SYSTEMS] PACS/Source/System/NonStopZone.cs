using UnityEngine;
using System.Collections;
using System.Collections.Generic; 

namespace STB.PACS
{  
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Class: NonStopZone
    /// # If a individual touch this won't stop (at least it's on a restrictive state as dead, for example)
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class NonStopZone : STB.Generic.GenericNonStopZone
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Awake
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void Awake()
        {
            this.gameObject.AddComponent<GenericObject>();
        }
    }
}