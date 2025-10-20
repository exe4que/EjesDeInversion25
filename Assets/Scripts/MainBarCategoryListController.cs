using System;
using System.Collections.Generic;
using UnityEngine;

namespace EjesDeInversion
{
    public class MainBarCategoryListController : MonoBehaviour
    {
        [SerializeField] private int _initialElementsPoolSize = 10;
        [SerializeField] private MainBarCategoryListElementController _categoryListElementPrefab;
        [SerializeField] private Transform _elementsContainer;

        private List<MainBarCategoryListElementController> _elementsPool = new();
        private bool _isVisible = false;

        private void Start()
        {
            InitializePool();
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

        public void Show(MainBarData.InvestmentAxisCategoryData[] categoriesData)
        {
            this.gameObject.SetActive(true);
            for (int i = 0; i < categoriesData.Length; i++)
            {
                MainBarCategoryListElementController element = GetPooledElement();
                element.SetData(categoriesData[i]);
                element.gameObject.SetActive(true);
            }
            _isVisible = true;
        }

        public void Hide()
        {
            foreach (var element in _elementsPool)
            {
                element.gameObject.SetActive(false);
            }
            this.gameObject.SetActive(false);
            _isVisible = false;
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

        public bool IsVisible()
        {
            return _isVisible;
        }
    }
}