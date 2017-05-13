using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace STB.PACS
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Class: GenericIndividualPool
    /// # To handle each individual type pool with no duplicates
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class GenericIndividualPool
    {
        // private static
        static int GENERIC_INDIVIDUAL_POOL_INDEX = 0;

        // public
        public GenericIndividual basePrefab = null;

        // private
        List<GenericIndividual> genericIndividualList = new List<GenericIndividual>();
        List<IndividualPath> linkedIndividualPath = new List<IndividualPath>();


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// GenericIndividualPool
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public GenericIndividualPool(GenericIndividual bp, IndividualPath parentPath)
        {
            GENERIC_INDIVIDUAL_POOL_INDEX++;
            //Debug.Log("GENERIC_INDIVIDUAL_POOL_INDEX: " + GENERIC_INDIVIDUAL_POOL_INDEX);

            basePrefab = bp;

            string containerExtraSufix = "";

            if (IndividualHandler.Singleton && IndividualHandler.Singleton.noNavMeshHandle) containerExtraSufix = "_" + parentPath.gameObject.name + "_pool" + GENERIC_INDIVIDUAL_POOL_INDEX;

            Transform individualMainContainer = Basics.BasicFunctions.CreateContainerIfNotExists(BasicDefines.INDIVIDUALS_CONTAINER_NAME + containerExtraSufix).transform;

            if (IndividualHandler.Singleton && IndividualHandler.Singleton.noNavMeshHandle) individualMainContainer.transform.parent = parentPath.transform;

            Transform actualIndividualContainer = Basics.BasicFunctions.CreateContainerIfNotExists(basePrefab.gameObject.name + "_Pool_" + GENERIC_INDIVIDUAL_POOL_INDEX.ToString()).transform;
            actualIndividualContainer.parent = individualMainContainer;

            if (IndividualHandler.Singleton)
            {
                for (int i = 0; i < IndividualHandler.Singleton.maxPool; i++)
                {
                    GenericIndividual actualIndividual = GameObject.Instantiate(basePrefab);
                    actualIndividual.gameObject.name = bp.gameObject.name + "_" + (i + 1).ToString();
                    actualIndividual.transform.parent = actualIndividualContainer;

                    genericIndividualList.Add(actualIndividual);
                }
            }
            else
            {
                Debug.Log("Warning: There is no IndividualHandler in the scene!");
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// AddLinkedIndividualPath
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void AddLinkedIndividualPath(IndividualPath cpp)
        {
            linkedIndividualPath.Add(cpp);
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Start
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public GenericIndividual GetFreeGenericIndividualPool(IndividualPath cp)
        {
            bool pathIsLinked = false;

            for (int i = 0; i < linkedIndividualPath.Count; i++)
            {
                if (linkedIndividualPath[i].Equals(cp))
                {
                    pathIsLinked = true;
                    break;
                }
            }

            if (pathIsLinked)
            {
                for (int i = 0; i < genericIndividualList.Count; i++)
                {
                    if (!genericIndividualList[i].gameObject.activeSelf)
                    {
                        return genericIndividualList[i];
                    }
                }
            }

            return null;
        }
    }
}
