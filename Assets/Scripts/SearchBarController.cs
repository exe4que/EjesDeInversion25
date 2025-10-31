using System;
using System.Collections.Generic;
using EjesDeInversion.Data;
using EjesDeInversion.Managers;
using UnityEngine;

namespace EjesDeInversion
{
    public class SearchBarController : MonoBehaviour
    {
        [Header("General")]
        [SerializeField] private TMPro.TMP_InputField _inputField;
        [SerializeField] private SearchBarResultElementController _barResultElementTemplate;
        [SerializeField] private Transform _resultsParent;
        
        [Header("Settings")]
        [SerializeField] private int _poolInitialSize = 20;
        [SerializeField] private int _elementsToProcessPerFrame = 50;
        
        [Header("Dynamic Size Settings")]
        [SerializeField] private float _verticalModeHorizontalPadding = 67f;
        [SerializeField] private float _horizontalModeWidth = 956f;
        [SerializeField] private Vector2 _horizontalModePosition = new Vector2(67f, -106f);
        
        private string _searchText;
        private int _elementsProcessed = 0;

        private List<PointerController> _allPointerControllers = null;
        private List<PointerController> _filteredPointerControllers = new();
        private List<SearchBarResultElementController> _resultElementsPool = new();
        private List<SearchBarResultElementController> _activeResultElements = new();
        
        private ScreenOrientation _currentOrientation = ScreenOrientation.Portrait;

        private void OnEnable()
        {
            _inputField.onValueChanged.AddListener(OnInputValueChanged);
        }
        
        private void OnDisable()
        {
            _inputField.onValueChanged.RemoveListener(OnInputValueChanged);
        }

        private void Start()
        {
            _allPointerControllers = PointersManager.GetAllActivePointers();
            InitializePool();
            _currentOrientation = Screen.orientation;
            AdjustDynamicSize();
        }

        private void AdjustDynamicSize()
        {
            RectTransform rectTransform = GetComponent<RectTransform>();
            if (Screen.width < Screen.height) // Vertical mode
            {
                // anchor to top stretch
                rectTransform.anchorMin = new Vector2(0f, 1f);
                rectTransform.anchorMax = new Vector2(1f, 1f);
                rectTransform.pivot = new Vector2(0.5f, 1f);
                rectTransform.anchoredPosition = new Vector2(0f, _horizontalModePosition.y);
                rectTransform.sizeDelta = new Vector2(-_verticalModeHorizontalPadding * 2, rectTransform.sizeDelta.y);
            }
            else // Horizontal mode
            {
                // anchor to top left corner
                rectTransform.anchorMin = new Vector2(0f, 1f);
                rectTransform.anchorMax = new Vector2(0f, 1f);
                rectTransform.pivot = new Vector2(0f, 1f);
                rectTransform.anchoredPosition = _horizontalModePosition;
                rectTransform.sizeDelta = new Vector2(_horizontalModeWidth, rectTransform.sizeDelta.y);
            }
        }

        private void InitializePool()
        {
            for (int i = 0; i < _poolInitialSize; i++)
            {
                SearchBarResultElementController element = Instantiate(_barResultElementTemplate, _resultsParent);
                element.gameObject.SetActive(false);
                _resultElementsPool.Add(element);
            }
        }

        private void OnInputValueChanged(string input)
        {
            _searchText = input.Trim().ToLower();
            _elementsProcessed = 0;
            _filteredPointerControllers.Clear();
            HideResults();
            MainBarManager.HideAll();
            PointersManager.ShowAllPointers();
        }

        private SearchBarResultElementController GetResultElement()
        {
            if (_resultElementsPool.Count > 0)
            {
                SearchBarResultElementController element = _resultElementsPool[0];
                _resultElementsPool.RemoveAt(0);
                _activeResultElements.Add(element);
                return element;
            }
            else
            {
                SearchBarResultElementController element = Instantiate(_barResultElementTemplate, _resultsParent);
                _activeResultElements.Add(element);
                return element;
            }
        }

        public void HideResults()
        {
            foreach (var element in _activeResultElements)
            {
                element.gameObject.SetActive(false);
                _resultElementsPool.Add(element);
            }
            _activeResultElements.Clear();
        }

        private void Update()
        {
            HandleSearch();
            HandleDeviceOrientationChange();
        }

        private void HandleDeviceOrientationChange()
        {
            if (Screen.orientation != _currentOrientation)
            {
                _currentOrientation = Screen.orientation;
                AdjustDynamicSize();
            }
        }

        private void HandleSearch()
        {
            if (!string.IsNullOrEmpty(_searchText) && _elementsProcessed < _allPointerControllers.Count)
            {
                int processedThisFrame = 0;
                while (processedThisFrame < _elementsToProcessPerFrame && _elementsProcessed < _allPointerControllers.Count)
                {
                    PointerController pointer = _allPointerControllers[_elementsProcessed];
                    if (pointer.Data.Name.ToLower().Contains(_searchText))
                    {
                        _filteredPointerControllers.Add(pointer);
                    }
                    _elementsProcessed++;
                    processedThisFrame++;
                }

                // After processing, update the results display
                if (_elementsProcessed >= _allPointerControllers.Count)
                {
                    foreach (var pointer in _filteredPointerControllers)
                    {
                        SearchBarResultElementController element = GetResultElement();
                        element.Initialize(pointer, this);
                        element.gameObject.SetActive(true);
                    }
                }
            }
        }

        public void ClearInputField()
        {
            _inputField.text = string.Empty;
        }
    }
}