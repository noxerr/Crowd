using UnityEngine;
using System.Collections;

namespace STB.PACS
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Class: GenericIndividualBehaviourChanger
    /// # 
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class GenericIndividualBehaviourChanger : MonoBehaviour
    {
        // protected
        protected GenericIndividual lastDetectedGenericIndividual = null;



        void Start()
        {
            StartExtended();
        }

        void OnTriggerEnter(Collider col)
        {
            GenericIndividual actualGenericIndividual = col.GetComponent<GenericIndividual>();

            if (actualGenericIndividual)
            {
                lastDetectedGenericIndividual = actualGenericIndividual;
            }

            OnTriggerEnterExtended(col);
        }

        void Update()
        {
            UpdateExtended();
        }

        protected virtual void OnTriggerEnterExtended(Collider col)
        {
        }

        protected virtual void StartExtended()
        {
        }

        protected virtual void UpdateExtended()
        {
        }
    }
}
