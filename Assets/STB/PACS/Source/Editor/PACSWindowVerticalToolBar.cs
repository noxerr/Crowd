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
	/// Class: PACSWindowVerticalToolBar
	/// # Main window class to handle all
	/// </summary>
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public class PACSWindowVerticalToolBar : PACSWindow
	{
		// Add shortcut in Window menu
		[MenuItem ("Tools/STB/PACS 1.0/Vertical ToolBar")]

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Init
		/// # Initialise the window and show it
		/// </summary>
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		static void Init ()
		{
			// Get existing open window or if none, make a new one:
			PACSWindowVerticalToolBar window = (PACSWindowVerticalToolBar)PACSWindowVerticalToolBar.GetWindow (typeof(PACSWindowVerticalToolBar));
			window.name = "PACS Vertical Toolbar";

#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_5_0
    window.title = window.name;
#else
            window.titleContent = new GUIContent(window.name);
#endif
            
			window.PACSMode = PACSWindowVerticalToolBar.cPACSMode.verticalToolbar;	
			
			//window.maxSize = new Vector2 (1000, 10);
			//window.minSize = new Vector2 (1, 100); 

			window.Show ();
		}
	}
}