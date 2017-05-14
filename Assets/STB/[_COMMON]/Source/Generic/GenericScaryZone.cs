using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace STB.Generic
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Class: GenericScaryZone
    /// # 
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class GenericScaryZone : MonoBehaviour
    {
        // public
        public bool hideRendererIfExists = false;
        public float scaredTime = 9;
        public Transform safeZone = null;

        // private
        List<GenericSafeZone> safeZonesList = new List<GenericSafeZone>();
        float safeZoneTotalProbability = 0;


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Awake
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected virtual void Awake()
        {
            if (hideRendererIfExists)
            {
                if (GetComponent<Renderer>()) GetComponent<Renderer>().enabled = false;
            }

            if (safeZone)
            {
                for (int i = 0; i < safeZone.childCount; i++)
                {
                    GenericSafeZone actualGenericSafeZone = safeZone.GetChild(i).GetComponent<GenericSafeZone>();

                    if (actualGenericSafeZone)
                    {
                        safeZonesList.Add(actualGenericSafeZone);

                        safeZoneTotalProbability += actualGenericSafeZone.probability;
                    }
                }

                //Debug.Log("Child cound: " + safeZone.childCount);
                //Debug.Log("safeZonesList count: " + safeZonesList.Count);

                //Debug.Log("safeZoneTotalProbability: " + safeZoneTotalProbability);
            }

            AwakeExtended();
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// AwakeExtended
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected virtual void AwakeExtended()
        {
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// GetSafeZone
        /// # 
        /// </summary>
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public Transform GetSafeZone()
        {
            if (safeZonesList.Count > 0)
            {
                float r = Random.Range(0, safeZoneTotalProbability);
                float actualSum = 0;

                for (int i = 0; i < safeZonesList.Count; i++)
                {
                    actualSum += safeZonesList[i].probability;

                    if (r < actualSum) return safeZonesList[i].transform;
                }
            }

            return safeZone;
        }
    }
}
