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
    /// Class: ADTWindow
    /// # Main window class to handle all
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public partial class PACSWindow : EditorWindow
    {
        //private
        //static int actualEnabledButtons = 1;

        // private
        public Color settings_normalPathPointsColor = Color.red;
        public Color settings_normalPathColor = Color.blue;
        public Color settings_normalArrowsColor = Color.red;
        public Color settings_specialPathPointsColor = Color.blue;
        public Color settings_specialPathColor = Color.yellow;
        public Color settings_specialArrowsColor = Color.red;
        public Color settings_borderColor = Color.black;

        // private
        public float settings_pointSize = 2;


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// OnDestroySettingsMode
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void OnDestroySettingsMode()
        {
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// UpdateSettingsMode
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void UpdateSettingsMode()
        {
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// HandleSettingsMode
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void HandleSettingsMode()
        {
            EditorGUILayout.Separator();

            // insert mode
            try
            {
                settings_normalPathPointsColor = EditorGUILayout.ColorField(new GUIContent("Normal points path color", "Select the color for the normal points"), settings_normalPathPointsColor, new GUILayoutOption[] { GUILayout.Width(0.81f * position.width) });
                settings_normalPathColor = EditorGUILayout.ColorField(new GUIContent("Normal path color", "Select the color for the normal path lines"), settings_normalPathColor, new GUILayoutOption[] { GUILayout.Width(0.81f * position.width) });
                settings_normalArrowsColor = EditorGUILayout.ColorField(new GUIContent("Normal arrows color", "Select the color for the normal point arrows"), settings_normalArrowsColor, new GUILayoutOption[] { GUILayout.Width(0.81f * position.width) });

                EditorGUILayout.Separator();

                settings_specialPathPointsColor = EditorGUILayout.ColorField(new GUIContent("Special points path color", "Select the color for the special points"), settings_specialPathPointsColor, new GUILayoutOption[] { GUILayout.Width(0.81f * position.width) });
                settings_specialPathColor = EditorGUILayout.ColorField(new GUIContent("Special path color", "Select the color for the special path lines"), settings_specialPathColor, new GUILayoutOption[] { GUILayout.Width(0.81f * position.width) });
                settings_specialArrowsColor = EditorGUILayout.ColorField(new GUIContent("Special arrows color", "Select the color for the special point arrows"), settings_specialArrowsColor, new GUILayoutOption[] { GUILayout.Width(0.81f * position.width) });

                EditorGUILayout.Separator();

                settings_borderColor = EditorGUILayout.ColorField(new GUIContent("Border color", "Select the border color"), settings_borderColor, new GUILayoutOption[] { GUILayout.Width(0.81f * position.width) });

                EditorGUILayout.Separator();

                settings_pointSize = EditorGUILayout.Slider(new GUIContent("Point size", "Select the point size"), settings_pointSize, 0.1f, 9, new GUILayoutOption[] { GUILayout.Width(0.81f * position.width) });
            }
            catch (UnityException e)
            {
                Debug.Log("Warning: " + e);
            }

            // apply changes
            foreach (PathLineDrawer pld in GameObject.FindObjectsOfType<PathLineDrawer>())
            {
                if (!pld.individualEdition)
                {
                    pld.normalPathPointsColor = settings_normalPathPointsColor;
                    pld.normalPathColor = settings_normalPathColor;
                    pld.normalArrowsColor = settings_normalArrowsColor;

                    pld.specialPathPointsColor = settings_specialPathPointsColor;
                    pld.specialPathColor = settings_specialPathColor;
                    pld.specialArrowsColor = settings_specialArrowsColor;

                    pld.borderColor = settings_borderColor;

                    pld.pointSize = settings_pointSize;
                }
            }

            EditorGUILayout.Separator();
            EditorGUILayout.Separator();
        }
    }
}