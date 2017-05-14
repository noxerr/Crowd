using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using STB.SpecialInput;

namespace STB.PACS
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Class: GenericIndividual
    /// # A generic individual killer thrower system
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class GenericIndividualKillerThrower : GenericObject
    {
        // private
        List<GameObject> objetctsToThrowList = new List<GameObject>();
        int actualIndex = 0;


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Start
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void Start()
        {
            for (int i = 0; i < 4; i++)
            {
                GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                go.AddComponent<Rigidbody>();
                go.AddComponent<GenericIndividualKiller>().damage = 25;
                go.GetComponent< GenericIndividualKiller>().realKillerTransform = this.transform;
                go.SetActive(false);

                objetctsToThrowList.Add(go);
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

            bool shoot = false;

#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBPLAYER || UNITY_WEBGL
            if (Input.GetMouseButtonDown(0))
            {
                shoot = true;
            }
#endif

#if MOBILE_INPUT
            if (STBCrossPlatformInputManager.GetButtonUp("ShootButton")) shoot = true;
#endif

            if (shoot)
            {
                Vector3 throwingDirection = this.transform.forward;

                Ray ray = actualCamera.ScreenPointToRay(new Vector2(0.5f * Screen.width, 0.5f * Screen.height));
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    //Debug.Log ("Collider name: " + hit.collider.name);

                    if (Vector3.Distance(this.transform.position, hit.point) < 400)
                    {
                        throwingDirection = (hit.point - this.transform.position).normalized;
                    }
                }

                GameObject go = objetctsToThrowList[actualIndex];
                go.SetActive(true);

                go.transform.position = this.transform.position + 0.2f * this.transform.forward;
                go.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                go.GetComponent<Rigidbody>().velocity = 40 * throwingDirection;

                actualIndex++;

                if (actualIndex >= objetctsToThrowList.Count) actualIndex = 0;
            }
        }
    }
}
