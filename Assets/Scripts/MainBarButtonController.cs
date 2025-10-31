using DG.Tweening;
using EjesDeInversion.Data;
using UnityEngine;
using UnityEngine.UI;

namespace EjesDeInversion
{
    public class MainBarButtonController : MonoBehaviour
    {
        [Header("General")]
        [SerializeField] private Image _icon;
        [SerializeField] private Image _background;
        [SerializeField] private Button _button;
        
        [Header("Animation")]
        public float AnimationTargetScale = 1.1f;
        [SerializeField] private float _animationDuration = 0.2f;
        [SerializeField] private Ease _animationInEase = Ease.OutCubic;
        [SerializeField] private Ease _animationOutEase = Ease.InCubic;
        
        private MainBarData.InvestmentAxisButtonData _buttonData;
        public MainBarData.InvestmentAxisButtonData ButtonData => _buttonData;

        private void OnEnable()
        {
            _button.onClick.AddListener(OnButtonClicked);
            MainBarCategoryListController.OnCategorySelected += CategorySelected;
        }
        
        private void OnDisable()
        {
            _button.onClick.RemoveListener(OnButtonClicked);
            MainBarCategoryListController.OnCategorySelected -= CategorySelected;
        }

        private void CategorySelected(string obj)
        {
            if (obj == _buttonData.Id)
            {
                AnimationIn();
            }
            else if (!string.IsNullOrEmpty(obj) && this.transform.localScale.x > 1)
            {
                AnimationOut();
            }
            else if (string.IsNullOrEmpty(obj))
            {
                _button.interactable = true;
            }
        }

        private void OnButtonClicked()
        {
            if (MainBarManager.IsCategoryListVisible(_buttonData.Id))
            {
                _button.interactable = false;
                MainBarManager.HideCategoryList();
                AnimationOut();
            }
            else
            {
                MainBarManager.ShowCategoryList(this);
            }
        }

        public void AnimationIn()
        {
            this.transform.DOKill();
            this.transform.localScale = Vector3.one;
            this.transform.DOScale(AnimationTargetScale, _animationDuration)
                .SetEase(_animationInEase).OnUpdate(() =>
                {
                    // update horizontal layout group
                    LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)this.transform.parent);
                });
        }
        
        public void AnimationOut()
        {
            this.transform.DOKill();
            this.transform.localScale = Vector3.one * AnimationTargetScale;
            this.transform.DOScale(1f, _animationDuration)
                .SetEase(_animationOutEase).OnUpdate(() =>
                {
                    // update horizontal layout group
                    LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)this.transform.parent);
                });
        }

        public void Initialize(MainBarData.InvestmentAxisButtonData data)
        {
            _buttonData = data;
            _icon.sprite = data.Icon;
            _background.color = data.Color;
        }
    }
}