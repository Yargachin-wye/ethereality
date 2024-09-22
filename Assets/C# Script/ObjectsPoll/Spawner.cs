using System;
using System.Collections;
using System.Collections.Generic;
using C__Script.Bots;
using C__Script.Generation;
using Unity.VisualScripting;
using UnityEngine;

public enum BotsList
{
    Circle,
    Pill,
    Pizdrich,
    Bee,
    Defender1,
    Defender2,
    Defender3,
    Null = -1,
}

public class Spawner : MonoBehaviour
{
    [SerializeField] private WorldGenerator worldGenerator;
    [SerializeField] float spawnerDistance = 20;
    [SerializeField] private List<CapsuleInfo> List = new List<CapsuleInfo>();
    private List<CapsuleInfo> list => List;

    private Dictionary<BotsList, CapsuleInfo> dict = new Dictionary<BotsList, CapsuleInfo>();

    [System.Serializable]
    private class CapsuleInfo
    {
        public ObjectPool enemyPool;
        public ObjectPool ruinedEnemyPool;
        public BotsList name = BotsList.Null;
    }

    private SpawnPointsData[] spawnpoints;
    private Harpoon player;

    public class SpawnPointsData
    {
        public WorldGenerator.GenerationPointData pointData;
        public GameObject gameObject = null;
        public bool spawned => gameObject != null && gameObject.activeSelf;
        public bool dead = false;
    }

    public static Spawner Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("Spawner.Instance != null");
        }

        foreach (var item in list)
        {
            dict.Add(item.name, item);
        }
    }

    private void Start()
    {
        worldGenerator.StartGeneration();

        player = Harpoon.instance;
        spawnpoints = new SpawnPointsData[worldGenerator.Points.Length];
        for (int i = 0; i < worldGenerator.Points.Length; i++)
        {
            spawnpoints[i] = new SpawnPointsData() { pointData = worldGenerator.Points[i] };
        }
    }

    public void AddPoint()
    {
    }

    private void FixedUpdate()
    {
        foreach (var point in spawnpoints)
        {
            if (Vector2.Distance(point.pointData.Position, player.transform.position) < spawnerDistance)
            {
                if (!point.spawned)
                {
                    point.gameObject = Spawn(point.pointData.Tipe, point.pointData.Position, point);
                }
            }
            else if (point.spawned)
            {
                Despawn(point.gameObject);
                point.gameObject = null;
            }
        }
    }

    public void Despawn(GameObject g)
    {
        g.SetActive(false);
    }

    public GameObject Spawn(BotsList name, Vector3 pos, SpawnPointsData spawnPointsData)
    {
        if (name == BotsList.Null)
        {
            return null;
        }

        GameObject g = dict[name].enemyPool.GetPooledObject();
        g.SetActive(true);
        g.transform.position = pos;
        g.GetComponent<Capsule>().InitDependencies(dict[name].ruinedEnemyPool);

        if (name == BotsList.Defender1)
        {
            g.GetComponent<Defender>().InitDependencies(pos, spawnPointsData.pointData.CircleCenter, 1);
        }
        else if (name == BotsList.Defender2)
        {
            g.GetComponent<Defender>().InitDependencies(pos, spawnPointsData.pointData.CircleCenter, 2);
        }
        else if (name == BotsList.Defender3)
        {
            g.GetComponent<Defender>().InitDependencies(pos, spawnPointsData.pointData.CircleCenter, 3);
        }

        return g;
    }
}