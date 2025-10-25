using EjesDeInversion.Data;
using EjesDeInversion.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace EjesDeInversion
{
    public class MainBarCategoryListSelectedElementInfoButtonController : MonoBehaviour
    {
        [SerializeField] private Button _button;
        
        private MainBarData.InvestmentAxisCategoryData _categoryData;
        
        private void OnEnable()
        {
            _button.onClick.AddListener(OnButtonClicked);
        }
        
        private void OnDisable()
        {
            _button.onClick.RemoveListener(OnButtonClicked);
        }
        
        private void OnButtonClicked()
        {
            if (_categoryData != null && !string.IsNullOrEmpty(_categoryData.Id))
            {
                FlyerManager.TryShow(_categoryData.Id);
            }
        }
        
        public void SetData(MainBarData.InvestmentAxisCategoryData categoryData)
        {
            _categoryData = categoryData;
        }
    }
}