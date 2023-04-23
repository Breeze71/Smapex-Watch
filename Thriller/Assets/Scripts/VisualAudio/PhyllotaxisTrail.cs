using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhyllotaxisTrail : MonoBehaviour
{
    [Header("Trail")]
    public float degree, dotScale;
    public int numberStart;
    private int num;
    private Vector2 phyllotaxisPos;
    private TrailRenderer trailRenderer;

    [Header("Lerp")]
    public bool useLerp;
    public bool isLerping;
    public float lerpInterval;
    public int maxIteration;
    public int stepSize;
    private int currentIteration; 
    private Vector3 startPosition, endPosition;
    private float timeStartLerping;

    void Awake()
    {
        num = numberStart;
        trailRenderer = GetComponent<TrailRenderer>();
        transform.localPosition = CauculatePhyllotaxis(degree, dotScale, num);

        if(useLerp)
        {
            StartLerping();
        }
    }
    void FixedUpdate()
    {
        GenerateTrail();
    }

    private Vector2 CauculatePhyllotaxis(float degree, float scale, int num)
    {
        double angle = num * (degree * Mathf.Deg2Rad);    // 生成角度

        float radius = scale * Mathf.Sqrt(num);   // x = r * cos(angle)
                                                    // y = r * sin(angle)
                                                    
        float x = radius * (float)System.Math.Cos(angle);   // system 才能 double -> float
        float y = radius * (float)System.Math.Sin(angle);

        return new Vector2(x, y);
    }

    private void GenerateTrail()
    {
        if(useLerp)
        {
            float timeSinceLerp = Time.time - timeStartLerping;
            float LerpSmooth = timeSinceLerp / lerpInterval; // 0 ~ 1
            
            transform.localPosition = Vector3.Lerp(startPosition, endPosition, LerpSmooth);
            
            if(LerpSmooth >= 0.97f)
            {
                transform.localPosition = endPosition;

                num += stepSize;
                currentIteration++;

                if(currentIteration <= maxIteration)
                    StartLerping();
                else
                    isLerping = false;
            }
        }
        else
        {
            Vector2 phyllotaxisPosition = CauculatePhyllotaxis(degree, dotScale, num);

            transform.localPosition = new Vector3(phyllotaxisPosition.x, phyllotaxisPosition.y, 0);
            num++;
            
        }
    }

    private void StartLerping()
    {
        isLerping = true;
        timeStartLerping = Time.time;

        phyllotaxisPos = CauculatePhyllotaxis(degree, dotScale, num);
        startPosition = this.transform.localPosition;
        endPosition = new Vector3(phyllotaxisPos.x, phyllotaxisPos.y, 0);
    }
}
