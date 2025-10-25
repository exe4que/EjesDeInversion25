using System;
using EjesDeInversion.Data;
using EjesDeInversion.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EjesDeInversion
{
    public class MainBarCategoryListElementController : MonoBehaviour
    {
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private Button _button;
        [SerializeField] private CanvasGroup _canvasGroup;

        private MainBarData.InvestmentAxisButtonData _buttonData;
        private MainBarData.InvestmentAxisCategoryData _categoryData;
        private MainBarController _mainBarController;
        
        public CanvasGroup CanvasGroup => _canvasGroup;
        public MainBarData.InvestmentAxisCategoryData CategoryData => _categoryData;

        private void OnEnable()
        {
            _button.onClick.AddListener(OnButtonClicked);
        }
        
        private void OnDisable()
        {
            _button.onClick.RemoveListener(OnButtonClicked);
        }
        
        public void Initialize(MainBarController mainBarController)
        {
            _mainBarController = mainBarController;
        }

        public void SetData(MainBarData.InvestmentAxisCategoryData categoryData)
        {
            _buttonData = null;
            _categoryData = categoryData;
            _nameText.text = categoryData.Name;
        }
        
        public void SetData(MainBarData.InvestmentAxisButtonData buttonData)
        {
            _buttonData = buttonData;
            _categoryData = null;
            _nameText.text = "Todos";
        }
        
        private void OnButtonClicked()
        {
            //Debug.Log($"Category {_categoryData.Id} clicked.");
            _mainBarController.SelectCategory(this);
            
            if (_buttonData != null)
            {
                PointersManager.FilterPointersByAxis(_buttonData);
            }
            else if (_categoryData != null)
            {
                FlyerController.TryShow(_categoryData.Id);
                PointersManager.FilterPointersBySubcategory(_categoryData);
            }
        }
    }
}