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
        [SerializeField] private MainBarCategoryListSelectedElementController _mainBarCategoryListSelectedElementController;
        [SerializeField] private CanvasGroup _mainContainerCanvasGroup;
        
        [Header("Animation")]
        [SerializeField] private float _categoryListDuration = 0.2f;
        [SerializeField] private Ease _categoryListShowEase = Ease.OutCubic;
        [SerializeField] private Ease _categoryListHideEase = Ease.InCubic;
        [SerializeField] private float _fadeDuration = 0.3f;
        [SerializeField] private float _fadeDelay = 0.2f;
        [SerializeField] private Ease _fadeInEase = Ease.OutCubic;
        [SerializeField] private Ease _fadeOutEase = Ease.InCubic;

        private MainBarCategoryListElementController _selectedCategoryElement = null;
        
        private void Start()
        {
            _currentCategoryText.alpha = 0;
            _mainBarButtonsContainerController.Initialize(this);
            _mainBarCategoryListController.Initialize(this);
            _mainBarCategoryListSelectedElementController.Initialize(this);
            _backgroundRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,
                _mainBarButtonsContainerController.GetDefaultContentWidth() + _backgroundHorizontalPadding * 2f);
        }
        
        public void ShowCategoryList(MainBarData.InvestmentAxisButtonData buttonData)
        {
            _mainBarCategoryListController.Show(buttonData);
            _currentCategoryText.text = buttonData.Name;
            _currentCategoryText.DOFade(1, _categoryListDuration).SetEase(_categoryListShowEase);
        }
        
        public bool IsCategoryListVisible(string id)
        {
            return _mainBarCategoryListController.IsOpen(id);
        }
        
        public void HideCategoryList()
        {
            _mainBarCategoryListController.Hide();
            _currentCategoryText.DOFade(0, _categoryListDuration).SetEase(_categoryListHideEase);
        }
        
        public void SelectCategory(MainBarCategoryListElementController categoryElement)
        {
            _selectedCategoryElement = categoryElement;
            _selectedCategoryElement.CanvasGroup.alpha = 0f;
            _mainBarCategoryListSelectedElementController.Show(categoryElement);
            _mainContainerCanvasGroup.interactable = false;
            FadeOut();
        }
        
        public void DeselectCategory()
        {
            _mainBarCategoryListSelectedElementController.Hide();
            _mainContainerCanvasGroup.interactable = true;
            FadeIn();
        }

        private void FadeIn()
        {
            _mainContainerCanvasGroup.DOKill();
            _mainContainerCanvasGroup.alpha = 0;
            _mainContainerCanvasGroup.DOFade(1, _fadeDuration)
                .SetEase(_fadeInEase)
                .SetDelay(_fadeDelay)
                .OnComplete(() =>
                {
                    _selectedCategoryElement.CanvasGroup.alpha = 1f;
                    _selectedCategoryElement = null;
                });
        }

        private void FadeOut()
        {
            _mainContainerCanvasGroup.DOKill();
            _mainContainerCanvasGroup.alpha = 1;
            _mainContainerCanvasGroup.DOFade(0, _fadeDuration)
                .SetEase(_fadeOutEase)
                .SetDelay(_fadeDelay);
        }
    }
}