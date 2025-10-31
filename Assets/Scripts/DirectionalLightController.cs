using UnityEngine;

namespace EjesDeInversion
{
    public class DirectionalLightController : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float _noiseAmplitude = 10f;
        [SerializeField] private float _noiseFrequency = 1f;
        
        private Quaternion _initialRotation;
        
        private void Start()
        {
            _initialRotation = transform.rotation;
        }
        
        private void Update()
        {
            float noiseX = Mathf.PerlinNoise(Time.time * _noiseFrequency, 0f) - 0.5f;
            float noiseY = Mathf.PerlinNoise(0f, Time.time * _noiseFrequency) - 0.5f;
            float noiseZ = Mathf.PerlinNoise(Time.time * _noiseFrequency, Time.time * _noiseFrequency) - 0.5f;

            Vector3 noiseEuler = new Vector3(noiseX, noiseY, noiseZ) * _noiseAmplitude;
            Quaternion noiseRotation = Quaternion.Euler(noiseEuler);

            transform.rotation = _initialRotation * noiseRotation;
        }
        
    }
}