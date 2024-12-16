using Com.UnBocal.Rush.Properties;
using DG.Tweening;
using UnityEngine;

public class JuicyCapter : MonoBehaviour
{
    [SerializeField] private Transform _transformRenderer;
    public Transform CapterTransform => _transformRenderer;
    private Vector3 _defaultRendererPosition;
    private Vector3 _defaultRendererScale;
    private Quaternion _defaultRendererRotation;
    private Vector3 _defaultPosition;
    private Vector3 _defaultScale;
    private Quaternion _defaultRotation;

    [SerializeField] private bool _canBeForceReset = true;

    // ----------------~~~~~~~~~~~~~~~~~~~~==========================# // Unity
    private void Awake()
    {
        SetDefaultProperties();
        ConnectEvents();
    }

    // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Initialization
    private void SetDefaultProperties()
    {
        _defaultRendererPosition = _transformRenderer.localPosition;
        _defaultRendererScale = _transformRenderer.localScale;
        _defaultRendererRotation = _transformRenderer.localRotation;

        _defaultPosition = transform.localPosition;
        _defaultScale = transform.localScale;
        _defaultRotation = transform.localRotation;
    }

    private void ConnectEvents()
    {
        if (_canBeForceReset) Game.Events.StopRunning.AddListener(ForceReset);
        Game.Events.LevelLoad.AddListener(HardReset);
    }

    // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Properties
    private void HardReset(int p)
    {
        _transformRenderer.localPosition = _defaultRendererPosition;
        _transformRenderer.localScale = _defaultRendererScale;
        _transformRenderer.localRotation = _defaultRendererRotation;

        transform.localPosition = _defaultPosition ;
        transform.localScale = _defaultScale ;
        transform.localRotation = _defaultRotation;
    }
    
    private void Reset()
    {
        if (DOTween.IsTweening(_transformRenderer)) return;
        ResetTween();
    }

    private void ResetTween()
    {
        float lDuration = .25f;
        Ease lEase = Ease.OutBack;
        _transformRenderer.DOLocalMove(_defaultRendererPosition, lDuration).SetEase(lEase);
        _transformRenderer.DOScale(_defaultRendererScale, lDuration).SetEase(lEase);
        _transformRenderer.DOLocalRotateQuaternion(_defaultRendererRotation, lDuration).SetEase(lEase);
    }

    private void ForceReset()
    {
        _transformRenderer.DOKill();
        ResetTween();
    }

    // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Level
    public void Spawn(float pDuration, float pDelay, Vector3 pFrom)
    {
        pDelay += Random.value * .6f;

        _transformRenderer.DOMoveZ(_transformRenderer.position.z, pDuration).From(pFrom).SetEase(Ease.OutExpo).SetDelay(pDelay);
        _transformRenderer.DOMoveX(_transformRenderer.position.x, pDuration).From(pFrom).SetEase(Ease.OutExpo).SetDelay(pDelay);
        _transformRenderer.DOMoveY(_transformRenderer.position.y, pDuration * 2f).From(pFrom).SetEase(Ease.OutElastic).SetDelay(pDelay);
        _transformRenderer.DOScale(pDuration, 1f).From(Vector3.zero).SetEase(Ease.OutElastic).SetDelay(pDelay);
        _transformRenderer.DOShakeRotation(pDuration, 10f).SetEase(Ease.OutExpo).SetDelay(pDelay + .5f);
    }

    public void End(float pDuration, float pDelay, Vector3 pTo)
    {
        pDelay += Random.value * .6f;

        pTo += Vector3.up + Vector3.one.Randomize();

        transform.DOMoveZ(pTo.z, pDuration).SetEase(Ease.InExpo).SetDelay(pDelay);
        transform.DOMoveX(pTo.x, pDuration).SetEase(Ease.InExpo).SetDelay(pDelay);
        transform.DOMoveY(pTo.y, pDuration * 1.2f).SetEase(Ease.InExpo).SetDelay(pDelay);
        transform.DOScale(0, pDuration).SetEase(Ease.InElastic).SetDelay(pDelay + .5f);
        transform.DOShakeRotation(pDuration, 10f).SetEase(Ease.InExpo).SetDelay(pDelay + .5f);
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

    public void ShakeCubeCollision(float pDuration, float pDelay)
    {
        // _transformRenderer.DOShakeScale(pDuration, 1f).SetEase(Ease.OutExpo).SetDelay(pDelay);
        _transformRenderer.DOShakeRotation(pDuration, 30f).SetEase(Ease.OutExpo).SetDelay(pDelay);
        _transformRenderer.DOShakePosition(pDuration, .5f).SetEase(Ease.OutExpo).SetDelay(pDelay).onComplete = Reset;
    }

    // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Kill
    public void Kill()
    {
        Destroy(gameObject);
    }

    public void KillIndicator()
    {
        _transformRenderer.DOScale(Vector3.zero, 1f).SetEase(Ease.InBack);
        _transformRenderer.DOMove(_transformRenderer.position + Vector3.up, 1).SetEase(Ease.InExpo).onComplete = Kill;
    }

    public void SpawnIndicator()
    {
        _transformRenderer.DOScale(Vector3.one, 1f).From(Vector3.zero).SetEase(Ease.OutExpo);
        _transformRenderer.DOMove(_transformRenderer.position + Vector3.down * .5f, 1).From(_transformRenderer.position + Vector3.up).SetEase(Ease.OutBounce).SetLoops(9999, LoopType.Yoyo);
    }

    // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Cubes
    public void CubeGoalAnimation(GameObject pBin)
    {
        _transformRenderer.DOKill();
        transform.DOKill();

        transform.DOJump(transform.position, 1f, 1, .75f).onComplete = Kill;
        transform.DOScale(0f, .75f).SetEase(Ease.InBack);
        transform.DORotate((transform.rotation * Quaternion.AngleAxis(180f, Vector2.up)).eulerAngles, 1f).SetEase(Ease.InBack);

        if (!pBin.TryGetComponent(out JuicyCapter pBinCaper)) return;
        pBinCaper.GoalAnimation(.6f);
    }

    public void GoalAnimation(float pDelay)
    {
        _transformRenderer.DOScale(_defaultRendererScale, .5f).From(new Vector3(1.2f, .5f, 1.2f)).SetEase(Ease.OutElastic).SetDelay(pDelay);
        _transformRenderer.localScale = _defaultRendererScale;
    }

    // ----------------~~~~~~~~~~~~~~~~~~~==========================# // IDK

    public void Shake(float pRatio = 0f)
    {
        _transformRenderer.DOKill();
        _transformRenderer.localRotation = _defaultRendererRotation;
        _transformRenderer.localPosition = _defaultRendererPosition;
        _transformRenderer.localScale = _defaultRendererScale;
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
