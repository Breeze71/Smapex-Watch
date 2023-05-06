using UnityEngine;

public class PhyllotaxisTrail : MonoBehaviour
{
    #region AudioSample
    [Header("AudioSample")]
    private Material trailMaterial;
    [SerializeField] private Color trailColor;
    #endregion
    
    [Header("Trail")]
    private int num;
    private Vector2 phyllotaxisPos;
    private TrailRenderer trailRenderer;
    [SerializeField] private float degree, dotScale;
    [SerializeField] private int numberStart;

    [Header("Lerp")]
    private bool isLerping;
    private int currentIteration; 
    private Vector3 startPosition, endPosition;
    [SerializeField] private bool useLerp;
    [SerializeField] private int maxIteration;
    [SerializeField] private int stepSize;

    [Header("Audio Lerp")]

    private float lerpPostimer, lerpPosSpeed;
    [SerializeField] private int lerpPosBand;
    [SerializeField] private Vector2 lerpPosSpeedMinMax;
    [SerializeField] AnimationCurve lerpPosAnimCurve;

    [Header("Control Audio Trail")]
    private bool forward;
    [SerializeField] private bool repeat, reverse;
    [SerializeField] private bool useScaleAnim, useScaleCurve;
    [SerializeField] private Vector2 scaleAnimMinMax;
    [SerializeField] private AnimationCurve scaleAnimCurve;
    [SerializeField] private float scaleAnimSpeed;
    [SerializeField] private int scaleBand;
    private float scaleTimer, currentScale;

    void Awake()
    {
        trailRenderer = GetComponent<TrailRenderer>();

        trailMaterial = new Material(trailRenderer.material);
        trailMaterial.SetColor("_TintColor", trailColor);
        trailRenderer.material = trailMaterial;

        num = numberStart;
        currentScale = dotScale;
        forward = true;

        transform.localPosition = CauculatePhyllotaxis(degree, currentScale, num);
        
        if(useLerp)
        {
            isLerping = true;
            SetLerpPosition();
        }
    }

    private void Update() 
    {
        UseScaleAnimation();
        GenerateTrail();
    }

    private void SetLerpPosition()
    {
        phyllotaxisPos = CauculatePhyllotaxis(degree, currentScale, num);

        startPosition = this.transform.localPosition;
        endPosition = new Vector3(phyllotaxisPos.x, phyllotaxisPos.y, 0);

        //Debug.Log("SetLerpPosition");
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
        if(!useLerp)
        {
            Vector2 phyllotaxisPosition = CauculatePhyllotaxis(degree, currentScale, num);
            transform.localPosition = new Vector3(phyllotaxisPosition.x, phyllotaxisPosition.y, 0);

            num += stepSize;
            currentIteration++;
            return;
        }

        UseLerp();
        //Debug.Log("GenerateTrail");
    }

    private void UseLerp()
    {
        lerpPosSpeed = Mathf.Lerp(lerpPosSpeedMinMax.x, lerpPosSpeedMinMax.y, 
                                                        lerpPosAnimCurve.Evaluate(AudioSample.frequencyBand[lerpPosBand]));

        lerpPostimer += Time.deltaTime * lerpPosSpeed;
        transform.localPosition = Vector3.Lerp(startPosition, endPosition, Mathf.Clamp01(lerpPostimer));

        if(lerpPostimer >= 1)
        {
            lerpPostimer -= 1;

            ForwardOrReverse();
            ReverseTrailOrNot();
        }
    }

    private void ReverseTrailOrNot()
    {
        if((currentIteration > 0) && (currentIteration < maxIteration))
        {
            SetLerpPosition();
            //Debug.Log("currentIteration < maxIteration");

            return;
        }

        if(repeat)
        {
            // 反向
            if(reverse)
            {
                Debug.Log("ReverseTrail");
                forward = !forward;
                SetLerpPosition();
            }
            
            else
            {
                Debug.Log("start");
                num = numberStart;
                currentIteration = 0;

                SetLerpPosition();
            }
        }
        else
        {
            isLerping = false;
        }
    }

    private void ForwardOrReverse()
    {
       if(forward)
        {
            num += stepSize;
            currentIteration++;
        }
        else
        {
            num -= stepSize;
            currentIteration--;
        }
    }

    private void UseScaleAnimation()
    {
        if(useScaleAnim)
        {   
            // auto control scale
            if(!useScaleCurve)
            {
                currentScale = Mathf.Lerp(scaleAnimMinMax.x, scaleAnimMinMax.y, AudioSample.ControlBand[scaleBand]);
                return;
            }

            // use animCurve to control
            scaleTimer += (scaleAnimSpeed * AudioSample.ControlBand[scaleBand]) * Time.deltaTime;
            if(scaleTimer >= 1)
            {
                scaleTimer -= 1;
            }

            currentScale = Mathf.Lerp(scaleAnimMinMax.x, scaleAnimMinMax.y, scaleAnimCurve.Evaluate(scaleTimer));
        }
    }
}
