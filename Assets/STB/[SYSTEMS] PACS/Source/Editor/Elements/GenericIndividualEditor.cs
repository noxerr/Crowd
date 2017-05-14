using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;

namespace STB.PACS
{
    [CustomEditor(typeof(GenericIndividual))]
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Class: GenericIndividualEditor
    /// # Editor handle for GenericIndividual
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class GenericIndividualEditor : Editor
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// OnInspectorGUI
        /// # To do on inspector GUI
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////	
        public override void OnInspectorGUI()
        {
            GenericIndividual actualGenericIndividual = (GenericIndividual)target;

            EditorGUILayout.Separator();

            actualGenericIndividual.baseLife = EditorGUILayout.Slider("Base life ", actualGenericIndividual.baseLife, 0, 100);
            actualGenericIndividual.gender = (STB.PACS.GenericIndividual.cGender)EditorGUILayout.EnumPopup("Gender ", actualGenericIndividual.gender);

            EditorGUILayout.Separator();

            actualGenericIndividual.baseRotationRange = EditorGUILayout.Vector2Field("Base rotation range ", actualGenericIndividual.baseRotationRange);

            EditorGUILayout.Separator();

            actualGenericIndividual.idleProbability = EditorGUILayout.Slider("Idle probability ", actualGenericIndividual.idleProbability, 0, 100);
            actualGenericIndividual.walkingrobability = EditorGUILayout.Slider("Walk probability ", actualGenericIndividual.walkingrobability, 0, 100);
            actualGenericIndividual.runningProbability = EditorGUILayout.Slider("Run probability ", actualGenericIndividual.runningProbability, 0, 100);

            EditorGUILayout.Separator();

            actualGenericIndividual.idleStateTimeRange = EditorGUILayout.Vector2Field("Idle state time range ", actualGenericIndividual.idleStateTimeRange);
            actualGenericIndividual.walkingStateTimeRange = EditorGUILayout.Vector2Field("Walk state time range ", actualGenericIndividual.walkingStateTimeRange);
            actualGenericIndividual.runningStateTimeRange = EditorGUILayout.Vector2Field("Run state time range ", actualGenericIndividual.runningStateTimeRange);

            EditorGUILayout.Separator();

            actualGenericIndividual.defaultBehavior = (GenericIndividual.cBehavior)EditorGUILayout.EnumPopup("Default behavior ", actualGenericIndividual.defaultBehavior);

            actualGenericIndividual.braveProbability = EditorGUILayout.Slider("Brave probability ", actualGenericIndividual.braveProbability, 0, 100);
            actualGenericIndividual.fearfulgrobability = EditorGUILayout.Slider("Fear probability ", actualGenericIndividual.fearfulgrobability, 0, 100);
            actualGenericIndividual.neutralProbability = EditorGUILayout.Slider("Neutral probability ", actualGenericIndividual.neutralProbability, 0, 100);

            EditorGUILayout.Separator();

            actualGenericIndividual.braveRunningDistance = EditorGUILayout.Slider("Brave run distance ", actualGenericIndividual.braveRunningDistance, 0, 100);
            actualGenericIndividual.braveWalkingDistance = EditorGUILayout.Slider("Brave walk distance ", actualGenericIndividual.braveWalkingDistance, 0, 100);

            if (actualGenericIndividual.braveRunningDistance <= actualGenericIndividual.braveWalkingDistance)
            {
                actualGenericIndividual.braveRunningDistance = actualGenericIndividual.braveWalkingDistance;
            }

            EditorGUILayout.Separator();

            actualGenericIndividual.walkingSpeed = EditorGUILayout.Slider("Walking speed ", actualGenericIndividual.walkingSpeed, 0, 100);
            actualGenericIndividual.runningSpeed = EditorGUILayout.Slider("Running speed", actualGenericIndividual.runningSpeed, 0, 100);
            actualGenericIndividual.attackingSpeed = EditorGUILayout.Slider("Attacking speed", actualGenericIndividual.attackingSpeed, 0, 100);
            actualGenericIndividual.runningFromAttackerSpeed = EditorGUILayout.Slider("Running from attacker speed", actualGenericIndividual.runningFromAttackerSpeed, 0, 100);
            actualGenericIndividual.scaredSpeed = EditorGUILayout.Slider("Scared speed", actualGenericIndividual.scaredSpeed, 0, 100);

            EditorGUILayout.Separator();

            actualGenericIndividual.showDebug = EditorGUILayout.Toggle("Show debug: ", actualGenericIndividual.showDebug);

            EditorGUILayout.Separator();

            GUIStyle boldtext = EditorBasicFunctions.GetBoldTextGUIStyle();

            EditorGUILayout.LabelField("Information...", boldtext);

            if (actualGenericIndividual.targetPathPoint) EditorGUILayout.LabelField("· Target point: " + actualGenericIndividual.targetPathPoint.name);
            else EditorGUILayout.LabelField("· Target point: None");

            EditorGUILayout.LabelField("· Actual state: " + actualGenericIndividual.state);


            // for extended
            OnInspectorGUIExtended();

            // on gui change
            if (GUI.changed)
            {
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// OnInspectorGUIExtended -- VIRTUAL
        /// # To do on inspector GUI
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////	
        public virtual void OnInspectorGUIExtended()
        {
            //PathLineDrawer actualPathLineDrawer = (PathLineDrawer)target;

            if (GUI.changed)
            {
            }
        }
    }
}
