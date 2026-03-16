using UnityEngine;

namespace Studio.PlatformerCharacterTool
{
    /// <summary>
    /// Animations are not required for the character to function, but this script can be used to drive an Animator on the character model if one is provided.
    /// Drives the Animator on the spawned model using state from PlayerController.
    /// Animator parameters expected:
    ///   Floats : "Strafe", "Walk"
    ///   Bools  : "IsWalking", "IsRunning", "Jumping", "Falling", "Dashing", "Attacking"
    ///   The animator for this script is expected to have the next states:
    ///   - Idle
    ///   - Walk (blend tree using "Strafe" and "Walk" parameters) for walking in any direction
    ///   - Run (blend tree using "Strafe" and "Walk" parameters) for running in any direction
    ///   - Jump
    ///   - Fall
    ///   - Dash
    ///   - Attack
    /// </summary>
    [RequireComponent(typeof(PlayerController))]
    public class PlayerAnimations : MonoBehaviour
    {
        // Cached references
        private PlayerController _player;
        private DashAblility     _dash;
        private AttackAbility    _attack;
        private Animator         _animator;

        // Animator parameters
        private static readonly int _hashStrafe    = Animator.StringToHash("Strafe");
        private static readonly int _hashWalk      = Animator.StringToHash("Walk");
        private static readonly int _hashIsWalking = Animator.StringToHash("IsWalking");
        private static readonly int _hashIsRunning = Animator.StringToHash("IsRunning");
        private static readonly int _hashJumping   = Animator.StringToHash("Jumping");
        private static readonly int _hashFalling   = Animator.StringToHash("Falling");
        private static readonly int _hashDashing   = Animator.StringToHash("Dashing");
        private static readonly int _hashAttacking = Animator.StringToHash("Attacking");

        // Get the PlayerController and and ability components on the same GameObject.
        private void Awake()
        {
            _player = GetComponent<PlayerController>();
            _dash   = GetComponent<DashAblility>();
            _attack = GetComponent<AttackAbility>();
        }

        private void Start()
        {
            if (_player.CharAnimator == null)
            {
                Debug.LogWarning("[PlayerAnimations] No Animator found on player model — animations disabled.");
                return;
            }
            // Animator is on the player model
            _animator = _player.CharAnimator;
        }

        private void Update()
        {
            // No animator or no data — nothing to drive
            if (_player.Data == null)
            {
                return;
            }
            if (_animator == null )
            {
                return;
            }

            UpdateMovementParams();
            UpdateAirParams();
            UpdateAbilityParams();
        }

        // Parameter Updates for the animator.
        private void UpdateMovementParams()
        {
            float h = _player.InputH;
            float v = _player.InputV;
            bool  moving    = _player.MoveDirection.sqrMagnitude > 0.01f;
            bool  sprinting = _player.IsSprinting && moving;

            // Blend tree values
            //   Walk  -> forward/back axis  (v)
            //   Strafe -> side axis         (h)
            _animator.SetFloat(_hashWalk,   v, 0.1f, Time.deltaTime);
            _animator.SetFloat(_hashStrafe, h, 0.1f, Time.deltaTime);

            _animator.SetBool(_hashIsWalking, moving);
            _animator.SetBool(_hashIsRunning, moving && sprinting);
        }

        // Parameter updates related to being in the air (jumping/falling), for the animator.
        private void UpdateAirParams()
        {
            Rigidbody rb = _player.Rb;

            bool jumping = !_player.IsGrounded && rb.linearVelocity.y > 0.1f;
            bool falling = !_player.IsGrounded && rb.linearVelocity.y < -0.1f;

            _animator.SetBool(_hashJumping, jumping);
            _animator.SetBool(_hashFalling, falling);
        }

        // Parameter updates related to abilities (dashing/attacking), for the animator.
        private void UpdateAbilityParams()
        {
            if (_dash != null)
                _animator.SetBool(_hashDashing, _dash.IsDashing);

            if (_attack != null)
                _animator.SetBool(_hashAttacking, _attack.IsAttacking);
        }
    }
}
