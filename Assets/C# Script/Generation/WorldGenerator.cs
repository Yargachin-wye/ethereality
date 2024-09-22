using UnityEngine;

namespace C__Script.Generation
{
    public class WorldGenerator : MonoBehaviour
    {
        [SerializeField] private Color debugColor;

        [Header("World Generator")] [SerializeField]
        private Vector2Int WorldSize;

        [SerializeField] private Vector2Int WorldCenter;
        [SerializeField] private float pointsSize = 1;
        [SerializeField] private Transform pointsConteiner;
        public Transform PointsConteiner => pointsConteiner;

        [Header("Perlin Noise")] [SerializeField]
        private int numberOfPoints = 100;

        [SerializeField] [Range(0, 1)] private float averageLerp;

        [SerializeField] private Vector2Int PerlinOffsetMin;
        [SerializeField] private Vector2Int PerlinOffsetMax;

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

        [Header("Circle")] [SerializeField] private float maxRadius = 100;
        [SerializeField] private float minRadius = 100;
        [SerializeField] private float thicknessCircle;
        [SerializeField] private int numCirclePoints = 0;
        [SerializeField] private int seed = 0;

        public GenerationPointData[] Points { get; private set; }

        private readonly int maxIters = 500000;
        private readonly System.Random rand = new System.Random();

        public void StartGeneration()
        {
            Points = new GenerationPointData[numberOfPoints + numCirclePoints];
            for (int i = 0; i < numberOfPoints + numCirclePoints; i++)
            {
                Points[i] = new GenerationPointData();
                Points[i].Init(this);
            }

            GenerateWorldPerlinNoize();
            int iters = 0;
            while (!GenerateCircle())
            {
                iters++;
                if (iters > 10)
                {
                    Debug.Log("Error: Iters > 10");
                    return;
                }

                GenerateWorldPerlinNoize();
            }
        }

        /*
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            rand = new System.Random(seed);

            GenerateWorldPerlinNoize();
            int Iters = 0;
            while (!GenerateCircle())
            {
                Iters++;
                if (Iters > 10)
                {
                    Debug.Log("Error: Iters > 10");
                    return;
                }

                GenerateWorldPerlinNoize();
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            seed = rand.Next(100000, 999999);
            rand = new System.Random(seed);

            GenerateWorldPerlinNoize();
            int Iters = 0;
            while (!GenerateCircle())
            {
                Iters++;
                if (Iters > 10)
                {
                    Debug.Log("Error: Iters > 10");
                    return;
                }

                GenerateWorldPerlinNoize();
            }
        }
    }
    */

        private bool GenerateCircle()
        {
            Vector2 center = new Vector2();
            float radius = -1;
            int j = 0;
            while (radius < 0)
            {
                if (j > 10)
                {
                    Debug.Log("x = " + center.x + "; y = " + center.y + "; r = " + radius);
                    return false;
                }

                j++;
                GenereteCircleParams(WorldSize.x / 3, WorldSize.y / 3, minRadius, maxRadius, out center, out radius);
            }

            Debug.Log("x = " + center.x + "; y = " + center.y + "; r = " + radius);

            int Iters = 0;
            for (int i = 0; i < numberOfPoints; i++)
            {
                Iters++;
                if (Iters > maxIters)
                {
                    return false;
                }

                float dist = Vector2.Distance(Points[i].Position, center);
                if (dist < radius + thicknessCircle && dist > radius - thicknessCircle)
                {
                    if (Points[i].Tipe == BotsList.Bee)
                    {
                        Points[i].SetColor(debugColor);
                        Points[i].SetTipe(BotsList.Null);
                    }
                }
                else if (dist < radius - thicknessCircle)
                {
                    if (Points[i].Tipe == BotsList.Circle)
                    {
                        Points[i].SetColor(color4);
                        Points[i].SetTipe(BotsList.Pill);
                    }
                }
            }

            return true;
        }

        private void GenereteCircleParams(float centreX, float centreY, float minR, float maxR,
            out Vector2 CircleCenter,
            out float CircleRadius)
        {
            Vector2 center = new Vector2(
                rand.Next((int)-centreX, (int)centreX) + WorldCenter.x,
                rand.Next((int)-centreY, (int)centreY) + WorldCenter.y);
            float radius = maxR;

            int Iters = 0;
            for (int i = 0; i < numberOfPoints; i++)
            {
                Iters++;
                if (Iters > maxIters)
                {
                    CircleCenter = Vector2.zero;
                    CircleRadius = -1;
                    return;
                }

                float dist = Vector2.Distance(Points[i].Position, center);
                if (dist < radius + thicknessCircle && dist > radius - thicknessCircle)
                {
                    if (Points[i].Tipe == BotsList.Bee)
                    {
                        radius -= 5;
                        if (radius < minR)
                        {
                            center = new Vector2(
                                rand.Next((int)-centreX, (int)centreX) + WorldCenter.x,
                                rand.Next((int)-centreY, (int)centreY) + WorldCenter.y);
                            radius = maxR;
                        }

                        i = 0;
                    }
                }
            }

            CircleCenter = center;
            CircleRadius = radius;

            GenerateCircleRound(center, radius, BotsList.Defender1);
        }

        private void GenerateCircleRound(Vector2 center, float radius, BotsList defenderType)
        {
            for (int i = numberOfPoints; i < numberOfPoints + numCirclePoints; i++)
            {
                float angle = i * (2 * Mathf.PI / numCirclePoints); // Вычисление угла
                float x = center.x + radius * Mathf.Cos(angle); // Вычисление x координаты
                float y = center.y + radius * Mathf.Sin(angle); // Вычисление y координаты

                Points[i].SetPosition(new Vector3(x, y, 0));

                Points[i].SetColor(roundColor5);
                Points[i].SetTipe(defenderType, center.x, center.y);
            }
        }

        void GenerateWorldPerlinNoize()
        {
            // <test
            // Time.timeScale = 0;
            // test>

            Vector2 perNOffset1 = new Vector2(
                rand.Next(PerlinOffsetMin.x, PerlinOffsetMax.x),
                rand.Next(PerlinOffsetMin.y, PerlinOffsetMax.y));
            Vector2 perNOffset2 = new Vector2(
                rand.Next(PerlinOffsetMin.x, PerlinOffsetMax.x),
                rand.Next(PerlinOffsetMin.y, PerlinOffsetMax.y));

            int Iters = 0;
            for (int i = 0; i < numberOfPoints; i++)
            {
                Iters++;
                if (Iters > maxIters)
                {
                    return;
                }

                float x = rand.Next(WorldCenter.x - WorldSize.y / 2,
                    WorldCenter.x + WorldSize.y / 2); // Случайное значение для x
                float y = rand.Next(WorldCenter.y - WorldSize.x / 2,
                    WorldCenter.y + WorldSize.x / 2); // Случайное значение для y

                float perlin1 = Mathf.PerlinNoise(x / scale + perNOffset1.x, y / scale + perNOffset1.y);
                float perlin2 = Mathf.PerlinNoise(x / scale + perNOffset2.x, y / scale + perNOffset2.y);

                float perlinAverage = (perlin1 + perlin2) / 2;

                float perlinNoiseVal = Mathf.Lerp(perlin1, perlinAverage, averageLerp);

                Vector3 position = new Vector3(y, x, 0); // Позиция точки с учетом значения шума

                Points[i].SetPosition(position); // Создание точки из префаба
                Points[i].SetSize(pointsSize);

                // Рассчитываем расстояние от текущей точки до центра круга

                if (perlinNoiseVal > threshold1 / threshold2)
                {
                    if (rand.Next(0, 100) < point2Probability)
                    {
                        Points[i].SetColor(color2);
                        Points[i].SetTipe(BotsList.Circle);
                    }
                    else
                    {
                        i--;
                    }
                }
                else if (perlinNoiseVal > threshold1)
                {
                    Points[i].SetColor(color1);
                    Points[i].SetTipe(BotsList.Bee);
                }
                else
                {
                    if (rand.Next(0, 100) < voidPointProbability)
                    {
                        Points[i].SetColor(color3);
                        Points[i].SetTipe(BotsList.Bee);
                    }
                    else
                    {
                        i--;
                    }
                }
            }
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

            public void Init(WorldGenerator generator)
            {
#if UNITY_EDITOR
                GameObject g = Instantiate(generator.PointPrefab); // Создание точки из префаба
                debugPoint = g.GetComponent<GenerationPoint>();
                debugPoint.transform.parent = generator.PointsConteiner;
#endif
            }
        }
    }
}