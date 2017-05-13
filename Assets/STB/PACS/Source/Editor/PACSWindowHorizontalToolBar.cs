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
    /// Class: PACSWindowHorizontalToolBar
    /// # Main window class to handle all
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class PACSWindowHorizontalToolBar : PACSWindow
    {
        // Add shortcut in Window menu
        [MenuItem("Tools/STB/PACS 1.0/Horizontal ToolBar")]

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Init
        /// # Initialise the window and show it
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            PACSWindowHorizontalToolBar window = (PACSWindowHorizontalToolBar)PACSWindowHorizontalToolBar.GetWindow(typeof(PACSWindowHorizontalToolBar));
            window.name = "PACS Horizontal Toolbar";

#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_5_0
    window.title = window.name;
#else
            window.titleContent = new GUIContent(window.name);
#endif

            window.PACSMode = PACSWindowHorizontalToolBar.cPACSMode.horizontalToolbar;

            window.Show();
        }
    }
}