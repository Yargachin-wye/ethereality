using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pole : MonoBehaviour
{
    private Transform mainCamera;
    private void Start()
    {
        mainCamera = Camera.main.transform;
    }
    private void Update()
    {
        transform.position = new Vector2(transform.position.x, mainCamera.position.y);
    }
}
