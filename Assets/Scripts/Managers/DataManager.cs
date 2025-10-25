using EjesDeInversion.Utilities;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace EjesDeInversion.Managers
{
    public class DataManager : Singleton<DataManager>
    {
        public static bool TryLoadData<T>(string id, out T res) where T : ScriptableObject
        {
            var locations = Addressables.LoadResourceLocationsAsync(id).WaitForCompletion();
            if(locations == null || locations.Count == 0)
            {
                Debug.LogWarning($"Addressable resource with id: {id} does not exist.");
                res = null;
                return false;
            }
            
            AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(id);
            handle.WaitForCompletion();
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                res = handle.Result;
                return true;
            }
            else
            {
                Debug.LogWarning($"Could not load data of type {typeof(T)} for id: {id}");
                res = null;
                return false;
            }
        }
    }
}