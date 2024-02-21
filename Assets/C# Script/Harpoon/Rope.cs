using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{
    [SerializeField] private LineRenderer _lineRenderer1;
    [SerializeField] private LineRenderer _lineRenderer2;
    [SerializeField] private LineRenderer _lineRenderer3;
    private bool lineRendererSpeedUp = false;

    private bool startFly = false;
    private bool stopFly = false;
    [Range(0.01f, 4)][SerializeField] private float StartWaveSize = 2;
    float waveSize = 0;
    [SerializeField][Range(1, 50)] private float ropeProgressionSpeed = 1;

    [SerializeField] private AnimationCurve ropeAnimationCurve1;
    [SerializeField] private AnimationCurve ropeAnimationCurve2;
    [SerializeField] private AnimationCurve ropeAnimationCurve3;
    [SerializeField] private AnimationCurve ropeProgressionCurve;

    private int numSections = 0;
    private float moveTime = 0;

    private LineRenderer[] lines;
    private AnimationCurve[] curves;
    private void Start()
    {
        lines = new LineRenderer[3] { _lineRenderer1, _lineRenderer2, _lineRenderer3 };
        curves = new AnimationCurve[3] { ropeAnimationCurve1, ropeAnimationCurve2, ropeAnimationCurve3 };
        waveSize = StartWaveSize;
    }
    private void Update()
    {
        if (startFly)
        {
            moveTime += Time.deltaTime;
            if (waveSize > 0)
            {
                waveSize -= Time.deltaTime;
            }
            else
            {
                waveSize = 0;
            }
            DrawRopeWaves();
        }
        else if (stopFly)
        {
            DrawRopePhisics();
        }
    }
    void DrawRopeWaves()
    {
        for (int k = 0; k < lines.Length; k++)
        {
            int j = lines[k].positionCount - 1;

            for (int i = 0; i < lines[k].positionCount; i++)
            {
                float delta = (float)i / ((float)lines[k].positionCount - 1f);
                Vector2 offset =
                    Vector2.Perpendicular(
                        transform.GetChild(transform.childCount - 1).position
                        - transform.GetChild(0).position).normalized * curves[k].Evaluate(delta) * waveSize;
                Vector2 targetPosition = Vector2.Lerp(
                    transform.GetChild(transform.childCount - 1).position,
                    transform.GetChild(0).position,
                    delta
                    ) + offset;
                Vector2 currentPosition = Vector2.Lerp(
                    transform.GetChild(transform.childCount - 1).position,
                    targetPosition,
                    ropeProgressionCurve.Evaluate(moveTime) * ropeProgressionSpeed
                    );

                lines[k].SetPosition(j, currentPosition);

                j--;
            }
        }
    }
    private void DrawRopePhisics()
    {
        for (int i = 0; i < numSections; i++)
        {
            Vector3 pose = new Vector3(transform.GetChild(i).position.x, transform.GetChild(i).position.y, 0);
            if (!lineRendererSpeedUp && i < numSections - 2)
            {
                _lineRenderer1.SetPosition(i, Vector3.MoveTowards(_lineRenderer1.GetPosition(i), pose, Time.deltaTime * 20));
                _lineRenderer2.SetPosition(i, Vector3.MoveTowards(_lineRenderer2.GetPosition(i), pose, Time.deltaTime * 20));
                _lineRenderer3.SetPosition(i, Vector3.MoveTowards(_lineRenderer3.GetPosition(i), pose, Time.deltaTime * 20));
            }
            else
            {
                _lineRenderer1.SetPosition(i, pose);
                _lineRenderer2.SetPosition(i, pose);
                _lineRenderer3.SetPosition(i, pose);
            }
        }
    }
    public void SetCount(int num)
    {
        numSections = num;
        _lineRenderer1.positionCount = num * 4;
        _lineRenderer2.positionCount = num * 4;
        _lineRenderer3.positionCount = num * 4;
    }
    public void StartFly()
    {
        startFly = true;
        _lineRenderer1.enabled = true;
        _lineRenderer2.enabled = true;
        _lineRenderer3.enabled = true;
    }
    public void StopFly()
    {
        int ost = _lineRenderer1.positionCount % 4;
        if (numSections <= 3)
        {
            _lineRenderer1.enabled = false;
            _lineRenderer2.enabled = false;
            _lineRenderer3.enabled = false;
            return;
        }
        for (int i = 0; i < numSections/* - 1*/; i++)
        {
            Vector2 v = _lineRenderer1.GetPosition(i * 4);
            _lineRenderer1.SetPosition(i, v);
            _lineRenderer2.SetPosition(i, v);
            _lineRenderer3.SetPosition(i, v);
        }
        if (numSections - 1 >= 0)
        {
            _lineRenderer1.positionCount = numSections;
            _lineRenderer2.positionCount = numSections;
            _lineRenderer3.positionCount = numSections;
        }
        else
        {
            _lineRenderer1.positionCount = 0;
            _lineRenderer2.positionCount = 0;
            _lineRenderer3.positionCount = 0;
        }
            
        startFly = false;
        stopFly = true;
        
        StartCoroutine(TimerRender());

        DebugUi.instance.LogText(
            "transform = " + transform.childCount +
            "; num = " + numSections +
            "; lineR = " + _lineRenderer1.positionCount);
    }
    public void OffRope()
    {
        enabled = false;
        _lineRenderer1.enabled = false;
        _lineRenderer2.enabled = false;
        _lineRenderer3.enabled = false;
    }
    IEnumerator TimerRender()
    {
        yield return new WaitForSeconds(0.1f);
        lineRendererSpeedUp = true;
    }
}
