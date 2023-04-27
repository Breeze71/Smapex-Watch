using UnityEngine;
using UnityEngine.UI;

public class LoadingProgressBar : MonoBehaviour
{
    private Image bar;
    private float startValue = 0;
    private float loadingValue;

    void Start()
    {
        bar = transform.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        loadingValue = Loader.GetLoadingProgress();

        bar.fillAmount = Mathf.Lerp(startValue, loadingValue, 0.1f);

        startValue = loadingValue;
    }
}
