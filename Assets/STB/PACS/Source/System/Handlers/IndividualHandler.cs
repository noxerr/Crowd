using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace STB.PACS
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Class: IndividualHandler
    /// # The main individual handler
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class IndividualHandler : MonoBehaviour
    {
        // public static
        public static IndividualHandler Singleton = null;

        // public
        public bool drawPathsInRealTime = true;
        public float workingDistance = 40;
        public int maxPool = 9;
        public int maxDensity = 99;
        public bool noNavMeshHandle = false;
        public float fixedYForIndividuals = 0;
        public float individualsScaleMultiplier = 1;
        public float individualsSpeedMultiplier = 1;

        // public
        public bool createScaryZonesOnIndividualsHit = false;
        public float scaryTime = 22;
        public float scaryRadious = 40;

        // public -- performance
        public int jumpForGenericObjectCounter = 10;
        public int jumpForIndividualPathPoint = 10;
        public bool forceNoJumpForGenericObject = false;

        // public
        public bool ignorePedestriansCollidersEnabled = false;
        public bool pedestrianCollisionsEnabled = true;
        public bool specialCollisionsMode = false;

        // public
        public List<AudioClip> maleScreamSoundList = new List<AudioClip>();
        public List<AudioClip> femaleScreamSoundList = new List<AudioClip>();

        // private
        int maleScreamAudiosourceIndex = 0;
        List<AudioSource> maleScreamAudiosourceList = new List<AudioSource>();

        int femaleScreamAudiosourceIndex = 0;
        List<AudioSource> femaleScreamAudiosourceList = new List<AudioSource>();

        // public
#if UNITY_EDITOR
        public bool debugKeysEnabled = false;
#endif

        // public -- hidden
        [HideInInspector]
        public List<GenericObject> genericObjectList = new List<GenericObject>();
        [HideInInspector]
        public List<IndividualPathPoint> individualPathPointList = new List<IndividualPathPoint>();

        // private
        bool firstUpdate = true;
        int actualGenericObjectCounter = 0;
        int actualIndividualPathPointCounter = 0;

        // private
        int scaryZoneIndex = 0;
        List<ScaryZone> scaryZoneList = new List<ScaryZone>();
        List<float> scaryZoneTimerList = new List<float>();


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Awake
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void Awake()
        {
            Singleton = this;

            // set other variables for special collisions mdoe
            if (specialCollisionsMode)
            {
                ignorePedestriansCollidersEnabled = false;
                pedestrianCollisionsEnabled = false;
            }

            // load scary zones
            LoadScaryZones();

            // create audiosources
            CreateAudioSources();

            // extended awake
            AwakeExtended();
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// CreateAudioSources
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void CreateAudioSources()
        {
            for (int i = 0; i < maleScreamSoundList.Count; i++)
            {
                AudioSource actualAudioSource = this.gameObject.AddComponent<AudioSource>();
                actualAudioSource.loop = false;
                actualAudioSource.playOnAwake = false;

                maleScreamAudiosourceList.Add(actualAudioSource);
            }

            for (int i = 0; i < femaleScreamSoundList.Count; i++)
            {
                AudioSource actualAudioSource = this.gameObject.AddComponent<AudioSource>();
                actualAudioSource.loop = false;
                actualAudioSource.playOnAwake = false;

                femaleScreamAudiosourceList.Add(actualAudioSource);
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// NewIndividualKilling -- VIRTUAL
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual void NewIndividualKilling(GameObject actualIndividual, string hudName)
        {
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// PlayScreamSoundByGender
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void PlayScreamSoundByGender(STB.PACS.GenericIndividual.cGender gender)
        {
            switch (gender)
            {
                case GenericIndividual.cGender.male:
                    {
                        PlayMaleScreamSound();
                    }
                    break;

                case GenericIndividual.cGender.female:
                    {
                        PlayFemaleScreamSound();
                    }
                    break;

                case GenericIndividual.cGender.other:
                    {
                        if (Random.Range(0, 100) < 50) PlayMaleScreamSound();
                        else PlayFemaleScreamSound();
                    }
                    break;
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// PlayMaleScreamSound
        /// </summary>
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void PlayMaleScreamSound()
        {
            if (maleScreamAudiosourceList.Count > 0)
            {
                int r = Random.Range(0, maleScreamSoundList.Count);
                r = Mathf.Clamp(r, 0, maleScreamSoundList.Count - 1);

                maleScreamAudiosourceList[maleScreamAudiosourceIndex].clip = maleScreamSoundList[r];
                maleScreamAudiosourceList[maleScreamAudiosourceIndex].Stop();
                maleScreamAudiosourceList[maleScreamAudiosourceIndex].Play();

                maleScreamAudiosourceIndex++;
                if (maleScreamAudiosourceIndex >= maleScreamAudiosourceList.Count) maleScreamAudiosourceIndex = 0;
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// PlayFemaleScreamSound
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void PlayFemaleScreamSound()
        {
            if (femaleScreamAudiosourceList.Count > 0)
            {
                int r = Random.Range(0, femaleScreamSoundList.Count);
                r = Mathf.Clamp(r, 0, femaleScreamSoundList.Count - 1);

                femaleScreamAudiosourceList[femaleScreamAudiosourceIndex].clip = femaleScreamSoundList[r];
                femaleScreamAudiosourceList[femaleScreamAudiosourceIndex].Stop();
                femaleScreamAudiosourceList[femaleScreamAudiosourceIndex].Play();

                femaleScreamAudiosourceIndex++;
                if (femaleScreamAudiosourceIndex >= femaleScreamAudiosourceList.Count) femaleScreamAudiosourceIndex = 0;
            }
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
        /// FirstUpdate
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool FirstUpdate
        {
            get { return firstUpdate; }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// LoadScaryZones
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void LoadScaryZones()
        {
            if (createScaryZonesOnIndividualsHit)
            {
                GameObject scaryZonesContainer = Basics.BasicFunctions.CreateContainerIfNotExists("_SCARY_ZONES");

                for (int i = 0; i < 22; i++)
                {
                    GameObject actualScaryZone = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    actualScaryZone.gameObject.SetActive(false);
                    actualScaryZone.name = "ScaryZone_" + (i + 1).ToString();

                    actualScaryZone.transform.localScale = scaryRadious * Vector3.one;
                    actualScaryZone.transform.position = Basics.BasicDefines.TOO_FAR_POSITION;
                    actualScaryZone.transform.parent = scaryZonesContainer.transform;

                    actualScaryZone.AddComponent<ScaryZone>();
                    actualScaryZone.GetComponent<Renderer>().enabled = false;
                    actualScaryZone.GetComponent<Collider>().isTrigger = true;

                    Rigidbody rb = actualScaryZone.AddComponent<Rigidbody>();
                    rb.isKinematic = true;


                    scaryZoneList.Add(actualScaryZone.GetComponent<ScaryZone>());
                    scaryZoneTimerList.Add(0);
                }
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// ThrowNewScaryZone
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void ThrowNewScaryZone(Vector3 position)
        {
            if (createScaryZonesOnIndividualsHit)
            {
                scaryZoneList[scaryZoneIndex].transform.position = position;
                scaryZoneList[scaryZoneIndex].gameObject.SetActive(true);

                scaryZoneTimerList[scaryZoneIndex] = scaryTime;

                scaryZoneIndex++;
                if (scaryZoneIndex >= scaryZoneList.Count) scaryZoneIndex = 0;
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// HandlePerformance
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void HandlePerformance()
        {
            //Debug.Log("HandlePerformance");

            // generic objects
            {
                int A = actualGenericObjectCounter * jumpForGenericObjectCounter;
                int B = (actualGenericObjectCounter + 1) * jumpForGenericObjectCounter;

                if (firstUpdate)
                {
                    A = 0;
                    B = genericObjectList.Count;
                }

                for (int i = A; i < Mathf.Min(B, genericObjectList.Count); i++)
                {
                    if (genericObjectList[i]) genericObjectList[i].HandleWorkingState();
                    else genericObjectList.RemoveAt(i);
                }

                actualGenericObjectCounter++;

                if (actualGenericObjectCounter * jumpForGenericObjectCounter >= genericObjectList.Count) actualGenericObjectCounter = 0;
            }

            // individual path points
            {
                int A = actualIndividualPathPointCounter * jumpForIndividualPathPoint;
                int B = (actualIndividualPathPointCounter + 1) * jumpForIndividualPathPoint;

                if (firstUpdate || forceNoJumpForGenericObject)
                {
                    A = 0;
                    B = individualPathPointList.Count;
                }

                //Debug.Log("A: " + A.ToString());
                //Debug.Log("B: " + B.ToString());

                for (int i = A; i < Mathf.Min(B, individualPathPointList.Count); i++)
                {
                    individualPathPointList[i].HandlePool(firstUpdate);
                }

                actualIndividualPathPointCounter++;

                if (actualIndividualPathPointCounter * jumpForIndividualPathPoint >= individualPathPointList.Count) actualIndividualPathPointCounter = 0;
            }

            firstUpdate = false;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Update
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void Update()
        {
            HandlePerformance();


            UpdateExtended();
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// UpdateExtended -- VIRTUAL
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        protected virtual void UpdateExtended()
        {
#if UNITY_EDITOR
            if (debugKeysEnabled)
            {
                if (Input.GetKeyDown(KeyCode.L))
                {
                    for (int i = 0; i < genericObjectList.Count; i++)
                    {
                        GenericIndividual actualGenericIndividual = genericObjectList[i].GetComponent<GenericIndividual>();

                        if (actualGenericIndividual) actualGenericIndividual.ManualResurrection();
                    }
                }

                if (Input.GetKeyDown(KeyCode.K))
                {
                    for (int i = 0; i < genericObjectList.Count; i++)
                    {
                        GenericIndividual actualGenericIndividual = genericObjectList[i].GetComponent<GenericIndividual>();

                        if (actualGenericIndividual) actualGenericIndividual.ManualDead();
                    }
                }
            }
#endif
        }
    }
}
