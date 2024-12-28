using UnityEditor;
using UnityEngine;
using System.IO;

public class CreateMaterialsFromTextures : EditorWindow
{
    private string folderPath = "Assets"; // Default folder path
    private Shader targetShader;

    [MenuItem("Tools/Create Materials from Textures")]
    public static void ShowWindow()
    {
        GetWindow<CreateMaterialsFromTextures>("Create Materials");
    }

    private void OnGUI()
    {
        GUILayout.Label("Material Creation Settings", EditorStyles.boldLabel);

        // Folder selection
        if (GUILayout.Button("Select Folder"))
        {
            string selectedPath = EditorUtility.OpenFolderPanel("Select Texture Folder", "Assets", "");
            if (!string.IsNullOrEmpty(selectedPath))
            {
                // Convert absolute path to Unity-relative path
                folderPath = "Assets" + selectedPath.Replace(Application.dataPath, "").Replace("\\", "/");
            }
        }
        GUILayout.Label($"Selected Folder: {folderPath}");

        // Shader selection
        targetShader = (Shader)EditorGUILayout.ObjectField("Shader", targetShader, typeof(Shader), false);

        // Create Materials button
        if (GUILayout.Button("Create Materials"))
        {
            if (targetShader == null)
            {
                Debug.LogError("Please assign a shader before creating materials.");
                return;
            }

            CreateMaterials();
        }
    }

    private void CreateMaterials()
    {
        string[] textureFiles = Directory.GetFiles(folderPath, "*.*", SearchOption.TopDirectoryOnly);
        int materialCount = 0;

        foreach (string file in textureFiles)
        {
            // Check if file is a texture
            string relativePath = file.Replace("\\", "/").Replace(Application.dataPath, "Assets");
            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(relativePath);

            if (texture != null)
            {
                // Create material
                string materialPath = Path.Combine(folderPath, texture.name + "_Material.mat");
                materialPath = AssetDatabase.GenerateUniqueAssetPath(materialPath);

                Material material = new Material(targetShader);
                material.mainTexture = texture;

                AssetDatabase.CreateAsset(material, materialPath);
                materialCount++;
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"{materialCount} materials created successfully in {folderPath}.");
    }
}
