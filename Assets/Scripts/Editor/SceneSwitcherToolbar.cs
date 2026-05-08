// SceneSwitcherToolbar.cs
// Put inside Assets/Editor

using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityToolbarExtender;

[InitializeOnLoad]
public static class SceneSwitcherToolbar
{
    private static string[] _sceneNames;
    private static string[] _scenePaths;

    private static int _selectedIndex;

    static SceneSwitcherToolbar()
    {
        LoadScenes();

        ToolbarExtender.LeftToolbarGUI.Add(OnToolbarGUI);

        EditorBuildSettings.sceneListChanged += LoadScenes;
    }

    private static void LoadScenes()
    {
        var scenes = EditorBuildSettings.scenes
            .Where(x => x.enabled)
            .ToArray();

        _scenePaths = scenes
            .Select(x => x.path)
            .ToArray();

        _sceneNames = scenes
            .Select(x => Path.GetFileNameWithoutExtension(x.path))
            .ToArray();

        string currentScene = SceneManager.GetActiveScene().path;

        _selectedIndex =
            System.Array.IndexOf(_scenePaths, currentScene);

        if (_selectedIndex < 0)
            _selectedIndex = 0;
    }

    private static void OnToolbarGUI()
    {
        GUILayout.Space(10);

        GUILayout.Label("Scene");

        int newIndex = EditorGUILayout.Popup(
            _selectedIndex,
            _sceneNames,
            GUILayout.Width(180)
        );

        if (newIndex != _selectedIndex)
        {
            _selectedIndex = newIndex;

            OpenScene(_scenePaths[_selectedIndex]);
        }
    }

    private static void OpenScene(string scenePath)
    {
        if (EditorApplication.isPlaying)
        {
            Debug.LogWarning(
                "Cannot switch scene while playing."
            );

            return;
        }

        bool canOpen =
            EditorSceneManager
                .SaveCurrentModifiedScenesIfUserWantsTo();

        if (!canOpen)
            return;

        EditorSceneManager.OpenScene(scenePath);
    }
}