using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CarouselBehaviour : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private CarouselDotBehaviour _carouselDotTemplate;
    [SerializeField] private Transform _dotsContainer;
    [SerializeField] private RectTransform _imagesContainer;
    [Header("Animation")]
    [SerializeField] private Button _leftButton;
    [SerializeField] private Button _rightButton;
    [SerializeField] private Ease _ease = Ease.OutCubic;
    [SerializeField] private float _duration = 0.5f;
    [SerializeField] LayoutElement _firstImageLayoutElement;

    private int _imagesCount = 0;
    private int _currentIndex = 0;
    private float _imageWidth = 0f;
    private List<CarouselDotBehaviour> _dots = new ();
    
    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        _imagesCount = _imagesContainer.childCount;
        _imageWidth = _firstImageLayoutElement.preferredWidth;
        
        for (int i = 0; i < _imagesCount; i++)
        {
            var dot = Instantiate(_carouselDotTemplate, _dotsContainer);
            dot.gameObject.SetActive(true);
            dot.Select(i == _currentIndex);
            _dots.Add(dot);
        }
    }
    
    public void OnLeftButtonClicked()
    {
        if (_currentIndex > 0)
        {
            _currentIndex--;
            UpdateCarousel();
        }
    }
    
    public void OnRightButtonClicked()
    {
        if (_currentIndex < _imagesCount - 1)
        {
            _currentIndex++;
            UpdateCarousel();
        }
    }
    
    private void UpdateCarousel()
    {
        _leftButton.gameObject.SetActive(_currentIndex > 0);
        _rightButton.gameObject.SetActive(_currentIndex < _imagesCount - 1);
        
        // Update image position
        float xPosition = -_currentIndex * _imageWidth;
        _imagesContainer.DOAnchorPosX(xPosition, _duration)
            .SetEase(_ease);

        // Update dots
        for (int i = 0; i < _dots.Count; i++)
        {
            _dots[i].Select(i == _currentIndex);
        }
    }
}
