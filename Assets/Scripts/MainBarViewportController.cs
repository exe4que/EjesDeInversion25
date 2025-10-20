using System;
using UnityEngine;

namespace EjesDeInversion
{
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
}
