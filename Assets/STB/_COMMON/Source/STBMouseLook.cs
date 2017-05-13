using System;
using UnityEngine;
using STB.SpecialInput;

namespace STB.Characters.FirstPerson
{
    [Serializable]
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Class: STBMouseLook
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class STBMouseLook
    {
        // public
        public bool clampVerticalRotation = true;
        public float MinimumX = -90F;
        public float MaximumX = 90F;
        public bool smooth;
        public float smoothTime = 5f;
        public bool lockCursor = true;

        // private
        Quaternion m_CharacterTargetRot;
        Quaternion m_CameraTargetRot;

        // private
        Vector3 lastEulerAngles = Vector3.zero;


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Init
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Init(Transform character, Transform camera)
        {
            m_CharacterTargetRot = character.localRotation;
            m_CameraTargetRot = camera.localRotation;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// LookRotation
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void LookRotation(Transform character, Transform camera, float XSensitivity, float YSensitivity, bool enableGyroscopeToMoveCamera, bool enableManualJoystickToMoveCamera)
        {
#if UNITY_EDITOR
            float actualFramespeed = 1 / Time.deltaTime;
            if (actualFramespeed < 5) return;
#endif

            float yRot = 0;
            float xRot = 0;

            if (enableGyroscopeToMoveCamera)
            {
                yRot = 4 * YSensitivity * Input.gyro.rotationRateUnbiased.y;
                xRot = 4 * XSensitivity * Input.gyro.rotationRateUnbiased.x;
            }

            if (enableManualJoystickToMoveCamera)
            {
#if STB_MOBILE_INPUT
                yRot = STBCrossPlatformInputManager.GetAxis("Mouse X") * XSensitivity;
                xRot = STBCrossPlatformInputManager.GetAxis("Mouse Y") * YSensitivity;
#else
                yRot = Input.GetAxis("Mouse X") * XSensitivity;
                xRot = Input.GetAxis("Mouse Y") * YSensitivity;
#endif
            }

            //Debug.Log("yRot: " + yRot);

            m_CharacterTargetRot *= Quaternion.Euler(0f, yRot * (100 * Time.deltaTime), 0f);
            m_CameraTargetRot *= Quaternion.Euler(-xRot * (100 * Time.deltaTime), 0f, 0f);

            if (clampVerticalRotation) m_CameraTargetRot = ClampRotationAroundXAxis(m_CameraTargetRot);

            if (smooth)
            {
                character.localRotation = Quaternion.Slerp(character.localRotation, m_CharacterTargetRot, smoothTime * Time.deltaTime);
                camera.localRotation = Quaternion.Slerp(camera.localRotation, m_CameraTargetRot, smoothTime * Time.deltaTime);
            }
            else
            {
                character.localRotation = m_CharacterTargetRot;
                camera.localRotation = m_CameraTargetRot;
            }

            //Debug.Log("camera.localRotation: " + camera.localRotation.eulerAngles.ToString());

            lastEulerAngles = camera.localRotation.eulerAngles;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// CameraRotation
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public Vector3 CameraRotation
        {
            get { return lastEulerAngles; }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// SetCursorLock
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void SetCursorLock(bool value)
        {
            lockCursor = value;

            if (!lockCursor)
            {
                //we force unlock the cursor if the user disable the cursor locking helper

#if STB_MOBILE_INPUT
#else
#if !UNITY_EDITOR
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
#endif
#endif
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// ClampRotationAroundXAxis
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        Quaternion ClampRotationAroundXAxis(Quaternion q)
        {
            q.x /= q.w;
            q.y /= q.w;
            q.z /= q.w;
            q.w = 1.0f;

            float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);

            angleX = Mathf.Clamp(angleX, MinimumX, MaximumX);

            q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

            return q;
        }
    }
}
