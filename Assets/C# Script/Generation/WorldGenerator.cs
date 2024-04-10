using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class WorldGenerator : MonoBehaviour
{
    [SerializeField] private Color debugColor;
    [Header("World Generator")]
    [SerializeField] private Vector2 WorldSize;
    [SerializeField] private Vector2 WorldCenter;
    [SerializeField] private float pointsSize = 1;
    [SerializeField] private Transform pointsConteiner;
    [Header("Perlin Noize")]
    [SerializeField] private int numberOfPoints = 100;
    [SerializeField][Range(0, 1)] private float averageLerp;

    [SerializeField] private Vector2 PositionPerlinOffset1;
    [SerializeField] private Vector2 PositionPerlinOffset2;


    [SerializeField] private float scale = 10f;
    [SerializeField] private GameObject pointPrefab;

    [SerializeField][Range(0, 1)] private float threshold1;
    [SerializeField][Range(0, 1)] private float threshold2;
    [SerializeField][Range(0, 1)] private float voidPointProbability;
    [SerializeField] private Color color1;
    [SerializeField] private Color color2;
    [SerializeField] private Color color3;
    [Header("Circle")]
    [SerializeField] private Color color4;
    [SerializeField] private float maxRadius = 100;
    [SerializeField] private float minRadius = 100;
    [SerializeField] private float thicknessCircle;

    private GenerationPoint[] points;
    private int maxIters = 500000;
    private void Awake()
    {
        points = new GenerationPoint[numberOfPoints];
        for (int i = 0; i < numberOfPoints; i++)
        {
            GameObject g = Instantiate(pointPrefab); // Создание точки из префаба
            points[i] = g.GetComponent<GenerationPoint>();
            points[i].transform.parent = pointsConteiner;
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GenerateWorldPerlinNoize();
            Generate();
        }
    }
    void Generate()
    {
        Vector2 center = new Vector2();
        float radius = -1;
        int j = 0;
        while (radius < 0 )
        {
            if (j > 10)
            {
                Debug.Log("x = " + center.x + "; y = " + center.y + "; r = " + radius);
                return;
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
                return;
            }

            float dist = Vector2.Distance(points[i].transform.position, center);
            if (dist < radius + thicknessCircle && dist > radius - thicknessCircle)
            {
                if (points[i].tipe == 1)
                {
                    points[i].SetColor(debugColor, -1);
                }

            }
            else if (dist < radius - thicknessCircle)
            {
                if (points[i].tipe == 2)
                {
                    points[i].SetColor(color4, 4);
                }
            }
        }
    }
    private void GenereteCircleParams(float centreX, float centreY, float minR, float maxR, out Vector2 CircleCenter, out float CircleRadius)
    {
        Vector2 center = new Vector2(
            Random.Range(-centreX, centreX) + WorldCenter.x,
            Random.Range(-centreY, centreY) + WorldCenter.y);
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
            float dist = Vector2.Distance(points[i].transform.position, center);
            if (dist < radius + thicknessCircle && dist > radius - thicknessCircle)
            {
                if (points[i].tipe == 1)
                {
                    radius -= 5;
                    if (radius <  minR)
                    {
                        center = new Vector2(
                            Random.Range(-centreX, centreX) + WorldCenter.x,
                            Random.Range(-centreY, centreY) + WorldCenter.y);
                        radius = maxR;
                    }
                    i = 0;
                }
            }
        }
        CircleCenter = center;
        CircleRadius = radius;

    }
    void GenerateWorldPerlinNoize()
    {
        int Iters = 0;
        for (int i = 0; i < numberOfPoints; i++)
        {
            Iters++;
            if (Iters > maxIters)
            {
                return;
            }

            float x = Random.Range(WorldCenter.x - WorldSize.y / 2, WorldCenter.x + WorldSize.y / 2); // Случайное значение для x
            float y = Random.Range(WorldCenter.y - WorldSize.x / 2, WorldCenter.y + WorldSize.x / 2); // Случайное значение для y

            float perlin1 = Mathf.PerlinNoise(x / scale + PositionPerlinOffset1.x, y / scale + PositionPerlinOffset1.y);
            float perlin2 = Mathf.PerlinNoise(x / scale + PositionPerlinOffset2.x, y / scale + PositionPerlinOffset2.y);

            float perlinAverage = (perlin1 + perlin2) / 2;

            float perlinNoiseVal = Mathf.Lerp(perlin1, perlinAverage, averageLerp);

            Vector3 position = new Vector3(y, x, 0); // Позиция точки с учетом значения шума

            points[i].SetPosition(position); // Создание точки из префаба
            points[i].SetSize(pointsSize);

            // Рассчитываем расстояние от текущей точки до центра круга

            if (perlinNoiseVal > threshold1 / threshold2)
            {
                points[i].gameObject.SetActive(true);
                points[i].SetColor(color2, 2);
            }
            else if (perlinNoiseVal > threshold1)
            {
                points[i].gameObject.SetActive(true);
                points[i].SetColor(color1, 1);
            }
            else
            {
                if (Random.Range(0f, 1f) < voidPointProbability) // Новая проверка для случайного пятна синего цвета
                {
                    points[i].gameObject.SetActive(true);
                    points[i].SetColor(color3, 3);
                }
                else
                {
                    i--; // Уменьшение счетчика, чтобы повторно попытаться установить цвет
                }
            }
        }
    }
}
