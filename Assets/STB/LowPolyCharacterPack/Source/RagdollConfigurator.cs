using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace STB.LowPolyCharacterPack
{
    ///////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Class: RagdollConfigurator
    /// # 
    /// </summary>
    ///////////////////////////////////////////////////////////////////////////////////////////////////////
    public class RagdollConfigurator : MonoBehaviour
    {
        // public
        public float massMultiplier = 6;


        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Start
        /// </summary>
        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        void Start()
        {
            ChangeAllChildJoints(this.transform);
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// ChangeAllChildJoints
        /// # 
        /// </summary>
        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        public void ChangeAllChildJoints(Transform src)
        {
            if (src.GetComponent<CharacterJoint>())
            {
                src.GetComponent<CharacterJoint>().enableProjection = true;
            }

            if (src.GetComponent<Rigidbody>())
            {
                src.GetComponent<Rigidbody>().mass = massMultiplier * src.GetComponent<Rigidbody>().mass;
            }

            foreach (Transform child in src)
            {
                ChangeAllChildJoints(child);
            }
        }
    }
}