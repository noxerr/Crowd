using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace STB.PACS
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Class: ExtendedIndividualBehaviourChanger
    /// # 
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class ExtendedIndividualBehaviourChanger : GenericIndividualBehaviourChanger
    {
        // public
        public KeyCode keycodeToScareAll = KeyCode.Alpha8;
        public float scaredTime = 9;
        public Transform safeZone = null;

        // private
        List<Generic.GenericSafeZone> safeZonesList = new List<Generic.GenericSafeZone>();
        float safeZoneTotalProbability = 0;


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// StartExtended -- OVERRIDED
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected override void StartExtended()
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
        /// OnTriggerEnterExtended -- OVERRIDED
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected override void OnTriggerEnterExtended(Collider col)
        {
            GenericIndividual actualGenericIndividual = col.GetComponent<GenericIndividual>();

            if (actualGenericIndividual)
            {
                Debug.Log("ExtendedIndividualBehaviourChanger -> new generic individual detected: " + actualGenericIndividual.gameObject.name);

                actualGenericIndividual.showDebug = true;

                this.GetComponent<AudioSource>().Stop();
                this.GetComponent<AudioSource>().Play();

                actualGenericIndividual.GoToIdle(true, 9999);
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
        /// UpdateExtended -- OVERRIDED
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected override void UpdateExtended()
        {
            if (!IndividualHandler.Singleton) return;

            if (lastDetectedGenericIndividual)
            {
                Vector3 lookAtPosition = this.transform.position;
                lookAtPosition.y = lastDetectedGenericIndividual.transform.position.y;

                Quaternion targetRotation = Quaternion.LookRotation(lookAtPosition - lastDetectedGenericIndividual.transform.position);
                lastDetectedGenericIndividual.transform.rotation = Quaternion.Slerp(lastDetectedGenericIndividual.transform.rotation, targetRotation, 4 * Time.deltaTime);

                if (Input.GetKeyDown(keycodeToScareAll))
                {
                    lastDetectedGenericIndividual.GoToSafeZone(GetSafeZone(), 9999);
                    lastDetectedGenericIndividual = null;
                }

                for (int j = 0; j < safeZonesList.Count; j++)
                {
                    if (Input.GetKeyDown(KeyCode.Alpha0 + j))
                    {
                        for (int i = 0; i < IndividualHandler.GenericIndividualList.Count; i++)
                        {
                            lastDetectedGenericIndividual.GoToSafeZone(safeZonesList[j].transform, scaredTime);
                        }
                    }
                }
            }
        }
    }
}
