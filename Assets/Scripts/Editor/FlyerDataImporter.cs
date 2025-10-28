using System.IO;
using EjesDeInversion.Data;
using UnityEditor;
using UnityEngine;

namespace EjesDeInversion.Editor
{
    public static class FlyerDataImporter
    {
        [MenuItem("Assets/Create FlyerData object from JSON", true)]
        private static bool ValidateCreateMainBarData()
        {
            // Solo habilita la opción si el archivo seleccionado es un .json
            var obj = Selection.activeObject;
            var path = AssetDatabase.GetAssetPath(obj);
            return path.EndsWith(".json");
        }

        [MenuItem("Assets/Create FlyerData object from JSON")]
        private static void CreateMainBarData()
        {
            // Obtiene la ruta del archivo seleccionado
            var obj = Selection.activeObject;
            var path = AssetDatabase.GetAssetPath(obj);

            if (!File.Exists(path))
            {
                EditorUtility.DisplayDialog("Error", "JSON file not found.", "OK");
                return;
            }

            try
            {
                // Lee el contenido del JSON
                string json = File.ReadAllText(path);

                // Crea una nueva instancia del ScriptableObject
                FlyerData asset = ScriptableObject.CreateInstance<FlyerData>();
                asset.LoadFromJson(json);

                // Define la ruta de guardado para el nuevo asset
                string assetPath = Path.ChangeExtension(path, ".asset");
                if (File.Exists(assetPath))
                {
                    AssetDatabase.DeleteAsset(assetPath);
                }
                AssetDatabase.CreateAsset(asset, assetPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                // reimporta el asset para asegurarse de que se actualice en el editor
                AssetDatabase.ImportAsset(assetPath);

                EditorUtility.DisplayDialog("Success", $"MainBarData object created:\n{assetPath}", "OK");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error creating MainBarData from JSON: {e.Message}");
                EditorUtility.DisplayDialog("Error", "An error occurred while processing JSON file.", "OK");
            }
        }
    }
}