using System;
using System.Collections.Generic;
using EjesDeInversion.Data;
using EjesDeInversion.Utilities;
using UnityEngine;

namespace EjesDeInversion.Managers
{
    public class PointersManager : Singleton<PointersManager>
    {
        [Header("General")]
        [SerializeField] private GameObject PointersParent;
        [SerializeField] private PointerController PointerPrefab;
        
        [Header("Pool")]
        private int _initialPoolSize = 30;
        
        private PointersData _pointersData;
        private List<PointerController> _pointers = new();
        private List<PointerController> _pointersPool = new();
        
        private void Start()
        {
            if(DataManager.TryLoad("PointersData", out PointersData data))
            {
                _pointersData = data;
                InitializeData();
                InitializePool();
                ShowAllPointersInternal();
            }
            else
            {
                Debug.LogError("PointersData could not be loaded.");
            }
        }
        
        private void InitializeData()
        {
            foreach (PointerData pointerData in _pointersData.Pointers)
            {
                pointerData.Initialize();
            }
        }
        
        private void InitializePool()
        {
            for (int i = 0; i < _initialPoolSize; i++)
            {
                PointerController pointer = Instantiate(PointerPrefab, PointersParent.transform);
                pointer.gameObject.SetActive(false);
                _pointersPool.Add(pointer);
            }
        }
        
        private void ShowAllPointersInternal()
        {
            ClearPointers();
            foreach (PointerData pointerData in _pointersData.Pointers)
            {
                PointerController pointer = GetPointerFromPool();
                pointer.Initialize(pointerData);
                pointer.gameObject.SetActive(true);
                _pointers.Add(pointer);
            }
        }
        
        public static void ShowAllPointers()
        {
            instance.ShowAllPointersInternal();
        }
        
        private PointerController GetPointerFromPool()
        {
            if (_pointersPool.Count > 0)
            {
                PointerController pointer = _pointersPool[0];
                _pointersPool.RemoveAt(0);
                return pointer;
            }
            else
            {
                PointerController pointer = Instantiate(PointerPrefab, PointersParent.transform);
                return pointer;
            }
        }
        
        private List<PointerController> FilterPointersByAxisInternal(MainBarData.InvestmentAxisButtonData axisData)
        {
            ClearPointers();
            foreach (PointerData pointerData in _pointersData.Pointers)
            {
                if (pointerData.AxisId == axisData.Id)
                {
                    PointerController pointer = GetPointerFromPool();
                    pointer.Initialize(pointerData);
                    pointer.gameObject.SetActive(true);
                    _pointers.Add(pointer);
                }
            }
            return _pointers;
        }

        public static List<PointerController> FilterPointersByAxis(MainBarData.InvestmentAxisButtonData axisData)
        {
            return instance.FilterPointersByAxisInternal(axisData);
        }
        
        private List<PointerController> FilterPointersBySubcategoryInternal(MainBarData.InvestmentAxisCategoryData categoryData)
        {
            ClearPointers();
            foreach (PointerData pointerData in _pointersData.Pointers)
            {
                if ($"{pointerData.AxisId}_{pointerData.SubcategoryId}" == categoryData.Id)
                {
                    PointerController pointer = GetPointerFromPool();
                    pointer.Initialize(pointerData);
                    pointer.gameObject.SetActive(true);
                    _pointers.Add(pointer);
                }
            }
            return _pointers;
        }

        public static List<PointerController> FilterPointersBySubcategory(MainBarData.InvestmentAxisCategoryData categoryData)
        {
            return instance.FilterPointersBySubcategoryInternal(categoryData);
        }

        private void ClearPointers()
        {
            foreach (PointerController pointer in _pointers)
            {
                pointer.gameObject.SetActive(false);
                _pointersPool.Add(pointer);
            }
            _pointers.Clear();
        }
    }
}