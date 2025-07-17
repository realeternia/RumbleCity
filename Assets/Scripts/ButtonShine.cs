using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonShine : MonoBehaviour
{
    private Image targetImage;
    public float blinkDuration = 2f;
    public Color startColor = Color.white;
    public Color endColor = new Color(1f, 1f, 1f, 0.5f);

    // Start is called before the first frame update
    void Start()
    {
        targetImage = GetComponent<Image>();
        if (targetImage != null)
        {
            StartCoroutine(BlinkImage());
        }
    }

    IEnumerator BlinkImage()
    {
        while (true)
        {
            targetImage.color = startColor;
            yield return new WaitForSeconds(blinkDuration / 2f);
            targetImage.color = endColor;
            yield return new WaitForSeconds(blinkDuration / 2f);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}
