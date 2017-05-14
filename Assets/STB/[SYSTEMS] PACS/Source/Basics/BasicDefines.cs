using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace STB.PACS
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// BasicDefines
    /// # Recopilation of some needed defines
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class BasicDefines
    {
        // public -- version
        public static string VERSION = "1.2.2.4"; // 2017/05/16


        // public -- paths
        public static string MAIN_PATH = "Assets/STB/[SYSTEMS] PACS/";
        public static string EDITOR_BUTTONS_PATH = MAIN_PATH + "Media/Textures/Editor/";
        public static string EDITOR_PATH_PAINTER_OPTIONS_BUTTONS_PATH = MAIN_PATH + "Media/Textures/Editor/PathPainter/";
        public static string EDITOR_INDIVIDUAL_PREFABS_PATH = MAIN_PATH + "Media/Prefabs/Editor/Individual/";
        public static string EDITOR_PATH_PAINTER_OPTIONS_PREFABS_PATH = MAIN_PATH + "Media/Prefabs/Editor/PathPainter/";


        // public -- names
        public static string INDIVIDUAL_PATH_PREFIX = "IndividualPath_";
        public static string NORMAL_INDIVIDUAL_PATH_POINT_PREFIX = "NormalPoint_";
        public static string SPECIAL_INDIVIDUAL_PATH_POINT_PREFIX = "SpecialPoint_";
        public static string INDIVIDUALS_CONTAINER_NAME = "INDIVIDUALS_CONTAINER";
        public static string INDIVIDUAL_HANDLER_NAME = "IndividualHandler";
    }
}
