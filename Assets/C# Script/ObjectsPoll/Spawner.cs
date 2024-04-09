using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private List<CapsuleInfo> List = new List<CapsuleInfo>();
    public List<CapsuleInfo> list => List;
    [System.Serializable]
    public class CapsuleInfo
    {
        public ObjectPool enemyPool;
        public ObjectPool ruinedEnemyPool;
    }
    private void Awake()
    {
        
    }
    private void Start()
    {
        Spawn(list[0], new Vector3(5, 5, 5), Quaternion.identity);
    }
    public void Spawn(CapsuleInfo info, Vector2 pos, Quaternion rotation)
    {
        GameObject g = info.enemyPool.GetPooledObject();
        g.SetActive(true);
        g.transform.position = pos;
        g.transform.rotation = rotation;
        g.GetComponent<Capsule>().InitDependencies(info.ruinedEnemyPool);
    }

}
