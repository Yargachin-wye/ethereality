using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeV2 : MonoBehaviour
{
    [SerializeField] private LineRenderer _lineRend;
    private bool lineRendererSpeedUp = false;

    private bool startFly = false;
    private bool stopFly = false;
    [Range(0.01f, 4)][SerializeField] private float StartWaveSize = 2;
    float waveSize = 0;
    [SerializeField][Range(1, 50)] private float ropeProgressionSpeed = 1;

    [SerializeField] private AnimationCurve ropeAnimationCurve1;
    [SerializeField] private AnimationCurve ropeProgressionCurve;

    private int numSections = 0;
    private float moveTime = 0;
    private void Start()
    {
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

        int j = _lineRend.positionCount - 1;

        for (int i = 0; i < _lineRend.positionCount; i++)
        {
            float delta = (float)i / ((float)_lineRend.positionCount - 1f);
            Vector2 offset =
                Vector2.Perpendicular(
                    transform.GetChild(transform.childCount - 1).position
                    - transform.GetChild(0).position).normalized * ropeAnimationCurve1.Evaluate(delta) * waveSize;
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

            _lineRend.SetPosition(j, currentPosition);

            j--;
        }

    }
    private void DrawRopePhisics()
    {
        for (int i = 0; i < numSections; i++)
        {
            Vector3 pose = new Vector3(transform.GetChild(i).position.x, transform.GetChild(i).position.y, 0);
            if (!lineRendererSpeedUp && i < numSections - 2)
            {
                _lineRend.SetPosition(i, Vector3.MoveTowards(_lineRend.GetPosition(i), pose, Time.deltaTime * 20));
            }
            else
            {
                _lineRend.SetPosition(i, pose);
            }
        }
    }
    public void SetCount(int num)
    {
        numSections = num;
        _lineRend.positionCount = num * 4;
    }
    public void StartFly()
    {
        startFly = true;
        _lineRend.enabled = true;
    }
    public void StopFly()
    {
        int ost = _lineRend.positionCount % 4;
        if (numSections <= 3)
        {
            _lineRend.enabled = false;
            return;
        }
        for (int i = 0; i < numSections/* - 1*/; i++)
        {
            Vector2 v = _lineRend.GetPosition(i * 4);
            _lineRend.SetPosition(i, v);
        }
        if (numSections - 1 >= 0)
        {
            _lineRend.positionCount = numSections;
        }
        else
        {
            _lineRend.positionCount = 0;
        }

        startFly = false;
        stopFly = true;

        StartCoroutine(TimerRender());

        DebugUi.instance.LogText(
            "transform = " + transform.childCount +
            "; num = " + numSections +
            "; lineR = " + _lineRend.positionCount);
    }
    public void OffRope()
    {
        enabled = false;
        _lineRend.enabled = false;
    }
    IEnumerator TimerRender()
    {
        yield return new WaitForSeconds(0.1f);
        lineRendererSpeedUp = true;
    }
}
