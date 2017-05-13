using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace STB.Generic
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Class: GenericBulletTimeZone
    /// # 
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class GenericBulletTimeZone : MonoBehaviour
    {
        // public
        public float slowmoTime = 2;

        // private
        float slowmoCounter = 0;

        // private
        Camera bulletTimeCamera = null;
        Transform transformForCamera = null;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Awake
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void Awake()
        {
            if (this.GetComponent<Renderer>()) this.GetComponent<Renderer>().enabled = false;

            bulletTimeCamera = Basics.BasicFunctions.GetTransformInChildsByName(this.transform, "bulletTimeCamera").GetComponent<Camera>();
            bulletTimeCamera.gameObject.SetActive(false);

            Basics.BasicFunctions.SetIgnoreRaycastLayer(this.gameObject);
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// OnTriggerEnter
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void OnTriggerEnter(Collider col)
        {
            //Debug.Log("col name: " + col.name);

            if (Basics.BasicFunctions.GetParentGenericVehicleController(col.transform))
            {
                //Debug.Log("col name: " + col.name);

                slowmoCounter = slowmoTime;

                transformForCamera = col.transform;
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Update
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void Update()
        {
            if (slowmoCounter > 0)
            {
                Time.timeScale = 0.2f;
                Time.fixedDeltaTime = 0.02f * Time.timeScale;

                STB.Basics.WeAreTheMainCamera.primaryCamera = bulletTimeCamera;
                if (STB.Basics.WeAreTheMainCamera.RealCamera) { }; // just to update actua camera
                bulletTimeCamera.gameObject.SetActive(true);

                bulletTimeCamera.transform.LookAt(transformForCamera);
            }
            else
            {
                if (STB.Basics.WeAreTheMainCamera.primaryCamera == bulletTimeCamera)
                {
                    STB.Basics.WeAreTheMainCamera.primaryCamera = null;
                    if (STB.Basics.WeAreTheMainCamera.RealCamera) { }; // just to update actua camera

                    Time.timeScale = 1;
                    Time.fixedDeltaTime = 0.02f * Time.timeScale;
                }

                bulletTimeCamera.gameObject.SetActive(false);
            }

            slowmoCounter -= Time.deltaTime / 0.2f;
        }
    }
}
