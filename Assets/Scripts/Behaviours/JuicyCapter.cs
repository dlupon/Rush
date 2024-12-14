using Com.UnBocal.Rush.Properties;
using DG.Tweening;
using UnityEngine;

public class JuicyCapter : MonoBehaviour
{
    [SerializeField] private Transform _transformRenderer;
    private Vector3 _defaultPosition;
    private Vector3 _defaultScale;
    private Quaternion _defaultRotation;

    // ----------------~~~~~~~~~~~~~~~~~~~~==========================# // Unity
    private void Awake()
    {
        SetDefaultProperties();
    }

    // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Initialization
    private void SetDefaultProperties()
    {
        _defaultPosition = _transformRenderer.localPosition;
        _defaultScale = _transformRenderer.localScale;
        _defaultRotation = _transformRenderer.localRotation;
    }

    // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Properties
    private void Reset()
    {
        if (DOTween.IsTweening(_transformRenderer)) return;
        float lDuration = .25f;
        Ease lEase = Ease.OutBack;
        _transformRenderer.DOLocalMove(_defaultPosition, lDuration).SetEase(lEase);
        _transformRenderer.DOScale(_defaultScale, lDuration).SetEase(lEase);
        _transformRenderer.DOLocalRotateQuaternion(_defaultRotation, lDuration).SetEase(lEase);
    }

    // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Spawn
    public void Spawn(float pDuration, float pDelay, Vector3 pFrom)
    {
        pDelay += Random.value * .6f;

        _transformRenderer.DOMoveZ(_transformRenderer.position.z, pDuration).From(pFrom).SetEase(Ease.OutExpo).SetDelay(pDelay);
        _transformRenderer.DOMoveX(_transformRenderer.position.x, pDuration).From(pFrom).SetEase(Ease.OutExpo).SetDelay(pDelay);
        _transformRenderer.DOMoveY(_transformRenderer.position.y, pDuration * 2f).From(pFrom).SetEase(Ease.OutElastic).SetDelay(pDelay);
        _transformRenderer.DOScale(pDuration, 1f).From(Vector3.zero).SetEase(Ease.OutElastic).SetDelay(pDelay);
        _transformRenderer.DOShakeRotation(pDuration, 10f).SetEase(Ease.OutExpo).SetDelay(pDelay + .5f);

        return;
    }

    // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Tile Placing
    public void Placed()
    {
        Vector3 lStart = new Vector3(1.5f, .5f, 1.5f);
        Vector3 lEnd = Vector3.one;
        _transformRenderer.DOScale(lEnd, 1f).From(lStart).SetEase(Ease.OutBack);
    }
    
    public void Wave(float pDuration, float pDelay)
    {
        _transformRenderer.DOShakeScale(pDuration, .1f).SetEase(Ease.OutExpo).SetDelay(pDelay );
        _transformRenderer.DOShakeRotation(pDuration, 5f).SetEase(Ease.OutExpo).SetDelay(pDelay ).onComplete = Reset;
    }

    public void Corrupt(float pDuration, float pDelay)
    {
        pDelay += - .25f + .5f * Random.value;

        _transformRenderer.DOShakeScale(pDuration, .5f).SetEase(Ease.OutExpo).SetDelay(pDelay);
        _transformRenderer.DOShakeRotation(pDuration, 10f).SetEase(Ease.OutExpo).SetDelay(pDelay).onComplete = Reset;
    }

    // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Motion

    // ----------------~~~~~~~~~~~~~~~~~~~==========================# // 

    public void Shake(float pRatio = 0f)
    {
        _transformRenderer.DOKill();
        _transformRenderer.localRotation = Quaternion.identity;
        _transformRenderer.localPosition = Vector3.zero;
        _transformRenderer.localScale = Vector3.one;
        // _transformRenderer.DOShakeRotation(1f, 50f).SetEase(Ease.OutExpo);
        // _transformRenderer.DOShakePosition(1f, .5f).SetEase(Ease.OutExpo);
        _transformRenderer.DOShakeScale(1f, .5f).SetEase(Ease.OutExpo).SetDelay(pRatio * 1f);
    }

    public void StartShake(int pIteration) => Shake(pIteration, pIteration);

    public void Shake(int pIteration, int pBaseIteration = default)
    {
        if (pBaseIteration == default) pBaseIteration = pIteration;
        if (--pIteration < 0) return;
        if (DOTween.IsTweening(_transformRenderer)) return;

        Shake(1f - (float)pIteration / (float)(pBaseIteration - 1));

        Vector3 lDirection = transform.right;
        float lDistance = 1.25f;
        for (int i = 0; i < 4; i++)
        {
            lDirection = Quaternion.AngleAxis(Game.Properties.ROTATION, transform.up) * lDirection;
            TryShake(pIteration, pBaseIteration, lDirection, lDistance);
        }
        
        lDirection = transform.up;
        TryShake(pIteration, pBaseIteration, lDirection, lDistance);

        lDirection = -transform.up;
        TryShake(pIteration, pBaseIteration, lDirection, lDistance);

    }

    private void TryShake(int pIteration, int pBaseIteration, Vector3 pDirection, float pDistance)
    {
        Debug.DrawRay(transform.position, pDirection, Color.cyan, 1f);
        if (!Physics.Raycast(transform.position, pDirection, out RaycastHit lHit, pDistance)
            || !lHit.collider.TryGetComponent(out JuicyCapter JT)) return;

        JT.StartShake(pIteration - 1);
    }
}
