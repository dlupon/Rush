using TMPro;
using UnityEngine;

namespace Com.UnBocal.Rush.Debugs
{
    public class DebugStateMachin : MonoBehaviour
    {
        [SerializeField] private TextMeshPro _TMPro;

        public void SetText(string ptext = "") => _TMPro.text = ptext;
    }
}