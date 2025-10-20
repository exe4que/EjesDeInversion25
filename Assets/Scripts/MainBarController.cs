using UnityEngine;

namespace EjesDeInversion
{
    public class MainBarController : MonoBehaviour
    {
        [SerializeField] private MainBarButtonsContainerController _mainBarButtonsContainerController;
        [SerializeField] private MainBarCategoryListController _mainBarCategoryListController;
        
        private void Start()
        {
            _mainBarButtonsContainerController.Initialize(this);
        }
        
        public void ShowCategoryList(MainBarData.InvestmentAxisCategoryData[] categoriesData)
        {
            _mainBarCategoryListController.Show(categoriesData);
        }
        
        public bool IsCategoryListVisible()
        {
            return _mainBarCategoryListController.IsVisible();
        }
        
        public void HideCategoryList()
        {
            _mainBarCategoryListController.Hide();
        }
    }
}