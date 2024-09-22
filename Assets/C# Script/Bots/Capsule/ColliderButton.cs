using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ColliderButton : MonoBehaviour
{
    private Rigidbody2D rigidbody2D = null;
    private void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void OnMouseDown()
    {
        Controller.instance.OnColliderButtonDown();
    }
    private void OnMouseUp()
    {
        Controller.instance.OnUp(transform.position);
    }
    private void Update()
    {
        transform.position = transform.parent.position;
    }
}
