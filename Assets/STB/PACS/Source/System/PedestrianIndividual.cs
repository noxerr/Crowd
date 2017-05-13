using UnityEngine;
using System.Collections;

namespace STB.PACS
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Class: PedestrianIndividual
    /// # Extended class for pedestrian handle
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class PedestrianIndividual : GenericIndividual
    {
        // private
        Transform Character = null;
        Animator actualAnimator = null;
        CharacterController actualController = null;

        // private
        string actualAnimation = "Idle";
        float handlingHitReceivement = 0;
        string animationBeforeHit = "Idle";

        // private
        Vector3 originalScale;


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// AwakeExtended -- OVERRIDED
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected override void AwakeExtended()
        {
            Character = Basics.BasicFunctions.GetTransformInChildsByName(this.transform, "Character");

            actualAnimator = Character.GetComponent<Animator>();

            actualController = this.GetComponent<CharacterController>();

            Basics.BasicFunctions.DeactivateChildsRigidBody(this.transform);

            originalScale = this.transform.localScale;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// GetActualAnimationIsPlaying
        /// # Return true if the actual animation is playing
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        bool GetActualAnimationIsPlaying()
        {
            if (actualAnimator)
            {
                return actualAnimator.IsInTransition(0);
            }

            return false;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// GoToIdleExtended -- OVERRIDE
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected override void GoToIdleExtended()
        {
            actualAnimation = "Idle";
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// GoToWalkExtended
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected override void GoToWalkExtended()
        {
            actualAnimation = "Walk";
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// GoToRunExtended -- OVERRIDE
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected override void GoToRunExtended()
        {
            actualAnimation = "Run";
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// GoToScaredExtended -- OVERRIDED
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected override void GoToScaredExtended()
        {
            actualAnimation = "Run";
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// DoOnBraveStateExtended -- OVERRIDED
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected override void DoOnBraveStateExtended()
        {
            if (targetToAttack)
            {
                float distanceToTarget = Vector3.Distance(this.transform.position, targetToAttack.transform.position);

                //Debug.Log("distanceToTarget: " + distanceToTarget);

                if (distanceToTarget > braveRunningDistance) actualAnimation = "Run";
                else if (distanceToTarget > braveWalkingDistance) actualAnimation = "Walk";
                else actualAnimation = "Attack_1";
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// DoOnFearFulStateExtended -- OVERRIDED
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected override void DoOnFearFulStateExtended()
        {
            actualAnimation = "Run";
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// DoIgnorePedestriansColliderList
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void DoIgnorePedestriansColliderList(bool resurrect)
        {
            if (IndividualHandler.Singleton && IndividualHandler.Singleton.ignorePedestriansCollidersEnabled)
            {
                IgnorePedestriansCollider[] ignorePedestriansColliderList = GameObject.FindObjectsOfType<IgnorePedestriansCollider>();

                for (int i = 0; i < ignorePedestriansColliderList.Length; i++)
                {
                    //Debug.Log("ignorePedestriansColliderList A -> " + ignorePedestriansColliderList[i].gameObject.name + " with " + this.gameObject.GetComponent<CharacterController>().name);

                    if (ignorePedestriansColliderList[i].ignoreEnabled && ignorePedestriansColliderList[i].GetComponent<Collider>())
                    {
                        //Debug.Log("ignorePedestriansColliderList B -> " + ignorePedestriansColliderList[i].gameObject.name + " with " + this.gameObject.GetComponent<CharacterController>().name);

                        Basics.BasicFunctions.IgnoreCollisionsWith(this.transform, ignorePedestriansColliderList[i].GetComponent<Collider>(), false);
                    }
                }
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// GoToDeadExtended -- OVERRIDED
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected override void GoToDeadExtended()
        {
            IndividualHandler.Singleton.NewIndividualKilling(this.gameObject, hudName);
            IndividualHandler.Singleton.PlayScreamSoundByGender(gender);

            Basics.BasicFunctions.ActivateChildsRigidBody(this.transform);

            actualAnimator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
            actualAnimator.enabled = false;

            actualController.enabled = false;

            DoIgnorePedestriansColliderList(false);

        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// ResurrectExtended -- OVERRRIDED
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected override void ResurrectExtended()
        {
            Basics.BasicFunctions.DeactivateChildsRigidBody(this.transform);

            actualAnimator.cullingMode = AnimatorCullingMode.CullCompletely;
            actualAnimator.enabled = true;

            if (IndividualHandler.Singleton)
            {
                this.transform.localScale = IndividualHandler.Singleton.individualsScaleMultiplier * originalScale;

                if (IndividualHandler.Singleton.specialCollisionsMode)
                {
                    bool weNeedToCreateSpecialCollider = true;

                    if (this.gameObject.GetComponent<BoxCollider>() && this.gameObject.GetComponent<BoxCollider>().isTrigger) weNeedToCreateSpecialCollider = false;

                    if (weNeedToCreateSpecialCollider)
                    {
                        BoxCollider bc = this.gameObject.AddComponent<BoxCollider>();
                        bc.size = new Vector3(1, 4, 1);
                        bc.isTrigger = true;
                    }
                }

                actualController.enabled = IndividualHandler.Singleton.pedestrianCollisionsEnabled && !IndividualHandler.Singleton.specialCollisionsMode;
            }
            else actualController.enabled = true;

            DoIgnorePedestriansColliderList(true);
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// UpdateExtended -- OVERRIDED
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected override void UpdateExtended()
        {
            //if (!GetActualAnimationIsPlaying()) animator.CrossFade(actualAnimation, 0.25f * Time.deltaTime);

            if (handlingHitReceivement > 0)
            {
                if (!GetActualAnimationIsPlaying())
                {
                    handlingHitReceivement -= Time.deltaTime;

                    if (handlingHitReceivement <= 0)
                    {
                        //Debug.Log("handlingHitReceivement to false");
                        actualAnimation = animationBeforeHit;
                    }
                }
            }
            else if (!GetActualAnimationIsPlaying()) actualAnimator.Play(actualAnimation);
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// ReceiveHitExtended -- OVERRIDED
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected override void ReceiveHitExtended(Generic.GenericKiller realKiller, Vector3 origin, Vector3 direction, float damage, float speed)
        {
            if ((actualLife > 0) && (handlingHitReceivement <= 0))
            {
                handlingHitReceivement = 0.3f;

                animationBeforeHit = actualAnimation;

                if (Random.Range(0, 100) < 50) actualAnimation = "Shooted_1";
                else actualAnimation = "Shooted_2";

                actualAnimator.StopPlayback();
                actualAnimator.Play(actualAnimation);
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// GetSpecialNoMovementState -- OVERRIDED
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected override bool GetSpecialNoMovementState()
        {
            return (handlingHitReceivement > 0);
        }
    }
}
