using UnityEngine;
using System.Collections;
#if UNITY_5_5_OR_NEWER
using UnityEngine.AI;
#endif

namespace STB.PACS
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Class: GenericIndividual
    /// # Base class to handle generic individual
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class GenericIndividual : GenericObject
    {
        // public enum -- states
        public enum cState { idle, walking, attacking, running, scared, runningFromAttacker, dead };

        public enum cGender { male, female, other };

        // public
        public float baseLife = 100;

        // public -- base rotation
        public Vector2 baseRotationRange;

        // public -- gender
        public cGender gender = cGender.other;

        // public -- state
        public cState state = cState.idle;

        // public -- state probability
        public float idleProbability = 20;
        public float walkingrobability = 75;
        public float runningProbability = 5;

        // private -- states probability
        float totalStatesProbability = 0;

        // public -- states time
        public Vector2 idleStateTimeRange = new Vector2(4, 9);
        public Vector2 walkingStateTimeRange = new Vector2(4, 9);
        public Vector2 runningStateTimeRange = new Vector2(4, 9);

        // public -- speeds
        public float walkingSpeed = 2;
        public float runningSpeed = 4;
        public float attackingSpeed = 4;
        public float runningFromAttackerSpeed = 4;
        public float scaredSpeed = 4;

        // private
        public IndividualPathPoint basePathPoint;
        public IndividualPathPoint targetPathPoint = null;

        // private
        float randomNormalStateCounter = 0;

        // private
        NavMeshAgent agent = null;
        ScaryZone lastScaryZone = null;
        Transform lastSafeZone = null;
        float scaredTime = 0;

        // protected 
        protected float actualLife;

        // public -- hidden
        [HideInInspector]
        public bool allowDebugApocalypse = false;

        // public enum -- behaviors
        public enum cBehavior { random, brave, fearful, neutral };

        // public -- behavior
        public cBehavior defaultBehavior = cBehavior.random;

        // private -- behavior
        cBehavior actualBehavior = cBehavior.neutral;

        // public -- behavior probability
        public float braveProbability = 40;
        public float fearfulgrobability = 40;
        public float neutralProbability = 10;

        // private -- states probability
        float totalBehaviorProbability = 0;

        // public -- behaviour distances
        public float braveRunningDistance = 6;
        public float braveWalkingDistance = 3;

        // protected
        protected Transform targetToAttack = null;

        // private
        float actualSpeed = 1;

        // private
        STB.Generic.GenericRendererBase genericRendererBase = null;


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Awake
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void Awake()
        {
            totalStatesProbability = idleProbability + walkingrobability + runningProbability;

            totalBehaviorProbability = braveProbability + fearfulgrobability + neutralProbability;

            agent = this.GetComponent<NavMeshAgent>();

            genericRendererBase = this.GetComponent<STB.Generic.GenericRendererBase>();

            if (!genericRendererBase)
            {
                genericRendererBase = this.gameObject.AddComponent<STB.Generic.GenericRendererBase>();
            }

            GoToRandomNormalState(false);

            Basics.BasicFunctions.SetIgnoreRaycastLayer(this.gameObject);


            AwakeExtended();
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// InitialiseIndividual
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void InitialiseIndividual(IndividualPathPoint cpp, Vector3 offset)
        {
            if (!basePathPoint || cpp)
            {
#if UNITY_EDITOR
                //if (showDebug) Debug.Log("InitialiseIndividual " + this.gameObject.name + " in " + cpp.linkedPath.name + " -> " + cpp.gameObject.name);
#endif

                basePathPoint = cpp;
                basePathPoint.linkedGenericIndividual = this;
                targetPathPoint = cpp.GetRandomPathPoint();

                //Debug.Log(this.name + " -> basePathPoint: " + basePathPoint.name + " -> targetPathPoint: " + targetPathPoint.name);

                agent = this.GetComponent<NavMeshAgent>();
                if (agent) agent.enabled = false;

                this.transform.position = basePathPoint.transform.position + offset;

                if (IndividualHandler.Singleton.noNavMeshHandle)
                {
                    this.transform.position += IndividualHandler.Singleton.fixedYForIndividuals * Vector3.up;
                }

                if (!targetPathPoint) this.transform.Rotate(Random.Range(baseRotationRange.x, baseRotationRange.y) * this.transform.up);

                if (agent) agent.enabled = true;


                if (!IndividualHandler.Singleton.FirstUpdate) genericRendererBase.HideRenderers();

                //Debug.Log("this.transform.position: " + this.transform.position);

                Resurrect("InitialiseIndividual");
                HandleWorkingState();
            }
            else
            {
                this.gameObject.SetActive(false);
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// OnDisable
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void OnDisable()
        {
            if (basePathPoint)
            {
                //Debug.Log(basePathPoint.gameObject.name + " -> " + this.gameObject.name + " -> linkedGenericIndividual = NULL");

                this.transform.position = Basics.BasicDefines.TOO_FAR_POSITION;
                basePathPoint.linkedGenericIndividual = null;
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// OnEnable
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void OnEnable()
        { }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// CanBeEnabled -- OVERRIDED
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected override bool CanBeEnabled()
        {
            return basePathPoint;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// OnTriggerStay
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void OnTriggerStay(Collider col)
        {
            //Debug.Log("col: " + col.name);

            if (targetPathPoint && col.gameObject.Equals(targetPathPoint.gameObject))
            {
                targetPathPoint = targetPathPoint.GetRandomPathPoint();
            }

            Generic.GenericKiller actualKiller = col.GetComponent<Generic.GenericKiller>();

            if (actualKiller)
            {
                ReceiveHit(actualKiller, col.transform.position, col.transform.forward, col.GetComponent<Generic.GenericKiller>().damage, col.GetComponent<Generic.GenericKiller>().speed);
            }

            //Debug.Log("col: " + col.name);

            //Debug.Log("OnTriggerEnterExtended-> col name: " + col.name);

            //if (col.GetComponent<NonStopZone>()) Debug.Log("we are on NonStopZone");

            if (col.GetComponent<GenericTrafficLight>())
            {
                if (col.GetComponent<GenericTrafficLight>().GetRed() && GetOnNormalMovement()) GoToIdle(false, 0);
                else if (!col.GetComponent<GenericTrafficLight>().GetRed() && !GetOnNormalMovement()) GoToWalk(false, 0);
            }
            else if (col.GetComponent<NonStopZone>() && !GetOnNormalMovement())
            {
                //Debug.Log("we are on NonStopZone and can't be idle");

                GoToWalk(false, 0);
            }
            else if (col.GetComponent<ScaryZone>())
            {
                GoToScared(col.GetComponent<ScaryZone>(), "ScaryZone -> OnTriggerStay");
            }


            OnTriggerStayExtended(col);
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// OnCollisionEnter
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void OnCollisionEnter(Collision col)
        {
            //Debug.Log("col.collider: " + col.collider.name);

            Generic.GenericKiller actualKiller = col.collider.GetComponent<Generic.GenericKiller>();

            if (actualKiller)
            {
                ReceiveHit(actualKiller, col.collider.transform.position, col.collider.transform.forward, col.collider.GetComponent<Generic.GenericKiller>().damage, col.collider.GetComponent<Generic.GenericKiller>().speed);
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// ReceiveHit
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void ReceiveHit(Generic.GenericKiller realKiller, Vector3 origin, Vector3 direction, float damage, float speed)
        {
            if (!IndividualHandler.Singleton) return;

            IndividualHandler.Singleton.ThrowNewScaryZone(this.transform.position);

            if (realKiller) targetToAttack = realKiller.realKillerTransform;
            else targetToAttack = null;

            actualLife -= damage;

            if (!GetDead())
            {
                if (actualLife <= 0)
                {
                    Kill();

                    bool applyHitSpeedBasedOnRadious = true;

                    if (realKiller)
                    {
                        //Debug.Log("realKiller: " + realKiller.name);
                        applyHitSpeedBasedOnRadious = realKiller.applyHitSpeedBasedOnRadious;
                    }

                    Basics.BasicFunctions.SetVelocityToAllChildsInRadiusOfPosition(this.transform, origin, direction, speed, 1, applyHitSpeedBasedOnRadious);
                }
                else
                {
                    if (defaultBehavior == cBehavior.neutral)
                    {
                    }
                    else if ((defaultBehavior == cBehavior.random) && (actualBehavior == cBehavior.neutral))
                    {
                        float r = Random.Range(0, totalBehaviorProbability);

                        if (r < fearfulgrobability) actualBehavior = cBehavior.fearful;
                        else if (r < fearfulgrobability + braveProbability) actualBehavior = cBehavior.brave;
                        else actualBehavior = cBehavior.neutral;
                    }
                    else actualBehavior = defaultBehavior;

                    if (!targetToAttack)
                    {
                        actualBehavior = cBehavior.neutral;
                    }
                }
            }

            //if (targetToAttack) Debug.Log("targetToAttack: " + targetToAttack.gameObject.name);
            //Debug.Log("actualBehavior: " + actualBehavior);

            ReceiveHitExtended(realKiller, origin, direction, damage, speed);
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// GetOnNormalMovement
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        bool GetOnNormalMovement()
        {
            switch (state)
            {
                case cState.walking:
                case cState.running:
                    return true;

                case cState.idle:
                case cState.attacking:
                case cState.runningFromAttacker:
                case cState.scared:
                case cState.dead:
                    return false;
            }

            return false;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// GetNormalState
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        bool GetNormalState()
        {
            switch (state)
            {
                case cState.idle:
                case cState.walking:
                case cState.running:
                case cState.attacking:
                case cState.runningFromAttacker:
                    return true;

                case cState.scared:
                case cState.dead:
                    return false;
            }

            return false;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// GetDead
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool GetDead()
        {
            return (state == cState.dead);
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// GoToIdle
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void GoToIdle(bool force, float forcedTime)
        {
            if (GetNormalState() || force)
            {
                //if (force) Debug.Log("Force GoToIdle");

                if (force) randomNormalStateCounter = forcedTime;

                lastScaryZone = null;
                lastSafeZone = null;

                state = cState.idle;

                GoToIdleExtended();
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// GoToWalk
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void GoToWalk(bool force, float forcedTime)
        {
            if (GetNormalState() || force)
            {
                if (force) randomNormalStateCounter = forcedTime;

                lastScaryZone = null;
                lastSafeZone = null;

                state = cState.walking;

                GoToWalkExtended();
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// GoToRun
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void GoToRun(bool force, float forcedTime)
        {
            if (GetNormalState() || force)
            {
                if (force) randomNormalStateCounter = forcedTime;

                lastScaryZone = null;
                lastSafeZone = null;

                state = cState.running;

                GoToRunExtended();
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// GoToSafeZone
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void GoToSafeZone(Transform sz, float timeLookingForSafeZone)
        {
            lastSafeZone = sz;
            scaredTime = timeLookingForSafeZone;

#if UNITY_EDITOR
            if (showDebug) Debug.Log("GoToSafeZone");
#endif

            state = cState.scared;

            GoToScaredExtended();
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// GoToScared
        /// </summary>
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void GoToScared(ScaryZone sz, string zone)
        {
            if (!GetDead())
            {
                if ((lastScaryZone != sz) || (lastScaryZone.transform.position != sz.transform.position))
                {
                    //Debug.Log("GoToScared B");

                    lastScaryZone = sz;
                    lastSafeZone = lastScaryZone.GetSafeZone();
                }

                scaredTime = lastScaryZone.scaredTime;

#if UNITY_EDITOR
                if (showDebug) Debug.Log("GoToSafeZone in " + zone);
#endif

                state = cState.scared;

                GoToScaredExtended();
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// GoToScared
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void GoToDead()
        {
            if (!GetDead())
            {
                //Debug.Log("GoToDead " + this.gameObject.name);

                actualBehavior = cBehavior.neutral;
                state = cState.dead;

                GoToDeadExtended();
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Kill -- OVERRRIDED
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override void Kill()
        {
            GoToDead();
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// GoToRandomNormalState
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void GoToRandomNormalState(bool force)
        {
            if (targetPathPoint)
            {
                if ((randomNormalStateCounter <= 0) || force)
                {
                    float r = Random.Range(0, totalStatesProbability);

                    if (r < idleProbability) GoToIdle(force, 0);
                    else if (r < idleProbability + walkingrobability) GoToWalk(force, 0);
                    else GoToRun(force, 0);

                    switch (state)
                    {
                        case cState.idle:
                            {
                                randomNormalStateCounter = Random.Range(idleStateTimeRange.x, idleStateTimeRange.y);
                                break;
                            }

                        case cState.walking:
                            {
                                randomNormalStateCounter = Random.Range(walkingStateTimeRange.x, walkingStateTimeRange.y);
                                break;
                            }

                        case cState.running:
                            {
                                randomNormalStateCounter = Random.Range(runningStateTimeRange.x, runningStateTimeRange.y);
                                break;
                            }

                        case cState.attacking:
                        case cState.runningFromAttacker:
                        case cState.dead:
                        case cState.scared:
                            {
                                break;
                            }
                    }
                }

                randomNormalStateCounter -= Time.deltaTime;
            }
            else
            {
                GoToIdle(force, 0);
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// HandleBehaviors
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void HandleBehaviors()
        {
            switch (actualBehavior)
            {
                case cBehavior.neutral:
                case cBehavior.random:
                    break;

                case cBehavior.brave:
                    {
                        state = cState.attacking;

                        DoOnBraveState();
                    }
                    break;

                case cBehavior.fearful:
                    {
                        state = cState.runningFromAttacker;

                        DoOnFearFulState();
                    }
                    break;
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// DoOnBraveState
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void DoOnBraveState()
        {
            DoOnBraveStateExtended();
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// DoOnFearFulState
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void DoOnFearFulState()
        {
            DoOnFearFulStateExtended();
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Update
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void Update()
        {
#if UNITY_EDITOR
            if (allowDebugApocalypse && Input.GetKeyDown(KeyCode.K)) Kill();
#endif

            HandleBehaviors();

            HandleStates();

            UpdateExtended();
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// GetAgentSpeedByState
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        float GetAgentSpeedByState()
        {
            if (!IndividualHandler.Singleton || GetSpecialNoMovementState()) return 0;

            switch (state)
            {
                case cState.idle:
                    return 0;

                case cState.walking:
                    return walkingSpeed * IndividualHandler.Singleton.individualsSpeedMultiplier;

                case cState.running:
                    return runningSpeed * IndividualHandler.Singleton.individualsSpeedMultiplier;

                case cState.attacking:
                    {
                        if (targetToAttack)
                        {
                            float distanceToTarget = Vector3.Distance(this.transform.position, targetToAttack.position);

                            return Mathf.Min(distanceToTarget, 4) * IndividualHandler.Singleton.individualsSpeedMultiplier;
                        }

                        return attackingSpeed * IndividualHandler.Singleton.individualsSpeedMultiplier;
                    }

                case cState.runningFromAttacker:
                    return runningFromAttackerSpeed * IndividualHandler.Singleton.individualsSpeedMultiplier;

                case cState.scared:
                    return scaredSpeed * IndividualHandler.Singleton.individualsSpeedMultiplier;

                case cState.dead:
                    return 0;
            }

            return 0;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// SetIndividualDestination
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void SetIndividualDestination(Vector3 position)
        {
            if (agent && agent.isActiveAndEnabled && agent.isOnNavMesh)
            {
                /*if (targetPathPoint && !forcePosition) agent.SetDestination(targetPathPoint.transform.position);
                else*/
                agent.SetDestination(position);
            }
            else if (IndividualHandler.Singleton && IndividualHandler.Singleton.noNavMeshHandle)
            {
                Vector3 lastPosition = this.transform.position;

                Vector3 actualTargetPosition = targetPathPoint.transform.position + IndividualHandler.Singleton.fixedYForIndividuals * Vector3.up;

                Vector3 actualPosition = Vector3.Slerp(this.transform.position, actualTargetPosition, 0.1f * actualSpeed * Time.deltaTime);

                Vector3 direction = (actualPosition - lastPosition).normalized;

                if (direction != Vector3.zero)
                {
                    this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime);
                }

                this.transform.position = actualPosition;
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// SetIndividualSpeed
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void SetIndividualSpeed(float speed)
        {
            if (agent && agent.isActiveAndEnabled && agent.isOnNavMesh)
            {
                agent.speed = speed;
            }

            actualSpeed = speed;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// HandleStates
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void HandleStates()
        {
#if UNITY_EDITOR 
            if (showDebug && targetPathPoint) Debug.Log("State: " + state + " -> Moving " + this.name + " to " + targetPathPoint.name);
#endif

            switch (state)
            {
                case cState.idle:
                case cState.walking:
                case cState.running:
                    {
                        if (targetPathPoint)
                        {
                            GoToRandomNormalState(false);

                            SetIndividualDestination(targetPathPoint.transform.position);

#if UNITY_EDITOR
                            if (showDebug) Debug.Log("targetPathPoint.position: " + targetPathPoint.transform.position.ToString());
#endif
                        }
                    }
                    break;

                case cState.attacking:
                    {
                        if (targetToAttack)
                        {
                            SetIndividualDestination(targetToAttack.transform.position);

#if UNITY_EDITOR
                            if (showDebug) Debug.Log("targetToAttack.position: " + targetToAttack.transform.position.ToString());
#endif
                        }
                        else
                        {
                            GoToRandomNormalState(true);
                        }
                    }
                    break;

                case cState.runningFromAttacker:
                    {
                        if (targetToAttack)
                        {
                            Vector3 scarePointBase = this.transform.position + 4 * (this.transform.position - targetToAttack.transform.position).normalized;

                            SetIndividualDestination(scarePointBase);

#if UNITY_EDITOR
                            if (showDebug) Debug.Log("targetToAttack.position: " + targetToAttack.transform.position.ToString());
#endif
                        }
                        else
                        {
                            GoToRandomNormalState(true);
                        }
                    }
                    break;

                case cState.scared:
                    {
                        if (scaredTime > 0)
                        {
                            scaredTime -= Time.deltaTime;
                        }
                        else
                        {
                            GoToRandomNormalState(true);
                        }

                        Vector3 scarePointBase = Basics.BasicDefines.TOO_FAR_POSITION;

                        if (lastScaryZone) scarePointBase = this.transform.position + 4 * (this.transform.position - lastScaryZone.transform.position).normalized;

                        if (lastSafeZone)
                        {
                            scarePointBase = lastSafeZone.position;
                        }

                        float distanceToSafeZone = Vector3.Distance(this.transform.position, lastSafeZone.position);

                        if (distanceToSafeZone > 4)
                        {
                            SetIndividualDestination(scarePointBase);
                        }
                        else
                        {
                            GoToIdle(true, 0);
                        }
                    }
                    break;

                case cState.dead:
                    {
                    }
                    break;
            }

            // set individual speed
            SetIndividualSpeed(GetAgentSpeedByState());
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// ManualResurrection
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void ManualResurrection()
        {
            Resurrect("ManualResurrection");
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// ManualDead
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void ManualDead()
        {
            GoToDead();
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Resurrect -- OVERRRIDED
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override void Resurrect(string zone)
        {
            //Debug.Log("Resurrecting " + this.gameObject.name);

            actualLife = baseLife;

            GoToRandomNormalState(true);

            actualBehavior = cBehavior.neutral;

            ResurrectExtended();
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// AwakeExtended -- VIRTUAL
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected virtual void AwakeExtended()
        {
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// UpdateExtended -- VIRTUAL
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected virtual void UpdateExtended()
        {
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// OnTriggerStayExtended -- VIRTUAL
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected virtual void OnTriggerStayExtended(Collider col)
        {
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// GoToIdleExtended -- VIRTUAL
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected virtual void GoToIdleExtended()
        {
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// GoToWalkExtended -- VIRTUAL
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected virtual void GoToWalkExtended()
        {
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// GoToRunExtended -- VIRTUAL
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected virtual void GoToRunExtended()
        {
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// GoToScaredExtended -- VIRTUAL
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected virtual void GoToScaredExtended()
        {
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// GoToDeadExtended -- VIRTUAL
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected virtual void GoToDeadExtended()
        {
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// ResurrectExtended -- VIRTUAL
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected virtual void ResurrectExtended()
        {
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// ReceiveHitExtended -- VIRTUAL
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected virtual void ReceiveHitExtended(Generic.GenericKiller realKiller, Vector3 origin, Vector3 direction, float damage, float speed)
        {
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// GetSpecialNoMovementState -- VIRTUAL
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected virtual bool GetSpecialNoMovementState()
        {
            return false;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// DoOnBraveStateExtended -- VIRTUAL
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected virtual void DoOnBraveStateExtended()
        {
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// DoOnFearFulStateExtended -- VIRTUAL
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected virtual void DoOnFearFulStateExtended()
        {
        }
    }
}
