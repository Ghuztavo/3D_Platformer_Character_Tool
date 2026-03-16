using System;
using UnityEngine;

/// <summary>
/// Optional dash stats. Only used if the character has dash enabled.
/// </summary>
[Serializable]
public class DashStats
{
    [Tooltip("Distance covered when performing a dash.")]
    [Min(0.1f)]
    public float dashDistance = 5f;

    [Tooltip("Cooldown in seconds before the character can dash again.")]
    [Min(0f)]
    public float dashCooldown = 1f;
}
