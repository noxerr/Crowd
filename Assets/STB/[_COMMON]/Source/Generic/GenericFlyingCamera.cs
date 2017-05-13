using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace STB.Generic
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Class: GenericFlyingCamera
    /// # 
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class GenericFlyingCamera : MonoBehaviour
    {
        // public
        public float mainSpeed = 100.0f;
        public float shiftAdd = 250.0f;
        public float maxShift = 1000.0f;
        public float camSens = 0.25f;

        // private
        Vector3 lastMouse = new Vector3(255, 255, 255);
        float totalRun = 1.0f;

        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// GetBaseInput
        /// </summary>
        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        Vector3 GetBaseInput()
        {
            Vector3 p_Velocity = new Vector3();
            if (Input.GetKey(KeyCode.W))
            {
                p_Velocity += new Vector3(0, 0, 1);
            }

            if (Input.GetKey(KeyCode.S))
            {
                p_Velocity += new Vector3(0, 0, -1);
            }

            if (Input.GetKey(KeyCode.A))
            {
                p_Velocity += new Vector3(-1, 0, 0);
            }

            if (Input.GetKey(KeyCode.D))
            {
                p_Velocity += new Vector3(1, 0, 0);
            }


            return p_Velocity;
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Update
        /// </summary>
        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        void Update()
        {
            lastMouse = Input.mousePosition - lastMouse;
            lastMouse = new Vector3(-lastMouse.y * camSens, lastMouse.x * camSens, 0);
            lastMouse = new Vector3(transform.eulerAngles.x + lastMouse.x, transform.eulerAngles.y + lastMouse.y, 0);
            transform.eulerAngles = lastMouse;
            lastMouse = Input.mousePosition;

            Vector3 p = GetBaseInput();

            if (Input.GetKey(KeyCode.LeftShift))
            {
                totalRun += Time.deltaTime;
                p = p * totalRun * shiftAdd;
                p.x = Mathf.Clamp(p.x, -maxShift, maxShift);
                p.y = Mathf.Clamp(p.y, -maxShift, maxShift);
                p.z = Mathf.Clamp(p.z, -maxShift, maxShift);
            }
            else
            {
                totalRun = Mathf.Clamp(totalRun * 0.5f, 1f, 1000f);
                p = p * mainSpeed;
            }

            p = p * Time.deltaTime;
            Vector3 newPosition = transform.position;

            if (Input.GetKey(KeyCode.Space))
            {
                transform.Translate(p);
                newPosition.x = transform.position.x;
                newPosition.z = transform.position.z;
                transform.position = newPosition;
            }
            else
            {
                transform.Translate(p);
            }
        }
    }
}
