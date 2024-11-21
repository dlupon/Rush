using UnityEngine;

public class Switch : MonoBehaviour
{
    public float Orientation { get => SwitchOrientation(); }
    [SerializeField] private float _orintantation = 1;

    private float SwitchOrientation()
    {
        _orintantation *= -1;
        return _orintantation * -1f;
    }
}