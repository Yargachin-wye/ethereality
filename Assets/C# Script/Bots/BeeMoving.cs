using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class BeeMoving : MonoBehaviour
{
    [SerializeField] private float minTimer = 5;
    [SerializeField] private float maxTimer = 25;

    [SerializeField] private float minSpeed = 5;
    [SerializeField] private float maxSpeed = 100;

    [SerializeField] private float minRotationSpeed = 1;
    [SerializeField] private float maxRotationSpeed = 100;


    private Rigidbody2D rb2d;
    private float speed;
    private float rotationSpeed = 1;
    private float dir = 1;
    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        StartCoroutine(Timer());
    }
    private void FixedUpdate()
    {
        Vector3 difference = dir * transform.right;
        difference.Normalize();
        float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, rotZ - 90);

        rb2d.MoveRotation(Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));
        rb2d.velocity = transform.up * speed;
    }
    public void Dead()
    {
        gameObject.SetActive(false);
    }
    IEnumerator Timer()
    {
        float timer = Random.Range(minTimer, maxTimer);
        speed = Random.Range(minSpeed, maxSpeed);
        rotationSpeed = Random.Range(minRotationSpeed, maxRotationSpeed);
        if (Random.Range(0,2) < 1)
        {
            dir = 1;
        }
        else
        {
            dir = -1;
        }

        yield return new WaitForSeconds(timer);

        StartCoroutine(Timer());
    }
}

