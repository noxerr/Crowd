using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace STB.Generic
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Class: KeyActionManager
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public partial class KeyActionManager : MonoBehaviour
    {
        // public static
        public static KeyActionManager Singleton = null;

        // public
        public KeyCode escapeKey = KeyCode.Escape;
        public KeyCode returnKey = KeyCode.Return;
        public KeyCode restartKey = KeyCode.R;
        public KeyCode reloadCharacterKey = KeyCode.E;
        public KeyCode actionKey = KeyCode.F;
        public KeyCode cameraKey = KeyCode.C;
        public KeyCode boostKey = KeyCode.B;
        public KeyCode jumpKey = KeyCode.Space;
        public KeyCode forwardKey = KeyCode.W;
        public KeyCode backwardKey = KeyCode.S;
        public KeyCode leftKey = KeyCode.A;
        public KeyCode rightKey = KeyCode.D;
        public KeyCode runKey = KeyCode.LeftShift;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Awake
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void Awake()
        {
            Singleton = this;
        }
    }
}