using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EjesDeInversion
{
    public class MainBarCategoryListSelectedElementController : MonoBehaviour
    {
        [Header("General")]
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private Button _backButton;
        [SerializeField] private RectTransform _backButtonRectTransform;
        [SerializeField] private Image _backgroundImage;
        
        [Header("Animation")]
        [SerializeField] private float _backButtonShownYPosition = 80f;
        [SerializeField] private float _backButtonHiddenYPosition = 0f;
        [SerializeField] private float _backButtonAnimationDuration = 0.2f;
        [SerializeField] private Ease _backButtonShowEase = Ease.OutCubic;
        [SerializeField] private Ease _backButtonHideEase = Ease.InCubic;
        [SerializeField] private float _animationDuration = 0.3f;
        [SerializeField] private float _animationShownYPosition = 92f;
        [SerializeField] private Ease _showEase = Ease.OutCubic;
        [SerializeField] private Ease _hideEase = Ease.InCubic;
        [SerializeField] private Color _selectedColor = Color.white;
        [SerializeField] private Color _deselectedColor = Color.white;
        

        private MainBarController _mainBarController;
        private Sequence _backButtonSequence;
        private MainBarCategoryListElementController _mainBarCategoryListElementController;
        private RectTransform _rectTransform => (RectTransform)this.transform;
        private float _animationHiddenYPosition;
        
        private void OnEnable()
        {
            _backButton.onClick.AddListener(OnBackButtonClicked);
        }
        
        private void OnDisable()
        {
            _backButton.onClick.RemoveListener(OnBackButtonClicked);
        }
        
        public void Initialize(MainBarController mainBarController)
        {
            _mainBarController = mainBarController;
        }
        
        public void Show(MainBarCategoryListElementController mainBarCategoryListElementController)
        {
            _mainBarCategoryListElementController = mainBarCategoryListElementController;
            _nameText.text = _mainBarCategoryListElementController.CategoryData.Name;
            _animationHiddenYPosition = _mainBarCategoryListElementController.transform.position.y;
            this.transform.position = _mainBarCategoryListElementController.transform.position;
            AnimateIn();
        }

        public void Hide()
        {
            AnimateOut();
        }
        
        private void OnBackButtonClicked()
        {
            _mainBarController.DeselectCategory();
            AnimateOut();
        }
        
        private void AnimateIn()
        {
            this.gameObject.SetActive(true);
            _backgroundImage.color = _selectedColor;
            _backButtonSequence?.Kill();
            _backButtonSequence = DOTween.Sequence();
            _backButtonSequence.Append(_backButtonRectTransform.DOAnchorPosY(_backButtonShownYPosition, _backButtonAnimationDuration).SetEase(_backButtonShowEase));
            _backButtonSequence.Append(_rectTransform.DOAnchorPosY(_animationShownYPosition, _animationDuration).SetEase(_showEase));
        }

        private void AnimateOut()
        {
            _backgroundImage.color = _deselectedColor;
            _backButtonSequence?.Kill();
            _backButtonSequence = DOTween.Sequence();
            _backButtonSequence.Append(_backButtonRectTransform.DOAnchorPosY(_backButtonHiddenYPosition, _backButtonAnimationDuration).SetEase(_backButtonHideEase));
            _backButtonSequence.Append(transform.DOMoveY(_animationHiddenYPosition, _animationDuration).SetEase(_hideEase));
            _backButtonSequence.OnComplete(() => this.gameObject.SetActive(false));
        }
    }
}