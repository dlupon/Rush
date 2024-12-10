using UnityEngine;
using Com.UnBocal.Rush.Properties;
using UnityEngine.Video;

public class Interface : MonoBehaviour
{
    // Components
    protected RectTransform m_rectTransform;

    // Input
    protected bool m_inputReactive = true;

    // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Unity
    protected virtual void Awake()
    {
        SetComponents();
        ConnectEvents();
    }

    // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Initializaion
    protected virtual void SetComponents()
    {
        m_rectTransform = GetComponent<RectTransform>();
    }

    protected virtual void ConnectEvents()
    {

    }

    // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Destroy
    protected virtual void DisconnectEvents()
    {

    }

    protected virtual void Disable()
    {
        m_inputReactive = false;
    }

    protected virtual void Delete()
    {
        DisconnectEvents();
        Destroy(gameObject);
    }
}