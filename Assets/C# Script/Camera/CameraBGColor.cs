using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBGColor : MonoBehaviour
{
    [SerializeField] private Camera _cam;
    [SerializeField] private float _minNum = 0;
    [SerializeField] private float _speed = 0;
    private float[] RGB = new float[3];
    private int oldFlag;
    private int newFlag;
    private void Awake()
    {
        RGB[0] = _minNum;
        RGB[1] = _minNum;
        RGB[2] = 1;
        oldFlag = 2;
        newFlag = 0;
    }
    private void FixedUpdate()
    {
        _cam.backgroundColor = new Color(RGB[0], RGB[1], RGB[2], 1);
        if (RGB[newFlag] < 1)
        {
            RGB[newFlag] += _speed * 0.01f;
        }
        else if (RGB[oldFlag] > _minNum)
        { 
            RGB[oldFlag] -= _speed * 0.01f;
        }
        else
        {
            oldFlag = newFlag;
            newFlag = GetNextFlag(newFlag);
        }
    }
    private int GetNextFlag(int f)
    {
        switch (f)
        {
            case 0:
                return 1;
            case 1:
                return 2;
            case 2:
                return 0;
            default:
                return -1;
        }
    }
}
