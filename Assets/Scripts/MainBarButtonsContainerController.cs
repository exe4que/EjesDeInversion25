using System;
using UnityEngine;
using UnityEngine.UI;

public class MainBarButtonsContainerController : MonoBehaviour
{
    [SerializeField] private RectTransform _viewportRectTransform;
    [SerializeField] private RectTransform _buttonsContainerRectTransform;
    [SerializeField] private ContentSizeFitter _buttonsContainerSizeFitter;
    [SerializeField] private ScrollRect _scrollRect;
    
    private Rect _originalButtonsContainerRect;
    private bool _isInitialized = false;

    public void AdjustButtonsContainerWidth()
    {
        if (!_isInitialized)
        {
            _originalButtonsContainerRect = _buttonsContainerRectTransform.rect;
            _isInitialized = true;
        }

        if (_originalButtonsContainerRect.width < _viewportRectTransform.rect.width)
        {
            _buttonsContainerRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _viewportRectTransform.rect.width);
            _buttonsContainerSizeFitter.enabled = false;
            _scrollRect.enabled = false;
        }
        else
        {
            // Restore original width if it was previously adjusted
            _buttonsContainerRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,
                _originalButtonsContainerRect.width);
            _buttonsContainerSizeFitter.enabled = true;
            _scrollRect.enabled = true;
        }
    }
}
