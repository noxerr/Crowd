using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace STB.PACS
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Class: ScaryZone
    /// # If a individual touch this will start running until the scared time turns zero
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class ScaryZone : Generic.GenericScaryZone
    {
        // public
        public float workingDistance = 0;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// AwakeExtended
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected override void AwakeExtended()
        {
            GenericObject actualGenericObject = this.gameObject.AddComponent<GenericObject>();

            if (workingDistance >= 0) actualGenericObject.manualWorkingDistance = workingDistance;

            //Debug.Log("Set Ignore Raycast layer");
            Basics.BasicFunctions.SetIgnoreRaycastLayer(this.gameObject);
        }
    }
}
