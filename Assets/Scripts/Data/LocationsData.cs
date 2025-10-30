using UnityEngine;

namespace EjesDeInversion.Data
{
    [CreateAssetMenu(fileName = "LocationsData", menuName = "DATA/LocationsData", order = 2)]
    public class LocationsData : ScriptableObject
    {
        public Location[] Locations;
        
        public void LoadFromJson(string json)
        {
            JsonUtility.FromJsonOverwrite(json, this);
        }
        
        [System.Serializable]
        public class Location
        {
            public string Id;
            public string Name;
            public string IconName;
            public Vector3 CameraPosition;
            public int CameraSize;
        }
    }
}