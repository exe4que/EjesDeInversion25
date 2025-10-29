using EjesDeInversion.Managers;
using UnityEngine;

namespace EjesDeInversion
{
    public class SideBarAlignMapButtonController : SideBarButtonController
    {
        protected override void OnEnable()
        {
            base.OnEnable();
            CameraManager.OnCameraZRotationChanged += OnCameraZRotationChanged;
        }
        
        protected override void OnDisable()
        {
            base.OnDisable();
            CameraManager.OnCameraZRotationChanged -= OnCameraZRotationChanged;
        }
        
        protected override void OnButtonClickInternal()
        {
            CameraManager.RealignRotation();
        }
        
        private void OnCameraZRotationChanged(float zRotation)
        {
            _icon.transform.localRotation = Quaternion.Euler(0f, 0f, zRotation);
        }
    }
}