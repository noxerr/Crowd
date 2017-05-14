using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace STB.Generic
{ 
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Class: GenericEnemyBaseExtraExplosion
    /// # 
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class GenericEnemyBaseExtraExplosion : MonoBehaviour
    {
        // public
        public Vector3 extraExplosionForce = Vector3.zero;

        // private
        GenericEnemyBase actualGenericEnemyBase = null;

        // private -- enum
        enum EnemyState { undefined, alive, dead };

        // private
        EnemyState enemyState = EnemyState.undefined;


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Awake
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void Awake()
        {
            actualGenericEnemyBase = this.gameObject.GetComponent<GenericEnemyBase>();

            if (!actualGenericEnemyBase)
            {
                Debug.Log("WARNING: There is no GenericEnemyBase in GenericEnemyBaseExtraExplosion: " + this.gameObject.name);
                this.gameObject.SetActive(false);
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Update
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void Update()
        {
            switch (enemyState)
            {
                case EnemyState.undefined:
                    {
                        if (actualGenericEnemyBase.GetDead()) enemyState = EnemyState.dead;
                        else enemyState = EnemyState.alive;
                    }
                    break;

                case EnemyState.alive:
                    {
                        //Debug.Log("alive " + actualGenericEnemyBase.GetDead());

                        if (actualGenericEnemyBase.GetDead())
                        {
                            if (actualGenericEnemyBase.GetExplosionExtraForceAllowed())
                            {
                                //Debug.Log("Add extra explosion force");

                                Basics.BasicFunctions.AddExtraVelocityToAllChilds(this.transform, extraExplosionForce);
                            }

                            enemyState = EnemyState.dead;
                        }
                    }
                    break;

                case EnemyState.dead:
                    {
                        //Debug.Log("dead " + actualGenericEnemyBase.GetDead());

                        if (!actualGenericEnemyBase.GetDead())
                        {
                            enemyState = EnemyState.alive;
                        }
                    }
                    break;
            }
        }
    }
}
