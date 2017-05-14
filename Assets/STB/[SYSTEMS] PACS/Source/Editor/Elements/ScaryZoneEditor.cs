using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;

namespace STB.PACS
{
    [CustomEditor(typeof(ScaryZone))]
    [CanEditMultipleObjects]
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Class: ScaryZoneEditor
    /// # Editor handle for ScaryZone
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class ScaryZoneEditor : Editor
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// OnInspectorGUI
        /// # To do on inspector GUI
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////	
        public override void OnInspectorGUI()
        {
            ScaryZone actualScaryZone = (ScaryZone)target;

            EditorGUILayout.Separator();

            actualScaryZone.workingDistance = EditorGUILayout.Slider("Working distance ", actualScaryZone.workingDistance, 0, 10000);
            actualScaryZone.scaredTime = EditorGUILayout.Slider("Scared time ", actualScaryZone.scaredTime, 0, 180);
            actualScaryZone.safeZone = EditorGUILayout.ObjectField("Safe Zone: ", actualScaryZone.safeZone, typeof(Transform), true) as Transform;
            actualScaryZone.hideRendererIfExists = EditorGUILayout.Toggle("Hide renderer if exists: ", actualScaryZone.hideRendererIfExists);


            // on gui change
            if (GUI.changed)
            {
            }
        }
    }
}
