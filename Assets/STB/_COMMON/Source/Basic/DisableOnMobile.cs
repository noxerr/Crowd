using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace STB.Basics
{ 
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Class: DisableOnMobile
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class DisableOnMobile : MonoBehaviour
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Awake
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void Awake()
        {
#if STB_MOBILE_INPUT
            this.gameObject.SetActive(false);
#endif

            this.enabled = false;
        }
    }
}
