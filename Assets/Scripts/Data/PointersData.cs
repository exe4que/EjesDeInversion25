using UnityEngine;

namespace EjesDeInversion.Data
{
    [CreateAssetMenu(fileName = "PointersData", menuName = "DATA/PointersData", order = 3)]
    public class PointersData : ScriptableObject
    {
        public PointerData[] Pointers;
    }
    
    [System.Serializable]
    public class PointerData
    {
        [Header("[Axis Name]_[Subcategory Name]_[Pointer Name]")]
        public string Id;
        public Vector3 Position;
        [TextArea]
        public string ShortDescription;
        
        public string Name => string.IsNullOrEmpty(Id) ? "" : Id.Split('_')[2];
    }
}