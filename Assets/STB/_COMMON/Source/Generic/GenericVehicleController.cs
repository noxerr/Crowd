using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace STB.Generic
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Class: GenericVehicleController
    /// # 
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class GenericVehicleController : MonoBehaviour
    {
        // public -- enum
        public enum cVehicleControllerType { car, motorbike, copter, plane, boat };

        // public -- hidden
        [HideInInspector]
        public cVehicleControllerType VehicleControllerType = cVehicleControllerType.car;


        // public
        public bool useSuperEasySettings = true;

        public float superSpeedSettings_torqueMultiplier = 9;
        public float superSpeedSettings_speedMultiplier = 1.25f;
        public float superSpeedSettings_maxSpeed = 40;
        public float superSpeedSettings_maximunSteerAngle = 30;

        // public -- for camera
        public int FOV = 45;
        public float minCameraDistance = 5;
        public float minHeightOffset = 0;
        public float maxHeightOffset = 0;
        public float distanceOffset = 0;
        public float heightDampingOffset = 0;
        public float rotationDampingOffset = 0;
        public float lateralDistanceOffset = 0;

        // private
        Transform CameraFocusPoint = null;


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Awake
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void Awake()
        {
            //Debug.Log("Initialising " + this.gameObject.name);

            // get the camera focus point
            this.CameraFocusPoint = Basics.BasicFunctions.GetTransformInChildsByName(this.transform, "CameraFocusPoint");

            if (!CameraFocusPoint)
            {
                Debug.Log("WARNING -> CameraFocusPoint not defined");
            }

            if (!this.GetComponent<Basics.ThirdPersonCameraAdapter>())
            {
                GameObject actualCameraAdapter = new GameObject("CameraAdapter");
                actualCameraAdapter.transform.parent = this.transform;
                actualCameraAdapter.transform.position = this.transform.position - 3 * this.transform.forward + 2 * this.transform.up;

                SphereCollider adapterCollider = actualCameraAdapter.AddComponent<SphereCollider>();
                adapterCollider.radius = 1;
                adapterCollider.isTrigger = true;

                Basics.ThirdPersonCameraAdapter actualThirdPersonCameraAdapter = actualCameraAdapter.AddComponent<Basics.ThirdPersonCameraAdapter>();

                Generic.GenericVehicleCamera actualGenericVehicleCamera = GameObject.FindObjectOfType<Generic.GenericVehicleCamera>();

                if (actualGenericVehicleCamera) actualThirdPersonCameraAdapter.cameraTransform = actualGenericVehicleCamera.transform;
                else
                {
                    //Debug.Log("NOTE -> There is no actualGenericVehicleCamera for " + this.gameObject.name);
                }
            }

            AwakeExtended();
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// GetCameraUpdateAllowed -- VIRTUAL
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual bool GetCameraUpdateAllowed()
        {
            return true;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// GetFixedCameraUpdate
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool GetFixedCameraUpdate()
        {
            switch (VehicleControllerType)
            {
                case cVehicleControllerType.car:
                case cVehicleControllerType.boat:
                case cVehicleControllerType.copter:
                case cVehicleControllerType.plane:
                    return true;

                case cVehicleControllerType.motorbike:

                    return false;
            }

            return true;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// AwakeExtended -- VIRTUAL
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected virtual void AwakeExtended()
        {
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// GetCameraFocusPoint
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public Transform GetCameraFocusPoint()
        {
            if (CameraFocusPoint) return CameraFocusPoint;

            return this.transform;
        }
    }
}
