using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioSample : MonoBehaviour
{
    private AudioSource audioSource;

    // 音訊處理中，一個信號可以被視為一系列的振幅值。採樣（sampling）就是以一定頻率讀取每一個時間點的振幅值
    public static float[] samples = new float[512];
    public static float[] frequencyBand = new float[8];

    // smothess(2)
    public static float[] bandBuffer = new float[8];
    private float[] bandSmoothness = new float[8];

    // Control Value
    private float[] frequcyBandHighest = new float[8];
    public static float[] ControlBand = new float[8];
    public static float[] ControlBandBuffer = new float[8];

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    private void Update()
    {
        getSpectrumAudioSource();
        makeFrequencyBands();

        bandSmooth();   // 能更精細控制

        CreateControlBands();
    }

    // 分頻率接收
    private void getSpectrumAudioSource()
    {
        audioSource.GetSpectrumData(samples, 0, FFTWindow.Blackman);
    }

    // 8 bands
    private void makeFrequencyBands()
    {
        /*
            22050 / 512 bands = 43 Hertz per samples

            20 - 60 Hertz
            60 - 250 Hertz
            250 - 500 Hertz
            500 - 2000 Hertz
            2000 - 4000 Hertz
            4000 - 6000 Hertz
            6000 - 20000 Hertz

            0 - 2 = 86 HZ
            1 - 4 = 172 HZ      87 - 258
            2 - 8 = 344 HZ      259 - 602
            3 - 16 = 688 HZ     603 - 1290
            4 - 32 = 1376 HZ    1291 - 2666
            5 - 64 = 2752 HZ    2677 - 5418
            6 - 128 = 5504 HZ   5419 - 10922
            7 - 256 = 11008 HZ  10923 - 21930
                +
                510
        */

        int count = 0;

        for(int i = 0; i < 8; i++)
        {
            float average = 0;

            int sampleCount = (int)Mathf.Pow(2, i) * 2; // 從 2 的 0 次開始，故乘 2

            if(i == 7)
                sampleCount += 2;

            for(int j = 0; j < sampleCount; j++)
            {
                average += samples[count] *(count + 1);
                count++;
            }

            average /= count;
            frequencyBand[i] = average * 10;
        }
    }
    
    // 能更精細控制 8 band
    private void bandSmooth()
    {
        for(int i = 0; i < 8; i++)
        {
            if(frequencyBand[i] > bandBuffer[i])
            {
                bandBuffer[i] = frequencyBand[i];

                bandSmoothness[i] = 0.005f;
            }
            if(frequencyBand[i] < bandBuffer[i])
            {
                bandBuffer[i] -= bandSmoothness[i];

                bandSmoothness[i] *= 1.2f;
            }
        }
    }

    private void CreateControlBands()
    {
        for(int i = 0; i < 8; i++)
        {
            if(frequencyBand[i] >= frequcyBandHighest[i])
            {
                frequcyBandHighest[i] = frequencyBand[i];
            }
            
            // 0 ~ 1 可以用來控制各種
            if(frequcyBandHighest[i] != 0)
            {
                // 避免分母為零
                ControlBand[i] = (frequencyBand[i] / frequcyBandHighest[i]);
                ControlBandBuffer[i] = (bandBuffer[i] / frequcyBandHighest[i]);
            }else
            {
                // 初始值
                frequcyBandHighest[i] = 1;
            }
            /* 生成
            if (AudioPeer.audioBand[1] > 0.8f )
            {
                GameObject myPrefabInstance = (GameObject)Instantiate(myPrefab);
            }
            */
        }
    }
}
