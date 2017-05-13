using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace STB.Generic
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Class: MaterialRandomizer
    /// # Change enemy materials randomly
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    [ExecuteInEditMode]
    public class MaterialRandomizer : MonoBehaviour
    {
        // public
        public List<Material> materialList = new List<Material>();


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Awake
        /// # Generate random enemy unit at the beginning
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////	
        void Awake()
        {
            RandomizeMaterial();
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// RandomizeMaterial
        /// # Randomize a material
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////	
        public void RandomizeMaterial()
        {
            try
            {
                if (Random.Range(0, 100) < 50)
                {
                    Vector3 actualScale = this.transform.localScale;
                    actualScale.x *= -1;
                    this.transform.localScale = actualScale;
                }

                int newMaterialIndex = Random.Range(0, materialList.Count);

                if (newMaterialIndex > materialList.Count - 1)
                {
                    newMaterialIndex = materialList.Count - 1;
                }

                this.GetComponent<Renderer>().material = materialList[newMaterialIndex];
            }
            catch
            {
                Debug.Log("NOTE -> perhaps some work is not finished");
            }
        }
    }
}
