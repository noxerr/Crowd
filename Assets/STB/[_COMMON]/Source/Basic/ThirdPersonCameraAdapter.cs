using UnityEngine;
using System.Collections;

namespace STB.Basics
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Class: ThirdPersonCameraAdapter
    /// # 
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class ThirdPersonCameraAdapter : MonoBehaviour
    {
        // public
        public Transform cameraTransform = null;
        public float nearestZPosition = -0.1f;
        public float adaptationSpeed = 4;

        // private
        Vector3 basePosition;
        float somethingBehindCounter = 0;

        // private
        Generic.GenericVehicleCamera actualCamera = null;


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Awake
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void Awake()
        {
            if (cameraTransform) basePosition = cameraTransform.localPosition;

            actualCamera = GameObject.FindObjectOfType<Generic.GenericVehicleCamera>();
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// OnTriggerStay
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void OnTriggerStay(Collider col)
        {
            if (actualCamera && !actualCamera.allowCameraAdapter) return;

            //Debug.Log("col A: " + col.name);

            if (!cameraTransform) return;
            if (col.name == "Impostor") return;
            if (col.GetComponent<Camera>()) return;
            if (col.GetComponent<Generic.GenericBulletTimeZone>()) return;

            if (cameraTransform.GetComponent<Generic.GenericVehicleCamera>())
            {
                //if (!col.gameObject.isStatic) return;
                if (col.GetComponent<Generic.GenericObjectBase>()) return;
                if (col.GetComponent<Generic.GenericGamePlayAreaDelimiter>()) return;
                if (col.GetComponent<Generic.GenericCameraCollisionIgnorer>()) return;
                if (col.GetComponent<Generic.GenericTrafficLightBase>()) return;
                if (col.GetComponent<STB.Generic.GenericPathPoint>()) return;
                if (col.GetComponent<STB.Generic.GenericNonStopZone>()) return;
                if (Basics.BasicFunctions.GetParentGenericVehicleController(col.transform)) return;
            }
            else
            {
                if (Basics.BasicFunctions.GetParentGenericMainCharacter(col.transform)) return;
                if (col.GetComponent<Generic.GenericMainCharacter>()) return;
                if (Basics.BasicFunctions.GetIsMainCharacter(col.name)) return;
                if (col.GetComponent<Generic.GenericBaseOfAll>()) return;
                if (col.GetComponent<Basics.ThirdPersonCameraAdapter>()) return;
                if (col.GetComponent<STB.Generic.GenericVehicleController>()) return;
                if (col.GetComponent<Generic.GenericGamePlayAreaDelimiter>()) return;
                if (col.GetComponent<Generic.GenericWeaponBase>()) return;
                if (col.GetComponent<STB.Generic.GenericTrafficLightBase>()) return;
                if (col.GetComponent<STB.Generic.GenericPathPoint>()) return;
                if (col.GetComponent<STB.Generic.GenericNonStopZone>()) return;
                if (col.GetComponent<STB.Generic.GenericScaryZone>()) return;
            }

            //Debug.Log("col B: " + col.name);

            somethingBehindCounter = 0.5f;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// ResetMissingScriptsFinderVariables
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void Update()
        {
            //Debug.Log("somethingBehindCounter: " + somethingBehindCounter);

            if (!cameraTransform)
            {
                Generic.GenericVehicleCamera actualGenericVehicleCamera = GameObject.FindObjectOfType<Generic.GenericVehicleCamera>();
                if (actualGenericVehicleCamera) cameraTransform = actualGenericVehicleCamera.transform;
            }

            if (cameraTransform)
            {
                if (cameraTransform.GetComponent<Generic.GenericVehicleCamera>())
                {
                    //Debug.Log("Vehicle camera");

                    if (somethingBehindCounter > 0)
                    {
                        cameraTransform.GetComponent<Generic.GenericVehicleCamera>().WarnSomethingCollidingWithCameraCounter();
                    }
                    else
                    {

                    }
                }
                else
                {
                    if (somethingBehindCounter > 0)
                    {
                        Vector3 nearestPosition = basePosition;
                        nearestPosition.z = nearestZPosition;

                        cameraTransform.localPosition = Vector3.Slerp(cameraTransform.localPosition, nearestPosition, adaptationSpeed * Time.deltaTime);
                    }
                    else
                    {
                        cameraTransform.localPosition = Vector3.Slerp(cameraTransform.localPosition, basePosition, adaptationSpeed * Time.deltaTime);
                    }
                }

                somethingBehindCounter -= Time.deltaTime;
            }
        }
    }
}
