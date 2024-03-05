using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class Slider : MonoBehaviour
{
    [SerializeField] private float _speed = 150;
    [SerializeField] private Animator _animator;
    private Rigidbody2D _rb2d;
    private Vector2 velocity = Vector2.zero;
    private float startX = 0;
    void Start()
    {
        _rb2d = GetComponent<Rigidbody2D>();
        _animator.SetBool("slide", true);
        startX = transform.position.x;
        velocity = new Vector2(0, _speed);
    }

    private void FixedUpdate()
    {
        if(transform.position.x > startX+0.02 || transform.position.x < startX - 0.02)
        {
            transform.position = new Vector2(startX, transform.position.y);
        }
        _rb2d.velocity = velocity * Time.fixedDeltaTime;
    }
}
