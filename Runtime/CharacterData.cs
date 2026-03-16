using UnityEngine;

/// <summary>
/// Main ScriptableObject asset representing a platformer character.
/// Created and edited via the Platformer Character Tool window.
/// Or right-click in the Project window > Create > Scriptable Objects > CharacterData
/// </summary>
[CreateAssetMenu(fileName = "CharacterData", menuName = "Scriptable Objects/CharacterData")]
public class CharacterData : ScriptableObject
{
    [Tooltip("Display name of the character.")]
    public string characterName = "New Character";

    [Tooltip("Maximum health points of the character.")]
    [Min(1f)]
    public float maxHealth = 100f;

    public MovementStats movement = new MovementStats();

    [Tooltip("Toggle to enable attack ability and show combat stats.")]
    public bool hasAttack = false;

    [Tooltip("Combat stats. Only relevant if hasAttack is true.")]
    public CombatStats combat = new CombatStats();

    [Tooltip("Toggle to enable dash ability and show dash stats.")]
    public bool hasDash = false;

    [Tooltip("Dash stats. Only relevant if hasDash is true.")]
    public DashStats dash = new DashStats();

    public VisualReferences visuals = new VisualReferences();
}
