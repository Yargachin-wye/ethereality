using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Time_Menager : MonoBehaviour
{
    [SerializeField] private float slouTimeScale = 1;
    bool normTime = true;
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.LeftShift) && normTime)
        {
            if (normTime)
                Time.timeScale = slouTimeScale;
            else
                Time.timeScale = 1;
        }
    }
}
