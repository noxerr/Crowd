using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using System.Linq;

namespace STB.PACS
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Class: PACSWindow
    /// # Main window class to handle all
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public partial class PACSWindow : EditorWindow
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// OnDestroyPathSettingsMode
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void OnDestroyPathSettingsMode()
        {
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// HandlePathSettingsMode
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void HandlePathSettingsMode()
        {
            if (lastSelectedIndividualPath)
            {
                // individual
                EditorBasicFunctions.DrawLineSeparator();
                EditorBasicFunctions.DrawEditorBox("Individual handle", Color.white);

                EditorGUILayout.Separator();
                EditorGUILayout.Separator();

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                if (EditorBasicFunctions.GetEditorTextButton("ADD SELECTED INDIVIDUAL", "Add the selected individual prefabs to the path"))
                {
                    AddSelectedIndividualToActualPath();
                }

                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                EditorGUILayout.Separator();
                EditorGUILayout.Separator();

                // show actual path individual prefabs
                if (lastSelectedIndividualPath.IndividualPrefabList.Count > 0)
                {
                    for (int i = 0; i < lastSelectedIndividualPath.IndividualPrefabList.Count; i++)
                    {
                        if (lastSelectedIndividualPath.IndividualPrefabList[i])
                        {
                            bool finish = false;

                            GUILayout.BeginHorizontal();
                            GUILayout.FlexibleSpace();

                            if (EditorBasicFunctions.GetEditorTextButton(lastSelectedIndividualPath.IndividualPrefabList[i].gameObject.name, lastSelectedIndividualPath.IndividualPrefabList[i].gameObject.name, position))
                            {
                                Selection.activeGameObject = lastSelectedIndividualPath.IndividualPrefabList[i].gameObject;
                            }

                            if (EditorBasicFunctions.GetEditorTextButton("<<", "Remove prefab: " + lastSelectedIndividualPath.IndividualPrefabList[i].gameObject.name, new Rect(0, 0, 0.1f * position.width, position.height)))
                            {
                                lastSelectedIndividualPath.IndividualPrefabList.RemoveAt(i);
                                finish = true;
                            }

                            GUILayout.FlexibleSpace();
                            GUILayout.EndHorizontal();

                            if (finish) break;
                        }
                        else
                        {
                            lastSelectedIndividualPath.IndividualPrefabList.RemoveAt(i);
                        }
                    }
                }
                else
                {
                    EditorBasicFunctions.DrawEditorBox("There is no individual prefabs in this path!", Color.red);
                }

                // specific options
                EditorGUILayout.Separator();
                EditorGUILayout.Separator();

                EditorBasicFunctions.DrawLineSeparator();
                EditorBasicFunctions.DrawEditorBox("Specific options", Color.white);

                EditorGUILayout.Separator();
                EditorGUILayout.Separator();

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                if (EditorBasicFunctions.GetEditorTextButton("CALCULATE CENTROID", "Calculate path's centroid using its child points"))
                {
                    CalculateCentroid();
                }

                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();


                // show selected individual path
                EditorGUILayout.Separator();
                EditorGUILayout.Separator();

                EditorBasicFunctions.DrawLineSeparator();
                EditorBasicFunctions.DrawEditorBox("Selected individual path: " + lastSelectedIndividualPath.gameObject.name, Color.white);

                EditorGUILayout.Separator();

                IndividualPathEditor.DrawIndividualPathEditorOptions(lastSelectedIndividualPath.GetComponent<PathLineDrawer>(), lastSelectedIndividualPath.GetComponent<IndividualPath>());
            }
            else
            {
                EditorBasicFunctions.DrawEditorBox("You need to select a individual path!", Color.red);
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// UpdatePathSettingsMode
        /// # To put objects in scene using mouse
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void UpdatePathSettingsMode()
        {
        }
    }
}