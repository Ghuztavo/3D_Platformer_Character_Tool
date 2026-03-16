using UnityEngine;
using System;

/// <summary>
/// Core movement stats for a platformer character.
/// </summary>
[Serializable]
public class MovementStats
{
    [Tooltip("Walking speed.")]
    public float walkSpeed = 5f;

    [Tooltip("Sprinting speed.")]
    public float sprintSpeed = 10f;

    [Tooltip("Jump Height.")]
    public float jumpForce = 10f;

    [Tooltip("Number of jumps the character can perform before touching the ground.")]
    [Min(1)]
    public int maxJumps = 1;

    [Tooltip("Physics mass of the character.")]
    [Min(0.1f)]
    public float mass = 1f;

    [Tooltip("Drag applied to the character when grounded.")]
    [Min(0f)]
    public float groundDrag = 5f;

}
