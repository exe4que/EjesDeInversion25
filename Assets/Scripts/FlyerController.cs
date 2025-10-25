using System;
using DG.Tweening;
using EjesDeInversion;
using EjesDeInversion.Data;
using EjesDeInversion.Managers;
using EjesDeInversion.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
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
    [SerializeField] private PdfLinkController _pdfLinkTemplate;
    [SerializeField] private GameObject _pdfLinksContainer;
    
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
    
    public static void TryShow(string id)
    {
        if(DataManager.TryLoadData(id, out FlyerData flyerData))
        {
            Show(flyerData);
        }
        else
        {
            Debug.LogWarning($"Could not show flyer for id: {id}");
        }
    }

    public static void Show(FlyerData flyerData)
    {
        instance.ShowInstance(flyerData);
    }
    
    private void ShowInstance(FlyerData flyerData)
    {
        _titleText.text = flyerData.Title;
        _subtitleText.text = flyerData.Subtitle;
        _descriptionTitleText.text = flyerData.DescriptionTitle;
        _descriptionText.text = flyerData.Description;
        
        if (flyerData.CarouselImageNames == null || flyerData.CarouselImageNames.Length == 0)
        {
            _carousel.gameObject.SetActive(false);
        }
        else
        {
            _carousel.gameObject.SetActive(true);
            _carousel.SetData(flyerData.CarouselImageNames);
        }
        
        PopulatePdfLinks(flyerData);
        
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

    private void PopulatePdfLinks(FlyerData flyerData)
    {
        foreach (Transform child in _pdfLinksContainer.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (var pdfLink in flyerData.PdfLinks)
        {
            var pdfLinkController = Instantiate(_pdfLinkTemplate, _pdfLinksContainer.transform);
            pdfLinkController.SetData(pdfLink.Name, pdfLink.Url);
            pdfLinkController.gameObject.SetActive(true);
        }
    }

}
