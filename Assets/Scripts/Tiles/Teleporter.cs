using UnityEngine;

public class Teleporter : MonoBehaviour
{
    [SerializeField] private Transform _nextPosition;
    public Vector3 TeleportPosition => _nextPosition.position;
}