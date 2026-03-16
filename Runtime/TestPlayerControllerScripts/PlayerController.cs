using UnityEngine;
using UnityEngine.InputSystem;

namespace Studio.PlatformerCharacterTool
{
    /// <summary>
    /// Example player controller. Reads all stats from a CharacterData ScriptableObject.
    /// Requires a Rigidbody on the same GameObject.
    /// Add a collider (e.g. CapsuleCollider) to the same GameObject as well.
    /// Attach DashAbility, AttackAbility and PlayerAnimations on the same GameObject.
    /// This player controller was created using Cinemachine 3.1.6 Unity package for camera control.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Character Data")]
        [Tooltip("Drag a CharacterData asset here. If empty, a capsule placeholder is spawned and no abilities work.")]
        public CharacterData characterData;

        [Header("Ground Check")]
        [Tooltip("Place an empty GameObject in the player object at the bottom and place it here.")]
        [SerializeField] private Transform feet;
        [Tooltip("Radius of the sphere used to check if grounded.")]
        [SerializeField] private float groundCheckRadius = 0.2f;
        [Tooltip("Which layers count as ground for jumping.")]
        [SerializeField] private LayerMask groundLayer;

        // Internal state
        private Rigidbody _rb;
        private DashAblility _dashAbility;
        private GameObject _spawnedModel;
        private Animator   _animator;

        private int   _jumpsLeft;
        private bool  _isGrounded;
        private bool  _isSprinting;

        // Cached input axes
        private float _inputH;
        private float _inputV;
        private Vector3 _moveDirection;

        // Public accessors for ability / animation scripts
        public CharacterData Data          => characterData;
        public Rigidbody     Rb            => _rb;
        public bool          IsGrounded    => _isGrounded;
        public bool          IsSprinting   => _isSprinting;
        public float         InputH        => _inputH;
        public float         InputV        => _inputV;
        public Vector3       MoveDirection => _moveDirection;
        public Animator      CharAnimator  => _animator;

        // Unity
        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _rb.freezeRotation = true;
            _rb.constraints = RigidbodyConstraints.FreezeRotation;
            _dashAbility = GetComponent<DashAblility>();

            if (characterData != null)
            {
                SpawnModel();
            }
            else
            {
                SpawnPlaceholderCapsule();
                Debug.LogWarning("[PlayerController] No CharacterData assigned — placeholder capsule spawned. Abilities disabled.");
            }
        }

        private void Start()
        {
            // Hide and lock cursor
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            if (characterData == null) return;

            // Apply physics mass from the character data
            _rb.mass = characterData.movement.mass;

            // Initialise jumps
            _jumpsLeft = characterData.movement.maxJumps;
            Debug.Log($"[PlayerController] Starting with {_jumpsLeft} jumps available.");
        }

        private void Update()
        {
            if (characterData == null) return;

            GatherInput();

            // Skip other logic if currently dashing
            if (_dashAbility != null && _dashAbility.IsDashing) return;

            HandleJump();
        }

        private void FixedUpdate()
        {
            if (characterData == null) return;

            CheckGround();

            // Skip normal movement and rotation if currently dashing
            if (_dashAbility != null && _dashAbility.IsDashing) return;

            Move();
            RotateToCamera();
        }

        // Input
        private void GatherInput()
        {
            var kb = Keyboard.current;
            if (kb == null) return;

            float h = 0f, v = 0f;
            if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) h += 1f;
            if (kb.aKey.isPressed || kb.leftArrowKey.isPressed)  h -= 1f;
            if (kb.wKey.isPressed || kb.upArrowKey.isPressed)    v += 1f;
            if (kb.sKey.isPressed || kb.downArrowKey.isPressed)  v -= 1f;

            _inputH      = h;
            _inputV      = v;
            _isSprinting = kb.leftShiftKey.isPressed;

            // Calculate movement relative to the camera
            Transform cam = Camera.main != null ? Camera.main.transform : transform;
            Vector3 camF = cam.forward;
            camF.y = 0f;
            camF.Normalize();
            
            Vector3 camR = cam.right;
            camR.y = 0f;
            camR.Normalize();
            
            _moveDirection = (camR * _inputH + camF * _inputV).normalized;
        }

        // Movement
        private void Move()
        {
            float speed = _isSprinting
                ? characterData.movement.sprintSpeed
                : characterData.movement.walkSpeed;

            Vector3 targetVelocity = _moveDirection * speed;

            targetVelocity.y = _rb.linearVelocity.y;
            _rb.linearVelocity = targetVelocity;
        }

        // Jump
        private void HandleJump()
        {
            var kb = Keyboard.current;
            if (kb == null) return;

            if (kb.spaceKey.wasPressedThisFrame && _jumpsLeft > 0)
            {
                _rb.AddForce(Vector3.up * characterData.movement.jumpForce, ForceMode.Impulse);
                _jumpsLeft--;
            }
        }

        // Ground Check
        private void CheckGround()
        {
            Transform checkOrigin = feet != null ? feet : transform;
            _isGrounded = Physics.CheckSphere(checkOrigin.position, groundCheckRadius, groundLayer);

            if (_isGrounded)
            {
                // Just landed — restore jumps
                _jumpsLeft = characterData.movement.maxJumps;
            }

            // Apply ground drag when grounded, 0 otherwise
            _rb.linearDamping = _isGrounded ? characterData.movement.groundDrag : 0f;
        }

        // Rotation
        private void RotateToCamera()
        {
            Transform cam = Camera.main != null ? Camera.main.transform : transform;
            Vector3 camF = cam.forward;
            camF.y = 0f;
            camF.Normalize();

            if (camF != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(camF);
                _rb.MoveRotation(Quaternion.Slerp(_rb.rotation, targetRotation, Time.fixedDeltaTime * 15f));
            }
        }

        // Visuals
        private void SpawnModel()
        {
            if (characterData.visuals.modelPrefab != null)
            {
                _spawnedModel = Instantiate(
                    characterData.visuals.modelPrefab,
                    transform.position + characterData.visuals.modelOffset,
                    Quaternion.identity,
                    transform);

                _spawnedModel.transform.localPosition = characterData.visuals.modelOffset;
                _spawnedModel.transform.localRotation = Quaternion.Euler(characterData.visuals.modelRotation);
                _spawnedModel.transform.localScale = characterData.visuals.modelScale;
            }
            else
            {
                // No model assigned — spawn a simple capsule as stand-in
                SpawnPlaceholderCapsule();
                Debug.LogWarning("[PlayerController] No model prefab assigned in CharacterData — placeholder capsule spawned.");
            }

            // Search the player hierarchy for an Animator
            _animator = GetComponentInChildren<Animator>();
            
            // Only add an Animator and assign the controller if a controller provided in the CharacterData
            if (characterData.visuals.animatorController != null)
            {
                if (_animator == null && _spawnedModel != null)
                {
                    _animator = _spawnedModel.AddComponent<Animator>();
                    Debug.Log("[PlayerController] Adding Animator to the spawned model.");
                }

                if (_animator != null)
                {
                    _animator.runtimeAnimatorController = characterData.visuals.animatorController;
                }
            }
        }

        private void SpawnPlaceholderCapsule()
        {
            _spawnedModel = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            _spawnedModel.transform.SetParent(transform);
            _spawnedModel.transform.localPosition = Vector3.zero;
            _spawnedModel.transform.localRotation = Quaternion.identity;

            Destroy(_spawnedModel.GetComponent<Collider>());
        }

        // Gizmos
        private void OnDrawGizmosSelected()
        {
            if (feet == null) return;
            Gizmos.color = _isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(feet.position, groundCheckRadius);
        }
    }
}
