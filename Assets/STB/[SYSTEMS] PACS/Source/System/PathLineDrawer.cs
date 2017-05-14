#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace STB.PACS
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Class: PathLineDrawer
    /// # To draw the path lines and points in the editor view inside
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    [ExecuteInEditMode]
    public class PathLineDrawer : MonoBehaviour
    {
        // public
        public bool individualEdition = false;
        public bool autocloseIfItsPossible = true;

        // public -- color options
        public Color normalPathPointsColor = Color.red;
        public Color normalPathColor = Color.blue;
        public Color normalArrowsColor = Color.red;
        public Color specialPathPointsColor = Color.blue;
        public Color specialPathColor = Color.yellow;
        public Color specialArrowsColor = Color.red;
        public Color borderColor = Color.black;

        // public
        public float pointSize = 2;


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Awake
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void Awake()
        {
            EditorApplication.update += Update;

#if !UNITY_EDITOR
            this.enabled = false;
#endif
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// OnDrawGizmos
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void OnDrawGizmos()
        {
            DrawGizmos();
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// OnDrawGizmosSelected
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void OnDrawGizmosSelected()
        {
            DrawGizmos();
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// DrawGizmos
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void DrawGizmos()
        {
            List<IndividualPathPoint> IndividualPathPointList = new List<IndividualPathPoint>();

            //Debug.Log("Draw Lines for " + this.name);

            // find childs
            foreach (Transform c in this.transform)
            {
                if (c.GetComponent<IndividualPathPoint>())
                {
                    IndividualPathPointList.Add(c.GetComponent<IndividualPathPoint>());
                }
                else
                {
                    //Debug.Log("WARNING: " + c.name + " is not a IndividualPathPoint but it's inside a IndividualPath " + this.name);
                }
            }

            // draw lines
            for (int i = 0; i < IndividualPathPointList.Count; i++)
            {
                if (IndividualPathPointList[i])
                {
                    if (IndividualPathPointList[i].type == IndividualPathPoint.cType.normal)
                    {
                        Handles.color = borderColor;
#if UNITY_5_5_OR_NEWER
                        Handles.CubeHandleCap(0, IndividualPathPointList[i].transform.position, Quaternion.identity, pointSize + 0.2f, EventType.repaint);
#else
                        Handles.CubeCap(0, IndividualPathPointList[i].transform.position, Quaternion.identity, pointSize + 0.2f);
#endif

                        Handles.color = normalPathPointsColor;
#if UNITY_5_5_OR_NEWER
                        Handles.CubeHandleCap(0, IndividualPathPointList[i].transform.position, Quaternion.identity, pointSize, EventType.repaint);
#else
                        Handles.CubeCap(0, IndividualPathPointList[i].transform.position, Quaternion.identity, pointSize);
#endif
                    }
                    else if (IndividualPathPointList[i].type == IndividualPathPoint.cType.special)
                    {
                        Handles.color = borderColor;
#if UNITY_5_5_OR_NEWER
                        Handles.CubeHandleCap(0, IndividualPathPointList[i].transform.position, Quaternion.identity, pointSize+0.2f, EventType.repaint);
#else
                        Handles.CubeCap(0, IndividualPathPointList[i].transform.position, Quaternion.identity, pointSize + 0.2f);
#endif

                        Handles.color = specialPathPointsColor;
#if UNITY_5_5_OR_NEWER
                        Handles.CubeHandleCap(0, IndividualPathPointList[i].transform.position, Quaternion.identity, pointSize, EventType.repaint);
#else
                        Handles.CubeCap(0, IndividualPathPointList[i].transform.position, Quaternion.identity, pointSize);
#endif
                    }

                    if (IndividualPathPointList[i].LinkedNormalIndividualPoint)
                    {
                        Vector3 p1 = IndividualPathPointList[i].transform.position;
                        Vector3 p2 = IndividualPathPointList[i].LinkedNormalIndividualPoint.transform.position;

                        if (p1.Equals(p2)) p2 += new Vector3(0, 0, 0.001f);

                        //Debug.Log("p1: " + p1);
                        //Debug.Log("p2: " + p2);

                        Vector3[] pointsArray = new[] { p1, p2 };

                        Handles.color = normalPathColor;
                        Handles.DrawAAPolyLine(6, pointsArray);

                        Handles.color = borderColor;
#if UNITY_5_5_OR_NEWER
                        Handles.ArrowHandleCap(0, p1, Quaternion.LookRotation(p2 - p1), 4.1f, EventType.repaint);
#else
                        Handles.ArrowCap(0, p1, Quaternion.LookRotation(p2 - p1), 4.1f);
#endif

                        Handles.color = normalArrowsColor;
#if UNITY_5_5_OR_NEWER
                        Handles.ArrowHandleCap(0, p1, Quaternion.LookRotation(p2 - p1), 4, EventType.repaint);
#else
                        Handles.ArrowCap(0, p1, Quaternion.LookRotation(p2 - p1), 4);
#endif
                    }

                    for (int j = 0; j < IndividualPathPointList[i].LinkedSpecialIndividualPathPointList.Count; j++)
                    {
                        if (IndividualPathPointList[i].LinkedSpecialIndividualPathPointList[j])
                        {
                            Vector3 p1 = IndividualPathPointList[i].transform.position;
                            Vector3 p2 = IndividualPathPointList[i].LinkedSpecialIndividualPathPointList[j].transform.position;

                            if (p1.Equals(p2)) p2 += new Vector3(0, 0, 0.001f);

                            //Debug.Log("p1: " + p1);
                            //Debug.Log("p2: " + p2);

                            Vector3[] pointsArray = new[] { p1, p2 };

                            Handles.color = borderColor;
                            Handles.DrawAAPolyLine(6.1f, pointsArray);

                            Handles.color = specialPathColor;
                            Handles.DrawAAPolyLine(6, pointsArray);

                            Handles.color = specialArrowsColor;
#if UNITY_5_5_OR_NEWER
                        Handles.ArrowHandleCap(0, p1, Quaternion.LookRotation(p2 - p1), 2, EventType.repaint);
#else
                            Handles.ArrowCap(0, p1, Quaternion.LookRotation(p2 - p1), 2);
#endif

                            Handles.color = borderColor;
#if UNITY_5_5_OR_NEWER
                        Handles.ArrowHandleCap(0, p1, Quaternion.LookRotation(p2 - p1), 4.1f, EventType.repaint);
#else
                            Handles.ArrowCap(0, p1, Quaternion.LookRotation(p2 - p1), 4.1f);
#endif

                            Handles.color = normalArrowsColor;
#if UNITY_5_5_OR_NEWER
                        Handles.ArrowHandleCap(0, p1, Quaternion.LookRotation(p2 - p1), 4, EventType.repaint);
#else
                            Handles.ArrowCap(0, p1, Quaternion.LookRotation(p2 - p1), 4);
#endif
                        }
                        else
                        {
                            IndividualPathPointList[i].LinkedSpecialIndividualPathPointList.RemoveAt(j);
                        }
                    }

                    if (Selection.activeGameObject && Selection.activeGameObject.Equals(IndividualPathPointList[i].gameObject))
                    {
                        GUIStyle labelStyle = new GUIStyle(GUI.skin.button);
                        labelStyle.fontSize = 9;

                        Handles.Label(IndividualPathPointList[i].transform.position + 6f * IndividualPathPointList[i].transform.forward, IndividualPathPointList[i].name, labelStyle);
                    }
                }
            }
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
#endif
