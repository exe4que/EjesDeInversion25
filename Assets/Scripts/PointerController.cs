using EjesDeInversion.Data;
using EjesDeInversion.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EjesDeInversion
{
    public class PointerController : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;
        [SerializeField] private TMP_Text _descriptionText;
        [SerializeField] private Button _button;
        [SerializeField] private Image _backgroundFillImage;
        [SerializeField] private Color _normalColor;
        [SerializeField] private Color _linkColor;
        
        private PointerData _data;
        public PointerData Data => _data;

        private void OnEnable()
        {
            _button.onClick.AddListener(OnButtonClicked);
        }
        
        private void OnDisable()
        {
            _button.onClick.RemoveListener(OnButtonClicked);
        }
        
        private void OnButtonClicked()
        {
            FlyerManager.TryShow(_data.Id);
        }

        public void Initialize(PointerData data)
        {
            _data = data;
            _text.text = $"<b>{data.Name}</b>";
            _descriptionText.text = data.ShortDescription;
            _backgroundFillImage.color = data.IsLink ? _linkColor : _normalColor;
        }

        private void LateUpdate()
        {
            Vector3 targetPosition = _data.Position;
            // world to screen
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(targetPosition);
            screenPosition.z = 0;
            // follow the screen position
            transform.position = screenPosition;
        }

        public Vector3 GetCameraPosition()
        {
            return _data.Position;
        }
    }
}