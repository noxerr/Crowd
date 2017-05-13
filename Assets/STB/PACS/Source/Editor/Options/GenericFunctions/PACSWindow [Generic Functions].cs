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
        /// CreateNewPathPrefab
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void CreateNewPathPrefab(GameObject actualPrefab, Vector3 prefabPosition, Vector3 prefabNormal)
        {
            GameObject actualObject = Instantiate(actualPrefab);

            Undo.RegisterCreatedObjectUndo(actualObject, "Create " + actualPrefab.name);

            actualObject.transform.position = prefabPosition + 0.5f * actualPrefab.transform.localScale.y * prefabNormal;

            actualObject.transform.localScale = actualObject.transform.localScale;
            actualObject.transform.rotation = Quaternion.FromToRotation(Vector3.up, prefabNormal);
            actualObject.transform.Rotate(actualPrefab.transform.rotation.eulerAngles);

            actualObject.transform.parent = lastSelectedIndividualPath.transform;

            // force selection
            actualObjectToForceSelect = actualObject;

            // do things dependings of prefab type
            if (actualObject.GetComponent<IndividualPathPoint>()) SetNewIndividualPathPoint(actualObject.GetComponent<IndividualPathPoint>());
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// SetIndividualPathPoint
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void SetNewIndividualPathPoint(IndividualPathPoint cpp)
        {
            string validName = "NOT_VALID_NAME_AVAILIABLE_FOR_INDIVIDUAL_PATH_POINT";
            int counter = 1;

            bool findAValidName = true;

            while (findAValidName)
            {
                findAValidName = false;

                if (cpp.type == IndividualPathPoint.cType.normal) validName = BasicDefines.NORMAL_INDIVIDUAL_PATH_POINT_PREFIX + Basics.BasicFunctions.GetStringNormalizedCounterForMiles(counter);
                else if (cpp.type == IndividualPathPoint.cType.special) validName = BasicDefines.SPECIAL_INDIVIDUAL_PATH_POINT_PREFIX + Basics.BasicFunctions.GetStringNormalizedCounterForMiles(counter);

                foreach (Transform cp in lastSelectedIndividualPath.transform)
                {
                    if (validName == cp.name) findAValidName = true;
                }

                counter++;
            }

            cpp.name = validName;
            cpp.index = counter;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// CloseActualPath
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void CloseActualPath()
        {
            //Debug.Log("Close actual path");

            List<IndividualPathPoint> individualPathPointList = new List<IndividualPathPoint>();

            foreach (Transform cpp in lastSelectedIndividualPath.transform)
            {
                if (cpp.GetComponent<IndividualPathPoint>() && (cpp.GetComponent<IndividualPathPoint>().type == IndividualPathPoint.cType.normal))
                {
                    individualPathPointList.Add(cpp.GetComponent<IndividualPathPoint>());
                }
            }

            //Debug.Log("individualPathPointList count: " + individualPathPointList.Count);

            if (individualPathPointList.Count > 1)
            {
                individualPathPointList = individualPathPointList.OrderBy(x => x.name).ToList();

                for (int i = 0; i < individualPathPointList.Count - 1; i++)
                {
                    individualPathPointList[i].LinkedNormalIndividualPoint = individualPathPointList[i + 1];
                    individualPathPointList[i + 1].LinkedNormalIndividualPointReverse = individualPathPointList[i];
                }

                individualPathPointList[individualPathPointList.Count - 1].LinkedNormalIndividualPoint = individualPathPointList[0];
                individualPathPointList[0].LinkedNormalIndividualPointReverse = individualPathPointList[individualPathPointList.Count - 1];
            }

            lastSelectedIndividualPath.GetComponent<PathLineDrawer>().DrawGizmos();
            HandleUtility.Repaint();
            SceneView.RepaintAll();
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// AddSelectedIndividualToActualPath
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void AddSelectedIndividualToActualPath()
        {
            if (Selection.gameObjects.Length > 0)
            {
                int numberOfAddedGenericIndividual = 0;

                foreach (GameObject go in Selection.gameObjects)
                {
                    if (go.GetComponent<GenericIndividual>())
                    {
                        lastSelectedIndividualPath.IndividualPrefabList.Add(go.GetComponent<GenericIndividual>());
                        numberOfAddedGenericIndividual++;
                    }
                    else
                    {
                        Debug.Log("WARNING: " + go.gameObject.name + " is not a individual prefab");
                    }
                }

                if (numberOfAddedGenericIndividual <= 0) EditorUtility.DisplayDialog("WARNING", "There is no valid individual prefab selected", "OK");
            }
            else
            {
                EditorUtility.DisplayDialog("WARNING", "There is nothing selected", "OK");
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// CreateNewPath
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void CreateNewPath()
        {
            IndividualPath[] actualPathList = GameObject.FindObjectsOfType(typeof(IndividualPath)) as IndividualPath[];

            // write the actual path list for debug
            //foreach (IndividualPath cp in actualPathList)
            //{
            //Debug.Log(cp.name);
            //}

            string validName = "NOT_VALID_NAME_AVAILIABLE_FOR_INDIVIDUAL_PATH";
            int counter = 1;

            bool findAValidName = true;

            while (findAValidName)
            {
                findAValidName = false;

                validName = BasicDefines.INDIVIDUAL_PATH_PREFIX + Basics.BasicFunctions.GetStringNormalizedCounterForMiles(counter);

                foreach (IndividualPath cp in actualPathList)
                {
                    if (validName == cp.name) findAValidName = true;
                }

                counter++;
            }


            IndividualPath newIndividualPath = Basics.BasicFunctions.CreateContainerIfNotExists(validName).AddComponent<IndividualPath>();
            newIndividualPath.name = validName;

            Undo.RegisterFullObjectHierarchyUndo(newIndividualPath.gameObject, "Created Individual Path: " + validName);

            Selection.activeGameObject = newIndividualPath.gameObject;
            lastSelectedIndividualPath = newIndividualPath;


            // create individual handler if not exists in scene
            CreateIndividualHandlerIfNotExistsInScene();
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// CreateIndividualHandlerIfNotExistsInScene
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void CreateIndividualHandlerIfNotExistsInScene()
        {
            IndividualHandler ch = GameObject.FindObjectOfType<IndividualHandler>();

            if (!ch)
            {
                GameObject newCh = Basics.BasicFunctions.CreateContainerIfNotExists(BasicDefines.INDIVIDUAL_HANDLER_NAME);
                newCh.AddComponent<IndividualHandler>();
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// CalculateCentroid
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void CalculateCentroid()
        {
            if (lastSelectedIndividualPath)
            {
                lastSelectedIndividualPath.CalculateCentroid();
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// GetPathPainterPrefabOptions
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void GetPathPainterPrefabOptions()
        {
            if (PathPainterPrefabList.Count <= 0)
            {
                PathPainterPrefabList = EditorBasicFunctions.GetPrefabListFromDirectory(BasicDefines.EDITOR_PATH_PAINTER_OPTIONS_PREFABS_PATH);

                // load buttons
                PathPainterButtonsNameList.Clear();

                for (int i = 0; i < PathPainterPrefabList.Count; i++)
                {
                    //Debug.Log ("prefab: " + PathPainterPrefabList [i].name);

                    PathPainterButtonsNameList.Add(PathPainterPrefabList[i].name);
                }
            }

            //Debug.Log ("PathPainterPrefabList count: " + PathPainterPrefabList.Count);
        }
    }
}