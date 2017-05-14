using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace STB.SpecialInput
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Class: STBTiltInput
    /// # helps with managing tilt input on mobile devices
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class STBTiltInput : MonoBehaviour
    {
        // public -- enum
        public enum AxisOptions // options for the various orientations
        {
            ForwardAxis,
            SidewaysAxis,
        }
        
        // public
        [Serializable]
        public class AxisMapping
        {
            public enum MappingType
            {
                NamedAxis,
                MousePositionX,
                MousePositionY,
                MousePositionZ
            };


            public MappingType type;
            public string axisName;
        }

        public AxisMapping mapping;
        public AxisOptions tiltAroundAxis = AxisOptions.ForwardAxis;
        public float fullTiltAngle = 25;
        public float centreAngleOffset = 0;

        // private
        STBVirtualAxis m_SteerAxis;


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// OnEnable
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void OnEnable()
        {
            if (mapping.type == AxisMapping.MappingType.NamedAxis)
            {
                m_SteerAxis = new STBVirtualAxis(mapping.axisName);
                STBCrossPlatformInputManager.RegisterSTBVirtualAxis(m_SteerAxis);
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Update
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void Update()
        {
            float angle = 0;
            if (Input.acceleration != Vector3.zero)
            {
                switch (tiltAroundAxis)
                {
                    case AxisOptions.ForwardAxis:
                        angle = Mathf.Atan2(Input.acceleration.x, -Input.acceleration.y)*Mathf.Rad2Deg +
                                centreAngleOffset;
                        break;
                    case AxisOptions.SidewaysAxis:
                        angle = Mathf.Atan2(Input.acceleration.z, -Input.acceleration.y)*Mathf.Rad2Deg +
                                centreAngleOffset;
                        break;
                }
            }

            float axisValue = Mathf.InverseLerp(-fullTiltAngle, fullTiltAngle, angle)*2 - 1;
            switch (mapping.type)
            {
                case AxisMapping.MappingType.NamedAxis:
                    m_SteerAxis.Update(axisValue);
                    break;
                case AxisMapping.MappingType.MousePositionX:
                    STBCrossPlatformInputManager.SetVirtualMousePositionX(axisValue*Screen.width);
                    break;
                case AxisMapping.MappingType.MousePositionY:
                    STBCrossPlatformInputManager.SetVirtualMousePositionY(axisValue*Screen.width);
                    break;
                case AxisMapping.MappingType.MousePositionZ:
                    STBCrossPlatformInputManager.SetVirtualMousePositionZ(axisValue*Screen.width);
                    break;
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// OnDisable
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void OnDisable()
        {
            m_SteerAxis.Remove();
        }
    }
}
