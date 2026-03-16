using UnityEngine;
using System;

/// <summary>
/// Optional combat stats. Only used if the character has attack enabled.
/// </summary>
[Serializable]
public class CombatStats
{
    [Tooltip("Damage dealt per attack.")]
    [Min(0f)]
    public float damage = 10f;

    [Tooltip("Minimum time in seconds between each attack.")]
    [Min(0.1f)]
    public float attackCooldown = 0.5f;
}
