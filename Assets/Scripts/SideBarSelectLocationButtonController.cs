using UnityEngine;

namespace EjesDeInversion
{
    public class SideBarSelectLocationButtonController : SideBarButtonController
    {
        [SerializeField] private LocationsPopupController _popupController;
        
        protected override void OnEnable()
        {
            base.OnEnable();
            LocationsPopupButtonController.OnLocationButtonClicked += OnButtonClick;
        }
        
        protected override void OnDisable()
        {
            base.OnDisable();
            LocationsPopupButtonController.OnLocationButtonClicked -= OnButtonClick;
        }
        
        protected override void OnButtonClickInternal()
        {
            if (_popupController.gameObject.activeSelf)
            {
                HidePopup();
            }
            else
            {
                ShowPopup();
            }
        }

        private void ShowPopup()
        {
            _popupController.ShowPopup();
        }

        private void HidePopup()
        {
            _popupController.HidePopup();
        }
    }
}