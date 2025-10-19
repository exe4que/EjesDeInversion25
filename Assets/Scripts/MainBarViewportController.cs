using System;
using UnityEngine;

public class MainBarViewportController : MonoBehaviour
{
    [SerializeField] private MainBarButtonsContainerController _mainBarButtonsController;

    private void OnRectTransformDimensionsChange()
    {
        if (_mainBarButtonsController != null)
        {
            _mainBarButtonsController.AdjustButtonsContainerWidth();
        }
    }
}
