using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;

namespace STB.PACS
{
    [CustomEditor(typeof(IndividualPathPoint))]
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Class: IndividualPathPointEditor
    /// # Editor handle for IndividualPathPoint
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class IndividualPathPointEditor : Editor
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// OnInspectorGUI
        /// # To do on inspector GUI
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////	
        public override void OnInspectorGUI()
        {
            IndividualPathPoint actualIndividualPathPoint = (IndividualPathPoint)target;

            DrawIndividualPathPointOptions(actualIndividualPathPoint);

            if (GUI.changed)
            {
                SceneView.RepaintAll();
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// DrawIndividualPathPointOptions
        /// # 
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void DrawIndividualPathPointOptions(IndividualPathPoint actualIndividualPathPoint)
        {
            if (actualIndividualPathPoint)
            {
                EditorGUILayout.Separator();

                actualIndividualPathPoint.probability = (int)EditorGUILayout.Slider("Probability ", actualIndividualPathPoint.probability, 0, 100);
                actualIndividualPathPoint.showDebug = EditorGUILayout.Toggle("Show debug ", actualIndividualPathPoint.showDebug);

                EditorGUILayout.Separator();

                GUIStyle boldtext = EditorBasicFunctions.GetBoldTextGUIStyle();

                EditorGUILayout.LabelField("Information...", boldtext);

                EditorGUILayout.LabelField("· Type: " + actualIndividualPathPoint.type.ToString());

                if (actualIndividualPathPoint.LinkedNormalIndividualPoint) EditorGUILayout.LabelField("· Linked normal point: " + actualIndividualPathPoint.LinkedNormalIndividualPoint.gameObject.name);
                else EditorGUILayout.LabelField("· Linked normal point: NO");

                EditorGUILayout.LabelField("· Linked special points number: " + actualIndividualPathPoint.LinkedSpecialIndividualPathPointList.Count.ToString());

                for (int i = 0; i < actualIndividualPathPoint.LinkedSpecialIndividualPathPointList.Count; i++)
                {
                    EditorGUILayout.LabelField("    - Linked special point: " + actualIndividualPathPoint.LinkedSpecialIndividualPathPointList[i].gameObject.name);
                }
            }
        }
    }
}