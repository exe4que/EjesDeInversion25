using EjesDeInversion.Managers;
using UnityEngine;

namespace EjesDeInversion
{
    public class SideBarZoomButtonController : SideBarButtonController
    {
        [SerializeField] private bool _zoomIn;
        protected override void OnButtonClickInternal()
        {
            if (_zoomIn)
            {
                CameraManager.ZoomIn();
            }
            else
            {
                CameraManager.ZoomOut();
            }
        }
    }
}