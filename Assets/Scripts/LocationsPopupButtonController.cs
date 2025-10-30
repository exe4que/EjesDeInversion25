using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EjesDeInversion
{
    public class LocationsPopupButtonController : MonoBehaviour
    {
        [SerializeField] private Image _iconImage;
        [SerializeField] private TMP_Text _nameText;

        private Data.LocationsData.Location _locationData;

        public void Initialize(Data.LocationsData.Location locationData)
        {
            _locationData = locationData;
            _nameText.text = _locationData.Name;
            
            // Load icon from Resources/Icons folder
            Sprite iconSprite = Resources.Load<Sprite>($"LocationIcon/{_locationData.IconName}");
            if (iconSprite != null)
            {
                _iconImage.sprite = iconSprite;
            }
            else
            {
                Debug.LogWarning($"Icon sprite '{_locationData.IconName}' not found in Resources/LocationIcon.");
            }
        }
    }
}