#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;


namespace STB.SpecialInput
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Class: STBMobileInputInitializer 
    /// Custom compiler defines
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    [InitializeOnLoad]
    public class STBMobileInputInitializer
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// STBMobileInputInitializer
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        static STBMobileInputInitializer()
        {
            var defines = GetDefinesList(buildTargetGroups[0]);
            if (!defines.Contains("CROSS_PLATFORM_INPUT"))
            {
                SetEnabled("CROSS_PLATFORM_INPUT", true, false);
                SetEnabled("STB_MOBILE_INPUT", true, true);
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Enable
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        [MenuItem("Tools/STB/Mobile Input/Enable")]
        private static void Enable()
        {
            SetEnabled("STB_MOBILE_INPUT", true, true);
            switch (EditorUserBuildSettings.activeBuildTarget)
            {
                case BuildTarget.Android:
                case BuildTarget.iOS:
#if !UNITY_5_3_OR_NEWER
                case BuildTarget.WP8Player:
                case BuildTarget.BlackBerry:
#endif
                case BuildTarget.PSM:
                case BuildTarget.Tizen:
                case BuildTarget.WSAPlayer:
                    EditorUtility.DisplayDialog("Mobile Input",
                                                "You have enabled Mobile Input. You'll need to use the Unity Remote app on a connected device to control your game in the Editor.",
                                                "OK");
                    break;

                default:
                    EditorUtility.DisplayDialog("Mobile Input",
                                                "You have enabled Mobile Input, but you have a non-mobile build target selected in your build settings. The mobile control rigs won't be active or visible on-screen until you switch the build target to a mobile platform.",
                                                "OK");
                    break;
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// EnableValidate
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        [MenuItem("Tools/STB/Mobile Input/Enable", true)]
        private static bool EnableValidate()
        {
            var defines = GetDefinesList(mobileBuildTargetGroups[0]);
            return !defines.Contains("STB_MOBILE_INPUT");
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Disable
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        [MenuItem("Tools/STB/Mobile Input/Disable")]
        private static void Disable()
        {
            SetEnabled("STB_MOBILE_INPUT", false, true);
            switch (EditorUserBuildSettings.activeBuildTarget)
            {
                case BuildTarget.Android:
                case BuildTarget.iOS:
#if !UNITY_5_3_OR_NEWER
                case BuildTarget.WP8Player:
                case BuildTarget.BlackBerry:
#else
                case BuildTarget.WSAPlayer:
#endif
                    EditorUtility.DisplayDialog("Mobile Input",
                                                "You have disabled Mobile Input. Mobile control rigs won't be visible, and the Cross Platform Input functions will always return standalone controls.",
                                                "OK");
                    break;
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// DisableValidate
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        [MenuItem("Tools/STB/Mobile Input/Disable", true)]
        private static bool DisableValidate()
        {
            var defines = GetDefinesList(mobileBuildTargetGroups[0]);
            return defines.Contains("STB_MOBILE_INPUT");
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// buildTargetGroups
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private static BuildTargetGroup[] buildTargetGroups = new BuildTargetGroup[]
            {
                BuildTargetGroup.Standalone,
#if !UNITY_5_3_OR_NEWER
                BuildTargetGroup.WebPlayer,
#endif
                BuildTargetGroup.Android,
                BuildTargetGroup.iOS,
#if !UNITY_5_3_OR_NEWER
                BuildTargetGroup.WP8,
                BuildTargetGroup.BlackBerry
#else
                BuildTargetGroup.WSA
#endif
            };
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// mobileBuildTargetGroups
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private static BuildTargetGroup[] mobileBuildTargetGroups = new BuildTargetGroup[]
            {
                BuildTargetGroup.Android,
                BuildTargetGroup.iOS,
#if !UNITY_5_3_OR_NEWER
                BuildTargetGroup.WP8,
                BuildTargetGroup.BlackBerry,
#else
                BuildTargetGroup.WSA,
#endif
                BuildTargetGroup.PSM,
                BuildTargetGroup.Tizen,
                BuildTargetGroup.WSA
            };
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// SetEnabled
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private static void SetEnabled(string defineName, bool enable, bool mobile)
        {
            //Debug.Log("setting "+defineName+" to "+enable);
            foreach (var group in mobile ? mobileBuildTargetGroups : buildTargetGroups)
            {
                var defines = GetDefinesList(group);
                if (enable)
                {
                    if (defines.Contains(defineName))
                    {
                        return;
                    }
                    defines.Add(defineName);
                }
                else
                {
                    if (!defines.Contains(defineName))
                    {
                        return;
                    }
                    while (defines.Contains(defineName))
                    {
                        defines.Remove(defineName);
                    }
                }
                string definesString = string.Join(";", defines.ToArray());
                PlayerSettings.SetScriptingDefineSymbolsForGroup(group, definesString);
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// GetDefinesList
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private static List<string> GetDefinesList(BuildTargetGroup group)
        {
            return new List<string>(PlayerSettings.GetScriptingDefineSymbolsForGroup(group).Split(';'));
        }
    }
}
#endif
