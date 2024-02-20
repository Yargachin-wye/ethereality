using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Harpoon : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private GameObject _arrowPrefab;
    [SerializeField] private Transform _arrowPosition;
    [SerializeField] private LayerMask _layerMaskObjects;
    [SerializeField] private float _speed;
    [SerializeField] private float _timeCd = 0.1f;
    [SerializeField] private float _rotationSpeed = 1;
    [SerializeField] private float _dashForce = 1;
    [SerializeField] private float _dashTimer = 1;
    private bool timeOut;
    
    private Rigidbody2D rigidbody;

    public Vector3 targetPosition;
    public bool rotating = false;
    public static Harpoon instance;
    public bool timeOutDash = false;

    private void Awake()
    {
        _animator.SetBool("isOpen", false);
        if (instance != null)
        {
            Debug.LogError("Harpoon.instance != null");
            return;
        }
        instance = this;
        rigidbody = GetComponent<Rigidbody2D>();
    }
    public void ShotToClosestPart(Vector2 vector)
    {
        if (timeOut)
        {
            return;
        }
        ShotToVector(vector);
    }
    public void ShotToVector(Vector3 vector)
    {
        if (timeOut)
        {
            return;
        }
        StartCoroutine(Shot(vector));
    }
    private IEnumerator Shot(Vector3 pos)
    {
        timeOut = true;

        Vector3 difference = pos - transform.position;
        difference.Normalize();
        float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, rotZ - 90);

        while (!CheckRotationEquality(transform.rotation, targetRotation, 0.1f))
        {
            difference = pos - transform.position;
            difference.Normalize();
            rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
            targetRotation = Quaternion.Euler(0f, 0f, rotZ - 90);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _rotationSpeed * Time.fixedDeltaTime);

            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }

        GameObject arrow = Instantiate(_arrowPrefab, _arrowPosition.transform.position, transform.rotation);
        arrow.GetComponent<Arrow>().Shot(transform);

        StartCoroutine(TimeOut());
    }
    void FixedUpdate()
    {
        if (rotating)
        {
            Vector3 difference = targetPosition - transform.position;
            difference.Normalize();
            float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, rotZ - 90);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _rotationSpeed * Time.fixedDeltaTime);
        }
    }

    public void OpenJaw()
    {
        _animator.SetBool("isOpen", true);
        rigidbody.velocity = Vector2.zero;
    }
    public void Dash()
    {
        StartCoroutine(DashTimeOut());
        rigidbody.AddForce(transform.up.normalized * _dashForce);
    }
    private bool CheckRotationEquality(Quaternion a, Quaternion b, float angleTolerance)
    {
        // Вычисляем угловую разницу между кватернионами
        float angle = Quaternion.Angle(a, b);

        // Если угловая разница меньше допустимой погрешности, возвращаем true
        return angle < angleTolerance;
    }
    IEnumerator DashTimeOut()
    {
        timeOutDash = true;
        yield return new WaitForSeconds(_dashTimer);
        _animator.SetBool("isOpen", false);
        timeOutDash = false;
    }
    IEnumerator TimeOut()
    {
        timeOut = true;
        yield return new WaitForSeconds(_timeCd);
        timeOut = false;
    }
}
