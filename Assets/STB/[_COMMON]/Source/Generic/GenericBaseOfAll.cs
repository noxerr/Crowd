using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace STB.Generic
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Class: GenericBaseOfAll
    /// # 
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class GenericBaseOfAll : MonoBehaviour
    {
        // public hidden 
        [HideInInspector]
        public bool forceImpostorStoppedIfExists = false;
        [HideInInspector]
        public bool makeItRealAllowed = true;
        [HideInInspector]
        public bool outofPathVehicle = false;
    }
}
