using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using System.Linq;

public class MissingScriptFinder : EditorWindow
{
    private Vector2 scrollPosition;
    private bool showInactiveObjects = true;

    [MenuItem("Tools/Find Missing Scripts")]
    public static void ShowWindow()
    {
        GetWindow<MissingScriptFinder>("Missing Script Finder");
    }

    void OnGUI()
    {
        showInactiveObjects = EditorGUILayout.Toggle("Include Inactive Objects", showInactiveObjects);

        if (GUILayout.Button("Find Missing Scripts in Active Scene"))
        {
            FindMissingScriptsInScene();
        }

        if (GUILayout.Button("Find Missing Scripts in All Prefabs"))
        {
            FindMissingScriptsInPrefabs();
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Results", EditorStyles.boldLabel);

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        EditorGUILayout.EndScrollView();
    }

    private void FindMissingScriptsInPrefabs()
    {
        string[] allPrefabs = AssetDatabase.FindAssets("t:Prefab")
            .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
            .ToArray();

        int missingCount = 0;

        foreach (string prefabPath in allPrefabs)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (prefab == null) continue;

            var components = prefab.GetComponentsInChildren<Component>(true);
            bool hasMissing = false;

            foreach (var component in components)
            {
                if (component == null)
                {
                    if (!hasMissing)
                    {
                        missingCount++;
                        hasMissing = true;
                    }

                    var go = prefab;
                    Debug.LogError(
                        $"Missing Script in Prefab:\n" +
                        $"Prefab: {prefabPath}\n" +
                        $"Other Components: {string.Join(", ", go.GetComponents<Component>().Where(c => c != null).Select(c => c.GetType().Name))}\n",
                        prefab
                    );
                }
            }
        }

        if (missingCount == 0)
        {
            Debug.Log("No missing scripts found in any prefabs");
        }
        else
        {
            Debug.LogWarning($"Found {missingCount} prefabs with missing scripts");
        }
    }

    private void FindMissingScriptsInScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        var rootObjects = currentScene.GetRootGameObjects();
        int missingCount = 0;

        foreach (var root in rootObjects)
        {
            var transforms = root.GetComponentsInChildren<Transform>(showInactiveObjects);
            foreach (var transform in transforms)
            {
                var components = transform.GetComponents<Component>();

                for (int i = 0; i < components.Length; i++)
                {
                    if (components[i] == null)
                    {
                        missingCount++;
                        var go = transform.gameObject;
                        var path = GetGameObjectPath(transform);

                        // Get component names for the GameObject
                        var componentList = go.GetComponents<Component>()
                            .Where(c => c != null)
                            .Select(c => c.GetType().Name)
                            .ToList();

                        string componentInfo = string.Join(", ", componentList);

                        Debug.LogError(
                            $"Missing Script Details:\n" +
                            $"GameObject: {path}\n" +
                            $"Active: {go.activeInHierarchy}\n" +
                            $"Layer: {LayerMask.LayerToName(go.layer)}\n" +
                            $"Tag: {go.tag}\n" +
                            $"Other Components: {componentInfo}\n" +
                            $"Position: {go.transform.position}\n",
                            go
                        );

                        // Force Selection
                        Selection.activeGameObject = go;
                        EditorGUIUtility.PingObject(go);
                    }
                }
            }
        }

        if (missingCount == 0)
        {
            Debug.Log($"No missing scripts found in scene: {currentScene.name}");
        }
        else
        {
            Debug.LogWarning($"Found {missingCount} missing script(s) in scene: {currentScene.name}");
        }
    }

    private string GetGameObjectPath(Transform transform)
    {
        string path = transform.name;
        while (transform.parent != null)
        {
            transform = transform.parent;
            path = transform.name + "/" + path;
        }
        return path;
    }
}