using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phyllotaxis : MonoBehaviour
{
    public GameObject dot;
    public float degree, dotCount, dotScale;
    public int num;

    void Start()
    {
        
    }

    void Update()
    {
        GenerateDots();
    }

    private Vector2 CauculatePhyllotaxis(float degree, float scale, int count)
    {
        double angle = count * (degree * Mathf.Deg2Rad);    // 生成角度

        float radius = scale * Mathf.Sqrt(count);   // x = r * cos(angle)
                                                    // y = r * sin(angle)
                                                    
        float x = radius * (float)System.Math.Cos(angle);   // system 才能 double -> float
        float y = radius * (float)System.Math.Sin(angle);

        Vector2 vector2 = new Vector2(x, y);
        return vector2;
    }

    private void GenerateDots()
    {
        if(Input.GetKey(KeyCode.Space))
        {
            Vector2 phyPos = CauculatePhyllotaxis(degree, dotScale, num);

            GameObject dotInstantiate = (GameObject)Instantiate(dot);
            dotInstantiate.transform.position = new Vector3(phyPos.x, phyPos.y, this.transform.position.z);
            dotInstantiate.transform.localScale = new Vector3(dotScale, dotScale, dotScale);
            
            num++;
        }
    }
}
