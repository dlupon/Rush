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

    public bool SetOn(bool pOn)
    {
        if (pOn) OnActive();
        else OnUnActive();
        return enabled = pOn;
    }

    protected virtual void OnActive()
    {

    }

    protected virtual void OnUnActive()
    {

    }
}
