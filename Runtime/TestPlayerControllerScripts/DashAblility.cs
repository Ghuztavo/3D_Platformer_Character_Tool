using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Studio.PlatformerCharacterTool
{
    /// <summary>
    /// Handles the dash ability. Reads dash stats from CharacterData via PlayerController.
    /// Only active if characterData.hasDash is true.
    /// Input: Left Ctrl or reassign dashKey in the Inspector.
    /// </summary>
    [RequireComponent(typeof(PlayerController))]
    public class DashAblility : MonoBehaviour
    {
        [Header("Input")]
        [Tooltip("Key used to trigger dash.")]
        [SerializeField] private Key dashKey = Key.E;

        // Internal state
        private PlayerController _player;
        private Rigidbody        _rb;
        private bool             _isDashing;
        private float            _cooldownTimer;

        // Unity
        private void Awake()
        {
            _player = GetComponent<PlayerController>();
            _rb     = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            // dash not enabled — do nothing
            if (_player.Data == null || !_player.Data.hasDash) return;

            // Tick cooldown
            if (_cooldownTimer > 0f)
                _cooldownTimer -= Time.deltaTime;

            var kb = Keyboard.current;
            if (kb == null) return;

            if (kb[dashKey].wasPressedThisFrame && !_isDashing && _cooldownTimer <= 0f)
            {
                StartCoroutine(DashRoutine());
            }
        }

        // Dash
        private IEnumerator DashRoutine()
        {
            _isDashing = true;

            DashStats dash = _player.Data.dash;

            // Direction: player's camera-relative move direction or current facing direction if no input
            Vector3 dashDir = _player.MoveDirection != Vector3.zero 
                ? _player.MoveDirection 
                : transform.forward;

            float dashDuration = 0.15f;
            float dashSpeed    = dash.dashDistance / dashDuration;

            _rb.useGravity    = false;
            _rb.linearVelocity = Vector3.zero;

            float elapsed = 0f;
            while (elapsed < dashDuration)
            {
                _rb.linearVelocity = dashDir * dashSpeed;
                elapsed += Time.deltaTime;
                yield return null;
            }

            _rb.linearVelocity = dashDir * (_player.IsSprinting
                ? _player.Data.movement.sprintSpeed
                : _player.Data.movement.walkSpeed);
            _rb.useGravity = true;
            _isDashing     = false;

            // Start cooldown
            _cooldownTimer = dash.dashCooldown;
        }

        // Public for the PlayerAnimations script
        public bool IsDashing => _isDashing;
    }
}
