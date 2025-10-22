using System.IO;
using UnityEditor;
using UnityEngine;

public static class JSONExporter
{
    // create json from scriptable object
    [MenuItem("Assets/Export to JSON", true)]
    private static bool ValidateExportToJSON()
    {
        var obj = Selection.activeObject;
        return obj is ScriptableObject;
    }

    [MenuItem("Assets/Export to JSON")]
    public static void ExportToJSON()
    {
        var dataToExport = Selection.activeObject;
        if (dataToExport == null)
        {
            Debug.LogError("No ScriptableObject assigned for export.");
            return;
        }

        // Convert the ScriptableObject to a JSON string
        string jsonString = JsonUtility.ToJson(dataToExport, true); // 'true' for pretty print

        // Define the file path
        string filePath = AssetDatabase.GetAssetPath(dataToExport);
        filePath = Path.ChangeExtension(filePath, ".json");

        // Write the JSON string to a file
        File.WriteAllText(filePath, jsonString);

        //refresh
        AssetDatabase.Refresh();
    }
}
