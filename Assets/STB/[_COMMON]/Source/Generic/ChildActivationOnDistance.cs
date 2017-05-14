using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace STB.Generic
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Class: ChildActivationOnDistance
    /// # 
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class ChildActivationOnDistance : MonoBehaviour
    {
        // public
        public float workingDistanceForChilds = 400;

        // private
        List<GameObject> childList = new List<GameObject>();
        int jumpIndex = 0;
        int jump = 20;


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Awake
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void Awake()
        {
            foreach (Transform t in this.transform)
            {
                childList.Add(t.gameObject);
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Update
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void Update()
        {
            Camera actualCamera = Camera.main;
            if (STB.Basics.WeAreTheMainCamera.RealCamera) actualCamera = STB.Basics.WeAreTheMainCamera.RealCamera;
            if (!actualCamera) return;

            for (int i = jumpIndex * jump; i < Mathf.Min((jumpIndex + 1) * jump, childList.Count); i++)
            {
                childList[i].SetActive(Vector3.Distance(childList[i].transform.position, actualCamera.transform.position) <= workingDistanceForChilds);
            }

            jump++;

            if ((jumpIndex + 1) * jump >= childList.Count)
            {
                jump = 0;
            }
        }
    }
}
