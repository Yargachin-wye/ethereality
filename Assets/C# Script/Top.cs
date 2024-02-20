using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Top : MonoBehaviour
{
    [SerializeField] private MainMenager mainMenager;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Harpoon>() != null)
        {
            mainMenager.Win();
        }
    }
}
