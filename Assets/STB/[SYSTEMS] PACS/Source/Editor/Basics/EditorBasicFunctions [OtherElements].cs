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
        /// GetBoldTextGUIStyle
        /// # Returns a bold text GUIStyle
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static GUIStyle GetBoldTextGUIStyle()
        {
            GUIStyle boldtext = new GUIStyle(GUI.skin.label);
            boldtext.fontStyle = FontStyle.Bold;

            return boldtext;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// GetThereIsSomethingSelected
        /// # Return true if there is something selected
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static bool GetThereIsSomethingSelected(bool showWarning)
        {
            bool r = (Selection.gameObjects.Count() > 0);

            if (!r && showWarning)
            {
                EditorUtility.DisplayDialog("WARNING", "There is nothing selected!", "OK");
            }

            return r;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// GetEditorTextButton
        /// # Return true if created text button is pressed
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static bool GetEditorTextButton(string text, string helpText)
        {
            return GUILayout.Button(new GUIContent(text, helpText));
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// GetEditorTextButton
        /// # Return true if created text button is pressed
        /// </summary>
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static bool GetEditorTextButton(string text, string helpText, Rect positionForLogicalText)
        {
            return GUILayout.Button(new GUIContent(text, helpText), new GUILayoutOption[] { GUILayout.Width(GetLogicalTextButtonsWidth(positionForLogicalText)) });
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// GetTextbuttonsWidth
        /// # Return a logical text button width accoding to actual position rect
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static float GetLogicalTextButtonsWidth(Rect position)
        {
            float buttonsWidth = 0.8f * position.width;

            if (buttonsWidth > 160)
            {
                buttonsWidth = 160;
            }

            return buttonsWidth;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// GetEditorTimeString
        /// # Return editor time since startup
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static string GetEditorTimeString()
        {
            return ((int)EditorApplication.timeSinceStartup).ToString() + ">> ";
            //return "[" + ((int)EditorApplication.timeSinceStartup).ToString () + "] ";
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// DrawLineSeparator
        /// # Draw a line as separator
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void DrawLineSeparator()
        {
            GUILayout.Box("", new GUILayoutOption[] {
                GUILayout.ExpandWidth (true),
                GUILayout.Height (1)
            });
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// DrawEditorBox
        /// # Draw an editor box given text, color and position
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void DrawEditorBox(string text, Color color)
        {
            GUIStyle boxStyle = new GUIStyle(GUI.skin.box);

            if (!UnityEditorInternal.InternalEditorUtility.HasPro())
            {
                Color actualGuiColor = GUI.color;

                GUI.color = Color.white;

                boxStyle.normal.textColor = Color.black;
                GUILayout.Box(text, boxStyle, new GUILayoutOption[] { GUILayout.ExpandWidth(true) });

                GUI.color = actualGuiColor;
            }
            else
            {
                boxStyle.normal.textColor = color;
                GUILayout.Box(text, boxStyle, new GUILayoutOption[] { GUILayout.ExpandWidth(true) });
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// DrawActualVersion
        /// # Draw actual version
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void DrawActualVersion(Rect position)
        {
            EditorBasicFunctions.DrawEditorBox("PACS [v" + BasicDefines.VERSION + "]", Color.white);
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// GetEditorButton
        /// # Create an editor button and return true if it is pressed
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static bool GetEditorButton(string path, string name, string helpText, Vector2 size, bool enabled, bool applyColorChange, bool justSeparator, bool fixedSize)
        {
            return GetEditorButton(path + name, helpText, size, enabled, applyColorChange, justSeparator, fixedSize);
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// GetEditorButton
        /// # Create an editor button and return true if it is pressed
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static bool GetEditorButton(string name, string helpText, Vector2 size, bool enabled, bool applyColorChange, bool justSeparator, bool fixedSize)
        {
            //Debug.Log("BasicDefines.EDITOR_BUTTONS_PATH: " + BasicDefines.EDITOR_BUTTONS_PATH);

            string buttonTexturePath = BasicDefines.EDITOR_BUTTONS_PATH + name + ".png";
            Texture texture = AssetDatabase.LoadAssetAtPath(buttonTexturePath, typeof(Texture)) as Texture;

            if (!texture)
            {
                //Debug.Log ("Texture error in " + buttonTexturePath);
            }

            Color guiColor = GUI.color;
            Color bgColor = GUI.backgroundColor;

            if (justSeparator)
            {
                GUI.color = new Color(1, 1, 1, 0.0f);
                GUI.backgroundColor = new Color(1, 1, 1, 0.0f);
            }
            else if (applyColorChange)
            {
                if (enabled)
                {
                    GUI.color = new Color(1, 1, 1, 1);
                    GUI.backgroundColor = new Color(0.6f, 0.0f, 0.6f, 1f);
                }
                else
                {
                    GUI.color = new Color(1, 1, 1, 0.6f);
                    GUI.backgroundColor = new Color(1, 1, 1, 0.3f);
                }
            }

            bool r = false;

            if (!fixedSize && !enabled)
            {
                GUIStyle newStyle = new GUIStyle();

                newStyle.fixedHeight = 0;
                newStyle.fixedWidth = 0;

                r = GUILayout.Button(new GUIContent(texture, helpText), newStyle, new GUILayoutOption[] {
                    GUILayout.Width (size.x),
                    GUILayout.Height (size.y),
                });
            }
            else
            {
                r = GUILayout.Button(new GUIContent(texture, helpText), new GUILayoutOption[] {
                    GUILayout.Width (size.x),
                    GUILayout.Height (size.y),
                });
            }


            GUI.color = guiColor;
            GUI.backgroundColor = bgColor;

            return r;
        }
    }
}