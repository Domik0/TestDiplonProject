using System.Collections.Generic;
using Assets;
using Unity.VisualScripting;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;
namespace StarterAssets
{

    public class ThirdPersonController : NetworkBehaviour
    {
        [Header("Player")]
        [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 2.0f;
        [Tooltip("Sprint speed of the character in m/s")]
        public float SprintSpeed = 5.335f;
        [Tooltip("Climbing speed of the character in m/s")]
        public float ClimbingSpeed = 0.5f;
        [Tooltip("How fast the character turns to face movement direction")]
        [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;
        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;
        public Renderer myObject;
        [Space(10)]
        [Tooltip("The height the player can jump")]
        public float JumpHeight = 1.2f;
        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;
        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float JumpTimeout = 0.50f;
        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float FallTimeout = 0.15f;
        [Tooltip("Time required to perform the dance")]
        public float DanceTimeout = 0.30f;
        public float PunchTimeout = 0.1f;
        public GameObject inventoryObject;

        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool Grounded = true;
        [Tooltip("Useful for rough ground")]
        public float GroundedOffset = -0.14f;
        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.28f;
        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;
        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;
        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 70.0f;
        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -30.0f;
        [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
        public float CameraAngleOverride = 0.0f;
        [Tooltip("For locking the camera position on all axis")]
        public bool LockCameraPosition = false;

        [SerializeField]
        private float minPucnhDistance=1.0f;
        [SerializeField]
        private GameObject hand;

        [Header("Network")] 
        [SerializeField]
        private NetworkVariable<float> networkAnimationBlend = new NetworkVariable<float>();
        [SerializeField]
        private NetworkVariable<int> networkDanceId = new NetworkVariable<int>();
        [SerializeField]
        private NetworkVariable<PlayerState> networkPlayerState= new NetworkVariable<PlayerState>();
        [SerializeField]
        private NetworkVariable<bool> GroundNetwork = new NetworkVariable<bool>();
        [SerializeField]
        private NetworkVariable<bool> animJumpNetwork = new NetworkVariable<bool>();
        [SerializeField]
        private NetworkVariable<bool> animFallNetwork = new NetworkVariable<bool>();
        [SerializeField]
        private NetworkVariable<int> countHits = new NetworkVariable<int>();
        [SerializeField]
        private NetworkVariable<bool> isTag = new NetworkVariable<bool>();


        // client caches positions
        private PlayerState _oldPlayerState = PlayerState.Move;
        private int _oldDanceId;
        private float _oldAnimationBlend;
        private bool _oldGround;
        private bool _oldAnimJump;
        private bool _oldAnimFall;
        private bool _oldTag;

        // player
        private bool _climbing;
        private bool _climbingZone;
        private float _speed;
        private float _animationBlend;
       
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;
        private float _throwPower = 4f;

        // timeout deltatime
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;
        private float _danceTimeoutDelta;
        private float _punchTimeoutDelta;

        // animation IDs
        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;
        private int _animIDDance;
        private int _animIDDanceID;
        private int _animIDClimbingUp;
        private int _animIDClimbingDown;


        private Animator _animator;
        private CharacterController _controller;
        private StarterAssetsInputs _input;
        private GameObject _mainCamera;
        private const float _threshold = 0.01f;
        private bool _hasAnimator;


        private void Awake()
        {
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
            _controller = GetComponent<CharacterController>();
            _animator = GetComponent<Animator>();
            _input = GetComponent<StarterAssetsInputs>();
        }

        private void Start()
        {
            
            if (IsClient && IsOwner)
            {
                PlayerFollow.Instance.FollowPlayer(transform.Find("PlayerCameraRoot"));
                _animator = GetComponent<Animator>();
                _hasAnimator = TryGetComponent(out _animator);
                UICanvasControllerInput.Instance.FollowPlayer(_input); 
                //СhangePlayerColor(gameObject.tag);
                AssignAnimationIDs();
                _jumpTimeoutDelta = JumpTimeout;
                _fallTimeoutDelta = FallTimeout;
            }

        }

        private void Update()
        {
            if (IsClient && IsOwner)
            {
                //if (!_climbing)
                //{
                GroundedCheck();
                JumpAndGravity();
                ClientMove();
                Dance();
                Beating();
                Throwing();
                //}
                //Climbing();
            }

            ClientVisuals();
        }

        private void LateUpdate()
        {
            if (IsClient && IsOwner)
            {
                CameraRotation();
            }
        }

        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
            _animIDDance = Animator.StringToHash("Dance");
            _animIDDanceID = Animator.StringToHash("DanceID");
            _animIDClimbingUp = Animator.StringToHash("ClimbingUp");
            _animIDClimbingDown = Animator.StringToHash("ClimbingDown");
        }

        private void Beating()
        {
            if (_input.punch)
            {
                if (_hasAnimator && _punchTimeoutDelta <= 0.0f)
                {
                    UpdatePlayerStateServerRpc(PlayerState.Punch);
                    CheckPunch(hand.transform,Vector3.down);
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
            if (_input.throwObject)
            {
                if (_hasAnimator)
                {
                    _animator.SetTrigger("Throw");
                }
            }
        }

        private void ThrowObject()
        {
            var invetoryObjectRb = inventoryObject.GetComponent<Rigidbody>();
            invetoryObjectRb.transform.parent = null;
            invetoryObjectRb.isKinematic = false;
            invetoryObjectRb.AddForce(transform.forward*20, ForceMode.Impulse);
            inventoryObject.GetComponent<Destruction>().isThrow = true;
        }

        private void GroundedCheck()
        {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);

            // update animator if using character
            if (_hasAnimator)
            {
                UpdateGroundServerRpc(Grounded);
            }
        }

        private void CameraRotation()
        {
            // if there is an input and camera position is not fixed
            if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
            {
                _cinemachineTargetYaw += _input.look.x * Time.deltaTime;
                _cinemachineTargetPitch += _input.look.y * Time.deltaTime;
            }
            // clamp our rotations so our values are limited 360 degrees
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

            // Cinemachine will follow this target
            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride, _cinemachineTargetYaw, 0.0f);
        }

        private void ClientMove()
        {
            // set target speed based on move speed, sprint speed and if sprint is pressed
            bool isSprint = CheckSprint();
            float targetSpeed = isSprint ? SprintSpeed : MoveSpeed;
            // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon
            // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is no input, set the target speed to 0
            if (_input.move == Vector2.zero) targetSpeed = 0.0f;
            // a reference to the players current horizontal velocity
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;
            float speedOffset = 0.1f;
            float inputMagnitude =1f;
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
            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

            // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is a move input rotate player when the player is moving
            if (_input.move != Vector2.zero)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + _mainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, RotationSmoothTime);

                // rotate to face input direction relative to camera position
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }

            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

            // move the player
            _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

            // update animator if using character
            if (_hasAnimator)
            {
                UpdatePlayerStateServerRpc(PlayerState.Move);
                UpdateAnimationBlendServerRpc(_animationBlend);
                
            }
        }


        private void ClientVisuals()
        {

            if (_oldPlayerState != networkPlayerState.Value)
            {
                _oldPlayerState = networkPlayerState.Value;
            }
            if (networkPlayerState.Value == PlayerState.Dance)
            {
              
                if (_oldDanceId != networkDanceId.Value)
                {
                    _oldDanceId = networkDanceId.Value;
                    _animator.SetFloat("DanceID", networkDanceId.Value);
                    _animator.SetTrigger("Dance");
                }
            }

            if (networkPlayerState.Value == PlayerState.Move)
            {
                if (_oldAnimationBlend != networkAnimationBlend.Value)
                {
                    _oldAnimationBlend = networkAnimationBlend.Value;
                    _animator.SetFloat("Speed", networkAnimationBlend.Value);
                }
                if (_oldAnimFall != animFallNetwork.Value)
                {
                    _oldAnimFall = animFallNetwork.Value;
                    _animator.SetBool("FreeFall", animFallNetwork.Value);
                }

                if (_oldAnimJump != animJumpNetwork.Value)
                {
                    _oldAnimJump = animJumpNetwork.Value;
                    _animator.SetBool("Jump", animJumpNetwork.Value);
                }

                if (_oldGround != GroundNetwork.Value)
                {
                    _oldGround = GroundNetwork.Value;
                    _animator.SetBool("Grounded",GroundNetwork.Value);
                }
            }

            if (networkPlayerState.Value == PlayerState.Punch)
            {
                _animator.SetTrigger("Punch");
                
            }

            if (_oldTag != isTag.Value)
            {
                _oldTag = isTag.Value;
                СhangePlayerColor(isTag.Value);
            }
        }



        [ServerRpc]
        private void UpdatePlayerStateServerRpc(PlayerState state)
        {
            networkPlayerState.Value = state;
          
        }

        [ServerRpc]
        private void UpdateDanceIdServerRpc(int danceId)
        {
            networkDanceId.Value = danceId;
        }

        [ServerRpc]
        private void UpdateAnimationBlendServerRpc(float blend)
        {
            networkAnimationBlend.Value = blend;
        }

        [ServerRpc]
        private void UpdateJumpServerRpc(bool isJump)
        {
            animJumpNetwork.Value = isJump;
        }

        [ServerRpc]
        private void UpdateFallServerRpc(bool isFall)
        {
            animFallNetwork.Value = isFall;
        }

        [ServerRpc]
        private void UpdateGroundServerRpc(bool isGround)
        {
            GroundNetwork.Value = isGround;
        }

        [ServerRpc]
        private void UpdateTagServerRpc(bool takeAwayPoint,ulong clientId)
        {
            var clientWithDamaged = NetworkManager.Singleton.ConnectedClients[clientId]
                .PlayerObject.GetComponent<ThirdPersonController>();

            if (clientWithDamaged != null)
            {
                clientWithDamaged.isTag.Value = takeAwayPoint;
                clientWithDamaged.countHits.Value += 1;
                isTag.Value = !takeAwayPoint;
            }
            NotifyHealthChangedClientRpc(takeAwayPoint, new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { clientId }
                }
            });
        }



        private bool CheckSprint()
        {
            var valueX = _input.move.x;
            var valueY = _input.move.y;
            if (valueX > 0.75 || valueX < -0.75 || valueY > 0.75 || valueY < -0.75)
            {
                return true;
            }
            return false;
        }

        private void CheckPunch(Transform hand, Vector3 aimDirection)
        {
            RaycastHit hit;

            int layerMask = LayerMask.GetMask("Player");

            if (Physics.Raycast(hand.position, hand.transform.TransformDirection(aimDirection), out hit, minPucnhDistance, layerMask))
            {
                Debug.DrawRay(hand.position, hand.transform.TransformDirection(aimDirection) * minPucnhDistance, Color.yellow);

                var playerHit = hit.transform.GetComponent<NetworkObject>();
                if (playerHit != null)
                {
                    
                        UpdateTagServerRpc(true, playerHit.OwnerClientId);
                }
            
            }
            else
            {
                Debug.DrawRay(hand.position, hand.transform.TransformDirection(aimDirection) * minPucnhDistance, Color.red);
            }
        }

        [ClientRpc]
        private void NotifyHealthChangedClientRpc(bool takeAwayPoint, ClientRpcParams clientRpcParams)
        {
            if (IsOwner) return;

            Debug.Log($"Client got punch {takeAwayPoint}");
        }


        private void JumpAndGravity()
        {
            if (GroundNetwork.Value)
            {
                // reset the fall timeout timer
                _fallTimeoutDelta = FallTimeout;

                // update animator if using character
                if (_hasAnimator)
                {
                    UpdateJumpServerRpc(false);
                    UpdateFallServerRpc(false);
                    //_animator.SetBool(_animIDJump, false);
                    //_animator.SetBool(_animIDFreeFall, false);
                }

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

                    // update animator if using character
                    if (_hasAnimator)
                    {
                     //   UpdatePlayerStateServerRpc(PlayerState.Jump);
                        UpdateJumpServerRpc(true);
                        //_animator.SetBool(_animIDJump, true);
                    }
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
                    // update animator if using character
                    if (_hasAnimator)
                    {
                        UpdateFallServerRpc(true);
                        //_animator.SetBool(_animIDFreeFall, true);
                    }
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


        private void Dance()
        {
            if (_input.dance > 0)
            {
                if (Grounded)
                {
                    if (_hasAnimator && _danceTimeoutDelta <= 0.0f)
                    {
                        _danceTimeoutDelta = DanceTimeout;
                        
                        UpdateDanceIdServerRpc(_input.dance);
                        UpdatePlayerStateServerRpc(PlayerState.Dance);
                    }

                    if (_danceTimeoutDelta >= 0.0f)
                    {
                        _danceTimeoutDelta -= Time.deltaTime;
                    }

                    if (_danceTimeoutDelta <= 0.0f)
                    {
                        _input.dance = 0;
                    }
                }
                else
                {
                    _input.dance = 0;
                }
            }
        }

      

        void OnControllerColliderHit(ControllerColliderHit hit)
        {
            switch (hit.gameObject.tag)
            {
                case "Trampoline":
                    JumpHeight = 2f;
                    _input.jump = true;
                    break;
                default:
                    JumpHeight = 1.2f;
                    break;
            }
        }



        private void СhangePlayerColor(bool tag)
        {
            if (!tag)
            {
                foreach (var items in myObject.materials)
                {
                    items.color = Color.white;
                }
            }

            if (tag)
            {
                foreach (var items in myObject.materials)
                {
                    items.color = Color.red;
                }
            }
        }

    }
}