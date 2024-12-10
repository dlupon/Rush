using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    public void Switch()
    {
        CameraBehaviour[] lAllCameraBehaviour = GetComponents<CameraBehaviour>();
        foreach (CameraBehaviour lBehaviour in lAllCameraBehaviour)
            lBehaviour.SetOn(lBehaviour == this);
    }

    public void SetOn(bool pOn)
    {
        enabled = pOn;
    }
}
