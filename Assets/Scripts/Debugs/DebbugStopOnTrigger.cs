using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopOnTrigger : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        Time.timeScale = 0;
    }
}
