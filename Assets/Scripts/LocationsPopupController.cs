using System;
using DG.Tweening;
using EjesDeInversion.Data;
using EjesDeInversion.Managers;
using UnityEngine;

namespace EjesDeInversion
{
    public class LocationsPopupController : MonoBehaviour
    {
        [Header("General")]
        [SerializeField] private LocationsPopupButtonController _buttonControllerTemplate;
        [SerializeField] private Transform _buttonsParent;
        [SerializeField] private CanvasGroup _canvasGroup;
        
        [Header("Animation")]
        [SerializeField] private float _fadeDuration = 0.2f;
        [SerializeField] private Ease _fadeInEase = Ease.OutCubic;
        [SerializeField] private Ease _fadeOutEase = Ease.InCubic;
        
        private LocationsData _locationsData;

        private void Start()
        {
            LoadLocationsData();
            CreateLocationButtons();
        }

        private void LoadLocationsData()
        {
            if (DataManager.TryLoad<LocationsData>("LocationsData", out var data))
            {
                _locationsData = data;
            }
            else
            {
                Debug.LogError("Failed to load LocationsData.");
            }
        }
        
        private void CreateLocationButtons()
        {
            if (_locationsData == null || _locationsData.Locations == null) return;

            foreach (var location in _locationsData.Locations)
            {
                var buttonController = Instantiate(_buttonControllerTemplate, _buttonsParent);
                buttonController.Initialize(location);
            }
        }
        
        public void ShowPopup()
        {
            this.gameObject.SetActive(true);
            _canvasGroup.alpha = 0f;
            _canvasGroup.DOFade(1f, _fadeDuration).SetEase(_fadeInEase);
        }
        
        public void HidePopup()
        {
            _canvasGroup.DOFade(0f, _fadeDuration).SetEase(_fadeOutEase).OnComplete(() =>
            {
                this.gameObject.SetActive(false);
            });
        }
    }
}