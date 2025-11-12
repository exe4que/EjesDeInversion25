using System.IO;
using System.Text;
using EjesDeInversion.Data;
using UnityEditor;
using UnityEngine;

namespace EjesDeInversion.Editor
{
    public static class FileTildeRemover
    {
        [MenuItem("Assets/Remove Tildes from selected File Names")]
        public static void RemoveTildes()
        {
            var selectedObjects = Selection.objects;
            foreach (var obj in selectedObjects)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                string fileName = Path.GetFileName(path);
                string directory = Path.GetDirectoryName(path);

                string newFileName = RemoveTildesFromString(fileName);
                if (newFileName != fileName)
                {
                    AssetDatabase.RenameAsset(path, newFileName);
                    Debug.Log($"Renamed: {fileName} -> {newFileName}");
                }
                else
                {
                    Debug.Log($"No tildes found in: {fileName}");
                }
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        
        private static string RemoveTildesFromString(string input)
        {
            string normalizedString = input.Normalize(NormalizationForm.FormD);
            char[] chars = normalizedString.ToCharArray();
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            foreach (char c in chars)
            {
                if (System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c) != System.Globalization.UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }

            return sb.ToString().Normalize(NormalizationForm.FormC);
        }
        
        //remove tildes from image names within flyerData assets
        [MenuItem("Assets/Remove Tildes from FlyerData Image Names", true)]
        public static bool ValidateRemoveTildesFromFlyerData()
        {
            var obj = Selection.activeObject;
            return obj is FlyerData;
        }

        [MenuItem("Assets/Remove Tildes from FlyerData Image Names")]
        public static void RemoveTildesFromFlyerData()
        {
            var selectedObjects = Selection.objects;
            foreach (var obj in selectedObjects)
            {
                if (obj is FlyerData flyerData)
                {
                    bool modified = false;
                    for (int i = 0; i < flyerData.CarouselImageNames.Length; i++)
                    {
                        string originalName = flyerData.CarouselImageNames[i];
                        string newName = RemoveTildesFromString(originalName);
                        if (newName != originalName)
                        {
                            flyerData.CarouselImageNames[i] = newName;
                            modified = true;
                            Debug.Log(
                                $"Updated image name: {originalName} -> {newName} in FlyerData: {flyerData.name}");
                        }
                    }

                    if (modified)
                    {
                        EditorUtility.SetDirty(flyerData);
                    }
                }
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

    }
}