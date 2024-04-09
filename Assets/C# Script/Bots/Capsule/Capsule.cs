using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Capsule : MonoBehaviour
{
    [SerializeField] private Sprite[] _sprites = new Sprite[4];
    [SerializeField] private GameObject _ruinedCapsulePrefab;
    [SerializeField] private ParticleSystem _particles;
    private int strength = 0;
    private SpriteRenderer spriteRenderer;
    public bool isDead = false;
    private ObjectPool objectPoolRuinedCapsule;

    private void Awake()
    {
        strength = 0;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void OnEnable()
    {

    }
    private void OnDisable()
    {

    }

    public void InitDependencies(ObjectPool objectPoolRuinedCapsule)
    {
        this.objectPoolRuinedCapsule = objectPoolRuinedCapsule;
    }

    public void TakeDamage(float timer, int dmg)
    {
        if (strength + dmg - 1 < _sprites.Length)
        {
            GetComponent<SpriteRenderer>().sprite = _sprites[strength + dmg - 1];
            strength += dmg;
            _particles.Play();
        }
        else
        {
            _particles.Play();
            StartCoroutine(DestroyCapsule(timer));
        }
    }
    public void Dead()
    {
        Debug.Log("DEAD");
        GameObject g = objectPoolRuinedCapsule.GetPooledObject();
        g.SetActive(true);
        g.transform.position = transform.position;
        g.transform.rotation = transform.rotation;

        g.GetComponent<RuinedCapsule>().Inst(spriteRenderer.color);

        isDead = false;

        gameObject.SetActive(false);
    }
    private IEnumerator DestroyCapsule(float timer)
    {
        enabled = false;
        yield return new WaitForSeconds(timer);
        Dead();
    }

}
