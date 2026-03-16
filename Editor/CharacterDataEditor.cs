using UnityEngine;
using UnityEditor;

/// <summary>
/// Custom inspector for CharacterData. Hides combat/dash stats
/// unless the respective ability toggles are enabled.
/// </summary>
[CustomEditor(typeof(CharacterData))]
public class CharacterDataEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        CharacterData character = (CharacterData)target;

        // --- Identity ---
        EditorGUILayout.LabelField("Identity", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("characterName"));
        EditorGUILayout.Space();

        // --- Health ---
        EditorGUILayout.LabelField("Health", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("maxHealth"));
        EditorGUILayout.Space();

        // --- Movement ---
        EditorGUILayout.LabelField("Movement", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("movement"), true);
        EditorGUILayout.Space();

        // --- Abilities ---
        EditorGUILayout.LabelField("Abilities", EditorStyles.boldLabel);

        SerializedProperty hasAttack = serializedObject.FindProperty("hasAttack");
        EditorGUILayout.PropertyField(hasAttack, new GUIContent("Has Attack"));
        if (character.hasAttack)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("combat"), true);
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.Space(4);

        SerializedProperty hasDash = serializedObject.FindProperty("hasDash");
        EditorGUILayout.PropertyField(hasDash, new GUIContent("Has Dash"));
        if (character.hasDash)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("dash"), true);
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.Space();

        // --- Visuals ---
        EditorGUILayout.LabelField("Visuals", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("visuals"), true);

        serializedObject.ApplyModifiedProperties();
    }
}
