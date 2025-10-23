using System;
using System.Collections.Generic;
using DG.Tweening;
using EjesDeInversion.Data;
using UnityEngine;

namespace EjesDeInversion
{
    public class MainBarCategoryListController : MonoBehaviour
    {
        [Header("General")]
        [SerializeField] private int _initialElementsPoolSize = 10;
        [SerializeField] private MainBarCategoryListElementController _categoryListElementPrefab;
        [SerializeField] private Transform _elementsContainer;
        [SerializeField] private CanvasGroup _categoryListCanvasGroup;
        
        [Header("Animation")]
        [SerializeField] private float _fadeDuration = 0.3f;
        

        private List<MainBarCategoryListElementController> _elementsPool = new();
        private string _idShowing = "";
        private MainBarController _mainBarController;
        
        public static Action<string> OnCategorySelected;

        public void Initialize(MainBarController mainBarController)
        {
            _mainBarController = mainBarController;
            InitializePool();
        }

        private void InitializePool()
        {
            for (int i = 0; i < _initialElementsPoolSize; i++)
            {
                MainBarCategoryListElementController element = Instantiate(_categoryListElementPrefab, _elementsContainer);
                element.gameObject.SetActive(false);
                element.Initialize(_mainBarController);
                _elementsPool.Add(element);
            }
        }

        public void Show(MainBarData.InvestmentAxisButtonData buttonData)
        {
            if (_idShowing != buttonData.Id)
            {
                HideInternal();
            }
            
            this.gameObject.SetActive(true);
            for (int i = 0; i < buttonData.Categories.Length; i++)
            {
                MainBarCategoryListElementController element = GetPooledElement();
                element.SetData(buttonData.Categories[i]);
                element.gameObject.SetActive(true);
            }
            _idShowing = buttonData.Id;
            OnCategorySelected?.Invoke(_idShowing);

            FadeIn();
        }

        public void Hide()
        {
            FadeOut();
        }

        private void HideInternal()
        {
            foreach (var element in _elementsPool)
            {
                element.gameObject.SetActive(false);
            }
            this.gameObject.SetActive(false);
            _idShowing = "";
            OnCategorySelected?.Invoke("");
        }
        private void FadeIn()
        {
            _categoryListCanvasGroup.DOKill();
            _categoryListCanvasGroup.alpha = 0;
            _categoryListCanvasGroup.DOFade(1, _fadeDuration).SetEase(Ease.Linear);
        }
        
        private void FadeOut()
        {
            _categoryListCanvasGroup.DOKill();
            _categoryListCanvasGroup.alpha = 1;
            _categoryListCanvasGroup.DOFade(0, _fadeDuration).SetEase(Ease.Linear)
                .OnComplete(HideInternal);
        }

        private MainBarCategoryListElementController GetPooledElement()
        {
            foreach (var element in _elementsPool)
            {
                if (!element.gameObject.activeInHierarchy)
                {
                    return element;
                }
            }

            MainBarCategoryListElementController newElement = Instantiate(_categoryListElementPrefab, _elementsContainer);
            newElement.gameObject.SetActive(false);
            _elementsPool.Add(newElement);
            return newElement;
        }

        public bool IsOpen(string id)
        {
            return _idShowing == id;
        }
    }
}