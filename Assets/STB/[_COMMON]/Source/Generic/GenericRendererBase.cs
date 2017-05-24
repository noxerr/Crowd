using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace STB.Generic
{ 
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Class: GenericRendererBase
    /// # 
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class GenericRendererBase : MonoBehaviour
    {
        // private
        bool renderersHaveToBeEnabled = true;
        float timeToEnableRenderers = 0;

        // private
        List<Renderer> rendererList = new List<Renderer>();

        
        void Start()
        {
            STB.Basics.BasicFunctions.GetAllChildRenderers(this.transform, ref rendererList);
        }

        public void HideRenderers()
        {
            renderersHaveToBeEnabled = true;
            timeToEnableRenderers = 2;

            for (int i = 0; i < rendererList.Count; i++) rendererList[i].enabled = false;
        }

        void Update()
        {
            if (renderersHaveToBeEnabled)
            {
                timeToEnableRenderers -= Time.deltaTime;

                if (timeToEnableRenderers <= 0)
                {
                    renderersHaveToBeEnabled = false;

                    for (int i = 0; i < rendererList.Count; i++) rendererList[i].enabled = true;
                }
            }
        }
    }
}
