using System;
using System.Collections.Generic;
using DG.Tweening;
using EjesDeInversion.Utilities;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace EjesDeInversion.Managers
{
    public class CameraManager : Singleton<CameraManager>
    {
        [Header("General")]
        [SerializeField] private Camera _camera;
        [SerializeField] private InputActionReference _touch1;
        [SerializeField] private InputActionReference _touch2;
        [SerializeField] private InputActionReference _touch1Button;
        [SerializeField] private InputActionReference _touch2Button;
        [SerializeField] private InputActionReference _scrollWheel;
        //Middle mouse button can be used for rotating the camera instead of touch
        [SerializeField] private InputActionReference _middleMouseButton;

        [Header("Zoom Settings")]
        [SerializeField] private float _deadZoneDistance = 30f;
        [SerializeField] private float _zoomSensitivityMobile = 0.04f;
        [SerializeField] private float _zoomSensitivityDesktop = 5f;
        [SerializeField] private float _orthographicSizeMin = 15f;
        [SerializeField] private float _orthographicSizeMax = 350f;
        [SerializeField] private float _goToPointerSize = 50f;
        
        [Header("Rotation Settings")]
        [SerializeField] private float _rotationGestureDiscerningSensitivity = 0.6f;
        [SerializeField] private float _rotationSensitivityDesktop = 0.2f;
        
        [Header("Realign Camera Animation")]
        [SerializeField] private float _rotationRealignDuration = 0.4f;
        [SerializeField] private Ease _rotationRealignEase = Ease.InOutSine;
        
        [Header("Go To Location Animation")]
        [SerializeField] private float _goToLocationDuration = 1.0f;
        [SerializeField] private Ease _goToLocationEase = Ease.InOutCubic;
        
        private CinemachineCamera _cinemachineCamera;
        private CinemachineConfiner2D _cinemachineConfiner2D;
        private int _touchCount = 0;
        private Vector3 _initialCameraPosition;
        private Quaternion _initialCameraRotation;
        private Vector2 _touch1StartPos;
        private Vector2 _touch2StartPos;
        private int _cameraMode = 0; // 0 = none/pan, 1 = zoom, 2 = rotate
        private float _initialOrthographicSize;
        private float _initialMiddleMouseXPos;
        private bool _isMiddleMousePressed = false;
        
        public static Action<float> OnCameraZRotationChanged;

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

            Vector2 touch1Pos = _touch1.action.ReadValue<Vector2>();
            Vector2 touch2Pos = _touch2.action.ReadValue<Vector2>();
            
            float middleMouseButtonDeltaX = 0f;
            // Check for middle mouse button for rotation
            if (_middleMouseButton.action.IsPressed())
            {
                if (!_isMiddleMousePressed)
                {
                    _initialMiddleMouseXPos = Mouse.current.position.ReadValue().x;
                    _isMiddleMousePressed = true;
                    _cameraMode = 2; // Rotate
                    _touchCount = 1;
                }
                
                float currentMouseXPos = Mouse.current.position.ReadValue().x;
                middleMouseButtonDeltaX = currentMouseXPos - _initialMiddleMouseXPos;
                //Debug.Log($"Middle Mouse Delta X: {middleMouseButtonDeltaX}".Color(Color.aquamarine));
            }
            else
            {
                _isMiddleMousePressed = false;
            }

            float scrollValue = _scrollWheel.action.ReadValue<Vector2>().y;
            if ((_touch1Button.action.IsPressed() && _touch2Button.action.IsPressed()) || Mathf.Abs(scrollValue) > 0f || _isMiddleMousePressed)
            {
                if (_touchCount != 2)
                {
                    _initialCameraPosition = this.transform.localPosition;
                    _initialCameraRotation = this.transform.rotation;
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

                if (_cameraMode == 2 || (_cameraMode == 0 && Mathf.Abs(dot) < _rotationGestureDiscerningSensitivity && touch1Distance > _deadZoneDistance))
                {
                    HandleRotation(touch2Pos, touch1Pos, middleMouseButtonDeltaX);
                }
                else if (_cameraMode == 1 || (_cameraMode == 0 &&  (Mathf.Abs(scrollValue) > 0 || (Mathf.Abs(dot) >= _rotationGestureDiscerningSensitivity && touch1Distance > _deadZoneDistance))))
                {
                    HandleZoom(scrollValue, distanceDelta);
                }
            }
            else if (_touch1Button.action.IsPressed() && _cameraMode == 0)
            {
                HandleMove(touch1Pos);
            }
            else
            {
                _touchCount = 0;
                _cameraMode = 0;
            }
            //Debug.Log($"Touch count: {_touchCount}, Touch1: {Touch1Button.action.IsPressed()}, Touch2: {Touch2Button.action.IsPressed()}" );
        }

        private void HandleRotation(Vector2 touch2Pos, Vector2 touch1Pos, float middleMouseDeltaX = 0f)
        {
            _cameraMode = 2; // Rotate
            float angle = Vector2.SignedAngle(touch2Pos - touch1Pos, _touch2StartPos - _touch1StartPos);
            angle += middleMouseDeltaX * _rotationSensitivityDesktop;
            this.transform.rotation = _initialCameraRotation * Quaternion.Euler(0f, 0f, angle);
            OnCameraZRotationChanged?.Invoke(this.transform.rotation.eulerAngles.z);
        }

        private void HandleMove(Vector2 touch1Pos)
        {
            if (_touchCount != 1)
            {
                _initialCameraPosition = this.transform.position;
                _touch1StartPos = touch1Pos;
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
                sensitivity = _zoomSensitivityDesktop;
            }
            else
            {
                zoomInput = distanceDelta;
                sensitivity = _zoomSensitivityMobile;
            }
            
            float logScale = Mathf.Log(_initialOrthographicSize + 1f); // smaller orthographic sizes -> smaller effect
            float sizeDelta = zoomInput * sensitivity * logScale;
                
            //set camera orthographic size
            float newSize = _initialOrthographicSize - sizeDelta;
            newSize = Mathf.Clamp(newSize, _orthographicSizeMin, _orthographicSizeMax);
            _cinemachineCamera.Lens.OrthographicSize = newSize;
        }
        
        private void RealignRotationInternal()
        {
            this.transform.DORotate(Vector3.zero, _rotationRealignDuration).SetEase(_rotationRealignEase)
                .OnUpdate(() =>
                {
                    OnCameraZRotationChanged?.Invoke(this.transform.rotation.eulerAngles.z);
                });
        }

        public static void RealignRotation()
        {
            instance.RealignRotationInternal();
        }

        private void ZoomInInternal()
        {
            _cameraMode = 0;
            HandleZoom(1f, 0f);
        }

        public static void ZoomIn()
        {
            instance.ZoomInInternal();
        }
        
        private void ZoomOutInternal()
        {
            _cameraMode = 0;
            HandleZoom(-1f, 0f);
        }

        public static void ZoomOut()
        {
            instance.ZoomOutInternal();
        }
        

        public enum Mode
        {
            None = 0,
            Zoom = 1,
            Rotate = 2
        }

        public static void GoToLocation(Vector3 locationDataCameraPosition, float locationDataCameraSize)
        {
            instance.GoToLocationInternal(locationDataCameraPosition, locationDataCameraSize);
        }

        private void GoToLocationInternal(Vector3 locationDataCameraPosition, float locationDataCameraSize)
        {
            _cameraMode = 0;
            Vector3 projectedPosition;
            if (Physics.Raycast(this.transform.position, this.transform.forward, out RaycastHit hit, 10000f))
            {
                projectedPosition = hit.point;
                Vector3 relativePosition = locationDataCameraPosition - projectedPosition;
                locationDataCameraPosition = this.transform.position + relativePosition;
                
                //tween to location position
                this.transform.DOMove(locationDataCameraPosition, _goToLocationDuration)
                    .SetEase(_goToLocationEase);
                //tween to location orthographic size
                DOVirtual.Float(_cinemachineCamera.Lens.OrthographicSize, locationDataCameraSize, _goToLocationDuration,
                    (value) => { _cinemachineCamera.Lens.OrthographicSize = value; }).SetEase(_goToLocationEase);
                RealignRotationInternal();
            }
        }
        
        private void GoToClosestPointerInternal(List<PointerController> pointers)
        {
            Vector3 cameraProjectedPosition;
            if (Physics.Raycast(this.transform.position, this.transform.forward, out RaycastHit hit, 10000f))
            {
                cameraProjectedPosition = hit.point;
                var closestPointer = pointers[0];
                float closestDistance = Vector3.Distance(cameraProjectedPosition, closestPointer.transform.position);
                for (int i = 1; i < pointers.Count; i++)
                {
                    float distance = Vector3.Distance(cameraProjectedPosition, pointers[i].transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestPointer = pointers[i];
                    }
                }
                GoToPointerInternal(closestPointer);
            }
        }

        public static void GoToClosestPointer(List<PointerController> pointers)
        {
            if (pointers == null || pointers.Count == 0)
            {
                return;
            }
            instance.GoToClosestPointerInternal(pointers);
        }
        
        private void GoToPointerInternal(PointerController pointer)
        {
            GoToLocation(pointer.GetCameraPosition(), _goToPointerSize);
        }

        public static void GoToPointer(PointerController pointer)
        {
            instance.GoToPointerInternal(pointer);
        }
    }
}
