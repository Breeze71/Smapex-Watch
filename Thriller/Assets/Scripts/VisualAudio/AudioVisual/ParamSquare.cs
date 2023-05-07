using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParamSquare : MonoBehaviour
{
    public int band;
    public float startScale, scaleMultiplier;

    // smooth
    public float smothness;
    public int isSmooth;   // smooth(1)

    // material
    private Material material;

    void Update()
    {
        smothControl();
        //material = GetComponent<MeshRenderer>().materials[0];   // 所有 materials
    }

    private void smothControl()
    {
        /*
        float originScaleY = this.transform.localScale.y;
        float newScaleY = (AudioSample.frequencyBand[band] * scaleMultiplier) + startScale;

        float scale = Mathf.Lerp(originScaleY, newScaleY, smothness);   // 簡潔

        if(isSmooth == 0)
        {
            // 別用了太爛
            transform.localScale = new Vector3(transform.localScale.x, newScaleY, transform.localScale.z);
        }
        else if(isSmooth == 1)
        {
            transform.localScale = new Vector3(transform.localScale.x, scale, transform.localScale.z);
        }
        else if(isSmooth == 2)
        {
            transform.localScale = new Vector3(transform.localScale.x, (AudioSample.bandBuffer[band] * scaleMultiplier) + startScale, transform.localScale.z);
        }
        */

        
        float newScaleY1 = (AudioSample.ControlBand[band] * scaleMultiplier) + startScale;
        float newScaleY2 = (AudioSample.ControlBandBuffer[band] * scaleMultiplier) + startScale;

        if(isSmooth == 0)
        {
            transform.localScale = new Vector3(transform.localScale.x, newScaleY1, transform.localScale.z);
            //Color color = new Color((AudioSample.ControlBand[band] * scaleMultiplier), (AudioSample.ControlBand[band] * scaleMultiplier), (AudioSample.ControlBand[band] * scaleMultiplier));   // change rgb
            
            //material.SetColor("EmissionColor", color);
        }
        
        if(isSmooth == 1)
        {
            transform.localScale = new Vector3(transform.localScale.x, newScaleY2, transform.localScale.z);
            //Color color = new Color((AudioSample.ControlBandBuffer[band] * scaleMultiplier), (AudioSample.ControlBandBuffer[band] * scaleMultiplier), (AudioSample.ControlBandBuffer[band] * scaleMultiplier));

            //material.SetColor("EmissionColor", color);
        }
        
        
    }
}
