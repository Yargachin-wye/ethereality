using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GenerateWorld();
        }
    }
    void GenerateWorld()
    {
        Debug.Log("Клавиша Space нажата!");
    }
}
