using UnityEngine;
using UnityEngine.VFX;

// 受擊特效
public class ShieldRipples : MonoBehaviour
{
    public GameObject shieldRipples;

    private VisualEffect shieldRipplesVFX;

    // 之後改成 event 強化效能
    private void OncollisionEnter(Collision co)
    {
        if(co.gameObject.tag == "Bullet")
        {
            // 於撞擊處法向量生成
            var ripples = Instantiate(shieldRipples , transform) as GameObject;
            shieldRipplesVFX = ripples.GetComponent<VisualEffect>();
            shieldRipplesVFX.SetVector3("SphereCenter" , co.contacts[0].point);

            Destroy(ripples , 2);
        }
    }
}
