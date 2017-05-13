using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace STB.Basics
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Class: LODHandler
    /// # 
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class LODHandler : MonoBehaviour
    {
        // public static
        public static LODHandler Singleton = null;

        // public
        public float fliyingDistance = 60;
        public bool manualMode = false;
        public int desiredLODIndex = -1;

        // private
        int previousDesiredLODIndex = -2;
        List<LODGroup> LODGroupList = new List<LODGroup>();


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Awake
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void Awake()
        {
            Singleton = this;

            LODGroupList = GameObject.FindObjectsOfType<LODGroup>().ToList();

            //Debug.Log("LODGroupList Count: " + LODGroupList.Count);


            ChangeAllLODS(desiredLODIndex);
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

            if (!manualMode)
            {
                if (actualCamera.transform.position.y > fliyingDistance)
                {
                    desiredLODIndex = 3;
                }
                else
                {
                    desiredLODIndex = -1;
                }
            }

#if SPECIAL_SUPER_HIGH_MODE
            desiredLODIndex = 0;
#endif

            if (previousDesiredLODIndex != desiredLODIndex)
            {
                //Debug.Log("New desiredLODIndex: " + desiredLODIndex);
                ChangeAllLODS(desiredLODIndex);

                previousDesiredLODIndex = desiredLODIndex;
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// FixedUpdate
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void FixedUpdate()
        {
            //Debug.Log("Fixed time: " + Time.fixedDeltaTime);
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// ChangeAllLODS
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void ChangeAllLODS(int desiredIndex)
        {
            for (int i = 0; i < LODGroupList.Count; i++)
            {
                int actualDesiredInxed = desiredIndex;

                if (actualDesiredInxed >= LODGroupList[i].lodCount) actualDesiredInxed = LODGroupList[i].lodCount - 1;

                LODGroupList[i].ForceLOD(actualDesiredInxed);
            }
        }
    }
}
