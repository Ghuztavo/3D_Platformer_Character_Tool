using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Studio.PlatformerCharacterTool
{
    /// <summary>
    /// Handles the attack ability. Reads combat stats from CharacterData via PlayerController.
    /// Only active if characterData.hasAttack is true.
    /// Spawns a sphere trigger in front of the player for a short duration.
    /// Input: Left Mouse Button or reassign dashKey in the Inspector.
    /// </summary>
    [RequireComponent(typeof(PlayerController))]
    public class AttackAbility : MonoBehaviour
    {
        [Header("Input")]
        [Tooltip("Mouse button used to trigger the attack (0 = left, 1 = right, 2 = middle).")]
        [SerializeField] private int attackMouseButton = 0;

        [Header("Hit Box Settings")]
        [Tooltip("How far in front of the player the hitbox spawns.")]
        [SerializeField] private float hitboxOffset   = 1f;
        [Tooltip("Radius of the sphere trigger hitbox.")]
        [SerializeField] private float hitboxRadius   = 0.5f;
        [Tooltip("How long the hitbox stays active (seconds).")]
        [SerializeField] private float hitboxDuration = 0.2f;

        // Internal state
        private PlayerController _player;
        private bool             _isAttacking;
        private float            _cooldownTimer;

        // Unity
        private void Awake()
        {
            _player = GetComponent<PlayerController>();
        }

        private void Update()
        {
            // No data or attack not enabled — do nothing
            if (_player.Data == null || !_player.Data.hasAttack) return;

            if (_cooldownTimer > 0f)
                _cooldownTimer -= Time.deltaTime;

            var mouse = Mouse.current;
            if (mouse == null) return;

            bool attackPressed = attackMouseButton switch
            {
                0 => mouse.leftButton.wasPressedThisFrame,
                1 => mouse.rightButton.wasPressedThisFrame,
                2 => mouse.middleButton.wasPressedThisFrame,
                _ => false
            };

            if (attackPressed && !_isAttacking && _cooldownTimer <= 0f)
            {
                StartCoroutine(AttackRoutine());
            }
        }

        // Attack
        private IEnumerator AttackRoutine()
        {
            _isAttacking = true;

            // Spawn position: in front of the player
            Vector3 spawnPos = transform.position + transform.forward * hitboxOffset;

            // Create the hitbox sphere
            GameObject hitbox = new GameObject("AttackHitbox");
            hitbox.transform.position = spawnPos;
            hitbox.transform.SetParent(transform);

            SphereCollider col = hitbox.AddComponent<SphereCollider>();
            col.isTrigger = true;
            col.radius    = hitboxRadius;

            // Add the helper that reports hits
            AttackHitbox hitboxLogic = hitbox.AddComponent<AttackHitbox>();
            hitboxLogic.Init(_player.Data.combat.damage);

            // Wait for duration
            yield return new WaitForSeconds(hitboxDuration);

            Destroy(hitbox);

            _isAttacking   = false;
            _cooldownTimer = _player.Data.combat.attackCooldown;
        }

        // Public so PlayerAnimations can query it
        public bool IsAttacking => _isAttacking;
    }

    /// <summary>
    /// Small helper placed on the hitbox sphere.
    /// Reports trigger collisions and applies damage.
    /// </summary>
    public class AttackHitbox : MonoBehaviour
    {
        private float _damage;

        public void Init(float damage) => _damage = damage;

        private void OnTriggerEnter(Collider other)
        {
            // Ignore the player itself and its children
            if (other.transform.IsChildOf(transform.root)) return;

            Debug.Log($"[AttackHitbox] Hit: {other.name} for {_damage} damage.");
            // Here you would apply damage to the hit object if it has a health component, etc.
        }
    }
}
