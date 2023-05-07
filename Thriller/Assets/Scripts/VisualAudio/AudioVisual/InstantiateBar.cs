using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateBar : MonoBehaviour
{
    public GameObject sampleBarPrefabs;
    GameObject[] sampleBar = new GameObject [512];
    [SerializeField] private float Scale;

    private void Start()
    {
        // initBar();
    }

    void Update()
    {
        //changeSize();
    }

    private void initBar()
    {
        // 生出 512 bar
        for(int i = 0; i < 512; i++)
        {
            GameObject instantiateSampleBar = (GameObject)Instantiate(sampleBarPrefabs);
            instantiateSampleBar.transform.position = this.transform.position;

            instantiateSampleBar.transform.parent = this.transform;
            instantiateSampleBar.name = "SampleBar" + i;
            
            /*
            // 生成完後調整下一個
            this.transform.eulerAngles = new Vector3 (0, -0.703125f * i, 0);     // 360/512 
            instantiateSampleBar.transform.position = Vector3.right * 100;       // x 軸
            */
            
            instantiateSampleBar.transform.position = new Vector3(1,0,0);
            sampleBar[i] = instantiateSampleBar; // 512 個 sampleBar
        }
    }

    private void changeSize()
    {
        for(int i = 0; i < 512; i++)
        {
            if(sampleBar != null)
            {
                // 隨 frequency 變化
                sampleBar[i].transform.localScale = new Vector3(1, (AudioSample.samples[i] * Scale) + 2, 0);
            }
        }
    }

}
