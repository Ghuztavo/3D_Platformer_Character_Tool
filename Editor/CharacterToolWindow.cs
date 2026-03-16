using UnityEngine;
using UnityEditor;

/// <summary>
/// Main editor window for the Platformer Character Tool.
/// Lets designers create, browse and edit CharacterData assets.
/// Open via: Tools > Platformer Character Tool
/// </summary>
public class CharacterToolWindow : EditorWindow
{
    private CharacterData _selected; // Currently selected CharacterData asset
    private UnityEditor.Editor _cachedEditor; // Cached inspector editor for the selected asset
    private Vector2 _scrollPos; // Scroll position for the editor area

    // Tool Window --------------------------------------------------------
    [MenuItem("Tools/Platformer Character Tool")]
    public static void Open()
    {
        var window = GetWindow<CharacterToolWindow>("Platformer Character Tool");
        window.minSize = new Vector2(420, 520);
    }

    // GUI ----------------------------------------------------------------
    private void OnGUI()
    {
        DrawHeader();
        DrawSelectionRow();
        DrawDivider();

        if (_selected == null)
        {
            EditorGUILayout.HelpBox(
                "Create a new character or select an existing CharacterData asset.",
                MessageType.Info);
            return;
        }

        DrawCharacterEditor();
    }

    // Header -------------------------------------------------------------
    private void DrawHeader()
    {
        GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 15,
            alignment = TextAnchor.MiddleLeft
        };

        EditorGUILayout.Space(6);
        EditorGUILayout.LabelField("Platformer Character Tool", titleStyle);
        EditorGUILayout.Space(4);
    }

    // Selection Row ------------------------------------------------------
    private void DrawSelectionRow()
    {
        EditorGUILayout.BeginHorizontal();

        // Object picker
        CharacterData previous = _selected;
        _selected = (CharacterData)EditorGUILayout.ObjectField(
            "Character Asset", _selected, typeof(CharacterData), false);

        // Rebuild cached editor if selection changed
        if (_selected != previous)
        {
            _cachedEditor = null;
        }

        // Create new button
        if (GUILayout.Button("New", GUILayout.Width(50)))
        {
            CreateNewCharacter();
        }

        EditorGUILayout.EndHorizontal();
    }

    // Character Editor ------------------------------------------------------
    private void DrawCharacterEditor()
    {
        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

        UnityEditor.Editor.CreateCachedEditor(_selected, null, ref _cachedEditor);
        _cachedEditor?.OnInspectorGUI();

        EditorGUILayout.Space(10);

        // Assets autosave in Unity editor, but its there just in case
        if (GUILayout.Button("Save Asset", GUILayout.Height(32)))
        {
            EditorUtility.SetDirty(_selected);
            AssetDatabase.SaveAssets();
            Debug.Log($"[Character Tool] Saved: {_selected.characterName}");
        }

        EditorGUILayout.EndScrollView();
    }

    // Helpers -------------------------------------------------------------
    private void CreateNewCharacter()
    {
        string path = EditorUtility.SaveFilePanelInProject(
            "Save New Character",
            "NewCharacterData",
            "asset",
            "Choose where to save the new CharacterData asset.");

        if (string.IsNullOrEmpty(path)) return;

        CharacterData newAsset = ScriptableObject.CreateInstance<CharacterData>();
        AssetDatabase.CreateAsset(newAsset, path);
        AssetDatabase.SaveAssets();

        _selected = newAsset;
        _cachedEditor = null;

        EditorGUIUtility.PingObject(newAsset);
    }

    private void DrawDivider()
    {
        EditorGUILayout.Space(4);
        Rect rect = EditorGUILayout.GetControlRect(false, 1);
        EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 0.4f));
        EditorGUILayout.Space(4);
    }
}
