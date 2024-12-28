using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine;
using System.Collections.Generic;

public class SceneSelectorWindow : EditorWindow
{
    [MenuItem("Tools/Scene Selector %#o")]
    static void OpenWindow()
    {
        var window = GetWindow<SceneSelectorWindow>();
        window.titleContent = new GUIContent("Scene Selector");
        window.minSize = new Vector2(300, 400);
    }

    [InitializeOnLoadMethod]
    static void RegisterCallbacks()
    {
        EditorApplication.playModeStateChanged += ReturnToPreviousScene;
    }

    // void OnEnable()
    // {
    //     RefreshUI();
    // }

    void RefreshUI()
    {
        rootVisualElement.Clear();
        CreateGUI();
    }

    void CreateGUI()
    {
        // Folder management section
        var folderPaths = SceneSelectorSettings.instance.FolderPaths;

        var folderManagementContainer = new VisualElement();
        folderManagementContainer.style.marginBottom = 10;

        var addFolderButton = new Button(() =>
        {
            string newFolderPath = EditorUtility.OpenFolderPanel("Select Folder", Application.dataPath, "");
            if (!string.IsNullOrEmpty(newFolderPath) && newFolderPath.StartsWith(Application.dataPath))
            {
                newFolderPath = "Assets" + newFolderPath.Substring(Application.dataPath.Length);
                if (!folderPaths.Contains(newFolderPath))
                {
                    folderPaths.Add(newFolderPath);
                    SceneSelectorSettings.instance.Save();
                    RefreshUI();
                }
            }
        })
        { text = "Add Folder" };

        folderManagementContainer.Add(addFolderButton);
        rootVisualElement.Add(folderManagementContainer);



        // Exit early if no folder paths exist
        if (folderPaths == null || folderPaths.Count == 0)
        {
            var noScenesLabel = new Label("No folder paths selected. Add folders to see scenes.");
            noScenesLabel.style.marginTop = 10;
            rootVisualElement.Add(noScenesLabel);
            return;
        }
    var scrollView = new ScrollView();
    scrollView.style.flexGrow = 1;
    rootVisualElement.Add(scrollView);

        // Display existing folder paths
        foreach (var folderPath in folderPaths)
        {
            var folderRow = new VisualElement();
            folderRow.style.flexDirection = FlexDirection.Row;

            var folderLabel = new Label(folderPath);
            folderLabel.style.flexGrow = 1;

            var removeButton = new Button(() =>
            {
                folderPaths.Remove(folderPath);
                SceneSelectorSettings.instance.Save();
                RefreshUI();
            })
            { text = "x" };

            folderRow.Add(removeButton);
            folderRow.Add(folderLabel);
            scrollView.Add(folderRow);
            
            // Scene listing
            var sceneGuids = AssetDatabase.FindAssets("t:Scene", new[] {folderPath});
            foreach (var sceneGuid in sceneGuids)
            {
                scrollView.Add(CreateSceneButton(sceneGuid));
            }
        }


    }

    static void ReturnToPreviousScene(PlayModeStateChange change)
    {
        if (change == PlayModeStateChange.EnteredEditMode)
        {
            EditorSceneManager.OpenScene(SceneSelectorSettings.instance.PreviousScenePath, OpenSceneMode.Single);
        }
    }

    VisualElement CreateSceneButton(string sceneGuid)
    {
        var scenePath = AssetDatabase.GUIDToAssetPath(sceneGuid);
        var buttonGroup = new VisualElement();
        buttonGroup.style.flexDirection = FlexDirection.Row;
        buttonGroup.style.marginBottom = 5;

        var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
        var label = new Label(sceneAsset.name);
        label.style.flexGrow = 1;

        var openButton = new Button(() =>
        {
            EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
        })
        { text = "✎" };
         openButton.style.fontSize = 12;

        var playButton = new Button(() =>
        {
            SceneSelectorSettings.instance.PreviousScenePath = SceneManager.GetActiveScene().path;
            EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            EditorApplication.EnterPlaymode();
        })
        { text = "▶" };
         playButton.style.fontSize = 8;

        buttonGroup.Add(label);
        buttonGroup.Add(openButton);
        buttonGroup.Add(playButton);

        return buttonGroup;
    }
}
