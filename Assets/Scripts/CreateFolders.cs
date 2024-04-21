using UnityEngine;
using UnityEditor;
using System.IO;

public class CreateFolders
{
    private static string[] defaultFolders = new string[]
    {
        "Assets/Art/Sprites/Characters",
        "Assets/Art/Sprites/Tilesets",
        "Assets/Art/Sprites/Props",
        "Assets/Art/Background",
        "Assets/Art/UI",
        "Assets/Audio/Music",
        "Assets/Audio/Sound",
        "Assets/Scripts",
        "Assets/Prefabs",
        "Assets/Scenes"
    };

    [MenuItem("Tools/Create Folders")]
    public static void Create()
    {
        string[] folders = defaultFolders;

        bool customFolderNames = EditorUtility.DisplayDialog("Customize Folder Names", "Do you want to customize the default folder names?", "Yes", "No");

        if (customFolderNames)
        {
            for (int i = 0; i < defaultFolders.Length; i++)
            {
                string newFolderName = EditorUtility.OpenFolderPanel("Select a new folder for " + defaultFolders[i], "", "");

                if (!string.IsNullOrEmpty(newFolderName))
                {
                    folders[i] = newFolderName.Replace(Application.dataPath, "Assets");
                }
            }
        }

        foreach (string folder in folders)
        {
            if (!AssetDatabase.IsValidFolder(folder))
            {
                Directory.CreateDirectory(folder);
                AssetDatabase.Refresh();
                Debug.Log("Created folder: " + folder);
            }
        }
    }
}