using System;
using UnityEngine;
using STB.SpecialInput.PlatformSpecific;

namespace STB.SpecialInput
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Class: STBVirtualButton
    /// # a controller gameobject (eg. a virtual GUI button) should call the
    /// # 'pressed' function of this class. Other objects can then read the
    /// # Get/Down/Up state of this button.
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class STBVirtualButton
    {
        // public
        public string name { get; private set; }
        public bool matchWithInputManager { get; private set; }

        // private
        int m_LastPressedFrame = -5;
        int m_ReleasedFrame = -5;
        bool m_Pressed;


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// STBVirtualButton
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public STBVirtualButton(string name) : this(name, true)
        {
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// STBVirtualButton
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public STBVirtualButton(string name, bool matchToInputSettings)
        {
            this.name = name;
            matchWithInputManager = matchToInputSettings;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Pressed
        /// # A controller gameobject should call this function when the button is pressed down
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Pressed()
        {
            if (m_Pressed)
            {
                return;
            }
            m_Pressed = true;
            m_LastPressedFrame = Time.frameCount;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Released
        /// # A controller gameobject should call this function when the button is released
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Released()
        {
            m_Pressed = false;
            m_ReleasedFrame = Time.frameCount;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Remove
        /// # the controller gameobject should call Remove when the button is destroyed or disabled
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Remove()
        {
            STBCrossPlatformInputManager.UnRegisterSTBVirtualButton(name);
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// GetButton
        /// # these are the states of the button which can be read via the cross platform input system
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool GetButton
        {
            get { return m_Pressed; }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// GetButtonDown
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool GetButtonDown
        {
            get
            {
                return m_LastPressedFrame - Time.frameCount == -1;
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// GetButtonUp
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool GetButtonUp
        {
            get
            {
                return (m_ReleasedFrame == Time.frameCount - 1);
            }
        }
    }
}