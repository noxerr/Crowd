using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace STB.Generic
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Class: GenericMainCharacter
    /// # 
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class GenericMainCharacter : MonoBehaviour
    {
        // public static
        public static GenericMainCharacter GenericSingleton = null;

        // public -- enum 
        public enum cControllableType { human, land, air, sea };

        // public
        public cControllableType controllableType = cControllableType.human;

        // public -- enum
        public enum cAIType { noAI, enemy, partner };
        public cAIType AIType = cAIType.noAI;


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// AIEnabled
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool AIEnabled
        {
            get
            {
                switch (AIType)
                {
                    case cAIType.noAI:
                        return false;

                    case cAIType.enemy:
                    case cAIType.partner:
                        return true;
                }

                return false;
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Update
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void Update()
        {
            if (!AIEnabled) GenericSingleton = this;

            //Debug.Log("GenericMainCharacter -> GenericSingleton: " + this.gameObject.name);

            UpdateExtended();
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// GetHaveRecenltyShooteed -- VIRTUAL
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual bool GetHaveRecenltyShooteed()
        {
            return false;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// UpdateExtended -- VIRTUAL
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected virtual void UpdateExtended()
        {
        }
    }
}