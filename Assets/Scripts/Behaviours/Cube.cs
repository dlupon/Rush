using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Cube : MonoBehaviour
{
    [SerializeField, Range(0.1f, 10f)] private float _Speed = 1f;
    [SerializeField] private string _GroundTag = "Tile";
    [SerializeField] private string _ArrowTag = "Arrow";

    private float _ElapsedTime = 0f;
    private float _TicksDuration = 1f;

    private float _Ratio = 0;

    private Action DoAction;


    private Vector3 _FromPosition = default;
    private Vector3 _ToPosition = default;
    private Quaternion _FromRotation = default;
    private Quaternion _ToRotation = default;

    private Vector3 _MovementDirection;
    private Quaternion _MovementRotation;

    private float _RotationOffsetY;
    private float _CubeSide = 1f;
    private float _CubeDiagonal;


    [SerializeField] private float _RayOffsetOutsideCube = 0.4f;
    private float _RaycastDistance = 0f;
    private Vector3 _Down;
    private RaycastHit _Hit;


    private void Start()
    {
        // Raycast 
        _RaycastDistance = _CubeSide / 2 + _RayOffsetOutsideCube;

        //Cube Elevation
        _CubeDiagonal = Mathf.Sqrt(2) * _CubeSide;
        _RotationOffsetY = _CubeDiagonal / 2 - _CubeSide / 2;

        // Cube movement prediction
        _MovementDirection = transform.forward;
        _MovementRotation = Quaternion.AngleAxis(90f, transform.right);
        //SetModeVoid();

        SetModeMove();
    }

    void Update()
    {
        Tick();
        DoAction();
    }

    private void Tick()
    {
        if (_ElapsedTime > _TicksDuration)
        {
            Debug.Log("Tick");

            //SetModeMove();
            CheckCollision();
            _ElapsedTime = 0;
        }

        _ElapsedTime += Time.deltaTime * _Speed;
        _Ratio = _ElapsedTime / _TicksDuration;
    }

    private void CheckCollision()
    {
        _Down = Vector3.down;

        if (Physics.Raycast(transform.position, _Down, out _Hit, _RaycastDistance))
        {
            Debug.DrawRay(transform.position, _Down * _RaycastDistance, Color.green);

            GameObject lCollided = _Hit.collider.gameObject;

            Debug.Log(lCollided);

            if (lCollided.CompareTag(_GroundTag))
            {
                SetModeMove();
            }

            else if (lCollided.CompareTag(_ArrowTag))
            {
                SetDirection(lCollided.transform.forward);
                SetModeMove();
            }
        }
        else
        {
            SetModeFall();
        }
    }

    private void SetDirection(Vector3 pDirection)
    {
        _MovementDirection = pDirection;
        _MovementRotation = Quaternion.AngleAxis(90f, Vector3.Cross(Vector3.up, _MovementDirection));
    }

    #region StateMachine
    private void SetModeVoid()
    {
        DoAction = DoActionVoid;
    }

    private void DoActionVoid() { }

    private void SetModeMove()
    {
        Debug.Log("Move");
        InitNextMovement();
        DoAction = DoActionMove;
    }

    private void InitNextMovement()
    {
        _FromPosition = transform.position;
        _FromRotation = transform.rotation;

        _ToPosition = _FromPosition + _MovementDirection;
        _ToRotation = _MovementRotation * _FromRotation;
    }
    private void DoActionMove()
    {
        // transform.rotation = Quaternion.Lerp(_FromRotation, _ToRotation, _Ratio);
        transform.position = Vector3.Lerp(_FromPosition, _ToPosition, _Ratio)
            + (Vector3.up * (_RotationOffsetY * Mathf.Abs(Mathf.Sin(Mathf.PI * _Ratio))));
    }

    private void InitFallingMovement()
    {
        _FromPosition = transform.position;
        _ToPosition = _FromPosition + Vector3.down;
    }

    private void SetModeFall()
    {
        Debug.Log("fall");
        InitFallingMovement();
        DoAction = DoActionFall;
    }

    private void DoActionFall()
    {
        transform.position = Vector3.Lerp(_FromPosition, _ToPosition, _Ratio);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(_FromPosition, 0.2f);
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(_ToPosition, 0.2f);
    }

    #endregion
}