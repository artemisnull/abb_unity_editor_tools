using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[FilePath("Assets/SceneSelectorTool/Editor/SceneSelectorSettings.asset", FilePathAttribute.Location.ProjectFolder)]
public class SceneSelectorSettings : ScriptableSingleton<SceneSelectorSettings>
{
    public string PreviousScenePath;
    public List<string> FolderPaths = new List<string>();

    public void Save()
    {
        Save(true);
    }

    void OnDisable()
    {
        Save();
    }
}
