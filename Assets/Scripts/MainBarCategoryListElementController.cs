using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EjesDeInversion
{
    public class MainBarCategoryListElementController : MonoBehaviour
    {
        [SerializeField] private TMP_Text _nameText;
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

        public void SetData(MainBarData.InvestmentAxisCategoryData categoryData)
        {
            _categoryData = categoryData;
            _nameText.text = categoryData.Name;
        }
        
        private void OnButtonClicked()
        {
            Debug.Log($"Category {_categoryData.Id} clicked.");
        }
    }
}