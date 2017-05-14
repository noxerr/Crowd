using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace STB.Basics
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
        public static string VERSION = "2.2.2.3"; // 2017/05/07


        // public -- other
        public static Vector3 TOO_FAR_POSITION = 99999 * Vector3.one;
        public static Vector3 Y_NULLIFY_VECTOR = new Vector3(1, 0, 1);


        // public -- names
        public static string NOT_DEFINED = "NOT_DEFINED";
    }
}
