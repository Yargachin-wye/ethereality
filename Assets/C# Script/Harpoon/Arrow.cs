using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] private RopeV2 _RopeRenderer;
    [SerializeField] private GameObject _sectionPrefab;
    [SerializeField] private GameObject _firstSection;
    [SerializeField] private float _timerStop = 0.75f;
    [SerializeField] private float _timerAddForse = 0.2f;
    [SerializeField] private LayerMask _layerMaskTargets;

    [SerializeField] private float _addForceSpeed = 200;
    [SerializeField] private float _speed = 2000;


    private float sectionLength = 0;
    private GameObject lastSection = null;
    private Rigidbody2D rigidbody2D = null;
    private Transform conector = null;
    private HingeJoint2D conectorHingeJoint2D = null;
    private List<GameObject> sections = new List<GameObject>();

    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        sectionLength = _sectionPrefab.GetComponent<HingeJoint2D>().anchor.y;
        lastSection = _firstSection;
        // sections.Add(lastSection);
        conector = null;
        StartCoroutine(TimerDestroy());
    }
    private void FixedUpdate()
    {
        if (conector == null)
            return;

        if (Vector2.Distance(conector.position, lastSection.transform.position) >= sectionLength)
            AddNewSection();
        rigidbody2D.velocity = transform.up * _speed * Time.fixedDeltaTime;
    }
    private void AddNewSection()
    {
        GameObject newLastSection = Instantiate(_sectionPrefab, transform);
        newLastSection.transform.position = conector.position;
        newLastSection.GetComponent<HingeJoint2D>().anchor = newLastSection.transform.InverseTransformPoint(new Vector2(lastSection.transform.position.x, lastSection.transform.position.y));
        newLastSection.GetComponent<HingeJoint2D>().connectedBody = lastSection.GetComponent<Rigidbody2D>();
        lastSection = newLastSection;
        sections.Add(lastSection);
        _RopeRenderer.SetCount(transform.childCount);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        int currentLayer = collision.gameObject.layer;
        if (_layerMaskTargets == (_layerMaskTargets | (1 << currentLayer)) && this.enabled)
        {
            Rigidbody2D rbcl = collision.GetComponent<Rigidbody2D>();
            rigidbody2D.velocity *= 0;
            rigidbody2D.simulated = false;
            transform.GetChild(1).GetComponent<HingeJoint2D>().connectedBody = rbcl;
            rbcl.velocity *= 0;
            StartCoroutine(TimerOff(rbcl));

            _RopeRenderer.StopFly();
            transform.parent = collision.transform;

            conectorHingeJoint2D = conector.gameObject.AddComponent<HingeJoint2D>();
            conectorHingeJoint2D.anchor = conector.InverseTransformPoint(conector.transform.position);
            conectorHingeJoint2D.connectedBody = lastSection.GetComponent<Rigidbody2D>();
            conectorHingeJoint2D.autoConfigureConnectedAnchor = false;

            if (collision.GetComponent<Capsule>() != null)
            {
                collision.GetComponent<Capsule>().TakeDamage(_timerAddForse + 0.01f);
            }

            this.enabled = false;
        }
    }
    IEnumerator TimerOff(Rigidbody2D rbcl)
    {
        yield return new WaitForSeconds(_timerAddForse);

        conector.GetComponent<Rigidbody2D>().AddForce((transform.position - conector.transform.position).normalized * _addForceSpeed);
        rbcl.AddForce(-1 * (transform.position - conector.transform.position).normalized * _addForceSpeed);

        lastSection.GetComponent<Rigidbody2D>().AddForce((transform.position - conector.transform.position).normalized * _addForceSpeed / 2);
        Destroy(conectorHingeJoint2D);

        yield return new WaitForSeconds(0.5f);

        foreach (GameObject section in sections)
        {
            Destroy(section);
        }
        enabled = false;
        _RopeRenderer.OffRope();
    }
    IEnumerator TimerDestroy()
    {
        yield return new WaitForSeconds(_timerStop);
        if (this.enabled)
        {
            rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
            //lastSection.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePosition;
            _RopeRenderer.StopFly();

            this.enabled = false;
        }
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }
    public void Shot(Transform cn)
    {
        conector = cn;
        _RopeRenderer.StartFly();
    }
}
