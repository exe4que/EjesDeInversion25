using UnityEngine;

namespace EjesDeInversion
{
    [CreateAssetMenu(fileName = "MainBarData", menuName = "DATA/MainBarData", order = 1)]
    public class MainBarData : ScriptableObject
    {
        public InvestmentAxisButtonData[] InvestmentAxisButtons;

        [System.Serializable]
        public class InvestmentAxisButtonData
        {
            public string Id;
            public string Name;
            public Sprite Icon;
            public Color Color;
            public InvestmentAxisCategoryData[] Categories;
        }

        [System.Serializable]
        public class InvestmentAxisCategoryData
        {
            public string Id;
            public string Name;
        }

        public void LoadFromJson(string json)
        {
            JsonUtility.FromJsonOverwrite(json, this);
        }
    }
}
