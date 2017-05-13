using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace STB.PACS
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Class: IndividualPath
    /// # Base class to handle individual paths
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
#if UNITY_EDITOR
    [RequireComponent(typeof(PathLineDrawer))]
#endif
    public class IndividualPath : MonoBehaviour
    {
        // private static
        static int PATH_INDEX = 0;
        static List<GenericIndividualPool> GenericIndividualPoolList = new List<GenericIndividualPool>();

        // public
        public int density = 9;
        public bool autoHalfWayPoints = false;
        public bool recalculatePathPivotUsingChildPoints = true;

        // public
        public List<GenericIndividual> IndividualPrefabList = new List<GenericIndividual>();

        // public
        public float pointsRandomVariation = 0;

        // public
        public bool bidirectional = false;

        // private
        List<IndividualPathPoint> NormalIndividualPathPointList = new List<IndividualPathPoint>();
        List<IndividualPathPoint> SpecialIndividualPathPointList = new List<IndividualPathPoint>();


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Start
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void Start()
        {
            PATH_INDEX++;

            if (IndividualPrefabList.Count <= 0) Debug.Log("WARNING: There are no individual prefabs in this path");

            // disable path drawing if we want
#if UNITY_EDITOR
            if (!IndividualHandler.Singleton.drawPathsInRealTime)
            {
                this.gameObject.GetComponent<PathLineDrawer>().enabled = false;
            }
#endif

            density = Mathf.Clamp(density, 0, (int)IndividualHandler.Singleton.maxDensity);

            // find childs and set this path to them
            foreach (Transform c in this.transform)
            {
                if (c.GetComponent<IndividualPathPoint>())
                {
                    if (c.GetComponent<IndividualPathPoint>().type == IndividualPathPoint.cType.normal) NormalIndividualPathPointList.Add(c.GetComponent<IndividualPathPoint>());
                    else if (c.GetComponent<IndividualPathPoint>().type == IndividualPathPoint.cType.special) SpecialIndividualPathPointList.Add(c.GetComponent<IndividualPathPoint>());

                    c.GetComponent<IndividualPathPoint>().linkedPath = this;
                }
                else Debug.Log("WARNING: " + c.name + " is not a IndividualPathPoint but it's inside a IndividualPath " + this.name);
            }

            // create half way points to match density if possible
            CreateHalfWayPointsToMatchDensityIfPossible();


            //Debug.Log("INFO: There are " + NormalIndividualPathPointList.Count.ToString() + " normal path points inside IndividualPath " + this.name);
            //Debug.Log("INFO: There are " + SpecialIndividualPathPointList.Count.ToString() + " special path points inside IndividualPath " + this.name);            

            if (IndividualPrefabList.Count > 0)
            {
                for (int i = 0; i < IndividualPrefabList.Count; i++)
                {
                    bool poolExists = false;

                    for (int j = 0; j < GenericIndividualPoolList.Count; j++)
                    {
                        if (GenericIndividualPoolList[j].basePrefab.Equals(IndividualPrefabList[i]))
                        {
                            poolExists = true;
                            GenericIndividualPoolList[j].AddLinkedIndividualPath(this);
                            break;
                        }
                    }

                    if (!poolExists)
                    {
                        GenericIndividualPool actualGcp = new GenericIndividualPool(IndividualPrefabList[i], this);
                        actualGcp.AddLinkedIndividualPath(this);

                        GenericIndividualPoolList.Add(actualGcp);
                    }
                }
            }
            else
            {
                Debug.Log("WARNING: There are no individual prefab list for this IndividualPath " + this.name);
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// CalculateCentroid
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void CalculateCentroid()
        {
            List<Transform> totalChildPoints = new List<Transform>();

            foreach (Transform c in this.transform)
            {
                if (c.GetComponent<IndividualPathPoint>())
                {
                    totalChildPoints.Add(c);
                }
            }

            // calculate path center
            Vector3 centroid = STB.Basics.BasicFunctions.GetCentroid(totalChildPoints);

            //GameObject centroidGO = new GameObject("Centroid");
            //centroidGO.transform.position = centroid;

            // repositionate path to center it using all childs centroid as pivot
            for (int i = 0; i < totalChildPoints.Count; i++) totalChildPoints[i].parent = null;

            this.transform.position = centroid;

            for (int i = 0; i < totalChildPoints.Count; i++) totalChildPoints[i].parent = this.transform;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// CreateHalfWayPointsToMatchDensityIfPossible
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void CreateHalfWayPointsToMatchDensityIfPossible()
        {
            if (autoHalfWayPoints && (NormalIndividualPathPointList.Count > 0))
            {
                while (density >= NormalIndividualPathPointList.Count)
                {
                    CreateHalfWayPoints(NormalIndividualPathPointList[Random.Range(0, NormalIndividualPathPointList.Count)]);

                    // recalculate
                    NormalIndividualPathPointList.Clear();
                    SpecialIndividualPathPointList.Clear();

                    // find childs and set this path to them
                    foreach (Transform c in this.transform)
                    {
                        if (c.GetComponent<IndividualPathPoint>())
                        {
                            if (c.GetComponent<IndividualPathPoint>().type == IndividualPathPoint.cType.normal) NormalIndividualPathPointList.Add(c.GetComponent<IndividualPathPoint>());
                            else if (c.GetComponent<IndividualPathPoint>().type == IndividualPathPoint.cType.special) SpecialIndividualPathPointList.Add(c.GetComponent<IndividualPathPoint>());

                            c.GetComponent<IndividualPathPoint>().linkedPath = this;
                        }
                        else Debug.Log("WARNING: " + c.name + " is not a IndividualPathPoint but it's inside a IndividualPath " + this.name);
                    }
                }
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// CreateHalfWayPoint
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void CreateHalfWayPoints(IndividualPathPoint actualCpp)
        {
            if (actualCpp && (actualCpp.type == IndividualPathPoint.cType.normal))
            {
                List<IndividualPathPoint> NormalIndividualPathPointList = new List<IndividualPathPoint>();

                IndividualPath actualPointPath = BasicFunctions.GetParentIndividualPath(actualCpp.transform);

                foreach (Transform c in actualPointPath.transform)
                {
                    if (c.GetComponent<IndividualPathPoint>())
                    {
                        if (c.GetComponent<IndividualPathPoint>().type == IndividualPathPoint.cType.normal) NormalIndividualPathPointList.Add(c.GetComponent<IndividualPathPoint>());
                    }
                }

                NormalIndividualPathPointList = NormalIndividualPathPointList.OrderBy(x => x.name).ToList();

                int actuaCppIndex = NormalIndividualPathPointList.IndexOf(actualCpp);

                IndividualPathPoint newCpp = Instantiate(actualCpp);
#if UNITY_EDITOR
                newCpp.showDebug = false;
#endif
                newCpp.LinkedNormalIndividualPoint = null;
                newCpp.LinkedNormalIndividualPointReverse = null;
                newCpp.LinkedSpecialIndividualPathPointList.Clear();
                newCpp.transform.parent = actualPointPath.transform;

                if (actualCpp.LinkedNormalIndividualPoint)
                {
                    //Debug.Log(actualCpp.LinkedNormalIndividualPoint + " -> CreateHalfWayPoints -> there is linked point");

                    newCpp.transform.position = (actualCpp.transform.position + actualCpp.LinkedNormalIndividualPoint.transform.position) / 2;
                }
                else
                {
                    Debug.Log(actualCpp.LinkedNormalIndividualPoint + " -> CreateHalfWayPoints -> linked point missing");

                    newCpp.transform.position = actualCpp.transform.position + 4 * actualCpp.transform.forward;
                }

                NormalIndividualPathPointList.Insert(actuaCppIndex + 1, newCpp);

                for (int i = 0; i < NormalIndividualPathPointList.Count; i++)
                {
                    NormalIndividualPathPointList[i].gameObject.name = BasicDefines.NORMAL_INDIVIDUAL_PATH_POINT_PREFIX + Basics.BasicFunctions.GetStringNormalizedCounterForMiles(i + 1);
                }

                // autoclose the path if its possible
                AutocloseThePath();
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// CloseActualPath
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void AutocloseThePath()
        {
#if UNITY_EDITOR
            if (this && this.GetComponent<PathLineDrawer>() && this.GetComponent<PathLineDrawer>().autocloseIfItsPossible)
#endif
            {
                CloseActualPath();
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// CloseActualPath
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void CloseActualPath()
        {
            //Debug.Log("Close actual path");

            List<IndividualPathPoint> individualPathPointList = new List<IndividualPathPoint>();

            foreach (Transform cpp in this.transform)
            {
                if (cpp.GetComponent<IndividualPathPoint>() && (cpp.GetComponent<IndividualPathPoint>().type == IndividualPathPoint.cType.normal))
                {
                    individualPathPointList.Add(cpp.GetComponent<IndividualPathPoint>());
                }
            }

            //Debug.Log("individualPathPointList count: " + individualPathPointList.Count);

            if (individualPathPointList.Count > 1)
            {
                individualPathPointList = individualPathPointList.OrderBy(x => x.name).ToList();

                for (int i = 0; i < individualPathPointList.Count - 1; i++)
                {
                    individualPathPointList[i].LinkedNormalIndividualPoint = individualPathPointList[i + 1];
                    individualPathPointList[i + 1].LinkedNormalIndividualPointReverse = individualPathPointList[i];
                }

                individualPathPointList[individualPathPointList.Count - 1].LinkedNormalIndividualPoint = individualPathPointList[0];
                individualPathPointList[0].LinkedNormalIndividualPointReverse = individualPathPointList[individualPathPointList.Count - 1];
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// OnDestroy
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void OnDestroy()
        {
            PATH_INDEX = 0;
            GenericIndividualPoolList.Clear();
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// ShuffleGenericIndividualPoolList
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static List<IndividualPathPoint> ShuffleGenericIndividualPathPointList(List<IndividualPathPoint> aList)
        {
            System.Random _random = new System.Random();

            IndividualPathPoint myGO;

            int n = aList.Count;
            for (int i = 0; i < n; i++)
            {
                // NextDouble returns a random number between 0 and 1.
                // ... It is equivalent to Math.random() in Java.
                int r = i + (int)(_random.NextDouble() * (n - i));
                myGO = aList[r];
                aList[r] = aList[i];
                aList[i] = myGO;
            }

            return aList;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// ShuffleGenericIndividualPoolList
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static List<GenericIndividualPool> ShuffleGenericIndividualPoolList(List<GenericIndividualPool> aList)
        {
            System.Random _random = new System.Random();

            GenericIndividualPool myGO;

            int n = aList.Count;
            for (int i = 0; i < n; i++)
            {
                // NextDouble returns a random number between 0 and 1.
                // ... It is equivalent to Math.random() in Java.
                int r = i + (int)(_random.NextDouble() * (n - i));
                myGO = aList[r];
                aList[r] = aList[i];
                aList[i] = myGO;
            }

            return aList;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// GetFreeGenericIndividualPool
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public GenericIndividual GetFreeGenericIndividualPool()
        {
            GenericIndividualPoolList = ShuffleGenericIndividualPoolList(GenericIndividualPoolList);

            for (int i = 0; i < GenericIndividualPoolList.Count; i++)
            {
                GenericIndividual gc = GenericIndividualPoolList[i].GetFreeGenericIndividualPool(this);

                if (gc) return gc;
            }

            return null;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Update
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void Update()
        {
        }
    }
}
