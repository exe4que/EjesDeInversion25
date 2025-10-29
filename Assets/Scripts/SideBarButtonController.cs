using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace EjesDeInversion
{
    public abstract class SideBarButtonController : MonoBehaviour
    {
        [Header("General")]
        [SerializeField] protected Image _iconBackground;
        [SerializeField] protected Image _icon;
        [SerializeField] protected Button _button;
        [SerializeField] protected RectTransform _mainContainer;
        [SerializeField] protected bool _isToggleButton = false;

        [Header("Settings")] 
        [SerializeField] protected Color _iconActiveColor = new Color(0.1843137f, 0.6901961f, 0.8705882f);
        [SerializeField] protected Color _iconInactiveColor = new Color(0.3f, 0.3f, 0.3f);
        [SerializeField] protected float _animationDuration = 0.2f;
        [SerializeField] protected float _animationTargetXPosition = -63f;
        [SerializeField] protected Ease _animationInEase = Ease.OutBack;
        [SerializeField] protected Ease _animationOutEase = Ease.InBack;
        [SerializeField] protected Ease _animationSimpleClickEase = Ease.InOutQuad;
        
        protected bool _isToggledOn = false;
        protected Sequence _currentAnimationSequence;

        protected virtual void OnEnable()
        {
            _button.onClick.AddListener(OnButtonClick);
        }
        
        protected virtual void OnDisable()
        {
            _button.onClick.RemoveListener(OnButtonClick);
        }

        protected virtual void OnButtonClick()
        {
            if (_isToggleButton)
            {
                _isToggledOn = !_isToggledOn;
                AnimateToggleClick();
            }
            else
            {
                AnimateSimpleClick();
            }

            OnButtonClickInternal();
        }

        protected abstract void OnButtonClickInternal();
        
        protected void AnimateSimpleClick()
        {
            _currentAnimationSequence?.Kill();
            _currentAnimationSequence = DOTween.Sequence();
            _iconBackground.color = _iconInactiveColor;
            _mainContainer.anchoredPosition = new Vector2(0f, _mainContainer.anchoredPosition.y);
            _currentAnimationSequence.Append(
                _iconBackground.DOColor(_iconActiveColor, _animationDuration * 2f)
                    .SetLoops(2, LoopType.Yoyo)
                    .SetEase(_animationSimpleClickEase));
            _currentAnimationSequence.Join(
                _mainContainer.DOAnchorPosX(_animationTargetXPosition, _animationDuration * 2f)
                    .SetLoops(2, LoopType.Yoyo)
                    .SetEase(_animationSimpleClickEase));
        }
        
        protected void AnimateToggleClick()
        {
            _currentAnimationSequence?.Kill();
            _currentAnimationSequence = DOTween.Sequence();

            if (_isToggledOn)
            {
                _iconBackground.color = _iconInactiveColor;
                _mainContainer.anchoredPosition = new Vector2(0f, _mainContainer.anchoredPosition.y);
                // Animate to active state
                _currentAnimationSequence.Append(
                    _iconBackground.DOColor(_iconActiveColor, _animationDuration)
                        .SetEase(_animationInEase));
                _currentAnimationSequence.Join(
                    _mainContainer.DOAnchorPosX(_animationTargetXPosition, _animationDuration)
                        .SetEase(_animationInEase));
            }
            else
            {
                _iconBackground.color = _iconActiveColor;
                _mainContainer.anchoredPosition = new Vector2(_animationTargetXPosition, _mainContainer.anchoredPosition.y);
                // Animate to inactive state
                _currentAnimationSequence.Append(
                    _iconBackground.DOColor(_iconInactiveColor, _animationDuration)
                        .SetEase(_animationOutEase));
                _currentAnimationSequence.Join(
                    _mainContainer.DOAnchorPosX(0f, _animationDuration)
                        .SetEase(_animationOutEase));
            }
        }

    } 
}