using Com.UnBocal.Rush.Properties;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.UnBocal.Rush.Managers
{
    public class CameraManager : MonoBehaviour
    {

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Unity
        private void Awake()
        {
            ConnectEvent();
        }

        private void Start()
        {
            SetCameraMainMenu();
        }

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Initialization
        private void ConnectEvent()
        {
            Game.Events.LaunchLevel.AddListener(SetLevelCamera);
        }

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Cameras
        private void SetLevelCamera()
        {
            ChangeBehaviour<CameraRotate>();
        }

        private void SetCameraMainMenu()
        {
            ChangeBehaviour<CameraMouseRotation>();
        }

        private void ChangeBehaviour<Behaviour>()
        {
            foreach(CameraBehaviour pBehaviour in GetComponents<CameraBehaviour>())
            {
                pBehaviour.SetOn(pBehaviour is Behaviour);
            }
        }

    }
}