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
            if(DataManager.TryLoadData("PointersData", out PointersData data))
            {
                _pointersData = data;
                InitializeData();
                InitializePool();
                InitializePointers();
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
        
        private void InitializePointers()
        {
            foreach (PointerData pointerData in _pointersData.Pointers)
            {
                PointerController pointer = GetPointerFromPool();
                pointer.Initialize(pointerData);
                pointer.gameObject.SetActive(true);
                _pointers.Add(pointer);
            }
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
    }
}