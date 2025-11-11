using UnityEngine;

[ExecuteAlways]
public class ConstantScreenSizeWithText : MonoBehaviour
{
    [Header("Camera Settings")]
    [Tooltip("Si está vacío, usará Camera.main")]
    public Camera targetCamera;

    [Header("Escala")]
    [Tooltip("Distancia de referencia (en unidades del mundo) donde el objeto mantiene su escala base")]
    public float referenceDistance = 10f;

    [Tooltip("Factor de escala base en la distancia de referencia")]
    public float referenceScale = 1f;

    [Header("Orientación del texto")]
    [Tooltip("Objeto de texto que debe rotar hacia la cámara (por ejemplo, hijo del prefab)")]
    public Transform textObject;

    [Tooltip("Mantiene el texto vertical respecto al mundo en lugar de mirar completamente a la cámara")]
    public bool keepUpright = true;

    [Tooltip("Si está activado, todo el prefab rotará para mirar a la cámara (billboard completo)")]
    public bool billboard = false;

    private Vector3 initialLocalScale;
    private float referenceFOV;
    private float referenceOrthoSize;

    void Start()
    {
        if (targetCamera == null) targetCamera = Camera.main;
        initialLocalScale = transform.localScale;

        if (targetCamera != null)
        {
            referenceFOV = targetCamera.fieldOfView;
            referenceOrthoSize = targetCamera.orthographicSize;
        }
    }

    void Update()
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
            if (targetCamera == null) return;
        }

        // Calcular la escala constante
        float distance = Vector3.Distance(transform.position, targetCamera.transform.position);
        float scaleFactor = 1f;

        if (targetCamera.orthographic)
        {
            if (Mathf.Approximately(referenceOrthoSize, 0f))
                referenceOrthoSize = targetCamera.orthographicSize;

            scaleFactor = referenceScale * (targetCamera.orthographicSize / referenceOrthoSize);
        }
        else
        {
            float fovRad = targetCamera.fieldOfView * Mathf.Deg2Rad;
            float refFovRad = referenceFOV * Mathf.Deg2Rad;
            float tanHalfFov = Mathf.Tan(fovRad * 0.5f);
            float tanHalfRefFov = Mathf.Tan(refFovRad * 0.5f);

            if (Mathf.Approximately(referenceDistance, 0f)) referenceDistance = distance;

            scaleFactor = referenceScale * (distance * tanHalfFov) / (referenceDistance * tanHalfRefFov);
        }

        transform.localScale = initialLocalScale * scaleFactor;

        // Billboard general
        if (billboard)
        {
            transform.rotation = Quaternion.LookRotation(transform.position - targetCamera.transform.position);
        }

        // Rotar solo el texto (si se asignó uno)
        if (textObject != null)
        {
            Vector3 camForward = targetCamera.transform.forward;
            Vector3 camUp = targetCamera.transform.up;

            if (keepUpright)
            {
                // Mantiene el texto recto (no se tumba)
                Vector3 lookDir = textObject.position - targetCamera.transform.position;
                lookDir.y = 0; // eliminamos inclinación vertical
                if (lookDir.sqrMagnitude < 0.001f) lookDir = targetCamera.transform.forward;

                textObject.rotation = Quaternion.LookRotation(lookDir, Vector3.up);
            }
            else
            {
                // Mira completamente hacia la cámara
                textObject.rotation = Quaternion.LookRotation(textObject.position - targetCamera.transform.position, camUp);
            }
        }
    }

    void OnValidate()
    {
        if (targetCamera == null) targetCamera = Camera.main;
        if (targetCamera != null)
        {
            if (referenceFOV == 0f) referenceFOV = targetCamera.fieldOfView;
            if (referenceOrthoSize == 0f) referenceOrthoSize = targetCamera.orthographicSize;
        }
    }
}
