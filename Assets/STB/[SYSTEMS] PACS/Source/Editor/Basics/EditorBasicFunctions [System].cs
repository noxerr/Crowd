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
    /// Class: EditorBasicFunctions
    /// # Compilation on functions used by editor
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public partial class EditorBasicFunctions : MonoBehaviour
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// DrawIndividualHandlerOptions
        /// # 
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void DrawIndividualHandlerOptions(IndividualHandler actualIndividualHandler)
        {
            if (!actualIndividualHandler)
            {
                EditorGUILayout.Separator();
                EditorGUILayout.Separator();

                EditorGUILayout.LabelField("There is no Individual Handler in the scene!", EditorBasicFunctions.GetBoldTextGUIStyle());

                EditorGUILayout.Separator();
                EditorGUILayout.Separator();

                return;
            }

            EditorGUILayout.Separator();

            actualIndividualHandler.workingDistance = EditorGUILayout.IntSlider(new GUIContent("Working distance ", "Distance where individuals inside each path can be viewed from the acive camera. More working distance, less performance"), (int)actualIndividualHandler.workingDistance, 0, 100000);
            actualIndividualHandler.maxPool = EditorGUILayout.IntSlider(new GUIContent("Max Pool ", "Max preloaded individuals of each type in each path. If there is soo much density maybe more pool will be needed to see all the individuals"), actualIndividualHandler.maxPool, 1, 400);
            actualIndividualHandler.maxDensity = EditorGUILayout.IntSlider(new GUIContent("Max density ", "Density limit for all the paths in the scene"), (int)actualIndividualHandler.maxDensity, 0, 1000);

            EditorGUILayout.Separator();
            EditorGUILayout.Separator();

            actualIndividualHandler.individualsSpeedMultiplier = EditorGUILayout.Slider(new GUIContent("Speed multiplier ", "Multiply all the pedestrians speed globally"), actualIndividualHandler.individualsSpeedMultiplier, 0.01f, 100);
            actualIndividualHandler.individualsScaleMultiplier = EditorGUILayout.Slider(new GUIContent("Scale multiplier ", "Multiply all the pedestrians scale globally"), actualIndividualHandler.individualsScaleMultiplier, 0.01f, 100);
            actualIndividualHandler.noNavMeshHandle = EditorGUILayout.Toggle(new GUIContent("No NavMesh handle: ", "If the scene has no Navigation Mesh, for example for pahs over not static objcets"), actualIndividualHandler.noNavMeshHandle);

            if (actualIndividualHandler.noNavMeshHandle)
            {
                actualIndividualHandler.fixedYForIndividuals = EditorGUILayout.Slider(new GUIContent("Fixed Y for individuals ", "An special offset for the height of each pedestrian in the scene"), actualIndividualHandler.fixedYForIndividuals, -10, 10);
            }

            EditorGUILayout.Separator();
            EditorGUILayout.Separator();

            actualIndividualHandler.createScaryZonesOnIndividualsHit = EditorGUILayout.Toggle(new GUIContent("Create scary zones: ", "If an individual is hit a scary zone will appear in hit position"), actualIndividualHandler.createScaryZonesOnIndividualsHit);
            actualIndividualHandler.scaryTime = EditorGUILayout.Slider(new GUIContent("Scary time ", "The time the scary zone will live after its creation"), actualIndividualHandler.scaryTime, 0, 300);
            actualIndividualHandler.scaryRadious = EditorGUILayout.Slider(new GUIContent("Scary radius ", "The scary zone radious"), actualIndividualHandler.scaryRadious, 1, 100);

            EditorGUILayout.Separator();
            EditorGUILayout.Separator();

            actualIndividualHandler.jumpForGenericObjectCounter = (int)EditorGUILayout.Slider(new GUIContent("Jump for generics ", "This jump is used to decide what individuals update each frame, great values = less lag = less performance"), (int)actualIndividualHandler.jumpForGenericObjectCounter, 0, 100);
            actualIndividualHandler.forceNoJumpForGenericObject = EditorGUILayout.Toggle(new GUIContent("Force no jump ", "update all generics all the time"), actualIndividualHandler.forceNoJumpForGenericObject);
            actualIndividualHandler.jumpForIndividualPathPoint = (int)EditorGUILayout.Slider(new GUIContent("Jump for paths ", "This jump is used to decide what paths update each frame, great values = less lag = less performance"), (int)actualIndividualHandler.jumpForIndividualPathPoint, 0, 100);

            EditorGUILayout.Separator();
            EditorGUILayout.Separator();

            actualIndividualHandler.pedestrianCollisionsEnabled = EditorGUILayout.Toggle(new GUIContent("Individual collisions ", "Enable individual's collisions"), actualIndividualHandler.pedestrianCollisionsEnabled);
            actualIndividualHandler.specialCollisionsMode = EditorGUILayout.Toggle(new GUIContent("Special collision mode ", "An special collision mode for specific external third party libraries"), actualIndividualHandler.specialCollisionsMode);

            EditorGUILayout.Separator();
            EditorGUILayout.Separator();

            actualIndividualHandler.drawPathsInRealTime = EditorGUILayout.Toggle(new GUIContent("Draw paths in real time ", "To draw path guizmos and lines in realtime in editor mode"), actualIndividualHandler.drawPathsInRealTime);
            actualIndividualHandler.debugKeysEnabled = EditorGUILayout.Toggle(new GUIContent("Debug Keys ", "Allow certain keys for debug"), actualIndividualHandler.debugKeysEnabled);
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// GetItsAValidClick
        /// # Return true if this is a valid click considering actual insert mode
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static bool GetItsAValidClick(cInsertMode insertMode)
        {
            return EditorBasicFunctions.GetMouseButtonDown(0, insertMode) && EditorBasicFunctions.GetInsertModeKeyPressed(insertMode) && !EditorBasicFunctions.GetDoingSomethingSpecial(insertMode);
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// GetDoingSomethingSpecial
        /// # Return true if we are doing something special with the input
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        static bool GetDoingSomethingSpecial(cInsertMode insertMode)
        {
            switch (insertMode)
            {
                case cInsertMode.controlAndMouse:
                    {
                        return ((Event.current.button == 1) || (Event.current.button == 2) || (Event.current.shift) || (Event.current.alt));
                    }

                case cInsertMode.shiftAndMouse:
                    {
                        return ((Event.current.button == 1) || (Event.current.button == 2) || (Event.current.control) || (Event.current.alt));
                    }

                case cInsertMode.justMouse:
                    {
                        return ((Event.current.button == 1) || (Event.current.button == 2) || (Event.current.control) || (Event.current.shift) || (Event.current.alt));
                    }

                case cInsertMode.disabled:
                    {
                        return true;
                    }
            }

            return false;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// GetInsertModeKeyPressed
        /// # Return true if selected insert mode key is pressed
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        static bool GetInsertModeKeyPressed(cInsertMode insertMode)
        {
            switch (insertMode)
            {
                case cInsertMode.controlAndMouse:
                    {
                        //Debug.Log("controlAndMouse");
                        return Event.current.control;
                    }

                case cInsertMode.shiftAndMouse:
                    {
                        //Debug.Log("shiftAndMouse");
                        return Event.current.shift;
                    }

                case cInsertMode.justMouse:
                    {
                        //Debug.Log("justMouse");
                        return true;
                    }

                case cInsertMode.disabled:
                    {
                        //Debug.Log("disabled");
                        return false;
                    }
            }

            Debug.Log("GetInsertModeKeyPressed false");
            return false;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// GetMouseButtonDown
        /// # Return true if mouse button is down (uses event current, not input)
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        static bool GetMouseButtonDown(int buttonIndex, cInsertMode insertMode)
        {
            switch (insertMode)
            {
                case cInsertMode.shiftAndMouse:
                case cInsertMode.controlAndMouse:
                case cInsertMode.justMouse:
                    {
                        return ((Event.current.type == EventType.MouseDown) && (Event.current.button == buttonIndex));
                    }

                case cInsertMode.disabled:
                    {
                        return false;
                    }
            }

            Debug.Log("GetMouseButtonDown false");
            return false;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// GetPrefabListFromDirectory
        /// # Return a prefab list from a folder
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static List<GameObject> GetPrefabListFromDirectory(string path)
        {
            List<Object> objectList = GetObjectListFromDirectory(path, ".prefab");
            List<GameObject> prefabList = new List<GameObject>();

            foreach (Object obj in objectList)
            {
                //Debug.Log(prefabList.Count + " -> " + prefabList.Count + ": " + obj.name);
                prefabList.Add(obj as GameObject);
            }

            return prefabList;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// GetDecalGetObjectList
        /// # Return a prefab list from a folder
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        static List<Object> GetObjectListFromDirectory(string basePath, DirectoryInfo directoryInfo, string fileExtension)
        {
            List<Object> materialList = new List<Object>();

            List<FileInfo> fileList = directoryInfo.GetFiles("*.*").ToList();

            for (int i = 0; i < fileList.Count; i++)
            {
                if (fileList[i].Extension.ToLower() == fileExtension)
                {
                    string actualGetObjectPath = basePath + fileList[i].Name;

                    Object actualGetObject = AssetDatabase.LoadAssetAtPath(actualGetObjectPath, typeof(Object)) as Object;

                    if (actualGetObject)
                    {
                        materialList.Add(actualGetObject);
                    }
                    else
                    {
                        Debug.Log(actualGetObjectPath + " not a object of extension: " + fileExtension);
                    }
                }
            }

            return materialList;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// GetObjectListFromDirectory
        /// # Return a prefab list from folder
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        static List<Object> GetObjectListFromDirectory(string basePath, string fileExtension)
        {
            List<Object> materialList = new List<Object>();

            DirectoryInfo actualDirInfo = new DirectoryInfo(basePath);

            List<DirectoryInfo> allDirInfoList = new List<DirectoryInfo>();

            allDirInfoList.Add(actualDirInfo);

            foreach (DirectoryInfo dirInfo in actualDirInfo.GetDirectories())
            {
                allDirInfoList.Add(dirInfo);
            }

            foreach (DirectoryInfo dirInfo in allDirInfoList)
            {
                string finalbasePah = basePath;

                if (dirInfo != actualDirInfo)
                {
                    finalbasePah += dirInfo.Name + "/";
                }

                List<Object> actualSubDirList = GetObjectListFromDirectory(finalbasePah, dirInfo, fileExtension);

                for (int i = 0; i < actualSubDirList.Count; i++)
                {
                    materialList.Add(actualSubDirList[i]);
                }
            }

            return materialList;
        }
    }
}
