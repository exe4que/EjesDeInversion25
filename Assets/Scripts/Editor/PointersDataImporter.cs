using EjesDeInversion.Data;
using UnityEditor;
using UnityEngine;

namespace EjesDeInversion.Editor
{
    public static class PointersDataImporter
    {
        [MenuItem("Assets/Create PointersData object from JSON", true)]
        private static bool ValidateCreatePointersData()
        {
            // Solo habilita la opción si el archivo seleccionado es un .json
            var obj = Selection.activeObject;
            var path = AssetDatabase.GetAssetPath(obj);
            return path.EndsWith(".json");
        }

        [MenuItem("Assets/Create PointersData object from JSON")]
        private static void CreatePointersData()
        {
            // Obtiene la ruta del archivo seleccionado
            var obj = Selection.activeObject;
            var path = AssetDatabase.GetAssetPath(obj);
            if (!System.IO.File.Exists(path))
            {
                EditorUtility.DisplayDialog("Error", "JSON file not found.", "OK");
                return;
            }
            try
            {
                // Lee el contenido del JSON
                string json = System.IO.File.ReadAllText(path);

                // Crea una nueva instancia del ScriptableObject
                PointersData asset = ScriptableObject.CreateInstance<EjesDeInversion.Data.PointersData>();
                asset.LoadFromJson(json);

                // Define la ruta de guardado para el nuevo asset
                string assetPath = System.IO.Path.ChangeExtension(path, ".asset");
                if (System.IO.File.Exists(assetPath))
                {
                    AssetDatabase.DeleteAsset(assetPath);
                }
                AssetDatabase.CreateAsset(asset, assetPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                // reimporta el asset para asegurarse de que se actualice en el editor
                AssetDatabase.ImportAsset(assetPath);

                EditorUtility.DisplayDialog("Success", $"PointersData object created:\n{assetPath}", "OK");
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.LogError($"Error creating PointersData from JSON: {e.Message}");
                EditorUtility.DisplayDialog("Error", "An error occurred while processing JSON file.", "OK");
            }
        }
    }
}