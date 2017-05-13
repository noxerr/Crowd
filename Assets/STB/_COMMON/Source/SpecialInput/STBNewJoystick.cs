using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace STB.SpecialInput
{ 
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// CLASS NAME: STBNewJoystick
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class STBNewJoystick : MonoBehaviour, IDragHandler, IEndDragHandler
    {
        // public static
        public static STBNewJoystick Singleton = null;

        // public
        public Vector2 touchZone = new Vector2(300, 300);
        public bool hideJoystick = false;

        // public -- hidden
        [HideInInspector]
        public Vector2 JoystickInput = Vector2.zero;

        // private 
        Vector3 startPos = Vector3.zero;
        Image joystickImage;

        // private
        Vector3 originalPosition;

        // private
        bool firstTime = true;


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Awake
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void Awake()
        {
            Singleton = this;

            //Debug.Log(this.gameObject.name + " -> originalPosition: " + originalPosition);

            originalPosition = this.transform.localPosition;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// OnEnable
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void OnEnable()
        {
            startPos = transform.position;

            if (gameObject.GetComponent<Image>())
            {
                joystickImage = gameObject.GetComponent<Image>();
            }

            if (joystickImage && hideJoystick == true)
            {
                joystickImage.CrossFadeAlpha(0, 0, true);
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Update
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void Update()
        {
            Singleton = this;

            if (firstTime)
            {
                firstTime = false;

                originalPosition = this.transform.localPosition;
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// UpdateVirtualAxes
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void UpdateVirtualAxes(Vector3 value, string zone)
        {
            //Debug.Log("UpdateVirtualAxes in " + zone);

            Vector3 delta = startPos - value;

            delta.y = -delta.y;
            delta.x /= touchZone.x / 2;
            delta.y /= touchZone.y / 2;

            JoystickInput = new Vector2(-delta.x, delta.y);
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// OnDrag
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void OnDrag(PointerEventData data)
        {
            Vector3 newPos = Vector3.zero;

            int deltax = (int)(data.position.x - startPos.x);
            deltax = Mathf.Clamp(deltax, -Mathf.FloorToInt(touchZone.x), Mathf.FloorToInt(touchZone.x));
            newPos.x = deltax;

            int deltay = (int)(data.position.y - startPos.y);
            deltay = Mathf.Clamp(deltay, Mathf.FloorToInt(-touchZone.x), Mathf.FloorToInt(touchZone.y));
            newPos.y = deltay;

            transform.position = new Vector3(startPos.x + newPos.x, startPos.y + newPos.y, startPos.z + newPos.z);
            UpdateVirtualAxes(transform.position, "OnDrag");
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// OnEndDrag
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void OnEndDrag(PointerEventData data)
        {
            //Debug.Log("OnEndDrag called.");

            //float aspectRatio = (float)Screen.width / (float)Screen.height;
            //Vector3 specialOffset = new Vector3(-aspectRatio * touchZone.x, -touchZone.y, 0);

            transform.localPosition = originalPosition;// + specialOffset;

            startPos = transform.position;

            UpdateVirtualAxes(startPos, "OnEndDrag");
        }
    }
}
