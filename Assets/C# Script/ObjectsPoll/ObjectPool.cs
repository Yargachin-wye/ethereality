using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [SerializeField] private GameObject _ObjectPrefab;
    [SerializeField] private int pooledAmount = 20;
    private List<GameObject> pooledObjects;

    private void Awake()
    {
        InitPool();
    }
    private void InitPool()
    {
        pooledObjects = new List<GameObject>();

        for (int i = 0; i < pooledAmount; i++)
        {
            GameObject obj = Instantiate(_ObjectPrefab);
            obj.SetActive(false);
            pooledObjects.Add(obj);
        }
    }
    public GameObject GetPooledObject()
    {
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (!pooledObjects[i].activeInHierarchy)
            {
                return pooledObjects[i];
            }
        }

        GameObject obj = Instantiate(_ObjectPrefab);
        pooledObjects.Add(obj);
        return obj;
    }
}
