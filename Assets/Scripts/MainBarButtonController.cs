using System;
using DG.Tweening;
using EjesDeInversion.Data;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
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
        private MainBarController _mainBarController;

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
            else if (this.transform.localScale.x > 1)
            {
                AnimationOut();
            }
        }

        private void OnButtonClicked()
        {
            if (_mainBarController.IsCategoryListVisible(_buttonData.Id))
            {
                _mainBarController.HideCategoryList();
            }
            else
            {
                _mainBarController.ShowCategoryList(_buttonData);
                OpenFlyer();
            }
        }

        private void OpenFlyer()
        {
            string id = _buttonData.Id;
            AsyncOperationHandle<FlyerData> handle = Addressables.LoadAssetAsync<FlyerData>(id);
            handle.Completed += op =>
            {
                if (op.Status == AsyncOperationStatus.Succeeded)
                {
                    FlyerData flyerData = op.Result;
                    FlyerController.Show(flyerData);
                }
                else
                {
                    Debug.LogError($"Failed to load FlyerData for id: {id}");
                }
            };
        }

        private void AnimationIn()
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
        
        private void AnimationOut()
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

        public void Initialize(MainBarData.InvestmentAxisButtonData data, MainBarController mainBarController)
        {
            _buttonData = data;
            _mainBarController = mainBarController;
            _icon.sprite = data.Icon;
            _background.color = data.Color;
        }
    }
}