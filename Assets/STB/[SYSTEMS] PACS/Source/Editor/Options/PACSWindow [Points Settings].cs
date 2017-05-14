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
        // public
        public bool showNormalPoints = false;
        public bool showSpecialPoints = false;

        // public
        public bool specialPointOperationsAllowed = false;


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// OnDestroyPointsSettingsMode
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void OnDestroyPointsSettingsMode()
        {
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// UnlinkPoints
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void UnlinkPoints()
        {
            int selectedPointNumber = 0;

            foreach (GameObject go in Selection.gameObjects)
            {
                if (go.GetComponent<IndividualPathPoint>()) selectedPointNumber++;
            }

            if (selectedPointNumber > 0)
            {
                foreach (GameObject go in Selection.gameObjects)
                {
                    IndividualPathPoint actualPathPoint = go.GetComponent<IndividualPathPoint>();

                    if (actualPathPoint)
                    {
                        //Debug.Log("actualPathPoint: " + actualPathPoint.name);

                        if (actualPathPoint.LinkedNormalIndividualPointReverse) actualPathPoint.LinkedNormalIndividualPointReverse.LinkedNormalIndividualPoint = null;
                        actualPathPoint.LinkedNormalIndividualPoint = null;

                        for (int i = 0; i < actualPathPoint.LinkedSpecialIndividualPathPointList.Count; i++)
                        {
                            actualPathPoint.LinkedSpecialIndividualPathPointList[i].LinkedSpecialIndividualPathPointList.Remove(actualPathPoint);
                        }

                        actualPathPoint.LinkedSpecialIndividualPathPointList.Clear();
                    }
                }
            }
            else
            {
                EditorUtility.DisplayDialog("WARNING", "No point selected!", "OK");
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// AutocloseThePathForGivenAPoint
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void AutocloseThePathForGivenAPoint(IndividualPathPoint cpp)
        {
            AutocloseThePath(BasicFunctions.GetParentIndividualPath(cpp.transform));
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// CloseActualPath
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void AutocloseThePath(IndividualPath cp)
        {
            if (cp && cp.GetComponent<PathLineDrawer>() && cp.GetComponent<PathLineDrawer>().autocloseIfItsPossible)
            {
                lastSelectedIndividualPath = cp;

                CloseActualPath();
            }

            if (cp && cp.recalculatePathPivotUsingChildPoints)
            {
                cp.CalculateCentroid();
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// LinkPoints
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void LinkPoints()
        {
            int selectedPointNumber = 0;

            foreach (GameObject go in Selection.gameObjects)
            {
                if (go.GetComponent<IndividualPathPoint>()) selectedPointNumber++;
            }

            if (selectedPointNumber > 1)
            {
                foreach (GameObject go in Selection.gameObjects)
                {
                    IndividualPathPoint actualPathPoint = go.GetComponent<IndividualPathPoint>();

                    if (actualPathPoint)
                    {
                        //Debug.Log("actualPathPoint: " + actualPathPoint.name);

                        actualPathPoint.LinkedSpecialIndividualPathPointList.Clear();

                        foreach (GameObject goSec in Selection.gameObjects)
                        {
                            IndividualPathPoint actualPathPointSec = goSec.GetComponent<IndividualPathPoint>();

                            //Debug.Log("actualPathPointSec: " + actualPathPointSec.name);

                            if (actualPathPointSec && !actualPathPoint.Equals(actualPathPointSec))
                            {
                                actualPathPoint.LinkedSpecialIndividualPathPointList.Add(actualPathPointSec);
                            }
                        }

                        // autoclose the path if its possible
                        AutocloseThePathForGivenAPoint(actualPathPoint);
                    }
                }
            }
            else
            {
                EditorUtility.DisplayDialog("WARNING", "You need at least 2 points selected!", "OK");
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// ShowPoints
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void ShowPoints(IndividualPathPoint.cType pointType)
        {
            try
            {
                int selectedPointNumber = 0;

                foreach (Transform go in lastSelectedIndividualPath.transform)
                {
                    IndividualPathPoint actualPathPoint = go.GetComponent<IndividualPathPoint>();

                    if (actualPathPoint && (actualPathPoint.type == pointType))
                    {
                        switch (pointType)
                        {
                            case IndividualPathPoint.cType.normal:
                                {
                                    selectedPointNumber++;
                                }
                                break;

                            case IndividualPathPoint.cType.special:
                                {
                                    selectedPointNumber++;
                                }
                                break;
                        }
                    }
                }

                if (selectedPointNumber > 0)
                {
                    switch (pointType)
                    {
                        case IndividualPathPoint.cType.normal:
                            {
                                showNormalPoints = EditorGUILayout.Foldout(showNormalPoints, "Show normal points");
                            }
                            break;

                        case IndividualPathPoint.cType.special:
                            {
                                showSpecialPoints = EditorGUILayout.Foldout(showSpecialPoints, "Show special points");
                            }
                            break;
                    }

                    foreach (Transform go in lastSelectedIndividualPath.transform)
                    {
                        IndividualPathPoint actualPathPoint = go.GetComponent<IndividualPathPoint>();

                        bool showThisPoint = false;

                        if (actualPathPoint && (actualPathPoint.type == pointType))
                        {
                            switch (pointType)
                            {
                                case IndividualPathPoint.cType.normal:
                                    {
                                        if (showNormalPoints) showThisPoint = true;
                                    }
                                    break;

                                case IndividualPathPoint.cType.special:
                                    {
                                        if (showSpecialPoints) showThisPoint = true;
                                    }
                                    break;
                            }
                        }

                        if (showThisPoint)
                        {
                            GUILayout.BeginHorizontal();
                            GUILayout.FlexibleSpace();

                            //Debug.Log("actualPathPoint: " + actualPathPoint.name);

                            if (specialPointOperationsAllowed && EditorBasicFunctions.GetEditorTextButton(">>", "Remove the point"))
                            {
                                RemovePointFromTheList(actualPathPoint);
                            }

                            if (EditorBasicFunctions.GetEditorTextButton(actualPathPoint.gameObject.name, "Select the point"))
                            {
                                Selection.activeGameObject = actualPathPoint.gameObject;
                            }

                            if (specialPointOperationsAllowed && EditorBasicFunctions.GetEditorTextButton("UP", "Remove the point"))
                            {
                                SetPointUpInTheList(actualPathPoint);
                            }

                            if (specialPointOperationsAllowed && EditorBasicFunctions.GetEditorTextButton("DOWN", "Remove the point"))
                            {
                                SetPointDownInTheList(actualPathPoint);
                            }

                            GUILayout.FlexibleSpace();
                            GUILayout.EndHorizontal();
                        }
                    }
                }
                else
                {
                    switch (pointType)
                    {
                        case IndividualPathPoint.cType.normal:
                            {
                                EditorGUILayout.LabelField("· There are no normal points in this path");
                            }
                            break;

                        case IndividualPathPoint.cType.special:
                            {
                                EditorGUILayout.Separator();
                                EditorGUILayout.LabelField("· There are no special points in this path");
                            }
                            break;
                    }
                }
            }
            catch (UnityException e)
            {
                Debug.Log("Warning: " + e);
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// RemovePointFromTheList
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void RemovePointFromTheList(IndividualPathPoint cpp)
        {
            switch (cpp.type)
            {
                case IndividualPathPoint.cType.normal:
                    {
                        List<IndividualPathPoint> NormalIndividualPathPointList = new List<IndividualPathPoint>();

                        IndividualPath actualPointPath = BasicFunctions.GetParentIndividualPath(cpp.transform);

                        foreach (Transform c in actualPointPath.transform)
                        {
                            if (c.GetComponent<IndividualPathPoint>())
                            {
                                if (c.GetComponent<IndividualPathPoint>().type == IndividualPathPoint.cType.normal) NormalIndividualPathPointList.Add(c.GetComponent<IndividualPathPoint>());
                            }
                        }

                        for (int i = 0; i < cpp.LinkedSpecialIndividualPathPointList.Count; i++)
                        {
                            cpp.LinkedSpecialIndividualPathPointList.Remove(cpp);
                        }

                        NormalIndividualPathPointList.Remove(cpp);
                        DestroyImmediate(cpp.gameObject);

                        NormalIndividualPathPointList = NormalIndividualPathPointList.OrderBy(x => x.name).ToList();

                        for (int i = 0; i < NormalIndividualPathPointList.Count; i++)
                        {
                            NormalIndividualPathPointList[i].gameObject.name = BasicDefines.NORMAL_INDIVIDUAL_PATH_POINT_PREFIX + Basics.BasicFunctions.GetStringNormalizedCounterForMiles(i + 1);
                        }

                        // autoclose the path if its possible
                        AutocloseThePath(actualPointPath);
                    }
                    break;

                case IndividualPathPoint.cType.special:
                    {
                        List<IndividualPathPoint> SpecialIndividualPathPointList = new List<IndividualPathPoint>();

                        IndividualPath actualPointPath = BasicFunctions.GetParentIndividualPath(cpp.transform);

                        foreach (Transform c in actualPointPath.transform)
                        {
                            if (c.GetComponent<IndividualPathPoint>())
                            {
                                if (c.GetComponent<IndividualPathPoint>().type == IndividualPathPoint.cType.special) SpecialIndividualPathPointList.Add(c.GetComponent<IndividualPathPoint>());
                            }
                        }

                        for (int i = 0; i < cpp.LinkedSpecialIndividualPathPointList.Count; i++)
                        {
                            cpp.LinkedSpecialIndividualPathPointList.Remove(cpp);
                        }

                        SpecialIndividualPathPointList.Remove(cpp);
                        DestroyImmediate(cpp.gameObject);

                        for (int i = 0; i < SpecialIndividualPathPointList.Count; i++)
                        {
                            SpecialIndividualPathPointList[i].gameObject.name = BasicDefines.SPECIAL_INDIVIDUAL_PATH_POINT_PREFIX + Basics.BasicFunctions.GetStringNormalizedCounterForMiles(i + 1);
                        }

                        // autoclose the path if its possible
                        AutocloseThePath(actualPointPath);
                    }
                    break;
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// CreateHalfWayPoint
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void CreateHalfWayPoint()
        {
            int numberOfSelectedPoints = 0;

            foreach (GameObject go in Selection.gameObjects)
            {
                //Debug.Log("go name" + go.name);

                IndividualPathPoint actualCpp = go.GetComponent<IndividualPathPoint>();

                if (actualCpp && (actualCpp.type == IndividualPathPoint.cType.normal))
                {
                    numberOfSelectedPoints++;

                    List<IndividualPathPoint> NormalIndividualPathPointList = new List<IndividualPathPoint>();

                    IndividualPath actualPointPath = BasicFunctions.GetParentIndividualPath(actualCpp.transform);

                    foreach (Transform c in actualPointPath.transform)
                    {
                        if (c.GetComponent<IndividualPathPoint>())
                        {
                            if (c.GetComponent<IndividualPathPoint>().type == IndividualPathPoint.cType.normal) NormalIndividualPathPointList.Add(c.GetComponent<IndividualPathPoint>());
                        }
                    }

                    NormalIndividualPathPointList = NormalIndividualPathPointList.OrderBy(x => x.name).ToList();

                    int actuaCppIndex = NormalIndividualPathPointList.IndexOf(actualCpp);

                    IndividualPathPoint newCpp = Instantiate(actualCpp);
                    newCpp.LinkedNormalIndividualPoint = null;
                    newCpp.LinkedNormalIndividualPointReverse = null;
                    newCpp.LinkedSpecialIndividualPathPointList.Clear();
                    newCpp.transform.parent = actualPointPath.transform;

                    Selection.activeGameObject = newCpp.gameObject;

                    if (actualCpp.LinkedNormalIndividualPoint)
                    {
                        newCpp.transform.position = (actualCpp.transform.position + actualCpp.LinkedNormalIndividualPoint.transform.position) / 2;
                    }
                    else newCpp.transform.position = actualCpp.transform.position + 4 * actualCpp.transform.forward;

                    NormalIndividualPathPointList.Insert(actuaCppIndex + 1, newCpp);

                    for (int i = 0; i < NormalIndividualPathPointList.Count; i++)
                    {
                        NormalIndividualPathPointList[i].gameObject.name = BasicDefines.NORMAL_INDIVIDUAL_PATH_POINT_PREFIX + Basics.BasicFunctions.GetStringNormalizedCounterForMiles(i + 1);
                    }

                    // autoclose the path if its possible
                    AutocloseThePath(actualPointPath);
                }
            }

            if (numberOfSelectedPoints <= 0)
            {
                EditorUtility.DisplayDialog("WARNING", "You need at least one normal point selected!", "OK");
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// SetPointUpInTheList
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void SetPointUpInTheList(IndividualPathPoint cpp)
        {
            switch (cpp.type)
            {
                case IndividualPathPoint.cType.normal:
                    {
                    }
                    break;

                case IndividualPathPoint.cType.special:
                    {
                    }
                    break;
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// SetPointDownInTheList
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void SetPointDownInTheList(IndividualPathPoint cpp)
        {
            switch (cpp.type)
            {
                case IndividualPathPoint.cType.normal:
                    {
                    }
                    break;

                case IndividualPathPoint.cType.special:
                    {
                    }
                    break;
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// RemovePoints
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void RemovePoints()
        {
            int numberOfSelectedPoints = 0;

            foreach (GameObject go in Selection.gameObjects)
            {
                //Debug.Log("go name" + go.name);

                IndividualPathPoint actualCpp = go.GetComponent<IndividualPathPoint>();

                if (actualCpp)
                {
                    numberOfSelectedPoints++;

                    RemovePointFromTheList(actualCpp);
                }
            }

            if (numberOfSelectedPoints <= 0) EditorUtility.DisplayDialog("WARNING", "No point selected!", "OK");
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// HandlePointsSettingsMode
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void HandlePointsSettingsMode()
        {
            // create half-way point
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (EditorBasicFunctions.GetEditorTextButton("CREATE HALF-WAY POINT", "Create a half-way point between a selected point and it's next"))
            {
                CreateHalfWayPoint();
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();


            EditorGUILayout.Separator();
            EditorGUILayout.Separator();

            // remove points
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (EditorBasicFunctions.GetEditorTextButton("REMOVE SELECTED POINTS", "Remove selected points"))
            {
                RemovePoints();
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            EditorGUILayout.Separator();
            EditorGUILayout.Separator();

            // link points
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (EditorBasicFunctions.GetEditorTextButton("LINK SELECTED POINTS", "Link selected points"))
            {
                LinkPoints();
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            EditorGUILayout.Separator();
            EditorGUILayout.Separator();

            // unlink points
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (EditorBasicFunctions.GetEditorTextButton("UNLINK SELECTED POINTS", "Unlink selected points"))
            {
                UnlinkPoints();
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            EditorGUILayout.Separator();
            EditorGUILayout.Separator();

            EditorBasicFunctions.DrawLineSeparator();

            if (lastSelectedIndividualPath)
            {
                EditorBasicFunctions.DrawEditorBox("Selected individual path: " + lastSelectedIndividualPath.gameObject.name, Color.white);

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

                ShowPoints(IndividualPathPoint.cType.normal);
                ShowPoints(IndividualPathPoint.cType.special);
            }
            else EditorBasicFunctions.DrawEditorBox("You need to select a individual path!", Color.red);
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// UpdatePointsSettingsMode
        /// # To put objects in scene using mouse
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void UpdatePointsSettingsMode()
        {
        }
    }
}
