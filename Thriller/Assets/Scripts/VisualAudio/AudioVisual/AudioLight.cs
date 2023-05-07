using UnityEngine;
using UnityEngine.Rendering.Universal;

public class AudioLight : MonoBehaviour
{
    [SerializeField] private int band;
    [SerializeField] private float minIntensity, maxIntensity;
    private Light2D newlight;
    void Start()
    {
        newlight = GetComponent<Light2D>();
    }

    // Update is called once per frame
    void Update()
    {
        float lightIntensity = (AudioSample.ControlBandBuffer[band] * (maxIntensity - minIntensity)) + minIntensity;
        newlight.intensity = lightIntensity;
    }
}
