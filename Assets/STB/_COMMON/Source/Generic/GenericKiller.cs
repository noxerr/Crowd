using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace STB.Generic
{ 
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Class: GenericKiller
    /// # 
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class GenericKiller : MonoBehaviour
    {
        // public
        public float speed = 1;
        public float damage = 100;
        public bool applyHitSpeedBasedOnRadious = true;
        public Transform realKillerTransform = null;


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// OnCollisionEnter
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void OnCollisionEnter(Collision col)
        {
            //Debug.Log("col.collider: " + col.collider.name);

            OnCollisionEnterExtended(col);
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// OnCollisionEnterExtended -- VIRTUAL
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected virtual void OnCollisionEnterExtended(Collision col)
        {
        }
    }
}
