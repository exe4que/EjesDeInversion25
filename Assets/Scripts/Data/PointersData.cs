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
        public string Name;
        public Vector3 Position;
        [TextArea]
        public string ShortDescription;
        public bool IsLink;
        
        
        private string _axisId;
        private string _subcategoryId;
        private string _pointerId;

        public string EditorId => string.IsNullOrEmpty(Id) ? "" : Id.Split('_')[2];
        public string AxisId => _axisId;
        public string SubcategoryId => _subcategoryId;
        public string PointerId => _pointerId;
        
        public void Initialize()
        {
            string[] idParts = Id.Split('_');
            if (idParts.Length != 3)
            {
                Debug.LogError($"Pointer ID '{Id}' is not in the correct format 'Axis_Subcategory_Pointer'.");
                return;
            }
            _axisId = idParts[0];
            _subcategoryId = idParts[1];
            _pointerId = idParts[2];
        }
    }
}