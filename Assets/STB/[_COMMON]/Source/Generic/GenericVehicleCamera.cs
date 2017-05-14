using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace STB.Generic
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Class: GenericVehicleCamera
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class GenericVehicleCamera : MonoBehaviour
    {
        // public static
        public static GenericVehicleCamera Singleton = null;
        public static Vector3 CAMERA_FOCUS_POINT_BASE = new Vector3(0, 2, -2);

        // public
        public KeyCode cameraKeyCode = KeyCode.C;

        // public
        public GenericVehicleController vehicleController = null;

        // public enum -- camera type
        public enum CameraType
        {
            normal,
            lateral,
            flying,
            specialFixed,
            extraClose
        }

        // public
        public bool allowCameraAdapter = true;
        public CameraType cameraType = CameraType.normal;

        // the height we want the camera to be above the target
        float minHeight = 0.0f;
        float maxHeight = 2.0f;
        float height = 1;

        // protected
        protected Transform target = null;

        // The distance in the x-z plane to the target
        float distance = 7.0f;
        float lateralDistance = 0;

        // How much we 
        float heightDamping = 9.0f;
        float rotationDamping = 9.0f;

        // protected -- focus
        protected Vector3 focusPointBase = CAMERA_FOCUS_POINT_BASE;
        protected float focusPercetange = 1;

        // private -- rotation
        bool rotationAllowed = false;
        Vector3 actualRotation = Vector3.zero;
        Vector3 extraRotationTarget = Vector3.zero;
        Vector3 extraRotationActual = Vector3.zero;

        // private
        float timeBeforeDetectCameraCollisions = 2;
        float somethingCollidingWithCameraCounter = 0;
        float finalDistance = 0;


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Start
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void Start()
        {
            Singleton = this;

            if (!vehicleController)
            {
                vehicleController = GameObject.FindObjectOfType<GenericVehicleController>();
            }

            StartExtended();
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// OnEnable
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void OnEnable()
        {
            finalDistance = distance;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// StartExtended -- virtual
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected virtual void StartExtended()
        {
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// WarnSomethingCollidingWithCameraCounter
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void WarnSomethingCollidingWithCameraCounter()
        {
            //Debug.Log("WarnSomethingCollidingWithCameraCounter");

            somethingCollidingWithCameraCounter = 0.5f;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// HandleCameraType
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void HandleCameraType()
        {
            // change type
            int actualCameraTypeIndex = (int)cameraType;

            KeyCode finalCameraKeyCode = cameraKeyCode;
            if (Generic.KeyActionManager.Singleton) finalCameraKeyCode = Generic.KeyActionManager.Singleton.cameraKey;

            if (Input.GetKeyDown(finalCameraKeyCode))
            {
                actualCameraTypeIndex++;
            }

            if (STB.SpecialInput.STBCrossPlatformInputManager.GetButtonUp("ChangeCamera"))
            {
                actualCameraTypeIndex++;
            }

            if (actualCameraTypeIndex > sizeof(CameraType))
            {
                actualCameraTypeIndex = 0;
            }

            cameraType = (CameraType)actualCameraTypeIndex;

            // handle types
            switch (cameraType)
            {
                case CameraType.normal:
                    {
                        minHeight = 2.0f;
                        maxHeight = 2.0f;
                        distance = 12.0f;
                        heightDamping = 9.0f;
                        rotationDamping = 9.0f;
                        lateralDistance = 0;
                    }
                    break;

                case CameraType.flying:
                    {
                        minHeight = 30.0f;
                        maxHeight = 30.0f;
                        distance = 20.0f;
                        heightDamping = 9.0f;
                        rotationDamping = 9.0f;
                        lateralDistance = 0;
                    }
                    break;

                case CameraType.specialFixed:
                    {
                        minHeight = 4.0f;
                        maxHeight = 9.0f;
                        distance = 0.0f;
                        heightDamping = 4.0f;
                        rotationDamping = 1.0f;
                        lateralDistance = 18;
                    }
                    break;

                case CameraType.lateral:
                    {
                        minHeight = 4.0f;
                        maxHeight = 6.0f;
                        distance = 0.0f;
                        heightDamping = 4.0f;
                        rotationDamping = 1.0f;
                        lateralDistance = 14;
                    }
                    break;

                case CameraType.extraClose:
                    {
                        minHeight = 1.5f;
                        maxHeight = 1.5f;
                        distance = 10.0f;
                        heightDamping = 9.0f;
                        rotationDamping = 9.0f;
                        lateralDistance = 0;
                    }
                    break;
            }


            // special values for actual vehicle controller
            if (vehicleController)
            {
                minHeight += vehicleController.minHeightOffset;
                maxHeight += vehicleController.maxHeightOffset;
                distance += vehicleController.distanceOffset;
                heightDamping += vehicleController.heightDampingOffset;
                rotationDamping += vehicleController.rotationDampingOffset;
                lateralDistance += vehicleController.lateralDistanceOffset;

                //Debug.Log("somethingCollidingWithCameraCounter: " + somethingCollidingWithCameraCounter);

                if (somethingCollidingWithCameraCounter > 0) finalDistance -= 9 * Time.deltaTime;
                else finalDistance += 9 * Time.deltaTime;

                finalDistance = Mathf.Clamp(finalDistance, vehicleController.minCameraDistance, distance);

                //Debug.Log("finalDistance: " + finalDistance);

                GetComponent<Camera>().fieldOfView = vehicleController.FOV;
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// NormalUpdate
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void NormalUpdate(float actualTime)
        {
            if (!vehicleController.GetCameraUpdateAllowed()) return;

            // focus distance
            if (vehicleController)
            {
                // camera type
                HandleCameraType();


                // handle focus percentage
#if STB_MOBILE_INPUT
#else
                focusPercetange += Input.GetAxis("Mouse ScrollWheel");
#endif

                //Debug.Log("focusPercetange: " + focusPercetange);

                if (focusPercetange <= 0)
                {
                    focusPercetange = 0;
                }

                if (focusPercetange >= 1)
                {
                    focusPercetange = 1;
                }

                Vector3 actualFocus = focusPointBase + focusPercetange * (new Vector3(0, -0.5f, 6));
                vehicleController.GetCameraFocusPoint().localPosition = actualFocus;
                target = vehicleController.GetCameraFocusPoint();

                // rotation
                if (rotationAllowed)
                {
                    float h = Input.GetAxis("Horizontal");

                    extraRotationTarget.y = 90 * h;

                    //print ("extraRotationTarget: " + extraRotationTarget);
                    extraRotationActual = Vector3.Lerp(extraRotationActual, extraRotationTarget, actualTime);

                    actualRotation += new Vector3(0, Input.GetAxis("Mouse X"), 0);

                    vehicleController.GetCameraFocusPoint().localRotation = Quaternion.identity;
                    vehicleController.GetCameraFocusPoint().Rotate(actualRotation + extraRotationActual);
                }

                // change height between limits
#if STB_MOBILE_INPUT
#else
                height += Input.GetAxis("Mouse Y");
#endif

                if (height > maxHeight)
                {
                    height = maxHeight;
                }

                if (height < minHeight)
                {
                    height = minHeight;
                }

                float distanceDif = 0;

                somethingCollidingWithCameraCounter -= Time.deltaTime;
                timeBeforeDetectCameraCollisions -= Time.deltaTime;

                if (target)
                {
                    // Calculate the current rotation angles
                    float wantedRotationAngle = target.eulerAngles.y;
                    float wantedHeight = target.position.y + height + 0.25f * distanceDif;

                    float currentRotationAngle = transform.eulerAngles.y;
                    float currentHeight = transform.position.y;

                    // Damp the rotation around the y-axis
                    currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle, rotationDamping * actualTime);

                    // Damp the height
                    currentHeight = Mathf.Lerp(currentHeight, wantedHeight, heightDamping * actualTime);

                    // Convert the angle into a rotation
                    Quaternion currentRotation = Quaternion.Euler(0, currentRotationAngle, 0);

                    //Debug.Log ("currentRotation: " + lateralDistance);
                    //Debug.Log ("lateralDistance: " + lateralDistance);

                    // Set the position of the camera on the x-z plane to:

                    // distance meters behind the target
                    transform.position = target.position;

                    if (cameraType == CameraType.lateral)
                    {
                        transform.position += lateralDistance * target.transform.right;
                    }
                    else
                    {
                        transform.position -= currentRotation * Vector3.forward * finalDistance;
                        transform.position += lateralDistance * Vector3.right;
                    }

                    // Set the height of the camera
                    transform.position = new Vector3(transform.position.x, currentHeight, transform.position.z);

                    // Always look at the target
                    transform.LookAt(target);
                }
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// FixedUpdate
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void FixedUpdate()
        {
            if (!vehicleController) return;

            if (vehicleController.GetFixedCameraUpdate())
            {
                NormalUpdate(Time.deltaTime);

                FixedUpdateExtended();
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// LateUpdate
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void LateUpdate()
        {
            if (!vehicleController) return;

            if (!vehicleController.GetFixedCameraUpdate())
            {
                NormalUpdate(Time.deltaTime);

                FixedUpdateExtended();
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// FixedUpdateExtended -- VIRTUAL
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected virtual void FixedUpdateExtended()
        {
        }
    }
}
