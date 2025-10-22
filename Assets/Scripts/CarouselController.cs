using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
namespace EjesDeInversion
{
    public class CarouselController : MonoBehaviour
    {
        [Header("General")] [SerializeField] private CarouselDotController _carouselDotTemplate;
        [SerializeField] private Transform _dotsContainer;
        [SerializeField] private RectTransform _imagesContainer;
        [Header("Animation")] [SerializeField] private Button _leftButton;
        [SerializeField] private Button _rightButton;
        [SerializeField] private Ease _ease = Ease.OutCubic;
        [SerializeField] private float _duration = 0.5f;
        [SerializeField] private LayoutElement _firstImageLayoutElement;
        [SerializeField] private GameObject _imagePrefab;

        private int _imagesCount = 0;
        private int _currentIndex = 0;
        private float _imageWidth = 0f;
        private List<CarouselDotController> _dots = new();

        /*private void Start()
        {
            Initialize();
        }*/

        public async void SetData(string[] imageNames)
        {
            ClearAll();
            await LoadAndCreateImages(imageNames);
            Initialize();
        }

        private void ClearAll()
        {
            // Clear existing images
            foreach (Transform child in _imagesContainer)
            {
                //avoid destroying the prefab template
                if(child.gameObject.activeSelf)
                {
                    Destroy(child.gameObject);
                }
            }

            // Clear dots
            foreach (var dot in _dots)
            {
                Destroy(dot.gameObject);
            }
            _dots.Clear();

            // Reset index and position
            _currentIndex = 0;
            _imagesContainer.anchoredPosition = new Vector2(0, _imagesContainer.anchoredPosition.y);
            _leftButton.gameObject.SetActive(false);
            _rightButton.gameObject.SetActive(true);
        }

        private async Task LoadAndCreateImages(string[] imageNames)
        {
            // load images from addressables
            foreach (var imageName in imageNames)
            {
                AsyncOperationHandle<Texture2D> handle = Addressables.LoadAssetAsync<Texture2D>(imageName);
                await handle.Task;
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    Texture2D texture = handle.Result;
                    CreateNewImage(texture);

                    // Release the handle if no longer needed
                    Addressables.Release(handle);
                }
                else
                {
                    Debug.LogError($"Failed to load image: {imageName}");
                }
            }
        }

        private void CreateNewImage(Texture2D texture)
        {
            Sprite sprite = Sprite.Create(texture,
                new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f));

            // Instantiate image prefab
            GameObject imageGO = Instantiate(_imagePrefab, _imagesContainer);
            Image imageComponent = imageGO.GetComponent<Image>();
            imageComponent.sprite = sprite;
            imageGO.SetActive(true);
        }

        private void Initialize()
        {
            //only active children are counted
            _imagesCount = 0;
            foreach (Transform child in _imagesContainer)
            {
                if (child.gameObject.activeSelf)
                {
                    _imagesCount++;
                }
            }
            _imageWidth = _firstImageLayoutElement.preferredWidth;

            InstantiateDots();
        }

        private void InstantiateDots()
        {
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
}
