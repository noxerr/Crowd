using UnityEngine;
using System.Collections;

namespace STB.Generic
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Class: GenericObjectBase
    /// # Handles the base activation of everything in the system depending on given parameters
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class GenericObjectBase : GenericBaseOfAll
    {
        // public
        public string hudName = "GENERIC OBJECT";
        public float manualWorkingDistance = 40;
        public Vector2 heightWorkingRange = new Vector2(-99999, 99999);

        // public
#if UNITY_EDITOR
        public bool showDebug = false;
#endif

        // public -- hidden
        [HideInInspector]
        public bool temporalInfiniteWorkingDistance = false;

        // private enum
        enum cWorkingState { undefined, enabled, disabled };

        // private -- performance
        cWorkingState workingState = cWorkingState.undefined;


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Start
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void Start()
        {
            StartExtended();
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// WorkingStateString
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
#if UNITY_EDITOR
        public string WorkingStateString
        {
            get { return workingState.ToString(); }
        }
#endif
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// StartExtended -- VIRTUAL
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected virtual void StartExtended()
        {
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// CanBeEnabled -- VIRTUAL
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected virtual bool CanBeEnabled()
        {
            return true;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Kill -- VIRTUAL
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual void Kill()
        {
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Resurrect -- VIRTUAL
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual void Resurrect(string zone)
        {
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// GetWorkingDistance -- VIRTUAL
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual float GetWorkingDistance()
        {
            return manualWorkingDistance;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// HandleWorkingState
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void HandleWorkingState()
        {
            Camera actualCamera = Camera.main;
            if (STB.Basics.WeAreTheMainCamera.RealCamera) actualCamera = STB.Basics.WeAreTheMainCamera.RealCamera;
            if (!actualCamera) return;

            bool haveToBeEnabled = CanBeEnabled() && (Vector3.Distance(actualCamera.transform.position, this.transform.position) < GetWorkingDistance()) && (this.transform.position.y > heightWorkingRange.x) && (this.transform.position.y < heightWorkingRange.y);

            switch (workingState)
            {
                case cWorkingState.undefined:
                    {
                        if (haveToBeEnabled)
                        {
                            Resurrect("HandleWorkingState A");
                            workingState = cWorkingState.enabled;

                            this.gameObject.SetActive(true);
                        }
                        else
                        {
                            workingState = cWorkingState.disabled;

                            this.gameObject.SetActive(false);
                        }
                    }
                    break;

                case cWorkingState.disabled:
                    {
                        if (haveToBeEnabled)
                        {
                            Resurrect("HandleWorkingState B");

                            this.gameObject.SetActive(true);
                            workingState = cWorkingState.enabled;
                        }
                        else this.gameObject.SetActive(false);
                    }
                    break;

                case cWorkingState.enabled:
                    {
                        if (!haveToBeEnabled)
                        {
                            this.gameObject.SetActive(false);
                            workingState = cWorkingState.disabled;

                            //Debug.Log("--------------------> disabling " + this.gameObject.name);
                        }
                        else this.gameObject.SetActive(true);
                    }
                    break;
            }
        }
    }
}
