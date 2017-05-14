using System;
using UnityEngine;
using STB.SpecialInput;
using Random = UnityEngine.Random;

namespace STB.Characters.FirstPerson
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(AudioSource))]
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Class: STBFirstPersonController
    /// </summary>
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class STBFirstPersonController : MonoBehaviour
    {
        // public static
        public static float XSENSITIBITY = 2;
        public static float YSENSITIBITY = 2;

        // public -- hidden
        [HideInInspector]
        public bool enableManualJoystickToMoveCamera = true;
        [HideInInspector]
        public bool enableGyroscopeToMoveCamera = false;
        [HideInInspector]
        public bool AIEnabled = false;
        [HideInInspector]
        public bool enableManualJoystickToMoveCharacter = true;
        [HideInInspector]
        public bool automaticForwardMovement = false;
        [HideInInspector]
        public bool automaticShootOnEnemies = false;

        // public
        public Camera m_Camera;

        // public
        public KeyCode jumpKeyCode = KeyCode.Space;
        public KeyCode forwardKeyCode = KeyCode.W;
        public KeyCode backwardKeyCode = KeyCode.S;
        public KeyCode leftKeyCode = KeyCode.A;
        public KeyCode rightKeyCode = KeyCode.D;
        public KeyCode runKeyCode = KeyCode.LeftShift;

        [SerializeField]
        public bool m_UseFovKick;

        // private
        [SerializeField]
        bool m_IsWalking;
        [SerializeField]
        float m_WalkSpeed;
        [SerializeField]
        float m_RunSpeed;
        [SerializeField]
        [Range(0f, 1f)]
        float m_RunstepLenghten;
        [SerializeField]
        float m_JumpSpeed;
        [SerializeField]
        float m_StickToGroundForce;
        [SerializeField]
        float m_GravityMultiplier;
        [SerializeField]
        STBMouseLook m_MouseLook;
        [SerializeField]
        Extra.STBFOVKick m_FovKick = new Extra.STBFOVKick();
        [SerializeField]
        bool m_UseHeadBob;
        [SerializeField]
        Extra.STBCurveControlled m_HeadBob = new Extra.STBCurveControlled();
        [SerializeField]
        Extra.STBLerpControlled m_JumpBob = new Extra.STBLerpControlled();
        [SerializeField]
        float m_StepInterval;
        [SerializeField]
        AudioClip[] m_FootstepSounds;    // an array of footstep sounds that will be randomly selected from.
        [SerializeField]
        AudioClip m_JumpSound;           // the sound played when character leaves the ground.
        [SerializeField]
        AudioClip m_LandSound;           // the sound played when character touches back on ground.

        bool m_Jump;
        Vector2 m_Input;
        Vector3 m_MoveDir = Vector3.zero;
        CharacterController m_CharacterController;
        CollisionFlags m_CollisionFlags;
        bool m_PreviouslyGrounded;
        Vector3 m_OriginalCameraPosition;
        float m_StepCycle;
        float m_NextStep;
        bool m_Jumping;
        AudioSource m_AudioSource;

        // private
        Vector2 movementVector = Vector3.zero;


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Start
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void Start()
        {
            m_CharacterController = GetComponent<CharacterController>();
            m_OriginalCameraPosition = m_Camera.transform.localPosition;
            m_FovKick.Setup(m_Camera);
            m_HeadBob.Setup(m_Camera, m_StepInterval);
            m_StepCycle = 0f;
            m_NextStep = m_StepCycle / 2f;
            m_Jumping = false;
            m_AudioSource = GetComponent<AudioSource>();
            m_MouseLook.Init(transform, m_Camera.transform);
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// RestartRotations
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void RestartRotations()
        {
            //Debug.Log("RestartRotations");
            m_MouseLook.Init(transform, m_Camera.transform);
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// PlayLandingSound
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool IsGrounded
        {
            get { return m_CharacterController.isGrounded; }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// GetMouseLook
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public STBMouseLook GetMouseLook()
        {
            return m_MouseLook;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// LastMovementVector
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public Vector2 LastMovementVector
        {
            get { return movementVector; }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Update
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void Update()
        {
            RotateView();

            // the jump state needs to read here to make sure it is not missed
            if (!m_Jump && !AIEnabled)
            {
                KeyCode finalJumpKeyCode = jumpKeyCode;
                if (Generic.KeyActionManager.Singleton) finalJumpKeyCode = Generic.KeyActionManager.Singleton.jumpKey;

                m_Jump = STBCrossPlatformInputManager.GetButtonUp("Jump") || Input.GetKeyDown(finalJumpKeyCode);
            }

            if (!m_PreviouslyGrounded && m_CharacterController.isGrounded)
            {
                StartCoroutine(m_JumpBob.DoBobCycle());
                PlayLandingSound();
                m_MoveDir.y = 0f;
                m_Jumping = false;
            }
            if (!m_CharacterController.isGrounded && !m_Jumping && m_PreviouslyGrounded)
            {
                m_MoveDir.y = 0f;
            }

            m_PreviouslyGrounded = m_CharacterController.isGrounded;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// PlayLandingSound
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void PlayLandingSound()
        {
            m_AudioSource.clip = m_LandSound;
            m_AudioSource.Play();
            m_NextStep = m_StepCycle + .5f;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// FixedUpdate
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void FixedUpdate()
        {
            float speed;

            GetInput(out speed);

            // always move along the camera forward as it is the direction that it being aimed at
            Vector3 desiredMove = transform.forward * m_Input.y + transform.right * m_Input.x;

            movementVector = new Vector2(m_Input.x, m_Input.y);

            if (automaticForwardMovement)
            {
                movementVector.y = 0.1f;
                desiredMove = transform.forward;
            }

            if (!AIEnabled)
            {
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBPLAYER || UNITY_WEBGL
                if (enableManualJoystickToMoveCharacter)
                {
                    KeyCode finalForwardKeyCode = forwardKeyCode;
                    KeyCode finalBackwardKeyCode = backwardKeyCode;
                    KeyCode finalLeftKeyCode = leftKeyCode;
                    KeyCode finalRightKeyCode = rightKeyCode;
                    KeyCode finalRunKeyCode = runKeyCode;

                    if (Generic.KeyActionManager.Singleton)
                    {
                        finalForwardKeyCode = Generic.KeyActionManager.Singleton.forwardKey;
                        finalBackwardKeyCode = Generic.KeyActionManager.Singleton.backwardKey;
                        finalLeftKeyCode = Generic.KeyActionManager.Singleton.leftKey;
                        finalRightKeyCode = Generic.KeyActionManager.Singleton.rightKey;
                        finalRunKeyCode = Generic.KeyActionManager.Singleton.runKey;
                    }

                    if (Input.GetKey(finalForwardKeyCode))
                    {
                        movementVector.y = 0.1f;
                        desiredMove = 13 * transform.forward;

                        if (Input.GetKey(finalRunKeyCode))
                        {
                            movementVector.y = 0.5f;
                        }
                    }

                    if (Input.GetKey(finalBackwardKeyCode))
                    {
                        movementVector.y = -0.1f;
                        desiredMove = -13 * transform.forward;
                    }

                    if (Input.GetKey(finalLeftKeyCode))
                    {
                        movementVector.x = -0.1f;
                        desiredMove = -13 * transform.right;
                    }

                    if (Input.GetKey(finalRightKeyCode))
                    {
                        movementVector.x = 0.1f;
                        desiredMove = 13 * transform.right;
                    }
                }
#endif
            }

            // get a normal for the surface that is being touched to move along it
            RaycastHit hitInfo;

#if UNITY_5_0
            Physics.SphereCast(transform.position, m_CharacterController.radius, Vector3.down, out hitInfo, m_CharacterController.height / 2f, Physics.AllLayers);
#else
            Physics.SphereCast(transform.position, m_CharacterController.radius, Vector3.down, out hitInfo, m_CharacterController.height / 2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
#endif

            desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

            m_MoveDir.x = desiredMove.x * speed;
            m_MoveDir.z = desiredMove.z * speed;


            if (m_CharacterController.isGrounded)
            {
                m_MoveDir.y = -m_StickToGroundForce;

                if (m_Jump)
                {
                    m_MoveDir.y = m_JumpSpeed;
                    PlayJumpSound();
                    m_Jump = false;
                    m_Jumping = true;
                }
            }
            else
            {
                m_MoveDir += Physics.gravity * m_GravityMultiplier * Time.fixedDeltaTime;
            }

            m_CollisionFlags = m_CharacterController.Move(m_MoveDir * Time.fixedDeltaTime);

            ProgressStepCycle(speed);
            UpdateCameraPosition(speed);
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// PlayJumpSound
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void PlayJumpSound()
        {
            m_AudioSource.clip = m_JumpSound;
            m_AudioSource.Play();
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// ProgressStepCycle
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void ProgressStepCycle(float speed)
        {
            if (m_CharacterController.velocity.sqrMagnitude > 0 && (m_Input.x != 0 || m_Input.y != 0))
            {
                m_StepCycle += (m_CharacterController.velocity.magnitude + (speed * (m_IsWalking ? 1f : m_RunstepLenghten))) * Time.fixedDeltaTime;
            }

            if (!(m_StepCycle > m_NextStep))
            {
                return;
            }

            m_NextStep = m_StepCycle + m_StepInterval;

            PlayFootStepAudio();
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// PlayFootStepAudio
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void PlayFootStepAudio()
        {
            if (!m_CharacterController.isGrounded)
            {
                return;
            }

            // pick & play a random footstep sound from the array,
            // excluding sound at index 0
            int n = Random.Range(1, m_FootstepSounds.Length);
            m_AudioSource.clip = m_FootstepSounds[n];
            m_AudioSource.PlayOneShot(m_AudioSource.clip);
            // move picked sound to index 0 so it's not picked next time
            m_FootstepSounds[n] = m_FootstepSounds[0];
            m_FootstepSounds[0] = m_AudioSource.clip;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// UpdateCameraPosition
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void UpdateCameraPosition(float speed)
        {
            Vector3 newCameraPosition;
            if (!m_UseHeadBob)
            {
                return;
            }

            if (m_CharacterController.velocity.magnitude > 0 && m_CharacterController.isGrounded)
            {
                m_Camera.transform.localPosition = m_HeadBob.DoHeadBob(m_CharacterController.velocity.magnitude + (speed * (m_IsWalking ? 1f : m_RunstepLenghten)));
                newCameraPosition = m_Camera.transform.localPosition;
                newCameraPosition.y = m_Camera.transform.localPosition.y - m_JumpBob.Offset();
            }
            else
            {
                newCameraPosition = m_Camera.transform.localPosition;
                newCameraPosition.y = m_OriginalCameraPosition.y - m_JumpBob.Offset();
            }

            m_Camera.transform.localPosition = newCameraPosition;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// IsJumping
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool IsJumping
        {
            get { return m_Jump; }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// GetInput
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void GetInput(out float speed)
        {
            // Read input
            float horizontal = STBCrossPlatformInputManager.GetAxis("Horizontal");
            float vertical = STBCrossPlatformInputManager.GetAxis("Vertical");

            if (SpecialInput.STBNewJoystick.Singleton)
            {
                horizontal = SpecialInput.STBNewJoystick.Singleton.JoystickInput.x;
                vertical = SpecialInput.STBNewJoystick.Singleton.JoystickInput.y;
            }

            bool waswalking = m_IsWalking;

            // On standalone builds, walk/run speed is modified by a key press.
            // keep track of whether or not the character is walking or running
            KeyCode finalRunKeyCode = runKeyCode;
            if (Generic.KeyActionManager.Singleton) finalRunKeyCode = Generic.KeyActionManager.Singleton.runKey;

            m_IsWalking = !Input.GetKey(finalRunKeyCode);

            // set the desired speed to be walking or running
            speed = m_IsWalking ? m_WalkSpeed : m_RunSpeed;
            m_Input = new Vector2(horizontal, vertical);

            // normalize input if it exceeds 1 in combined length:
            if (m_Input.sqrMagnitude > 1)
            {
                m_Input.Normalize();
            }

            // handle speed change to give an fov kick
            // only if the player is going to a run, is running and the fovkick is to be used
            if (m_IsWalking != waswalking && m_UseFovKick && m_CharacterController.velocity.sqrMagnitude > 0)
            {
                StopAllCoroutines();
                StartCoroutine(!m_IsWalking ? m_FovKick.FOVKickUp() : m_FovKick.FOVKickDown());
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// RotateView
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void RotateView()
        {
            if (!AIEnabled) m_MouseLook.LookRotation(transform, m_Camera.transform, XSENSITIBITY, YSENSITIBITY, enableGyroscopeToMoveCamera, enableManualJoystickToMoveCamera);
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// OnControllerColliderHit
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void OnControllerColliderHit(ControllerColliderHit hit)
        {
            Rigidbody body = hit.collider.attachedRigidbody;

            //dont move the rigidbody if the character is on top of it
            if (m_CollisionFlags == CollisionFlags.Below)
            {
                return;
            }

            if (body == null || body.isKinematic)
            {
                return;
            }
            body.AddForceAtPosition(m_CharacterController.velocity * 0.1f, hit.point, ForceMode.Impulse);
        }
    }
}
