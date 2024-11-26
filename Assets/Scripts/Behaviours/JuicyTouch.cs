using DG.Tweening;
using UnityEngine;

public class JuicyTouch : MonoBehaviour
{
    [SerializeField] private Transform _transformRenderer;

    public void Shake()
    {
        _transformRenderer.DOKill();
        _transformRenderer.localRotation = Quaternion.identity;
        _transformRenderer.DOShakeRotation(1f, 50f);
    }
}
