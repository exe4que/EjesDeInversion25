using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace EjesDeInversion
{
    public class MainBarButtonsContainerController : MonoBehaviour
    {
        [SerializeField] private RectTransform _viewportRectTransform;
        [SerializeField] private RectTransform _buttonsContainerRectTransform;
        [SerializeField] private HorizontalLayoutGroup _buttonsContainerLayoutGroup;
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private MainBarButtonController _buttonTemplate;

        private Rect _originalButtonsContainerRect;
        private bool _isInitialized = false;
        private List<MainBarButtonController> _buttonControllers = new ();
        private MainBarController _mainBarController;

        public void Initialize(MainBarController mainBarController)
        {
            _mainBarController = mainBarController;
            CreateButtons();
            AdjustContainerSizeToContent();
            AdjustButtonsContainerWidth();
        }

        public float GetDefaultContentWidth()
        {
            float buttonWidth = ((RectTransform)_buttonTemplate.transform).rect.width;
            float spacing = _buttonsContainerLayoutGroup.spacing;
            int buttonCount = _buttonControllers.Count;
            float paddingLeft = _buttonsContainerLayoutGroup.padding.left;
            float paddingRight = _buttonsContainerLayoutGroup.padding.right;
            
            return (buttonWidth * (buttonCount - 1)) + // all buttons but one in normal size
                   (buttonWidth * _buttonTemplate.AnimationTargetScale) + // one selected (bigger) button
                   (spacing * (buttonCount - 1)) + // spacings
                   paddingLeft + paddingRight; // paddings
        }
        
        private void AdjustContainerSizeToContent()
        {
            _buttonsContainerRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, GetDefaultContentWidth());
            
            _originalButtonsContainerRect = _buttonsContainerRectTransform.rect;
            _isInitialized = true;
        }

        private void CreateButtons()
        {
            MainBarData mainBarData = DataManager.MainBarData;
            for (int i = 0; i < mainBarData.InvestmentAxisButtons.Length; i++)
            {
                MainBarData.InvestmentAxisButtonData buttonData = mainBarData.InvestmentAxisButtons[i];
                GameObject buttonObj = Instantiate(_buttonTemplate.gameObject, _buttonsContainerRectTransform);
                buttonObj.SetActive(true);
                MainBarButtonController buttonController = buttonObj.GetComponent<MainBarButtonController>();
                buttonController.Initialize(buttonData, _mainBarController);
                _buttonControllers.Add(buttonController);
            }
        }
        
        public void AdjustButtonsContainerWidth()
        {
            if (!_isInitialized)
            {
                return;
            }

            if (_originalButtonsContainerRect.width < _viewportRectTransform.rect.width)
            {
                _buttonsContainerRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,
                    _viewportRectTransform.rect.width);
                _scrollRect.enabled = false;
            }
            else
            {
                // Restore original width if it was previously adjusted
                _buttonsContainerRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,
                    _originalButtonsContainerRect.width);
                _scrollRect.enabled = true;
            }
        }
    }
}
