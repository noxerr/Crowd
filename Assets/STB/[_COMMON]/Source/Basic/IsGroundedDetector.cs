using UnityEngine;
using System.Collections;

namespace STB.Basics
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Class: IsGroundedDetector
    /// # 
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class IsGroundedDetector : MonoBehaviour
    {
        // public
        public bool IsOnGrounded = true; 


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// OnCollisionEnter
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void OnCollisionEnter(Collision collision)
        {
            IsOnGrounded = true;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// OnCollisionExit
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void OnCollisionExit(Collision collision)
        {
            IsOnGrounded = false;
        }
    }
}