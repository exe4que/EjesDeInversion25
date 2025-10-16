using UnityEngine;
using DG.Tweening;

public class CarouselDotBehaviour : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private GameObject _fill;
    [Header("Animation")]
    [SerializeField] private Ease _ease = Ease.OutBack;
    [SerializeField] private float _duration = 0.2f;
    
    public void Select(bool value)
    {
        _fill.transform.DOScale(value ? Vector3.one : Vector3.zero, _duration)
            .SetEase(_ease);
    }
}
