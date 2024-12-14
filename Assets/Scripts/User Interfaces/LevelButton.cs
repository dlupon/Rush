using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;
using UnityEngine.EventSystems;
using Com.UnBocal.Rush.Properties;

public class LevelButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // Events
    [HideInInspector] public UnityEvent<int> Click = new UnityEvent<int>();

    // Components
    private Button _button;
    private RectTransform _rectTransform;
    private RawImage _rawImage;

    // Render
    [SerializeField] private Vector2Int _size = Vector2Int.one * 100;
    private RenderTexture _renderTexture;

    // Camera
    [SerializeField] private Camera _camera;
    [SerializeField] private float _cameraDistance = 5f;
    private Transform _mainCamera;

    // Button
    private bool _hover = false;

    // Level
    [SerializeField] private Text _name;
    private int _levelIndex;

    // Folder
    [SerializeField] private Transform _folder;
    private Vector3 _defaultPosition;
    private Quaternion _defaultRotation;
    private float _folderX = 0;

    // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Unity
    private void Awake()
    {
        SetComponents();
        ConnectEvents();
        SetRenderTexture();
    }

    private void Start()
    {
        SetFolderProperties();
    }

    private void Update()
    {
        UpdateFolder();
    }

    public void OnPointerEnter(PointerEventData pPointer)
    {
        FolderOverAnimation();
        _hover = true;
    }

    public void OnPointerExit(PointerEventData pPointer)
    {
        ResetFolder();
        _hover = false;
    }

    // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Initialization
    private void SetComponents()
    {
        _rectTransform = GetComponent<RectTransform>();
        _rawImage = GetComponent<RawImage>();
        _button = GetComponent<Button>();
        _mainCamera = Camera.main.transform;
    }

    private void ConnectEvents()
    {
        _button.onClick.AddListener(OnClick);
    }

    private void SetFolderProperties()
    {
        _defaultPosition = _folder.localPosition;
        _defaultRotation = _folder.rotation;
    }

    public void SetLevel(GameObject pLevel)
    {
        _levelIndex = Game.Properties.Levels.IndexOf(pLevel);
        _name.text = pLevel.name;
    }

    // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Button
    private void OnClick()
    {
        Select();
    }

    public void Select()
    {
        Click.Invoke(_levelIndex);
        Game.Events.LevelLoad.Invoke(_levelIndex);
        FolderClickAnimation();
    }

    // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Rendere
    #region Renderer
    private void SetRenderTexture()
    {
        _renderTexture = new RenderTexture(_size.x, _size.y, 16);
        _renderTexture.filterMode = FilterMode.Point;
        _camera.targetTexture = _renderTexture;
        _rawImage.texture = _renderTexture;
    }

    private void SetLayer(Transform pTarget)
    {
        pTarget.gameObject.layer = gameObject.layer;
        if (pTarget.childCount == 0) return;
        int lChildCout = pTarget.childCount;

        for (int lChildIndex = 0; lChildIndex < lChildCout; lChildIndex++)
            SetLayer(pTarget.GetChild(lChildIndex).transform);
    }
    #endregion

    // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Folder
    private void UpdateFolder()
    {
        if (_hover) FolderRotate();
    }

    private void FolderRotate()
    {
        _folderX += Time.deltaTime;
        _folder.rotation = Quaternion.AngleAxis(Mathf.Sin(_folderX) * 45f, Vector3.up) * _defaultRotation;
    }

    private void ResetFolder()
    {
        _folderX = 0;
        _folder.DORotate(_defaultRotation.eulerAngles, 1f, RotateMode.Fast).SetEase(Ease.OutElastic);
    }

    private void FolderOverAnimation()
    {
        _folder.DOKill();
        _folder.DOScale(Vector3.one, 1f).From(new Vector3(.7f, 1.3f, .7f)).SetEase(Ease.OutElastic);
    }
 
    private void FolderClickAnimation()
    {
        _folder.DOScale(Vector3.one, 1f).From(Vector3.one * .87f).SetEase(Ease.OutElastic);
        _folder.DOLocalJump(_defaultPosition, .25f, 1, .5f).SetEase(Ease.OutBounce);
    }

}