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
        private List<MainBarCategoryListElementController> _activeElements = new();
        private MainBarButtonController _axisButtonShowing = null;
        private float _initialWidth;
        private const float MIN_PADDING = 20f;
        
        public static Action<string> OnCategorySelected;

        public void Initialize()
        {
            InitializePool();
        }

        private void AdjustToScreenSize()
        {
            _initialWidth = ((RectTransform)this.transform).rect.width;
            float containerSize = ((RectTransform)this.transform.parent).rect.width;
            float totalSize = _initialWidth + MIN_PADDING * 2;
            if (totalSize > containerSize)
            {
                float newWidth = containerSize - MIN_PADDING * 2;
                ((RectTransform)this.transform).SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newWidth);
            }
        }

        private void InitializePool()
        {
            for (int i = 0; i < _initialElementsPoolSize; i++)
            {
                MainBarCategoryListElementController element = Instantiate(_categoryListElementPrefab, _elementsContainer);
                element.gameObject.SetActive(false);
                _elementsPool.Add(element);
            }
        }

        public void Show(MainBarButtonController button)
        {
            AdjustToScreenSize();
            if (_axisButtonShowing?.ButtonData?.Id != button.ButtonData.Id)
            {
                HideInternal();
            }
            
            var buttonData = button.ButtonData;
            this.gameObject.SetActive(true);
            MainBarCategoryListElementController element1 = GetPooledElement();
            element1.SetData(buttonData);
            element1.gameObject.SetActive(true);
            _elementsPool.Remove(element1);
            _activeElements.Add(element1);
            element1.transform.SetAsFirstSibling();
            for (int i = 0; i < buttonData.Categories.Length; i++)
            {
                MainBarCategoryListElementController element = GetPooledElement();
                element.SetData(buttonData.Categories[i]);
                element.gameObject.SetActive(true);
                _elementsPool.Remove(element);
                _activeElements.Add(element);
                element.transform.SetSiblingIndex(i + 1);
            }
            _axisButtonShowing = button;
            OnCategorySelected?.Invoke(_axisButtonShowing.ButtonData.Id);

            FadeIn();
        }

        public void Hide()
        {
            FadeOut();
        }

        private void HideInternal()
        {
            foreach (var element in _activeElements)
            {
                element.gameObject.SetActive(false);
                _elementsPool.Add(element);
            }
            _activeElements.Clear();
            this.gameObject.SetActive(false);
            _axisButtonShowing = null;
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
            if (_elementsPool.Count > 0)
            {
                var element = _elementsPool[0];
                _elementsPool.RemoveAt(0);
                _activeElements.Add(element);
                return element;
            }

            MainBarCategoryListElementController newElement = Instantiate(_categoryListElementPrefab, _elementsContainer);
            newElement.gameObject.SetActive(false);
            _activeElements.Add(newElement);
            return newElement;
        }

        public bool IsOpen(string id = "")
        {
            if (string.IsNullOrEmpty(id))
            {
                return _axisButtonShowing != null;
            }

            if (_axisButtonShowing == null)
            {
                return false;
            }
            
            if (_axisButtonShowing.ButtonData == null)
            {
                return false;
            }

            return _axisButtonShowing.ButtonData.Id == id;
        }
        
        public void DeselectSelectedButton()
        {
            _axisButtonShowing.AnimationOut();
        }

        public MainBarCategoryListElementController GetCategoryElementById(string dataCategoryId)
        {
            foreach (var element in _activeElements)
            {
                if (element.CategoryData != null && element.CategoryData.Id == dataCategoryId)
                {
                    return element;
                }
            }
            return null;
        }
    }
}