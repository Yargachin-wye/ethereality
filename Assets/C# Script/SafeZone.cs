using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeZone : MonoBehaviour
{
    [SerializeField] private float _spead;
    float scale;
    private void Start()
    {
        scale = transform.localScale.x;
    }
    private void FixedUpdate()
    {
        scale -= _spead * Time.fixedDeltaTime;
        transform.localScale = new Vector2(scale, scale);
    }
}
