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
        // private
        IndividualPath lastSelectedIndividualPath = null;

        // private
        EditorBasicFunctions.cInsertMode PathPainterInsertMode = EditorBasicFunctions.cInsertMode.shiftAndMouse;
        List<string> PathPainterButtonsNameList = new List<string>();
        List<GameObject> PathPainterPrefabList = new List<GameObject>();
        Vector3 lastPathPainterHitPoint = Basics.BasicDefines.TOO_FAR_POSITION;

        // private -- buttons
        int numPathPainterButtonsPerLine = 2;
        int actualSelectedPathPainterButon = 0;


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// OnDestroyPathPainterMode
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void OnDestroyPathPainterMode()
        {
            PathPainterPrefabList.Clear();
            PathPainterButtonsNameList.Clear();
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// HandlePathPainterMode
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void HandlePathPainterMode()
        {
            // define extended prefab
            GetPathPainterPrefabOptions();


            // create new path
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (EditorBasicFunctions.GetEditorTextButton("CREATE NEW PATH", "Create a new path", position))
            {
                CreateNewPath();
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            EditorGUILayout.Separator();
            EditorGUILayout.Separator();

            // insert mode
            PathPainterInsertMode = (EditorBasicFunctions.cInsertMode)EditorGUILayout.EnumPopup(new GUIContent("Insert mode", "Select the way to insert things into the scene"), PathPainterInsertMode, new GUILayoutOption[] { GUILayout.Width(0.81f * position.width) });
            EditorGUILayout.Separator();
            EditorGUILayout.Separator();

            if (lastSelectedIndividualPath)
            {
                // show selected individual path
                EditorGUILayout.Separator();
                EditorGUILayout.Separator();

                EditorBasicFunctions.DrawLineSeparator();
                EditorBasicFunctions.DrawEditorBox("Selected individual path: " + lastSelectedIndividualPath.gameObject.name, Color.white);

                EditorGUILayout.Separator();
                EditorGUILayout.Separator();


                // prefab buttons
                Vector2 buttonsScale = ((0.98f / (float)numPathPainterButtonsPerLine) * position.width) * Vector2.one;

                //Debug.Log("buttonsScale A: " + buttonsScale);

                float pathPainterButtonsMaxScale = 110;

                if (buttonsScale.x >= pathPainterButtonsMaxScale)
                {
                    buttonsScale.x = pathPainterButtonsMaxScale;
                    buttonsScale.y = pathPainterButtonsMaxScale;
                }

                //Debug.Log("buttonsScale B: " + buttonsScale);
                //Debug.Log("pathPainterButtonsMaxScale: " + pathPainterButtonsMaxScale);

                int cont = 0;
                bool beginUnfinished = false;

                for (int i = 0; i < PathPainterButtonsNameList.Count; i++)
                {
                    if (cont == 0)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();
                        beginUnfinished = true;
                    }

                    if (EditorBasicFunctions.GetEditorButton("PathPainter/", PathPainterButtonsNameList[i], PathPainterButtonsNameList[i], buttonsScale, (i == actualSelectedPathPainterButon), true, false, true))
                    {
                        actualSelectedPathPainterButon = i;
                    }

                    cont++;

                    if (cont > numPathPainterButtonsPerLine - 1)
                    {
                        cont = 0;

                        GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();
                        beginUnfinished = false;
                    }
                }

                if (beginUnfinished)
                {
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                }

                EditorGUILayout.Separator();
                EditorGUILayout.Separator();

                EditorBasicFunctions.DrawLineSeparator();

                EditorGUILayout.Separator();
                EditorGUILayout.Separator();


                // close the actual path
                if (lastSelectedIndividualPath.GetComponent<PathLineDrawer>() && !lastSelectedIndividualPath.GetComponent<PathLineDrawer>().autocloseIfItsPossible)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();

                    if (EditorBasicFunctions.GetEditorTextButton("CLOSE PATH", "Close path", position))
                    {
                        CloseActualPath();
                    }

                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();

                    EditorGUILayout.Separator();
                    EditorGUILayout.Separator();
                }
            }
            else
            {
                EditorBasicFunctions.DrawEditorBox("You need to select a individual path!", Color.red);
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// UpdatePathPainterMode
        /// # To put objects in scene using mouse
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void UpdatePathPainterMode()
        {
            if ((GetEditorTimeDiff() > 0.1f) && EditorBasicFunctions.GetItsAValidClick(PathPainterInsertMode) && lastSelectedIndividualPath)
            {
                previousEditorTime = EditorApplication.timeSinceStartup;

                GameObject actualPrefab = null;

                if (actualSelectedPathPainterButon < PathPainterButtonsNameList.Count)
                {
                    actualPrefab = PathPainterPrefabList[actualSelectedPathPainterButon];
                }

                if (actualPrefab)
                {
                    Vector3 prefabPosition = Vector3.zero;
                    Vector3 prefabNormal = Vector3.up;

                    // throw a ray
                    Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit))
                    {
                        if (lastPathPainterHitPoint.ToString() == hit.point.ToString())
                        {
                            Debug.Log("NOTE: same point duplicate -> lastPathPainterHitPoint: " + lastPathPainterHitPoint);
                        }
                        else
                        {
                            //Debug.Log ("lastPathPainterHitPoint A: " + lastPathPainterHitPoint);

                            prefabPosition = hit.point;
                            prefabNormal = hit.normal;

                            //Debug.Log ("Put new: " + actualPrefab.name);
                            //Debug.Log ("New Object");
                            //Debug.Log ("Hit position: " + hit.point);
                            //Debug.Log ("Collider Name: " + hit.collider.name);

                            lastPathPainterHitPoint = prefabPosition;

                            // create prefab
                            CreateNewPathPrefab(actualPrefab, prefabPosition, prefabNormal);

                            // close actual path if possible
                            if (lastSelectedIndividualPath.GetComponent<PathLineDrawer>() && lastSelectedIndividualPath.GetComponent<PathLineDrawer>().autocloseIfItsPossible)
                            {
                                CloseActualPath();
                            }
                        }
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("WARNING", "You need to hit something to paint!", "OK");
                    }
                }
            }
        }
    }
}