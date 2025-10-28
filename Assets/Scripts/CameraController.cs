using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace EjesDeInversion
{
    public class CameraController : MonoBehaviour
    {
        [Header("General")]
        [SerializeField] private Camera _camera;
        [SerializeField] private InputActionReference Touch1;
        [SerializeField] private InputActionReference Touch2;
        [SerializeField] private InputActionReference Touch1Button;
        [SerializeField] private InputActionReference Touch2Button;
        [SerializeField] private InputActionReference ScrollWheel;

        [Header("Zoom Settings")]
        [SerializeField] private float DeadZoneDistance = 100f;
        [SerializeField] private float ZoomSensitivityMobile = 1f;
        [SerializeField] private float ZoomSensitivityDesktop = 20f;
        [SerializeField] private float OrthographicSizeMin = 5f;
        [SerializeField] private float OrthographicSizeMax = 350f;
        
        [Header("Rotation Settings")]
        [SerializeField] private float RotationSensitivity = 0.1f;
        
        private CinemachineCamera _cinemachineCamera;
        private CinemachineConfiner2D _cinemachineConfiner2D;
        private int _touchCount = 0;
        private Vector3 _initialCameraPosition;
        private Vector2 _touch1StartPos;
        private Vector2 _touch2StartPos;
        private int _cameraMode = 0; // 0 = none/pan, 1 = zoom, 2 = rotate
        private float _initialOrthographicSize;

        private void Awake()
        {
            _cinemachineCamera = GetComponent<CinemachineCamera>();
        }

        // Update is called once per frame
        void Update()
        {
            // if cursor is over UI, do not process camera movement
            if (_touchCount == 0 && 
                UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
            
            this.transform.position = _camera.transform.position;

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
                }

                Vector2 spanVector = (_touch1StartPos - _touch2StartPos).normalized;
                Vector2 vector1 = (touch1Pos - _touch1StartPos).normalized;
                
                // Both touches are active, handle pinch to zoom
                float initialDistance = Vector2.Distance(_touch1StartPos, _touch2StartPos);
                float currentDistance = Vector2.Distance(touch1Pos, touch2Pos);
                float distanceDelta = (currentDistance - initialDistance);
                
                float dot = Vector2.Dot(vector1, spanVector);
                //Debug.Log($"Dot: {dot}, Distance Delta: {distanceDelta}");
                
                float touch1Distance = Vector2.Distance(touch1Pos, _touch1StartPos);

                if (_cameraMode == 2 || (_cameraMode == 0 && Mathf.Abs(dot) < RotationSensitivity && touch1Distance > DeadZoneDistance))
                {
                    HandleRotation(touch2Pos, touch1Pos);
                }
                else if (_cameraMode == 1 || (_cameraMode == 0 && Mathf.Abs(dot) >= RotationSensitivity && touch1Distance > DeadZoneDistance))
                {
                    HandleZoom(scrollValue, distanceDelta);
                }
            }
            else if (Touch1Button.action.IsPressed() && _cameraMode == 0)
            {
                HandleMove(touch1Pos, touch2Pos);
            }
            else
            {
                _touchCount = 0;
                _cameraMode = 0;
            }
            //Debug.Log($"Touch count: {_touchCount}, Touch1: {Touch1Button.action.IsPressed()}, Touch2: {Touch2Button.action.IsPressed()}" );
        }

        private void HandleRotation(Vector2 touch2Pos, Vector2 touch1Pos)
        {
            _cameraMode = 2; // Rotate
            float angle = Vector2.SignedAngle(_touch2StartPos - _touch1StartPos, touch2Pos - touch1Pos);
            this.transform.rotation = Quaternion.Euler(0f, 0f, -angle);
        }

        private void HandleMove(Vector2 touch1Pos, Vector2 touch2Pos)
        {
            if (_touchCount != 1)
            {
                _initialCameraPosition = this.transform.position;
                _touch1StartPos = touch1Pos;
                _touch2StartPos = touch2Pos;
                _touchCount = 1;
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
        }

        private void HandleZoom(float scrollValue, float distanceDelta)
        {
            if(_cameraMode != 1)
            {
                _cameraMode = 1; // Zoom
                _initialOrthographicSize = _cinemachineCamera.Lens.OrthographicSize;
            }
            float zoomInput = 0f;
            float sensitivity = 0f;
            if (Mathf.Abs(scrollValue) > 0f)
            {
                zoomInput = scrollValue;
                sensitivity = ZoomSensitivityDesktop;
            }
            else
            {
                zoomInput = distanceDelta;
                sensitivity = ZoomSensitivityMobile;
            }
            
            float logScale = Mathf.Log(_initialOrthographicSize + 1f); // smaller orthographic sizes -> smaller effect
            float sizeDelta = zoomInput * sensitivity * logScale;
                
            //set camera orthographic size
            float newSize = _initialOrthographicSize - sizeDelta;
            newSize = Mathf.Clamp(newSize, OrthographicSizeMin, OrthographicSizeMax);
            _cinemachineCamera.Lens.OrthographicSize = newSize;
        }
    }
}
