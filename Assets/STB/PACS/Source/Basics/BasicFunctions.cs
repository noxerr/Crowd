using UnityEngine;
using System.Collections; 
using System.Collections.Generic;
using System.Linq;

namespace STB.PACS
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Class: EditorBasicFunctions
    /// # Compilation on functions used by editor
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class BasicFunctions : MonoBehaviour
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// AddToChildsIgnoreCollisionsWith
        /// # 
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void AddToChildsIgnoreCollisionsWith(Transform src)
        {
            if (src.GetComponent<Collider>() && !src.GetComponent<WheelCollider>() && !src.GetComponent<IgnorePedestriansCollider>())
            {
                src.gameObject.AddComponent<IgnorePedestriansCollider>();
            }

            foreach (Transform child in src)
            {
                AddToChildsIgnoreCollisionsWith(child);
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// GetParentIndividualPath
        /// # Returns this transform parent individual path in the hierarchy (if exists)
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static IndividualPath GetParentIndividualPath(Transform t)
        {
            Transform actualParent = t;

            while (actualParent != null)
            {
                IndividualPath actualIndividualPath = actualParent.gameObject.GetComponent<IndividualPath>();

                if (actualIndividualPath)
                {
                    return actualIndividualPath;
                }

                actualParent = actualParent.parent;
            }

            return null;
        }
    }
}