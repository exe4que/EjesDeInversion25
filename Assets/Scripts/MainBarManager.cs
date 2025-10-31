using DG.Tweening;
using EjesDeInversion.Data;
using EjesDeInversion.Managers;
using EjesDeInversion.Utilities;
using TMPro;
using UnityEngine;

namespace EjesDeInversion
{
    public class MainBarManager : Singleton<MainBarManager>
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
            _mainBarButtonsContainerController.Initialize();
            _mainBarCategoryListController.Initialize();
            _backgroundRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,
                _mainBarButtonsContainerController.GetDefaultContentWidth() + _backgroundHorizontalPadding * 2f);
        }
        
        private void ShowCategoryListInternal(MainBarButtonController button)
        {
            _mainBarCategoryListController.Show(button);
            _currentCategoryText.text = button.ButtonData.Name;
            _currentCategoryText.DOFade(1, _categoryListDuration).SetEase(_categoryListShowEase);
        }
        
        private bool IsCategoryListVisibleInternal(string id = "")
        {
            return _mainBarCategoryListController.IsOpen(id);
        }
        
        private void HideCategoryListInternal()
        {
            _mainBarCategoryListController.Hide();
            _currentCategoryText.DOFade(0, _categoryListDuration).SetEase(_categoryListHideEase);
        }
        
        private void SelectCategoryInternal(MainBarCategoryListElementController categoryElement)
        {
            _selectedCategoryElement = categoryElement;
            _selectedCategoryElement.CanvasGroup.alpha = 0f;
            _mainBarCategoryListSelectedElementController.Show(categoryElement);
            _mainContainerCanvasGroup.interactable = false;
            FadeOut();
        }
        
        private void DeselectCategoryInternal()
        {
            _mainBarCategoryListSelectedElementController.Hide();
            _mainContainerCanvasGroup.interactable = true;
            FadeIn();
            PointersManager.ShowAllPointers();
        }

        private void HideAllInternal()
        {
            if (IsCategoryListVisible())
            {
                _mainBarCategoryListController.DeselectSelectedButton();
                if (IsAnyCategorySelected())
                {
                    DeselectCategory();
                }
                HideCategoryList();
            }
        }
        
        private void SelectAxisAndCategoryForPointerInternal(PointerController pointerController)
        {
            MainBarButtonController axisButton =
                _mainBarButtonsContainerController.GetButtonByAxisId(pointerController.Data.AxisId);
            if (axisButton == null)
            {
                Debug.LogWarning($"No axis button found for axis ID: {pointerController.Data.AxisId}");
                return;
            }
            ShowCategoryList(axisButton);
            MainBarCategoryListElementController categoryElement =
                _mainBarCategoryListController.GetCategoryElementById(
                    $"{pointerController.Data.AxisId}_{pointerController.Data.CategoryId}");
            if (categoryElement == null)
            {
                Debug.LogWarning($"No category element found for category ID: {pointerController.Data.CategoryId}");
                return;
            }
            this.DelayedCallInFrames(1, () => SelectCategory(categoryElement));
        }
        
        private bool IsAnyCategorySelectedInternal()
        {
            return _selectedCategoryElement != null;
        }
        
        public static void ShowCategoryList(MainBarButtonController button)
        {
            instance.ShowCategoryListInternal(button);
        }
        
        public static bool IsCategoryListVisible(string id = "")
        {
            return instance.IsCategoryListVisibleInternal(id);
        }
        
        public static void HideCategoryList()
        {
            instance.HideCategoryListInternal();
        }
        
        public static void SelectCategory(MainBarCategoryListElementController categoryElement)
        {
            instance.SelectCategoryInternal(categoryElement);
        }
        
        public static void DeselectCategory()
        {
            instance.DeselectCategoryInternal();
        }
        
        public static bool IsAnyCategorySelected()
        {
            return instance.IsAnyCategorySelectedInternal();
        }
        
        public static void HideAll()
        {
            instance.HideAllInternal();
        }
        
        public static void SelectAxisAndCategoryForPointer(PointerController pointerController)
        {
            instance.SelectAxisAndCategoryForPointerInternal(pointerController);
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