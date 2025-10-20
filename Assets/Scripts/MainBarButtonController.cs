using System;
using DG.Tweening;
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
        [SerializeField] private float _animationTargetScale = 1.1f;
        [SerializeField] private float _animationDuration = 0.2f;
        [SerializeField] private Ease _animationInEase = Ease.OutCubic;
        [SerializeField] private Ease _animationOutEase = Ease.InCubic;
        
        private MainBarData.InvestmentAxisButtonData _buttonData;
        private MainBarController _mainBarController;

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
            if (_mainBarController.IsCategoryListVisible())
            {
                _mainBarController.HideCategoryList();
                AnimationOut();
            }
            else
            {
                _mainBarController.ShowCategoryList(_buttonData.Categories);
                AnimationIn();
            }
        }
        
        private void AnimationIn()
        {
            this.transform.DOKill();
            this.transform.localScale = Vector3.one;
            this.transform.DOScale(_animationTargetScale, _animationDuration)
                .SetEase(_animationInEase);
        }
        
        private void AnimationOut()
        {
            this.transform.DOKill();
            this.transform.localScale = Vector3.one * _animationTargetScale;
            this.transform.DOScale(1f, _animationDuration)
                .SetEase(_animationOutEase);
        }

        public void Initialize(MainBarData.InvestmentAxisButtonData data, MainBarController mainBarController)
        {
            _buttonData = data;
            _mainBarController = mainBarController;
            _icon.sprite = data.Icon;
            _background.color = data.Color;
        }
    }
}