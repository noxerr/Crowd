﻿using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;

namespace STB.PACS
{
    [CustomEditor(typeof(PedestrianIndividual))]
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Class: PedestrianIndividualEditor
    /// # Editor handle for PedestrianIndividual
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class PedestrianIndividualEditor : GenericIndividualEditor
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// OnInspectorGUIExtended -- OVERRIDED
        /// # To do on inspector GUI
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////	
        public override void OnInspectorGUIExtended()
        {
            //PedestrianIndividual actualPedestrianIndividual = (PedestrianIndividual)target;

            if (GUI.changed)
            {
            }
        }
    }
}