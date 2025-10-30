using System;
using EjesDeInversion.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EjesDeInversion
{
    public class LocationsPopupButtonController : MonoBehaviour
    {
        [SerializeField] private Image _iconImage;
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private Button _button;

        private Data.LocationsData.Location _locationData;
        public static Action OnLocationButtonClicked;

        private void OnEnable()
        {
            _button.onClick.AddListener(OnClick);
        }
        
        private void OnDisable()
        {
            _button.onClick.RemoveListener(OnClick);
        }

        public void Initialize(Data.LocationsData.Location locationData)
        {
            _locationData = locationData;
            _nameText.text = _locationData.Name;
            
            // Load icon from Resources/Icons folder
            if (DataManager.TryLoad<Sprite>(_locationData.IconName, out var iconSprite))
            {
                _iconImage.sprite = iconSprite;
            }
            else
            {
                Debug.LogError($"Failed to load icon sprite: {_locationData.IconName}");
            }
        }

        public void OnClick()
        {
            CameraManager.GoToLocation(_locationData.CameraPosition, _locationData.CameraSize);
            OnLocationButtonClicked?.Invoke();
        }
    }
}