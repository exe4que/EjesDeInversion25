using UnityEngine;

namespace EjesDeInversion
{
    public class SideBarSelectLocationButtonController : SideBarButtonController
    {
        [SerializeField] private LocationsPopupController _popupController;
        protected override void OnButtonClickInternal()
        {
            if (_popupController.gameObject.activeSelf)
            {
                _popupController.HidePopup();
            }
            else
            {
                _popupController.ShowPopup();
            }
        }
    }
}