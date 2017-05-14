using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace STB.PACS
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Class: IndividualPathPoint
    /// # Base class to handle individual path points
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class IndividualPathPoint : STB.Generic.GenericPathPoint
    {
        // public enum
        public enum cType { normal, special };

        // public
        public int index = 0;
        public int probability = 100;
        public cType type = cType.normal;
        public IndividualPathPoint LinkedNormalIndividualPoint = null;
        public IndividualPathPoint LinkedNormalIndividualPointReverse = null;
        public List<IndividualPathPoint> LinkedSpecialIndividualPathPointList = new List<IndividualPathPoint>();
        public IndividualPath linkedPath = null;

        // public
#if UNITY_EDITOR
        public bool showDebug = false;
#endif

        // public
        public GenericIndividual linkedGenericIndividual = null;

        // private
        List<IndividualPathPoint> LinkedTotalIndividualPathPointList = new List<IndividualPathPoint>();
        int totalProbability = 0;

        // private
        Vector3 originalBoxColliderSize = Vector3.one;


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Awake
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void Awake()
        {
            if (LinkedNormalIndividualPoint) LinkedTotalIndividualPathPointList.Add(LinkedNormalIndividualPoint);

            for (int i = 0; i < LinkedSpecialIndividualPathPointList.Count; i++)
            {
                if (LinkedSpecialIndividualPathPointList[i]) LinkedTotalIndividualPathPointList.Add(LinkedSpecialIndividualPathPointList[i]);
            }

            // get total probability
            for (int i = 0; i < LinkedTotalIndividualPathPointList.Count; i++)
            {
                totalProbability += LinkedTotalIndividualPathPointList[i].probability;
            }

#if !UNITY_EDITOR
            if (this.GetComponent<Renderer>()) this.GetComponent<Renderer>().enabled = false;
#endif
            originalBoxColliderSize = this.GetComponent<BoxCollider>().size;

            if (!this.gameObject.GetComponent<Rigidbody>())
            {
                this.gameObject.AddComponent<Rigidbody>();
            }

            Rigidbody actualRigidBody = this.gameObject.GetComponent<Rigidbody>();
            actualRigidBody.isKinematic = true;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Start
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void Start()
        {
            if (IndividualHandler.Singleton) IndividualHandler.Singleton.individualPathPointList.Add(this);
            else Debug.Log("Warning: There is no IndividualHandler in the scene!");
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// GetRandomPathPoint
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public IndividualPathPoint GetRandomPathPoint()
        {
            if (LinkedTotalIndividualPathPointList.Count > 0)
            {
                int randomProbability = Random.Range(0, totalProbability);
                int actualProbability = 0;
                int actuaPathIndex = 0;

                for (int i = 0; i < LinkedTotalIndividualPathPointList.Count; i++)
                {
                    actualProbability += LinkedTotalIndividualPathPointList[i].probability;

                    if (randomProbability < actualProbability)
                    {
                        actuaPathIndex = i;

                        //if (LinkedTotalIndividualPathPointList.Count > 1)
                        //{
                        //Debug.Log("----------------------------------------------");
                        //Debug.Log("totalProbability: " + totalProbability);
                        //Debug.Log("actualProbability: " + actualProbability);
                        //Debug.Log("randomProbability: " + randomProbability);
                        //Debug.Log("actuaPathIndex: " + actuaPathIndex);
                        //}
                        break;
                    }
                }

                return LinkedTotalIndividualPathPointList[actuaPathIndex];
            }

            return null;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// HandlePool
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void HandlePool(bool force)
        {
            //Debug.Log("HandlePool -> " + this.name);

            Camera actualCamera = Camera.main;
            if (STB.Basics.WeAreTheMainCamera.RealCamera) actualCamera = STB.Basics.WeAreTheMainCamera.RealCamera;
            if (!actualCamera) return;

            if (linkedPath)
            {
                float pointsRadiousMultiplier = 1;

                Vector3 actualBoxColliderSize = originalBoxColliderSize;
                actualBoxColliderSize.y += linkedPath.pointsRandomVariation;
                actualBoxColliderSize *= pointsRadiousMultiplier;
                this.GetComponent<BoxCollider>().size = actualBoxColliderSize;
            }

            //Debug.Log("HandlePool for " + this.gameObject.name);

#if UNITY_EDITOR
            if (showDebug)
            {
                //if (linkedGenericIndividual) Debug.Log("linkedGenericIndividual: " + linkedGenericIndividual.gameObject.name);
                //if (linkedPath) Debug.Log("linkedPath: " + linkedPath);
            }
#endif
            if (linkedGenericIndividual && !linkedGenericIndividual.gameObject.activeSelf)
            {
                linkedGenericIndividual = null;
            }

            if (linkedPath)
            {
                float distanceToMainCamera = Vector3.Distance(actualCamera.transform.position, transform.position);

                if (linkedGenericIndividual)
                {
                    if (distanceToMainCamera >= IndividualHandler.Singleton.workingDistance)
                    {
                        linkedGenericIndividual.gameObject.SetActive(false);
                        linkedGenericIndividual = null;
                    }
                }
                else
                {
#if UNITY_EDITOR
                    if (showDebug)
                    {
                        //Debug.Log("distanceToMainCamera: " + distanceToMainCamera);
                        //Debug.Log("IndividualHandler.Singleton.workingDistance: " + IndividualHandler.Singleton.workingDistance);
                    }
#endif

                    if (force || (distanceToMainCamera < IndividualHandler.Singleton.workingDistance))
                    {
                        Vector3 actualSpecialOffset = Vector3.zero;

                        if (linkedPath) actualSpecialOffset = Random.Range(-linkedPath.pointsRandomVariation, linkedPath.pointsRandomVariation) * this.transform.right;

                        GenericIndividual gc = linkedPath.GetFreeGenericIndividualPool();

                        if (gc)
                        {
#if UNITY_EDITOR
                            //if (showDebug) Debug.Log(this.gameObject.name + " -> InitialiseIndividual gc: " + gc.name);

                            //gc.showDebug = showDebug;
#endif

                            gc.manualWorkingDistance = 100000;

#if UNITY_EDITOR
                            gc.InitialiseIndividual(this, actualSpecialOffset);
#else
                            gc.InitialiseIndividual(this, actualSpecialOffset, false);
#endif
                        }
                    }
                }
            }
        }
    }
}
