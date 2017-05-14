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
    public class GlobalScarer : MonoBehaviour
    {
        // public
        public KeyCode keycodeToScareAll = KeyCode.Alpha9;
        public float scaredTime = 9;
        public Transform safeZone = null;

        // private
        List<Generic.GenericSafeZone> safeZonesList = new List<Generic.GenericSafeZone>();
        float safeZoneTotalProbability = 0;


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// AwakeExtended
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void Awake()
        {
            if (safeZone)
            {
                for (int i = 0; i < safeZone.childCount; i++)
                {
                    Generic.GenericSafeZone actualGenericSafeZone = safeZone.GetChild(i).GetComponent<Generic.GenericSafeZone>();

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
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// AwakeExtended
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void Update()
        {
            if (!IndividualHandler.Singleton) return;

            if (Input.GetKeyDown(keycodeToScareAll))
            {
                Debug.Log("Scare all: " + IndividualHandler.GenericIndividualList.Count);

                for (int i = 0; i < IndividualHandler.GenericIndividualList.Count; i++)
                {
                    IndividualHandler.GenericIndividualList[i].GoToSafeZone(GetSafeZone(), scaredTime);
                }
            }

            for (int j = 0; j < safeZonesList.Count; j++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha0 + j))
                {
                    Debug.Log("Scare all: " + IndividualHandler.GenericIndividualList.Count);

                    for (int i = 0; i < IndividualHandler.GenericIndividualList.Count; i++)
                    {
                        IndividualHandler.GenericIndividualList[i].GoToSafeZone(safeZonesList[j].transform, scaredTime);
                    }
                }
            }
        }
    }
}
