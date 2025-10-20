using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace EjesDeInversion
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private InputActionReference Touch1;
        [SerializeField] private InputActionReference Touch2;
        [SerializeField] private InputActionReference Touch1Button;
        [SerializeField] private InputActionReference Touch2Button;
        [SerializeField] private InputActionReference ScrollWheel;

        [SerializeField] private float ZoomSensitivityMobile = 1f;
        [SerializeField] private float ZoomSensitivityDesktop = 20f;

        private CinemachineCamera _cinemachineCamera;
        private MyCinemachineConfiner3D _cinemachineConfiner3D;
        private int _touchCount = 0;
        private Vector3 _initialCameraPosition;
        private Vector2 _touch1StartPos;
        private Vector2 _touch2StartPos;

        private void Awake()
        {
            _cinemachineCamera = GetComponent<CinemachineCamera>();
            _cinemachineConfiner3D = GetComponent<MyCinemachineConfiner3D>();
        }

        // Update is called once per frame
        void Update()
        {
            // if cursor is over UI, do not process camera movement
            if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            //if cursor is out of screen, do not process camera movement
            if (Mouse.current != null)
            {
                Vector2 mousePosition = Mouse.current.position.ReadValue();
                if (mousePosition.x < 0 || mousePosition.x > Screen.width || mousePosition.y < 0 ||
                    mousePosition.y > Screen.height)
                {
                    return;
                }
            }

            if (_cinemachineConfiner3D.CameraWasDisplaced(_cinemachineCamera))
            {
                this.transform.position = _camera.transform.position;
            }

            Vector2 touch1Pos = Touch1.action.ReadValue<Vector2>();
            Vector2 touch2Pos = Touch2.action.ReadValue<Vector2>();

            float scrollValue = ScrollWheel.action.ReadValue<Vector2>().y;
            if ((Touch1Button.action.IsPressed() && Touch2Button.action.IsPressed()) || Mathf.Abs(scrollValue) > 0f)
            {
                if (_touchCount != 2)
                {
                    _initialCameraPosition = this.transform.localPosition;
                    _touch1StartPos = touch1Pos;
                    _touch2StartPos = touch2Pos;
                    _touchCount = 2;
                    _cinemachineConfiner3D.FreezeY = false;
                }

                // Both touches are active, handle pinch to zoom
                float initialDistance = Vector2.Distance(_touch1StartPos, _touch2StartPos);
                float currentDistance = Vector2.Distance(touch1Pos, touch2Pos);
                float distanceDelta = (currentDistance - initialDistance) * ZoomSensitivityMobile;

                // If using scroll wheel, override distanceDelta
                distanceDelta += scrollValue * ZoomSensitivityDesktop;

                //Debug.Log($"Distance Delta: {distanceDelta}");
                this.transform.position = _initialCameraPosition + this.transform.forward * distanceDelta;
            }
            else if (Touch1Button.action.IsPressed())
            {
                if (_touchCount != 1)
                {
                    _initialCameraPosition = this.transform.position;
                    _touch1StartPos = touch1Pos;
                    _touch2StartPos = touch2Pos;
                    _touchCount = 1;
                    _cinemachineConfiner3D.FreezeY = true;
                }

                // Single touch active, handle panning
                Vector3 startWorldPos =
                    _camera.ScreenToWorldPoint(new Vector3(_touch1StartPos.x, _touch1StartPos.y,
                        this.transform.position.y));
                Vector3 currentWorldPos =
                    _camera.ScreenToWorldPoint(new Vector3(touch1Pos.x, touch1Pos.y, this.transform.position.y));
                Vector3 touchWorldDelta = currentWorldPos - startWorldPos;
                //translate from screen space to world space
                this.transform.position = _initialCameraPosition - touchWorldDelta;
                //Debug.Log($"touchWorldDelta: {touchWorldDelta}");
            }
            else
            {
                _touchCount = 0;
            }

            this.transform.position = _cinemachineConfiner3D.ConfinePoint(this.transform.position);

            //Debug.Log($"Touch count: {_touchCount}, Touch1: {Touch1Button.action.IsPressed()}, Touch2: {Touch2Button.action.IsPressed()}" );
        }

    }
}
