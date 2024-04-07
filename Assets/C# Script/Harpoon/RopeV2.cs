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
            waveSize = Mathf.Max(0, waveSize - Time.deltaTime);
            DrawRopeWaves();
        }
        else if (stopFly)
        {
            DrawRopePhysics();
        }
    }

    private void DrawRopeWaves()
    {
        int j = _lineRend.positionCount - 1;
        Transform childEnd = transform.GetChild(transform.childCount - 1);
        Transform childStart = transform.GetChild(0);
        Vector2 startToEnd = childEnd.position - childStart.position;
        float moveTimeEval = ropeProgressionCurve.Evaluate(moveTime);

        for (int i = 0; i < _lineRend.positionCount; i++)
        {
            float delta = (float)i / (_lineRend.positionCount - 1f);
            float waveEval = ropeAnimationCurve1.Evaluate(delta);
            Vector2 offset = Vector2.Perpendicular(startToEnd).normalized * waveEval * waveSize;
            Vector2 targetPosition = Vector2.Lerp(childEnd.position, childStart.position, delta) + offset;
            Vector2 currentPosition = Vector2.Lerp(childEnd.position, targetPosition, moveTimeEval * ropeProgressionSpeed);
            _lineRend.SetPosition(j, currentPosition);
            j--;
        }
    }

    private void DrawRopePhysics()
    {
        for (int i = 0; i < numSections - 1; i++)
        {
            Vector3 pose = transform.GetChild(i).position;
            _lineRend.SetPosition(i, Vector3.MoveTowards(_lineRend.GetPosition(i), pose, Time.deltaTime * 20));
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
        if (numSections <= 3)
        {
            _lineRend.enabled = false;
            return;
        }

        for (int i = 0; i < numSections; i++)
        {
            _lineRend.SetPosition(i, _lineRend.GetPosition(i * 4));
        }

        _lineRend.positionCount = numSections - 1;
        startFly = false;
        stopFly = true;
    }

    public void OffRope()
    {
        enabled = false;
        _lineRend.enabled = false;
    }
}