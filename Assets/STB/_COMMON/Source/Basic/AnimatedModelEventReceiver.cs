﻿using UnityEngine;
using System.Collections;

namespace STB.Basics
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Class: AnimatedModelEventReceiver
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class AnimatedModelEventReceiver : MonoBehaviour
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// ExecuteEvent
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void ExecuteEvent(float theValue)
        {
            //Debug.Log("ExecuteEvent is called with a value of " + theValue);
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// ExecuteEventNoUpperTransition
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void ExecuteEventNoUpperTransition(float theValue)
        {
            //Debug.Log("ExecuteEventNoUpperTransition is called with a value of " + theValue);
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// ItemUsed
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void ItemUsed(float theValue)
        {
            //Debug.Log("ItemUsed is called with a value of " + theValue);
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// TurnoffCollider
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void TurnoffCollider(float theValue)
        {
            //Debug.Log("TurnoffCollider is called with a value of " + theValue);
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// footStep
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void footStep(float theValue)
        {
            //Debug.Log("footStep is called with a value of " + theValue);
        }
    }
}
