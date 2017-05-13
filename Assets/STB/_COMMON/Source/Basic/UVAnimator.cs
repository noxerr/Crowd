using UnityEngine;
using System.Collections;

namespace STB.Basics
{ 
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Class: UVAnimator
    /// # 
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class UVAnimator : MonoBehaviour
    {
        // public
        public Vector2 animationSpeed = new Vector2(0, 0);

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Start
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void Start()
        {
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// LateUpdate
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void LateUpdate()
        {
            this.GetComponent<Renderer>().material.SetTextureOffset("_MainTex", Time.time * animationSpeed);
        }
    }
}
