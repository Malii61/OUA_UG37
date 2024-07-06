using Photon.Pun;
using System.Collections;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif
using UnityEngine.SceneManagement;

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class PlayerController : MonoBehaviourPunCallbacks
    {
        private const float INTERACTION_DISTANCE = 3f;
        private const float INTERACT_ALTERNATE_DISTANCE = 8f;
        private const float MAX_HEALTH = 100f;
        private const float MAX_ENERGY = 100f;
        public static PlayerController LocalInstance { get; private set; }
        private bool isPlayerAtLobby = true;

        #region

        [Header("Player")] [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 6.0f;

        [Tooltip("Sprint speed of the character in m/s")]
        public float SprintSpeed = 8.0f;

        [Tooltip("Rotation speed of the character")]
        public float RotationSpeed = 1.0f;

        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;

        [Space(10)] [Tooltip("The height the player can jump")]
        public float JumpHeight = 1.2f;

        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;

        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float JumpTimeout = 0.1f;

        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float FallTimeout = 0.15f;

        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool Grounded = true;

        [Tooltip("Useful for rough ground")] public float GroundedOffset = -0.14f;

        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.5f;

        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;

        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;

        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 90.0f;

        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -90.0f;

        // cinemachine
        [SerializeField] private GameObject cinemachine;
        [SerializeField] private GameObject cam;
        private float _cinemachineTargetPitch;

        // player
        private float _speed;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;

        // timeout deltatime
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

#if ENABLE_INPUT_SYSTEM
        private PlayerInput _playerInput;
#endif
        private CharacterController _controller;
        private StarterAssetsInputs _input;

        private const float _threshold = 0.01f;

        #endregion

        [Header("Photon")] [SerializeField] private PhotonView PV;
        private bool interacted;
        private I_Interactable lastInteracted;
        private bool facedAlready;
        [SerializeField] Animator animator;
        private float currentHealth;
        private float currentEnergy;
        private bool canRun = true;

        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return _playerInput.currentControlScheme == "KeyboardMouse";
#else
                return false;
#endif
            }
        }

        private void Awake()
        {
            if (!PV.IsMine)
            {
                Destroy(cam);
                Destroy(cinemachine);
                Destroy(GetComponent<StarterAssetsInputs>());
            }
            else
            {
                LocalInstance = this;
                SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
                cam.SetActive(false);
                _input = GetComponent<StarterAssetsInputs>();
                _playerInput = GetComponent<PlayerInput>();
                _playerInput.enabled = true;
                cinemachine.SetActive(false);
            }

            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            _controller = GetComponent<CharacterController>();
            // reset our timeouts on start
            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;

            currentHealth = MAX_HEALTH;
            currentEnergy = MAX_ENERGY;
        }

        private void OnDestroy()
        {
            SceneManager.activeSceneChanged -= SceneManager_activeSceneChanged;
            if (!PV.IsMine)
                return;
            // BarManager.Instance.Hide();
            GameFlowManager.Instance.AddInfo(PhotonNetwork.LocalPlayer.NickName + " is dead!");
            if (PhotonNetwork.IsConnected)
                PhotonNetwork.Instantiate("PhotonPrefabs/Ghost", transform.position, transform.rotation);
        }

        private void SceneManager_activeSceneChanged(Scene arg0, Scene arg1)
        {
            //currentTexture = new Texture2D(2, 2);
            if (arg1.name == Loader.Scene.GameScene.ToString())
            {
                StartCoroutine(PhotonLevelLoaderChecker());
            }
        }

        private IEnumerator PhotonLevelLoaderChecker()
        {
            yield return PhotonNetwork.LevelLoadingProgress >= 1;
            cam.SetActive(true);
            cinemachine.SetActive(true);
            isPlayerAtLobby = false;
            transform.position = new Vector3(2 + Random.Range(-2, 2), 4, 2 + Random.Range(-2, 2));
        }

        private void Update()
        {
            if (!PV.IsMine || isPlayerAtLobby)
            {
                if (_input)
                {
                    _input.look = Vector2.zero;
                    _input.move = Vector2.zero;
                    _input.cursorInputForLook = false;
                }

                return;
            }

            _input.cursorInputForLook = true;

            JumpAndGravity();
            GroundedCheck();
            Move();
            HandleInteractions();
        }

        private void HandleInteractions()
        {
            Ray ray = cam.GetComponent<Camera>().ViewportPointToRay(new Vector3(0.5f, 0.5f));
            ray.origin = cam.transform.position;
            if (Physics.Raycast(ray, out RaycastHit raycastHit, INTERACT_ALTERNATE_DISTANCE))
            {
                if (raycastHit.transform.TryGetComponent(out I_Interactable interactableObj))
                {
                    // interactable object has found
                    Interact(interactableObj);
                }
                else
                {
                    if (interacted && lastInteracted != null)
                    {
                        lastInteracted.OnInteractEnded();
                        interacted = false;
                        lastInteracted = null;
                    }

                    facedAlready = false;
                }
            }
            else
            {
                if (interacted && lastInteracted != null)
                {
                    lastInteracted.OnInteractEnded();
                    interacted = false;
                    lastInteracted = null;
                }

                facedAlready = false;
            }
        }

        private void Interact(I_Interactable interactable)
        {
            //interactable object has found
            interacted = true;
            lastInteracted = interactable;
            if (!facedAlready)
            {
                interactable.OnFaced();
                facedAlready = true;
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                interactable.Interact();
            }
        }

        private void LateUpdate()
        {
            if (!PV.IsMine)
                return;
            CameraRotation();
        }

        private void GroundedCheck()
        {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
                transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
                QueryTriggerInteraction.Ignore);
        }

        private void CameraRotation()
        {
            // if there is an input
            if (_input.look.sqrMagnitude >= _threshold)
            {
                //Don't multiply mouse input by Time.deltaTime
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                _cinemachineTargetPitch += _input.look.y * RotationSpeed * deltaTimeMultiplier;
                _rotationVelocity = _input.look.x * RotationSpeed * deltaTimeMultiplier;

                // clamp our pitch rotation
                _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

                // Update Cinemachine camera target pitch
                CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);

                // rotate the player left and right
                transform.Rotate(Vector3.up * _rotationVelocity);
            }
        }

        private void Move()
        {
            // set target speed based on move speed, sprint speed and if sprint is pressed
            float targetSpeed = _input.sprint && canRun ? SprintSpeed : MoveSpeed;

            // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

            // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is no input, set the target speed to 0
            if (_input.move == Vector2.zero) targetSpeed = 0.0f;

            // a reference to the players current horizontal velocity
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            // accelerate or decelerate to target speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                // creates curved result rather than a linear one giving a more organic speed change
                // note T in Lerp is clamped, so we don't need to clamp our speed
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.deltaTime * SpeedChangeRate);

                // round speed to 3 decimal places
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            animator.SetFloat("Speed", _speed, 0.05f, Time.deltaTime);
            // set the player energy
            if (_speed > 0.1f && _input.sprint && currentEnergy > 0 && canRun)
            {
                currentEnergy -= 0.25f;
            }
            else if (currentEnergy <= MAX_ENERGY)
                currentEnergy += 0.5f;

            // can player run?
            if (currentEnergy <= 0)
                canRun = false;
            else if (currentEnergy >= MAX_ENERGY)
                canRun = true;

            // if (canRun)
            // {
            //     BarManager.Instance.SetValue(currentEnergy, MAX_ENERGY, BarManager.BarType.energy);
            // }
            // else
            //     BarManager.Instance.SetValue(currentEnergy, MAX_ENERGY, BarManager.BarType.energy, Color.red);

            //else
            // normalise input direction
            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;
            // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is a move input rotate player when the player is moving
            if (_input.move != Vector2.zero)
            {
                // move
                inputDirection = transform.right * _input.move.x + transform.forward * _input.move.y;
            }

            // move the player
            _controller.Move(inputDirection.normalized * (_speed * Time.deltaTime) +
                             new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
        }

        private void JumpAndGravity()
        {
            if (Grounded)
            {
                // reset the fall timeout timer
                _fallTimeoutDelta = FallTimeout;

                // stop our velocity dropping infinitely when grounded
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                // Jump
                if (_input.jump && _jumpTimeoutDelta <= 0.0f)
                {
                    // the square root of H * -2 * G = how much velocity needed to reach desired height
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
                }

                // jump timeout
                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                // reset the jump timeout timer
                _jumpTimeoutDelta = JumpTimeout;

                // fall timeout
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }

                // if we are not grounded, do not jump
                _input.jump = false;
            }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (Grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
            Gizmos.DrawSphere(
                new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
                GroundedRadius);
        }

        public void TakeDamage(Vector3 hitPosition, Vector3 enemyForward, float damage)
        {
            PV.RPC(nameof(TakeDamagePunRpc), PV.Owner, hitPosition, enemyForward, damage);
        }

        [PunRPC]
        private void TakeDamagePunRpc(Vector3 hitPosition, Vector3 enemyForward, float damage)
        {
            currentHealth -= damage;
            if (currentHealth <= 0)
                Die();
            BarManager.Instance.SetValue(currentHealth, MAX_HEALTH, BarManager.BarType.health);
            Vector3 direction = enemyForward;
            direction.y = 0f;
            direction.Normalize();
            float force = 30f; // Geri savrulma kuvveti
            _controller.Move(direction * force * Time.deltaTime);
            PV.RPC(nameof(CreateBloodEffectPunRpc), RpcTarget.All, hitPosition);
        }

        private void Die()
        {
            PhotonNetwork.Destroy(gameObject);
        }

        [PunRPC]
        private void CreateBloodEffectPunRpc(Vector3 hitPosition)
        {
            // hitFX example
            // Instantiate(m_hitEffect, hitPosition, Quaternion.identity);
        }
    }
}