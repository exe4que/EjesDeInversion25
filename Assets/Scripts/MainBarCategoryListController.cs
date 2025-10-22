using System;
using System.Collections.Generic;
using EjesDeInversion.Data;
using UnityEngine;

namespace EjesDeInversion
{
    public class MainBarCategoryListController : MonoBehaviour
    {
        [SerializeField] private int _initialElementsPoolSize = 10;
        [SerializeField] private MainBarCategoryListElementController _categoryListElementPrefab;
        [SerializeField] private Transform _elementsContainer;

        private List<MainBarCategoryListElementController> _elementsPool = new();
        private string _idShowing = "";
        
        public static Action<string> OnCategorySelected;

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

        public void Show(MainBarData.InvestmentAxisButtonData buttonData)
        {
            if (_idShowing != buttonData.Id)
            {
                Hide();
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
        }

        public void Hide()
        {
            foreach (var element in _elementsPool)
            {
                element.gameObject.SetActive(false);
            }
            this.gameObject.SetActive(false);
            _idShowing = "";
            OnCategorySelected?.Invoke("");
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