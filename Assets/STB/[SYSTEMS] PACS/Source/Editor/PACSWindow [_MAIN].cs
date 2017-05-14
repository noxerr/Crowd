using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.Linq;
using System.IO;

namespace STB.PACS
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Class: ADTWindow
    /// # Main window class to handle all
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public partial class PACSWindow : EditorWindow
    {
        // protected static
        static List<EditorWindow> ETTWindowsList = new List<EditorWindow>();

        // public
        public int lastSelectedButtonIndex = 0;

        // protected
        protected enum cPACSMode
        {
            verticalToolbar,
            horizontalToolbar,
            options,
            allInOne
        }
        ;
        protected cPACSMode PACSMode = cPACSMode.allInOne;

        // private -- enum
        enum cPivotMode
        {
            useOriginalPivot,
            autoCalculate
        }
        ;

        // private delegate
        delegate void DelegateButtonFunction();
        delegate void DelegateUpdateFunction();
        delegate void DelegateOnDestroyFunction();

        // private -- buttons
        int previousSelectedMainButton = -1;
        int numMainButtonsPerLine = 3;
        float mainButtonsMaxScale = 90;
        List<string> mainButtonsNameList = new List<string>();
        List<string> mainButtonsTitleList = new List<string>();
        List<DelegateButtonFunction> mainButtonsFunctionsList = new List<DelegateButtonFunction>();
        List<DelegateUpdateFunction> mainUpdateFunctionsList = new List<DelegateUpdateFunction>();
        List<DelegateOnDestroyFunction> mainDestroyFunctionsList = new List<DelegateOnDestroyFunction>();

        // private -- control
        bool needsRepaint = false;
        int lastSelectionCount = -1;
        double previousEditorTime = 0;
        Vector2 actualScrollViewPosition = Vector2.zero;

        // private -- selection
        GameObject actualObjectToForceSelect = null;
        double actualObjectToForceSelectCounter = 0;
        double previousEditorTimeToForceSelect = 0;

        // private -- style
        GUIStyle customFoldoutStyle = null;

        // private
        bool weHaveTheFocus = true;

        // public
        public bool showIndividualHandlerOptions = false;


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// OnEnable
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void OnEnable()
        {
            SceneView.onSceneGUIDelegate += SceneGUI;

            ETTWindowsList.Add(this);
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// OnFocus
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void OnFocus()
        {
            //Debug.Log("OnFocus");
            weHaveTheFocus = true;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// OnLostFocus
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void OnLostFocus()
        {
            //Debug.Log("OnLostFocus");
            weHaveTheFocus = true;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// OnSelectionChange
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void OnSelectionChange()
        {
            if (!weHaveTheFocus) return;

            //Debug.Log("OnSelectionChange");
            RepaintAllWindows();
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// DefineButtonList
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void DefineButtonList()
        {
            // define buttons
            if ((mainButtonsNameList.Count <= 0) || (mainButtonsFunctionsList.Count <= 0))
            {
                mainButtonsNameList.Clear();
                mainButtonsFunctionsList.Clear();

                AddNewButton("PathPainterButton", "Path painter options", HandlePathPainterMode, UpdatePathPainterMode, OnDestroyPathPainterMode);
                AddNewButton("PathSettingsButton", "Path settings options", HandlePathSettingsMode, UpdatePathSettingsMode, OnDestroyPathSettingsMode);
                AddNewButton("PointsSettingsButton", "Points settings options", HandlePointsSettingsMode, UpdatePointsSettingsMode, OnDestroyPointsSettingsMode);

                AddNewButton("SettingsButton", "Main PACS settings", HandleSettingsMode, UpdateSettingsMode, OnDestroySettingsMode);
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// CreateCustomStyles
        /// # Create custom styles for GUI
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void CreateCustomStyles()
        {
            if (customFoldoutStyle == null)
            {
                customFoldoutStyle = new GUIStyle(EditorStyles.foldout);
                customFoldoutStyle.fontStyle = FontStyle.Bold;
                customFoldoutStyle.normal.textColor = Color.black;
                customFoldoutStyle.onNormal.textColor = Color.black;
                customFoldoutStyle.hover.textColor = Color.black;
                customFoldoutStyle.onHover.textColor = Color.black;
                customFoldoutStyle.focused.textColor = Color.blue;
                customFoldoutStyle.onFocused.textColor = Color.blue;
                customFoldoutStyle.active.textColor = Color.blue;
                customFoldoutStyle.onActive.textColor = Color.blue;
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// GetEditorTimeDiff
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        double GetEditorTimeDiff()
        {
            return (EditorApplication.timeSinceStartup - previousEditorTime);
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// GeneralUpdate
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void GeneralUpdate()
        {
            if (!weHaveTheFocus) return;

            //Debug.Log ("GeneralUpdate in " + this.name);

            // general
            if (Selection.activeGameObject)
            {
                if (Selection.activeGameObject.GetComponent<IndividualPath>()) lastSelectedIndividualPath = Selection.activeGameObject.GetComponent<IndividualPath>();
                else if (Selection.activeGameObject.GetComponent<IndividualPathPoint>())
                {
                    if (Selection.activeGameObject.transform.parent && Selection.activeGameObject.transform.parent.GetComponent<IndividualPath>())
                    {
                        lastSelectedIndividualPath = Selection.activeGameObject.transform.parent.GetComponent<IndividualPath>();
                    }
                }
            }

            // main update of each option
            if (mainUpdateFunctionsList.Count > 0) mainUpdateFunctionsList[lastSelectedButtonIndex]();

            // relculate centroid if possible
            if (lastSelectedIndividualPath && lastSelectedIndividualPath.recalculatePathPivotUsingChildPoints) lastSelectedIndividualPath.CalculateCentroid();
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// DetermineRealPACSMode
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void DetermineRealPACSMode()
        {
#if UNITY_5_0
            if (title == "PACS Vertical Toolbar")
            {
                //Debug.Log ("position: " + position);
                PACSMode = cPACSMode.verticalToolbar;
            }
            else if (title == "PACS Horizontal Toolbar")
            {
                PACSMode = cPACSMode.horizontalToolbar;
            }
            else if (title == "PACS Options")
            {
                PACSMode = cPACSMode.options;
            }
#else
            if (titleContent.text == "PACS Vertical Toolbar")
            {
                //Debug.Log ("position: " + position);
                PACSMode = cPACSMode.verticalToolbar;
            }
            else if (titleContent.text == "PACS Horizontal Toolbar")
            {
                PACSMode = cPACSMode.horizontalToolbar;
            }
            else if (titleContent.text == "PACS Options")
            {
                PACSMode = cPACSMode.options;
            }
#endif
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Update
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Update()
        {
            if (!weHaveTheFocus) return;

            // force objects selection
            double editorDeltaTimeToForceSelect = EditorApplication.timeSinceStartup - previousEditorTimeToForceSelect;
            previousEditorTimeToForceSelect = EditorApplication.timeSinceStartup;

            if (actualObjectToForceSelect)
            {
                if (actualObjectToForceSelect)
                {
                    //Debug.Log ("actualObjectToForceSelect: " + actualObjectToForceSelect.name);
                    Selection.activeObject = actualObjectToForceSelect;
                }

                actualObjectToForceSelectCounter += editorDeltaTimeToForceSelect;

                if (actualObjectToForceSelectCounter > 0.25f)
                {
                    //Debug.Log ("actualObjectToForceSelectCounter: " + actualObjectToForceSelectCounter);

                    actualObjectToForceSelectCounter = 0;
                    actualObjectToForceSelect = null;
                }
            }

            // more
            DetermineRealPACSMode();

            if (previousSelectedMainButton != lastSelectedButtonIndex)
            {
                previousSelectedMainButton = lastSelectedButtonIndex;

                RepaintAllWindows();
            }

            if (lastSelectionCount != Selection.gameObjects.Count())
            {
                //Debug.Log ("Selection changes");
                lastSelectionCount = Selection.gameObjects.Count();

                RepaintAllWindows();
            }

            // needs repaint
            if (needsRepaint)
            {
                needsRepaint = false;
                RepaintAllWindows();
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// RepaintAllWindows
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void RepaintAllWindows()
        {
            if (!weHaveTheFocus) return;

            for (int i = 0; i < ETTWindowsList.Count; i++)
            {
                ETTWindowsList[i].Repaint();
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// OnDestroy
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void OnDestroy()
        {
            // destroy every mode first
            for (int i = 0; i < mainDestroyFunctionsList.Count; i++) mainDestroyFunctionsList[i]();

            ETTWindowsList.Clear();

            // main
            mainButtonsNameList.Clear();
            mainButtonsTitleList.Clear();
            mainButtonsFunctionsList.Clear();
            mainUpdateFunctionsList.Clear();
            mainDestroyFunctionsList.Clear();
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// SceneGUI
        /// # Handle SceneGUI
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected void SceneGUI(SceneView sceneView)
        {
            GeneralUpdate();
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// HandleSelectedButtonOptions
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void HandleSelectedButtonOptions()
        {
            try
            {
                actualScrollViewPosition = EditorGUILayout.BeginScrollView(actualScrollViewPosition);

                //Debug.Log ("mainButtonsFunctionsList.Count A: " + mainButtonsFunctionsList.Count);
                //Debug.Log ("actualSelectedMainButton: " + actualSelectedMainButton);

                if (mainButtonsFunctionsList.Count > 0) mainButtonsFunctionsList[lastSelectedButtonIndex]();

                EditorGUILayout.EndScrollView();
            }
            catch (UnityException e)
            {
                Debug.Log("Warning: " + e);
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// HandleButtonPression
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void HandleButtonPression(int i)
        {
            //Debug.Log("HandleButtonPression: " + i.ToString()); 
            lastSelectedButtonIndex = i;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// AddNewButton
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void AddNewButton(string name, string tittle, DelegateButtonFunction buttonFunction, DelegateUpdateFunction updateFunction, DelegateOnDestroyFunction destroyFunction)
        {
            mainButtonsNameList.Add(name);
            mainButtonsTitleList.Add(tittle);
            mainButtonsFunctionsList.Add(buttonFunction);
            mainUpdateFunctionsList.Add(updateFunction);
            mainDestroyFunctionsList.Add(destroyFunction);

            //Debug.Log ("mainButtonsFunctionsList.Count Z: " + mainButtonsFunctionsList.Count);
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// DrawMainButtons
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void DrawMainButtons()
        {
            // draw buttons
            Vector2 buttonsScale = ((0.98f / (float)numMainButtonsPerLine) * position.width) * Vector2.one;

            if (buttonsScale.x >= mainButtonsMaxScale)
            {
                buttonsScale.x = mainButtonsMaxScale;
                buttonsScale.y = mainButtonsMaxScale;
            }

            int cont = 0;
            bool beginUnfinished = false;

            bool finishDrawing = false;

            for (int i = 0; i < mainButtonsNameList.Count; i++)
            {
                if (finishDrawing) break;

                if (cont == 0)
                {
                    try
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();
                        beginUnfinished = true;
                    }
                    catch (UnityException e)
                    {
                        Debug.Log("Warning: " + e);

                        finishDrawing = true;
                        break;
                    }
                }

                if (!finishDrawing)
                {
                    if (GetDrawMiniMainButtons())
                    {
                        if (EditorBasicFunctions.GetEditorButton("Tools/", mainButtonsNameList[i], mainButtonsTitleList[i], buttonsScale, (i == lastSelectedButtonIndex), true, false, false))
                        {
                            HandleButtonPression(i);
                        }
                    }
                    else
                    {
                        if (EditorBasicFunctions.GetEditorButton("Tools/" + mainButtonsNameList[i], mainButtonsTitleList[i], buttonsScale, (i == lastSelectedButtonIndex), true, false, false))
                        {
                            HandleButtonPression(i);
                        }
                    }

                    cont++;

                    if (cont > numMainButtonsPerLine - 1)
                    {
                        cont = 0;

                        GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();
                        beginUnfinished = false;
                    }
                }
            }

            if (!finishDrawing && beginUnfinished)
            {
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// GetDrawMiniMainButtons
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        bool GetDrawMiniMainButtons()
        {
            return false;
            //return ((PACSMode == cPACSMode.verticalToolbar) || (PACSMode == cPACSMode.horizontalToolbar));
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// OnGUI
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void OnGUI()
        {
            if (!weHaveTheFocus) return;

            //Debug.Log ("OnGUI: " + name);

            CreateCustomStyles();

            DetermineRealPACSMode();

            // define button list
            DefineButtonList();

            // determinate what draw and what not
            bool drawVersion = false;
            bool drawMainButtons = false;
            bool drawOptions = false;
            bool drawTittle = true;

            switch (PACSMode)
            {
                case cPACSMode.verticalToolbar:
                    {
                        drawVersion = false;
                        drawMainButtons = true;
                        drawOptions = false;
                        drawTittle = false;
                        numMainButtonsPerLine = 1;
                        mainButtonsMaxScale = 110;
                    }
                    break;

                case cPACSMode.horizontalToolbar:
                    {
                        drawVersion = false;
                        drawMainButtons = true;
                        drawOptions = false;
                        drawTittle = false;
                        numMainButtonsPerLine = 4;
                        mainButtonsMaxScale = 110;
                    }
                    break;

                case cPACSMode.options:
                    {
                        drawVersion = false;
                        drawMainButtons = false;
                        drawOptions = true;
                        drawTittle = true;
                        numMainButtonsPerLine = 4;
                        mainButtonsMaxScale = 70;
                    }
                    break;

                case cPACSMode.allInOne:
                    {
                        drawVersion = true;
                        drawMainButtons = true;
                        drawOptions = true;
                        numMainButtonsPerLine = 2;
                        mainButtonsMaxScale = 110;
                    }
                    break;
            }


            // draw
            GUILayout.BeginArea(new Rect(0, 0, position.width, position.height));

            if (drawVersion)
            {
                EditorGUILayout.Separator();

                EditorBasicFunctions.DrawActualVersion(position);
            }

            EditorGUILayout.Separator();

            if (drawMainButtons)
            {
                DrawMainButtons();

                if (drawTittle)
                {
                    EditorGUILayout.Separator();

                    EditorBasicFunctions.DrawLineSeparator();
                }
            }

            // draw individual handler options
            EditorBasicFunctions.DrawEditorBox("Individual handler - Options", Color.white);

            showIndividualHandlerOptions = EditorGUILayout.Foldout(showIndividualHandlerOptions, new GUIContent("Expand Individual Handler options", "Expand Individual Handler options"));

            if (showIndividualHandlerOptions)
            {
                IndividualHandler actualIndividualHandler = GameObject.FindObjectOfType<IndividualHandler>();
                EditorBasicFunctions.DrawIndividualHandlerOptions(actualIndividualHandler);
            }

            if (drawTittle)
            {
                if (mainButtonsTitleList.Count > 0) EditorBasicFunctions.DrawEditorBox(mainButtonsTitleList[lastSelectedButtonIndex], Color.white);

                EditorGUILayout.Separator();
            }

            if (drawOptions)
            {
                HandleSelectedButtonOptions();
            }

            GUILayout.EndArea();


            // gui has change
            if (GUI.changed)
            {
                //Debug.Log("RepaintAll");
                SceneView.RepaintAll();
            }

            //Debug.Log("actualSelectedMainButton: " + actualSelectedMainButton);
        }
    }
}
