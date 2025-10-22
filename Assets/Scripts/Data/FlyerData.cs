using UnityEngine;

namespace EjesDeInversion.Data
{
    [CreateAssetMenu(fileName = "FlyerData", menuName = "DATA/FlyerData", order = 1)]
    public class FlyerData : ScriptableObject
    {
        public string Id;
        public string Title;
        [TextArea]
        public string Subtitle;
        public string DescriptionTitle;
        [TextArea]
        public string Description;
        public string[] CarouselImageNames;
        
        public void LoadFromJson(string json)
        {
            JsonUtility.FromJsonOverwrite(json, this);
        }
    }
}