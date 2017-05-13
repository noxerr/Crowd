using UnityEngine;
using System.Collections;

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
        public Transform SafeZone = null;


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// StartExtended -- OVERRIDED
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected override void StartExtended()
        {
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
        /// UpdateExtended -- OVERRIDED
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected override void UpdateExtended()
        {
            if (lastDetectedGenericIndividual)
            {
                Vector3 lookAtPosition = this.transform.position;
                lookAtPosition.y = lastDetectedGenericIndividual.transform.position.y;

                Quaternion targetRotation = Quaternion.LookRotation(lookAtPosition - lastDetectedGenericIndividual.transform.position);
                lastDetectedGenericIndividual.transform.rotation = Quaternion.Slerp(lastDetectedGenericIndividual.transform.rotation, targetRotation, 4 * Time.deltaTime);

                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    lastDetectedGenericIndividual.GoToSafeZone(SafeZone, 9999);
                    lastDetectedGenericIndividual = null;
                }
            }
        }
    }
}
