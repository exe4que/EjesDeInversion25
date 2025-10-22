using System;
using DG.Tweening;
using EjesDeInversion;
using EjesDeInversion.Data;
using EjesDeInversion.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FlyerController : Singleton<FlyerController>
{
    [Header("General")]
    [SerializeField] private TMP_Text _titleText;
    [SerializeField] private TMP_Text _subtitleText;
    [SerializeField] private TMP_Text _descriptionTitleText;
    [SerializeField] private TMP_Text _descriptionText;
    [SerializeField] private CarouselController _carousel;
    [SerializeField] private Button _closeButton;
    
    [Header("Animation")]
    [SerializeField] private float _animationDuration = 0.3f;
    [SerializeField] private Ease _animationEaseIn = Ease.OutCubic;
    [SerializeField] private Ease _animationEaseOut = Ease.InCubic;
    
    private Vector2 _initialPosition;
    private RectTransform _rectTransform => (RectTransform)transform;

    private void OnEnable()
    {
        _closeButton.onClick.AddListener(AnimateClose);
    }
    
    private void OnDisable()
    {
        _closeButton.onClick.RemoveListener(AnimateClose);
    }

    private void Start()
    {
        _initialPosition = ((RectTransform)transform).anchoredPosition;
    }

    public static void Show(FlyerData flyerData)
    {
        Instance.ShowInstance(flyerData);
    }
    
    private void ShowInstance(FlyerData flyerData)
    {
        _titleText.text = flyerData.Title;
        _subtitleText.text = flyerData.Subtitle;
        _descriptionTitleText.text = flyerData.DescriptionTitle;
        _descriptionText.text = flyerData.Description;
        _carousel.SetData(flyerData.CarouselImageNames);
        AnimateOpen();
    }

    private void AnimateOpen()
    {
        _rectTransform.anchoredPosition = _initialPosition;
        _rectTransform.DOKill();
        _rectTransform.DOAnchorPosX(0, _animationDuration).SetEase(_animationEaseIn);
    }
    
    private void AnimateClose()
    {
        _rectTransform.DOKill();
        _rectTransform.DOAnchorPosX(_initialPosition.x, _animationDuration).SetEase(_animationEaseOut);
    }


}
