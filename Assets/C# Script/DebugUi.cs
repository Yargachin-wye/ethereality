using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DebugUi : MonoBehaviour
{
    public static DebugUi instance;
    [SerializeField] private TextMeshProUGUI _text;
    private void Awake()
    {
        instance = this;
    }
    public void LogText(string text)
    {
        _text.text = text;
    }
}
