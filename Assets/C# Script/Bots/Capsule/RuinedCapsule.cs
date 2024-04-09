using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuinedCapsule : MonoBehaviour
{
    [SerializeField] private int _powerRangeMin = 150;
    [SerializeField] private int _powerRangeMax = 250;
    [SerializeField] private float _decaySpeed = 0.5f;
    Color parentColor;

    GameObject[] parts;
    Vector3[] partsPos;
    Quaternion[] partsRot;
    private void Start()
    {
        
    }
    private void OnEnable()
    {
        if(parts == null)
        {
            parts = new GameObject[transform.childCount];
            partsPos = new Vector3[transform.childCount];
            partsRot = new Quaternion[transform.childCount];
            for (int i = 0; i < parts.Length; i++)
            {
                parts[i] = transform.GetChild(i).gameObject;
                partsPos[i] = parts[i].transform.localPosition;
                partsRot[i] = parts[i].transform.rotation;
            }
        }

        for (int i = 0; i < parts.Length; i++)
        {
            parts[i].transform.localPosition = partsPos[i];
            parts[i].transform.rotation = partsRot[i];
        }

        
        parentColor = Color.white;
    }

    public void Inst(Color color)
    {
        for(int i =0; i < parts.Length; i++)
        {
            parts[i].GetComponent<SpriteRenderer>().color = color;
            int power = Random.Range(_powerRangeMin, _powerRangeMax);
            parts[i].GetComponent<Rigidbody2D>().AddForce((transform.GetChild(i).position - transform.position).normalized * power);
        }
        parentColor = color;
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < parts.Length; i++)
        {
            parts[i].GetComponent<SpriteRenderer>().color = parentColor;
        }

        parentColor.a -= _decaySpeed * Time.fixedDeltaTime;

        if (parentColor.a <= 0)
        {
            gameObject.SetActive(false);
            return;
        }
    }
}
