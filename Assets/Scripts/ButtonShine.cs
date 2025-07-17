using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonShine : MonoBehaviour
{
    private Image targetImage;
    public float blinkDuration = 1f;
    public Color startColor = Color.white;
    public Color endColor = new Color(1f, 1f, 1f, 0.5f);
    private float timer = 0f;
    private bool isFadingIn = true;

    // Start is called before the first frame update
    void Start()
    {
  		targetImage = GetComponent<Image>();
        if (targetImage != null)
        {
            targetImage.color = startColor;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (targetImage != null)
        {
            timer += Time.deltaTime;
            if (timer >= blinkDuration / 2f)
            {
                isFadingIn = !isFadingIn;
                targetImage.color = isFadingIn ? startColor : endColor;
                timer = 0f;
            }
        }
    }
}
