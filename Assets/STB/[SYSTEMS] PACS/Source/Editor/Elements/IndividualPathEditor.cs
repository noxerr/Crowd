using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;

namespace STB.PACS
{
    [CustomEditor(typeof(IndividualPath))]
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Class: IndividualPathEditor
    /// # Editor handle for IndividualPath
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class IndividualPathEditor : Editor
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// OnInspectorGUI
        /// # To do on inspector GUI
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////	
        public override void OnInspectorGUI()
        {
            IndividualPath actualIndividualPath = (IndividualPath)target;

            PathLineDrawer actualPathLineDrawer = actualIndividualPath.GetComponent<PathLineDrawer>();

            DrawIndividualPathEditorOptions(actualPathLineDrawer, actualIndividualPath);

            if (GUI.changed)
            {
                SceneView.RepaintAll();
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// DrawIndividualPathEditorOptions
        /// # 
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void DrawIndividualPathEditorOptions(PathLineDrawer actualPathLineDrawer, IndividualPath actualIndividualPath)
        {
            if (actualPathLineDrawer && actualIndividualPath)
            {
                EditorGUILayout.Separator();

                actualIndividualPath.density = EditorGUILayout.IntSlider(new GUIContent("Density: ", "Set the paths density"), actualIndividualPath.density, 1, 400);

                actualIndividualPath.autoHalfWayPoints = EditorGUILayout.Toggle(new GUIContent("Auto half-way points: ", "Auto-create half-way points to match density if it's possible"), actualIndividualPath.autoHalfWayPoints);

                actualIndividualPath.recalculatePathPivotUsingChildPoints = EditorGUILayout.Toggle(new GUIContent("Recalculate centroid: ", "Recalculate path's pivot point using its child points. Just if is not already calculated"), actualIndividualPath.recalculatePathPivotUsingChildPoints);

                actualIndividualPath.pointsRandomVariation = EditorGUILayout.Slider(new GUIContent("Points variation range: ", "Set the points variation range to generate a random offset for each point and each pedestrian"), actualIndividualPath.pointsRandomVariation, 0, 4);

                actualPathLineDrawer.individualEdition = EditorGUILayout.Toggle(new GUIContent("Individual edition: ", "Set on/off the path's individual edition instead of using general settings"), actualPathLineDrawer.individualEdition);

                actualPathLineDrawer.autocloseIfItsPossible = EditorGUILayout.Toggle(new GUIContent("Autoclose if possible: ", "AUtoclose the path if it's possible"), actualPathLineDrawer.autocloseIfItsPossible);


                if (actualPathLineDrawer.individualEdition)
                {
                    try
                    {
                        EditorGUILayout.Separator();

                        actualPathLineDrawer.normalPathPointsColor = EditorGUILayout.ColorField("Normal points color", actualPathLineDrawer.normalPathPointsColor);
                        actualPathLineDrawer.normalPathColor = EditorGUILayout.ColorField("Normal path color", actualPathLineDrawer.normalPathColor);
                        actualPathLineDrawer.normalArrowsColor = EditorGUILayout.ColorField("Normal arrows color", actualPathLineDrawer.normalArrowsColor);

                        EditorGUILayout.Separator();

                        actualPathLineDrawer.specialPathPointsColor = EditorGUILayout.ColorField("Special points color", actualPathLineDrawer.specialPathPointsColor);
                        actualPathLineDrawer.specialPathColor = EditorGUILayout.ColorField("Special path color", actualPathLineDrawer.specialPathColor);
                        actualPathLineDrawer.specialArrowsColor = EditorGUILayout.ColorField("Special arrows color", actualPathLineDrawer.specialArrowsColor);

                        EditorGUILayout.Separator();

                        actualPathLineDrawer.borderColor = EditorGUILayout.ColorField("Border color", actualPathLineDrawer.borderColor);

                        EditorGUILayout.Separator();

                        actualPathLineDrawer.pointSize = EditorGUILayout.Slider("Point size", actualPathLineDrawer.pointSize, 0.1f, 9);
                    }
                    catch
                    { }
                }
            }
        }
    }
}