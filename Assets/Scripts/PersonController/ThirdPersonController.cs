using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using Assets.Scripts.Networking;
using Assets.Scripts.PersonController;
using Unity.VisualScripting;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

namespace StarterAssets
{

    public class ThirdPersonController : NetworkBehaviour
    {
        private float _terminalVelocity = 53.0f;


        [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 2.0f;
        [Tooltip("Sprint speed of the character in m/s")]
        public float SprintSpeed = 5.335f;
        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool Grounded = true;
        [Tooltip("Useful for rough ground")]
        public float GroundedOffset = -0.14f;
        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.28f;
        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;
        public Renderer myObject;
        [SerializeField]
        private float rotationSpeed = 2.5f;
        [SerializeField]
        private AudioSource punchSound;

        [SerializeField]
        private float _targetRotation = 0.0f;

        private float _rotationVelocity;
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        private NetworkVariable<bool> _flagVisibility = new NetworkVariable<bool>(true);
        private bool _flagOldVisibility = true;

        public float RotationSmoothTime = 0.12f;


        private float _verticalVelocity;
        private float _speed;
        public float TopClamp = 70.0f;
        public float JumpHeight = 1.2f;

        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -30.0f;
        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;
        [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
        public float CameraAngleOverride = 0.0f;
        [Tooltip("For locking the camera position on all axis")]
        public bool LockCameraPosition = false;
        public GameObject CinemachineCameraTarget;
        [Tooltip("Time required to perform the dance")]
        public float DanceTimeout = 0.2f;
        public float PunchTimeout = 0.1f;
        public float TagTimeout = 0.1f;
        [SerializeField]
        public NetworkVariable<TimeSpan> timeTag = new NetworkVariable<TimeSpan>();
        [SerializeField]
        public NetworkVariable<bool> isTag = new NetworkVariable<bool>();
        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float JumpTimeout = 0.50f;
        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float FallTimeout = 0.15f;

        public float SpeedChangeRate = 10.0f;
        private const float _threshold = 0.01f;
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;
        private int _danceId;
        private GameObject _mainCamera;
        private bool _oldTag;

        private float _animationBlend;
        private bool isJump;
        private bool isDance;

        [SerializeField]
        public NetworkVariable<NetworkString> nickName = new NetworkVariable<NetworkString>();

        private PlayerControlAsset playerInput;
        private CharacterController playerController;
        private Animator animator;
        private InventoryWindow targetInventoryWindow;

        private Vector2 currentMovementInput;
        private double _danceTimeoutDelta;
        private double _punchTimeoutDelta;
        private double _throwTimeoutDelta;
        private double _tagTimeoutDelta;
        private bool isPunch;
        private bool isThrow;
        private Item currentBonus;

        void Awake()
        {
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
            playerInput = new PlayerControlAsset();
            playerController = GetComponent<CharacterController>();
            animator = GetComponent<Animator>();
            playerInput.Player.Move.started += Move_input;
            playerInput.Player.Move.canceled += Move_input;
            playerInput.Player.Move.performed += Move_input;
            playerInput.Player.Jump.started += OnJump;
            playerInput.Player.Jump.canceled += OnJump;
            playerInput.Player.Dance.started += OnDance;
            playerInput.Player.Dance.canceled += OnDance;
            playerInput.Player.Punch.started += OnPunch;
            playerInput.Player.Punch.performed += OnPunch;
            playerInput.Player.Punch.canceled += OnPunch;
            playerInput.Player.Throw.started += OnThrow;
            playerInput.Player.Throw.performed += OnThrow;
            playerInput.Player.Throw.canceled += OnThrow;

        }

        private void OnPunch(InputAction.CallbackContext obj)
        {
            isPunch = obj.ReadValueAsButton();
        }

        private void OnThrow(InputAction.CallbackContext obj)
        {
            isThrow = obj.ReadValueAsButton();
        }

        private void OnDance(InputAction.CallbackContext obj)
        {
            isDance = obj.ReadValueAsButton();
        }

        private void OnJump(InputAction.CallbackContext obj)
        {
            isJump = obj.ReadValueAsButton();
        }

        private void Move_input(InputAction.CallbackContext obj)
        {
            Debug.Log(obj.ReadValue<Vector2>());
            currentMovementInput = obj.ReadValue<Vector2>();

        }

        private void Move()
        {
            bool isSprint = CheckSprint(currentMovementInput);
            float targetSpeed = isSprint ? SprintSpeed : MoveSpeed;
            // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon
            // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is no input, set the target speed to 0
            if (currentMovementInput == Vector2.zero) targetSpeed = 0.0f;
            // a reference to the players current horizontal velocity
            float currentHorizontalSpeed = new Vector3(playerController.velocity.x, 0.0f, playerController.velocity.z).magnitude;
            float speedOffset = 0.1f;
            float inputMagnitude = 1f;
            // accelerate or decelerate to target speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                // creates curved result rather than a linear one giving a more organic speed change
                // note T in Lerp is clamped, so we don't need to clamp our speed
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);
                // round speed to 3 decimal places
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);

            // normalise input direction
            Vector3 inputDirection = new Vector3(currentMovementInput.x, 0.0f, currentMovementInput.y).normalized;

            // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is a move input rotate player when the player is moving
            if (currentMovementInput != Vector2.zero)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + _mainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, RotationSmoothTime);

                // rotate to face input direction relative to camera position
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }

            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

            // move the player
            playerController.Move(targetDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

            if (currentMovementInput == Vector2.zero)
            {
                if (!IsServer)
                {
                    UpdateAnimatorServerRpc("Walk", false);
                    UpdateAnimatorServerRpc("Run", false);
                }

                animator.SetBool("Run", false);
                animator.SetBool("Walk", false);
            }
            else if (isSprint)
            {
                if (!IsServer)
                {
                    UpdateAnimatorServerRpc("Run", true);
                }
                animator.SetBool("Run", true);
            }
            else if (!isSprint)
            {
                if (!IsServer)
                {
                    UpdateAnimatorServerRpc("Walk", true);
                }
                animator.SetBool("Walk", true);
            }

        }

        private void TimeTagChange()
        {
            if (isTag.Value && UIManager.Instance.timerActive.Value)
            {
                UpdateTagTimeServerRpc();
            }
        }

        [ServerRpc]
        private void UpdateTagTimeServerRpc()
        {
            timeTag.Value += TimeSpan.FromSeconds(Time.deltaTime);
        }

        void Start()
        {
            if (IsClient && IsOwner)
            {
                _jumpTimeoutDelta = JumpTimeout;
                _fallTimeoutDelta = FallTimeout;
                _danceTimeoutDelta = DanceTimeout;
                _punchTimeoutDelta = PunchTimeout;
                PlayerFollow.Instance.FollowPlayer(transform.Find("PlayerCameraRoot"));
                targetInventoryWindow = GameObject.FindWithTag("Inventory").GetComponent<InventoryWindow>();
                targetInventoryWindow.StartAddInventory(gameObject.GetComponent<Inventory>());
            }
        }

        private void OnEnable()
        {
            playerInput.Enable();
        }

        private void OnDisable()
        {
            playerInput.Disable();
        }

        // Update is called once per frame
        void Update()
        {
            if (IsClient && IsOwner)
            {
                JumpAndGravity();
                GroundCheck();
                Move();
                Beating();
                Dance();
                TimeTagChange();
                Throwing();
            }

            if (_oldTag != isTag.Value)
            {
                _oldTag = isTag.Value;
                СhangePlayerColor();
            }

            if (_flagOldVisibility != _flagVisibility.Value)
            {
                _flagOldVisibility = _flagVisibility.Value;
                СhangePlayerVisibility();
            }
        }

        private void СhangePlayerVisibility()
        {
            if (_flagVisibility.Value == false)
            {
                myObject.enabled = false;
            }

            if (_flagVisibility.Value == true)
            {
                myObject.enabled = true;
            }
        }

        private void СhangePlayerColor()
        {
            if (isTag.Value == false)
            {
                foreach (var items in myObject.materials)
                {
                    items.color = Color.white;
                }
            }

            if (isTag.Value == true)
            {
                foreach (var items in myObject.materials)
                {
                    items.color = Color.red;
                }
            }
        }

        private void Dance()
        {
            if (Grounded)
            {
                if (_danceTimeoutDelta <= 0.0f && isDance)
                {
                        _danceTimeoutDelta = DanceTimeout;
                        _danceId = Random.Range(1, 7);
                        if (!IsServer)
                        {
                            UpdateAnimatorServerRpc("DanceID", _danceId);
                            UpdateAnimatorServerRpc("Dance", true);
                        }
                        animator.SetFloat("DanceID", _danceId);
                        animator.SetBool("Dance", true);
                       

                }
                if (_danceTimeoutDelta >= 0.0f)
                {
                    _danceTimeoutDelta -= Time.deltaTime;
                }

                if (!isDance)
                {
                    if (!IsServer)
                    {
                        UpdateAnimatorServerRpc("Dance", false);
                    }

                    animator.SetBool("Dance", false);
                }
            }
            else
            { 
                if (!IsServer)
                {
                    UpdateAnimatorServerRpc("Dance", false);
                }
                animator.SetBool("Dance", false);

            }
        }

        private void Beating()
        {
            if (isPunch == false)
            {
                if (!IsServer)
                {
                    UpdateAnimatorServerRpc("Punch", false);
                }
                animator.SetBool("Punch", false);
            }
            if (isPunch)
            {
                if (_punchTimeoutDelta <= 0.0f)
                {

                    if (!IsServer)
                    {
                        UpdateAnimatorServerRpc("Punch", true);
                    }
                    animator.SetBool("Punch", true);
                    _punchTimeoutDelta = PunchTimeout;
                }

                if (_punchTimeoutDelta >= 0.0f)
                {
                    _punchTimeoutDelta -= Time.deltaTime;
                }
            }
        }

        private void Throwing()
        {
            if (isThrow)
            {
                if (_throwTimeoutDelta <= 0.0f)
                {
                    currentBonus = targetInventoryWindow.targetInventory.inventoryItems.First();
                    _throwTimeoutDelta = currentBonus.timeDurationBonus;
                }
            }

            if (_throwTimeoutDelta <= 0.0f && currentBonus != null)
            {
                switch (currentBonus.title)
                {
                    case "InvisibilityPotion":
                        UpdateVisibilityServerRpc(true);
                        targetInventoryWindow.targetInventory.RemoveItemAt(0);
                        currentBonus = null;
                        break;
                }
            }

            if (_throwTimeoutDelta >= 0.0f && currentBonus != null)
            {
                switch (currentBonus.title)
                {
                    case "InvisibilityPotion":
                        UpdateVisibilityServerRpc(false);
                        break;
                }
                _throwTimeoutDelta -= Time.deltaTime;
            }
        }

        private void GroundCheck()
        {
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
            bool ground = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
            Grounded = ground;
            if (!IsServer)
            {
                UpdateAnimatorServerRpc("Grounded", ground);
            }
            animator.SetBool("Grounded", ground);
        }

        void FixedUpdate()
        {
            if (IsClient && IsOwner)
            {
                CameraRotation();
            }
        }

        private bool CheckSprint(Vector2 movementInput)
        {
            var valueX = movementInput.x;
            var valueY = movementInput.y;
            if (valueX > 0.75 || valueX < -0.75 || valueY > 0.75 || valueY < -0.75)
            {
                return true;
            }
            return false;
        }

        private void CameraRotation()
        {
            var input = playerInput.Player.Look.ReadValue<Vector2>();
            // if there is an input and camera position is not fixed
            if (input.sqrMagnitude >= _threshold && !LockCameraPosition)
            {
                _cinemachineTargetYaw += input.x * Time.deltaTime;
                _cinemachineTargetPitch += input.y * Time.deltaTime;
            }
            // clamp our rotations so our values are limited 360 degrees
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

            // Cinemachine will follow this target
            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride, _cinemachineTargetYaw, 0.0f);
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private void JumpAndGravity()
        {
            if (Grounded)
            {
                // reset the fall timeout timer
                _fallTimeoutDelta = FallTimeout;

                if (!IsServer)
                {
                    UpdateAnimatorServerRpc("Jump", false);
                    UpdateAnimatorServerRpc("FreeFall", false);
                }
                animator.SetBool("Jump", false);
                animator.SetBool("FreeFall", false);

                // stop our velocity dropping infinitely when grounded
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                // Jump
                if (isJump && _jumpTimeoutDelta <= 0.0f)
                {
                    // the square root of H * -2 * G = how much velocity needed to reach desired height

                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
                    if (!IsServer)
                    {
                        UpdateAnimatorServerRpc("Jump", true);
                    }
                    animator.SetBool("Jump", true);
                    // update animator if using character

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
                else
                {
                    if (!IsServer)
                    {
                        UpdateAnimatorServerRpc("FreeFall", true);
                    }
                    animator.SetBool("FreeFall", true);

                }

                // if we are not grounded, do not jump
                isJump = false;
            }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }

        void OnControllerColliderHit(ControllerColliderHit hit)
        {
            switch (hit.gameObject.tag)
            {
                case "Player":
                    {
                        var playerHit = hit.transform.GetComponent<NetworkObject>();
                        var playerTag = hit.transform.GetComponent<ThirdPersonController>().isTag.Value;
                        if (playerHit != null)
                        {
                            if (_tagTimeoutDelta <= 0)
                            {
                                if (isTag.Value)
                                {
                                    UpdateTagServerRpc(true, playerHit.OwnerClientId);
                                }

                                if (!isTag.Value)
                                {
                                    if (playerTag)
                                    {
                                        UpdateTagServerRpc(false, playerHit.OwnerClientId);
                                    }

                                }
                                _tagTimeoutDelta = TagTimeout;
                            }
                            if (_tagTimeoutDelta >= 0)
                            {
                                _tagTimeoutDelta -= Time.deltaTime;
                            }

                        }
                    }
                    break;
                case "Trampoline":
                    JumpHeight = 2f;
                    isJump = true;
                    break;
                default:
                    JumpHeight = 1.2f;
                    break;
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void UpdateTagServerRpc(bool tagStatus, ulong clientId)
        {
            punchSound.Play();
            var clientWithDamaged = NetworkManager.Singleton.ConnectedClients[clientId]
                .PlayerObject.GetComponent<ThirdPersonController>();

            if (clientWithDamaged != null)
            {
                clientWithDamaged.isTag.Value = tagStatus;
                isTag.Value = !tagStatus;
            }
        }

        [ServerRpc]
        private void UpdateVisibilityServerRpc(bool param)
        {
            _flagVisibility.Value = param;
        }

        [ServerRpc]
        public void UpdateAnimatorServerRpc(string parametr, float value)
        {
            animator.SetFloat(parametr, value);
        }

        [ServerRpc]
        public void UpdateAnimatorServerRpc(string parametr, bool status)
        {
            animator.SetBool(parametr, status);
        }

        void OnTriggerEnter(Collider col)
        {
            if (col.gameObject.tag == "Chest")
            {
                col.gameObject.GetComponentInParent<ChestAnimation>().ChestOpen(targetInventoryWindow);
            }
        }

    }
}