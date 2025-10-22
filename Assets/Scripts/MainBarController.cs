using DG.Tweening;
using EjesDeInversion.Data;
using TMPro;
using UnityEngine;

namespace EjesDeInversion
{
    public class MainBarController : MonoBehaviour
    {
        [Header("General")]
        [SerializeField] private MainBarButtonsContainerController _mainBarButtonsContainerController;
        [SerializeField] private MainBarCategoryListController _mainBarCategoryListController;
        [SerializeField] private TMP_Text _currentCategoryText;
        [SerializeField] private RectTransform _backgroundRectTransform;
        [SerializeField] private float _backgroundHorizontalPadding;
        
        [Header("Animation")]
        [SerializeField] private float _duration = 0.2f;
        [SerializeField] private Ease _showEase = Ease.OutCubic;
        [SerializeField] private Ease _hideEase = Ease.InCubic;
        
        private void Start()
        {
            _currentCategoryText.alpha = 0;
            _mainBarButtonsContainerController.Initialize(this);
            _backgroundRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,
                _mainBarButtonsContainerController.GetDefaultContentWidth() + _backgroundHorizontalPadding * 2f);
        }
        
        public void ShowCategoryList(MainBarData.InvestmentAxisButtonData buttonData)
        {
            _mainBarCategoryListController.Show(buttonData);
            _currentCategoryText.text = buttonData.Name;
            _currentCategoryText.DOFade(1, _duration).SetEase(_showEase);
        }
        
        public bool IsCategoryListVisible(string id)
        {
            return _mainBarCategoryListController.IsOpen(id);
        }
        
        public void HideCategoryList()
        {
            _mainBarCategoryListController.Hide();
            _currentCategoryText.DOFade(0, _duration).SetEase(_hideEase);
        }
    }
}