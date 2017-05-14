using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace STB.LowPolyCharacterPack
{
    ///////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Class: CharactersShowHandler
    /// # 
    /// </summary>
    ///////////////////////////////////////////////////////////////////////////////////////////////////////
    public class CharactersShowHandler : MonoBehaviour
    { 
        // public
        public List<Transform> containersList = new List<Transform>();

        // private
        int actualCharacterIndex = 0;
        List<Transform> CharacterList = new List<Transform>();

        // private
        bool toLeft = false;
        bool toRight = false;


        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Start
        /// </summary>
        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        void Start()
        {
            for (int i = 0; i < containersList.Count; i++)
            {
                foreach (Transform t in containersList[i]) CharacterList.Add(t);
            }


            HandleAll();
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// GoToLeft
        /// </summary>
        //////////////////////////////////////////////////////////////////////////////////////////////////////
        public void GoToLeft()
        {
            toLeft = true;
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// GoToRight
        /// </summary>
        //////////////////////////////////////////////////////////////////////////////////////////////////////
        public void GoToRight()
        {
            toRight = true;
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// GetTransformInChildsByName
        /// # Return a child transform inside another transform using a name
        /// </summary>
        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        public static Transform GetTransformInChildsByName(Transform mainTransform, string name)
        {
            if (mainTransform.name == name)
            {
                return mainTransform;
            }

            foreach (Transform child in mainTransform)
            {
                Transform t = GetTransformInChildsByName(child, name);

                if (t != null)
                {
                    return t;
                }
            }

            return null;
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// HandleCamera
        /// </summary>
        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        void HandleAll()
        {
            if (Input.GetKeyDown(KeyCode.A) || toLeft) actualCharacterIndex--;
            if (Input.GetKeyDown(KeyCode.D) || toRight) actualCharacterIndex++;

            toLeft = false;
            toRight = false;

            if (actualCharacterIndex < 0)
            {
                actualCharacterIndex = CharacterList.Count - 1;
            }

            if (actualCharacterIndex > CharacterList.Count - 1)
            {
                actualCharacterIndex = 0;
            }

            //Debug.Log("actualCharacterIndex: " + actualCharacterIndex);

            for (int i = 0; i < CharacterList.Count; i++)
            {
                CharacterList[i].gameObject.SetActive(i == actualCharacterIndex);
            }
        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Update
        /// </summary>
        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        void Update()
        {
            HandleAll();
        }
    }
}
