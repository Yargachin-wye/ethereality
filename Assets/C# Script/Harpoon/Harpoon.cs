using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Harpoon : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private List<Animator> _animatorsEyes;
    [SerializeField] private GameObject _arrowPrefab;
    [SerializeField] private Transform _arrowPosition;
    [SerializeField] private LayerMask _layerMaskTargets;
    [SerializeField] private float _speed;
    [SerializeField] private float _timeCd = 0.1f;
    [SerializeField] private float _rotationSpeed = 1;
    [SerializeField] private float _dashForce = 1;
    [SerializeField] private float _dashTimer = 1;
    [SerializeField] private float _dashCD = 1;
    
    [SerializeField] private int _dashDMG = 2;
    private bool timeOut;
    private Rigidbody2D rb2d;

    public Vector3 targetDirection;
    public bool rotating = false;
    public static Harpoon instance;
    public bool timeOutDash = false;
    public bool dashCD = false;
    public bool hangUp = false;
    public UnityEvent shot;

    private void Awake()
    {
        _animator.SetBool("mouthIsOpen", false);
        if (instance != null)
        {
            Debug.LogError("Harpoon.instance != null");
            return;
        }
        instance = this;
        rb2d = GetComponent<Rigidbody2D>();
    }
    public void ShotToClosestPart(Vector2 vector)
    {
        if (timeOut || timeOutDash)
        {
            return;
        }
        ShotToVector(vector);
    }
    public void ShotToVector(Vector3 vector)
    {
        if (timeOut || timeOutDash)
        {
            return;
        }
        StartCoroutine(Shot(vector));
    }
    private IEnumerator Shot(Vector3 pos)
    {
        shot?.Invoke();
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
    private void FixedUpdate()
    {
        if (hangUp)
        {
            rb2d.velocity = Vector2.zero;
        }
        if (rotating)
        {
            targetDirection.Normalize();
            float rotZ = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, rotZ - 90);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _rotationSpeed * Time.fixedDeltaTime);
        }
    }
    public void RepulsionFromPos(Vector3 Pos)
    {
        Vector3 dir = (transform.position - Pos).normalized;
        float force = Vector3.Distance(Pos, transform.position);
        rb2d.AddForce(dir * force * 8);
    }
    public void OpenJaw()
    {
        _animator.SetBool("mouthIsOpen", true);
        foreach (var anim in _animatorsEyes)
        {
            anim.SetBool("mouthIsOpen", true);
        }
        hangUp = true;
        
    }
    public void Dash()
    {
        StartCoroutine(DashTimeOut());
    }
    private bool CheckRotationEquality(Quaternion a, Quaternion b, float angleTolerance)
    {
        // Вычисляем угловую разницу между кватернионами
        float angle = Quaternion.Angle(a, b);

        // Если угловая разница меньше допустимой погрешности, возвращаем true
        return angle < angleTolerance;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!timeOutDash)
        {
            return;
        }
        int currentLayer = collision.gameObject.layer;
        if (_layerMaskTargets == (_layerMaskTargets | (1 << currentLayer)))
        {
            if (collision.GetComponent<Capsule>() != null)
            {
                DashEnd();
                collision.GetComponent<Rigidbody2D>().AddForce(transform.up.normalized * _dashForce / 2);
                collision.GetComponent<Capsule>().TakeDamage(0.02f, _dashDMG);
                
            }
            if (collision.GetComponent<BeeMoving>() != null)
            {
                collision.GetComponent<BeeMoving>().Dead();
                Dash();
            }
        }
    }
    private IEnumerator DashTimeOut()
    {
        hangUp = false;
        timeOutDash = true;
        dashCD = true;
        rb2d.AddForce(transform.up.normalized * _dashForce);
        yield return new WaitForSeconds(_dashTimer);
        if (timeOutDash)
        {
            DashEnd();
        }
        yield return new WaitForSeconds(_dashCD);
        dashCD = false;
    }
    private void DashEnd()
    {
        _animator.SetBool("mouthIsOpen", false);
        foreach(var anim in _animatorsEyes)
        {
            anim.SetBool("mouthIsOpen", false);
        }
        timeOutDash = false;
    }
    private IEnumerator TimeOut()
    {
        timeOut = true;
        yield return new WaitForSeconds(_timeCd);
        timeOut = false;
    }
}
