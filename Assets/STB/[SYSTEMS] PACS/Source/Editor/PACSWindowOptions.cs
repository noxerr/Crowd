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
	/// Class: PACSWindowOptions
	/// # Main window class to handle all
	/// </summary>
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public class PACSWindowOptions : PACSWindow
	{		
		// Add shortcut in Window menu
		[MenuItem ("Tools/STB/PACS 1.0/Options")]

		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Init
		/// # Initialise the window and show it
		/// </summary>
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		static void Init ()
		{
			// Get existing open window or if none, make a new one:
			PACSWindowOptions window = (PACSWindowOptions)PACSWindowOptions.GetWindow (typeof(PACSWindowOptions));
			window.name = "PACS Options";

#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_5_0
    window.title = window.name;
#else
            window.titleContent = new GUIContent(window.name);
#endif

            window.PACSMode = PACSWindowOptions.cPACSMode.options;	

			window.Show ();
		}
	}
}