using UnityEngine;
using System;

/// <summary>
/// Optional visual references for the character. Model and animations are not required.
/// </summary>
[Serializable]
public class VisualReferences
{
    [Tooltip("3D model prefab for this character. Can be left empty.")]
    public GameObject modelPrefab;

    [Tooltip("Offset to apply to the model when spawned. Adjust if the model's pivot is not at the feet.")]
    public Vector3 modelOffset = new Vector3(0f, 0f, 0f);

    [Tooltip("Rotation to apply to the model when spawned. Adjust if the model faces the wrong direction.")]
    public Vector3 modelRotation = new Vector3(0f, 0f, 0f);

    [Tooltip("Scale to apply to the model when spawned. Adjust if the model is too big or small.")]
    public Vector3 modelScale = new Vector3(1f, 1f, 1f);

    [Tooltip("Animator Controller for the model. Can be left empty.")]
    public RuntimeAnimatorController animatorController;
}
