/*using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;

namespace STB.PACS
{
    [CustomEditor(typeof(IndividualHandler))]
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Class: GenericIndividualEditor
    /// # Editor handle for GenericIndividual
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class IndividualHandlerEditor : Editor
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// OnInspectorGUI
        /// # To do on inspector GUI
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////	
        public override void OnInspectorGUI()
        {
            IndividualHandler actualIndividualHandler = (IndividualHandler)target;

            EditorGUILayout.Separator();

            actualIndividualHandler.workingDistance = EditorGUILayout.IntSlider("Working distance ", actualIndividualHandler.workingDistance, 0, 100000);

            actualIndividualHandler.maxPool = EditorGUILayout.IntSlider("Max pool ", actualIndividualHandler.maxPool, 1, 90);

            actualIndividualHandler.noNavMeshHandle = EditorGUILayout.Toggle("No NavMesh handle: ", actualIndividualHandler.noNavMeshHandle);

            if (actualIndividualHandler.noNavMeshHandle)
            {
                actualIndividualHandler.fixedYForIndividuals = EditorGUILayout.Slider("Fixed Y for individuals ", actualIndividualHandler.fixedYForIndividuals, -10, 10);
            }

            EditorGUILayout.Separator();


            // for extended
            OnInspectorGUIExtended();

            // on gui change
            if (GUI.changed)
            {
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// OnInspectorGUIExtended -- VIRTUAL
        /// # To do on inspector GUI
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////	
        public virtual void OnInspectorGUIExtended()
        {
            //PathLineDrawer actualPathLineDrawer = (PathLineDrawer)target;

            if (GUI.changed)
            {
            }
        }
    }
}*/