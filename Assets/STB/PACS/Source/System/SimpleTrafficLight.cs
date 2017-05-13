using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace STB.PACS
{
#if UNITY_EDITOR
    [RequireComponent(typeof(PathLineDrawer))]
#endif
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Class: SimpleTrafficLight
    /// # Simple extended traffic light system
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class SimpleTrafficLight : GenericTrafficLight
    {
        // public
        public Material materialForRed = null;
        public Material materialForGreen = null;
        public List<Renderer> listOfObjectsToChangeMaterial = new List<Renderer>();
        public int SubMaterialIndex = -1;

        // private
        Renderer actualRenderer = null;
        Transform TimeText = null;


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// AwakeExtended -- OVERRIDED
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected override void AwakeExtended()
        {
            this.gameObject.AddComponent<GenericObject>();

            actualRenderer = this.gameObject.GetComponent<Renderer>();
            TimeText = Basics.BasicFunctions.GetTransformInChildsByName(this.transform, "TimeText");
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// DoOnGreen -- OVERRIDED
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected override void DoOnGreen()
        {
            if (materialForGreen && materialForRed)
            {
                if (actualRenderer) actualRenderer.material = materialForGreen;

                for (int i = 0; i < listOfObjectsToChangeMaterial.Count; i++)
                {
                    if (SubMaterialIndex == -1) listOfObjectsToChangeMaterial[i].material = materialForRed;
                    else if (SubMaterialIndex < listOfObjectsToChangeMaterial[i].materials.Length)
                    {
                        Material[] mats = listOfObjectsToChangeMaterial[i].materials;

                        //Debug.Log("Material change " + materialForRed.name + " for " + listOfObjectsToChangeMaterial[i].name + " in SubMaterialIndex " + SubMaterialIndex);

                        mats[SubMaterialIndex] = materialForGreen;

                        listOfObjectsToChangeMaterial[i].materials = mats;

                        //Debug.Log("New material name: " + listOfObjectsToChangeMaterial[i].materials[SubMaterialIndex].name);
                    }
                }
            }
            else if (actualRenderer)
            {
                Color color = Color.green;
                color.a = 0.9f;

                actualRenderer.material.color = color;
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// DoOnRed -- OVERRIDED
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected override void DoOnRed()
        {
            if (materialForGreen && materialForRed)
            {
                if (actualRenderer) actualRenderer.material = materialForRed;

                for (int i = 0; i < listOfObjectsToChangeMaterial.Count; i++)
                {
                    if (SubMaterialIndex == -1) listOfObjectsToChangeMaterial[i].material = materialForRed;
                    else if (SubMaterialIndex < listOfObjectsToChangeMaterial[i].materials.Length)
                    {
                        Material[] mats = listOfObjectsToChangeMaterial[i].materials;

                        //Debug.Log("Material change " + materialForRed.name + " for " + listOfObjectsToChangeMaterial[i].name + " in SubMaterialIndex " + SubMaterialIndex);

                        mats[SubMaterialIndex] = materialForRed;

                        listOfObjectsToChangeMaterial[i].materials = mats;

                        //Debug.Log("New material name: " + listOfObjectsToChangeMaterial[i].materials[SubMaterialIndex].name);
                    }
                }
            }
            else if (actualRenderer)
            {
                Color color = Color.red;
                color.a = 0.9f;

                actualRenderer.material.color = color;
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// UpdateExtended -- OVERRIDED
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected override void UpdateExtended()
        {
            TimeText.GetComponent<TextMesh>().text = ((int)actualStateTime).ToString() + "s";
        }
    }
}
