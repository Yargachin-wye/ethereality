using System.Collections.Generic;
using UnityEngine;

namespace C__Script.Generation
{
    public class WorldGenerator2 : MonoBehaviour
    {
        [SerializeField] private Color debugColor;

        [Header("World Generator")] 
        [SerializeField] private int chunkSize = 100;
        [SerializeField] private float pointsSize = 1;
        [SerializeField] private Transform pointsContainer;
        public Transform PointsContainer => pointsContainer;

        [Header("Perlin Noise")] 
        [SerializeField] private int numberOfPointsPerChunk = 100;
        [SerializeField] [Range(0, 1)] private float averageLerp;
        [SerializeField] private Vector2Int perlinOffsetMin;
        [SerializeField] private Vector2Int perlinOffsetMax;
        [SerializeField] private float scale = 10f;
        [SerializeField] private GameObject pointPrefab;
        public GameObject PointPrefab => pointPrefab;
        [SerializeField] [Range(0, 100)] private int point2Probability;
        [SerializeField] [Range(0, 1)] private float threshold1;
        [SerializeField] [Range(0, 1)] private float threshold2;
        [SerializeField] [Range(0, 100)] private int voidPointProbability;

        [SerializeField] private Color color1;
        [SerializeField] private Color color2;
        [SerializeField] private Color color3;
        [SerializeField] private Color color4;
        [SerializeField] private Color roundColor5;

        [Header("Circle")] 
        [SerializeField] private float maxRadius = 100;
        [SerializeField] private float minRadius = 100;
        [SerializeField] private float thicknessCircle;
        [SerializeField] private int numCirclePoints = 0;
        [SerializeField] private int seed = 0;

        public Dictionary<Vector2Int, GenerationPointData[]> Chunks { get; private set; }
        private readonly int maxIters = 500000;
        private System.Random rand;

        private void Start()
        {
            rand = new System.Random(seed);
            Chunks = new Dictionary<Vector2Int, GenerationPointData[]>();
            Vector2Int initialChunk = GetChunkCoordinate(Vector2.zero);
            GenerateChunk(initialChunk);
        }

        private void FixedUpdate()
        {
            Vector2 playerPosition = new Vector2(transform.position.x, transform.position.y);
            Vector2Int currentChunk = GetChunkCoordinate(playerPosition);
            LoadAdjacentChunks(currentChunk);
        }

        private Vector2Int GetChunkCoordinate(Vector2 position)
        {
            int x = Mathf.FloorToInt(position.x / chunkSize);
            int y = Mathf.FloorToInt(position.y / chunkSize);
            return new Vector2Int(x, y);
        }

        private void LoadAdjacentChunks(Vector2Int centerChunk)
        {
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    Vector2Int chunkCoordinate = new Vector2Int(centerChunk.x + x, centerChunk.y + y);
                    if (!Chunks.ContainsKey(chunkCoordinate))
                    {
                        GenerateChunk(chunkCoordinate);
                    }
                }
            }
        }

        private void GenerateChunk(Vector2Int chunkCoordinate)
        {
            Vector2 perlinOffset1 = new Vector2(rand.Next(perlinOffsetMin.x, perlinOffsetMax.x), rand.Next(perlinOffsetMin.y, perlinOffsetMax.y));
            Vector2 perlinOffset2 = new Vector2(rand.Next(perlinOffsetMin.x, perlinOffsetMax.x), rand.Next(perlinOffsetMin.y, perlinOffsetMax.y));

            GenerationPointData[] points = new GenerationPointData[numberOfPointsPerChunk + numCirclePoints];
            for (int i = 0; i < numberOfPointsPerChunk + numCirclePoints; i++)
            {
                points[i] = new GenerationPointData();
                points[i].Init(this);
            }

            for (int i = 0; i < numberOfPointsPerChunk; i++)
            {
                float x = rand.Next(chunkCoordinate.x * chunkSize, (chunkCoordinate.x + 1) * chunkSize);
                float y = rand.Next(chunkCoordinate.y * chunkSize, (chunkCoordinate.y + 1) * chunkSize);

                float perlin1 = Mathf.PerlinNoise(x / scale + perlinOffset1.x, y / scale + perlinOffset1.y);
                float perlin2 = Mathf.PerlinNoise(x / scale + perlinOffset2.x, y / scale + perlinOffset2.y);
                float perlinAverage = (perlin1 + perlin2) / 2;
                float perlinNoiseVal = Mathf.Lerp(perlin1, perlinAverage, averageLerp);

                Vector3 position = new Vector3(x, y, 0);
                points[i].SetPosition(position);
                points[i].SetSize(pointsSize);

                if (perlinNoiseVal > threshold1 / threshold2)
                {
                    if (rand.Next(0, 100) < point2Probability)
                    {
                        points[i].SetColor(color2);
                        points[i].SetTipe(BotsList.Circle);
                    }
                    else
                    {
                        i--;
                    }
                }
                else if (perlinNoiseVal > threshold1)
                {
                    points[i].SetColor(color1);
                    points[i].SetTipe(BotsList.Bee);
                }
                else
                {
                    if (rand.Next(0, 100) < voidPointProbability)
                    {
                        points[i].SetColor(color3);
                        points[i].SetTipe(BotsList.Bee);
                    }
                    else
                    {
                        i--;
                    }
                }
            }

            GenerateCirclePoints(chunkCoordinate, points);

            Chunks.Add(chunkCoordinate, points);
        }

        private void GenerateCirclePoints(Vector2Int chunkCoordinate, GenerationPointData[] points)
        {
            // Similar logic to your current GenerateCircle function, adapted for chunk-based generation
        }

        public class GenerationPointData
        {
#if UNITY_EDITOR
            private GenerationPoint debugPoint;
#endif
            private BotsList _tipe;
            private Vector3 _position;
            private Vector2 _circleCenter;
            public BotsList Tipe => _tipe;
            public Vector3 Position => _position;
            public Vector2 CircleCenter => _circleCenter;

            public void SetTipe(BotsList tipe, float xCircle = 0, float yCircle = 0)
            {
                _tipe = tipe;
                _circleCenter = new Vector2(xCircle, yCircle);
            }

            public void SetPosition(Vector3 position)
            {
                _position = position;
#if UNITY_EDITOR
                debugPoint.SetPosition(position);
#endif
            }

            public void SetColor(Color color)
            {
#if UNITY_EDITOR
                debugPoint.SetColor(color);
#endif
            }

            public void SetSize(float size)
            {
#if UNITY_EDITOR
                debugPoint.SetSize(size);
#endif
            }

            public void Init(WorldGenerator2 generator)
            {
#if UNITY_EDITOR
                GameObject g = Instantiate(generator.PointPrefab);
                debugPoint = g.GetComponent<GenerationPoint>();
                debugPoint.transform.parent = generator.PointsContainer;
#endif
            }
        }
    }
}
