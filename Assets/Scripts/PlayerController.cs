using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public Image targetImage;
    public Button targetButton;
    public Sprite humanSprite;
    public Sprite robotSprite;
    private bool isHuman = false;

    // Start is called before the first frame update
    void Start()
    {
        if (targetButton != null)
        {
            targetButton.onClick.AddListener(SwitchImage);
        }
    }

    void SwitchImage()
    {
        if (targetImage != null)
        {
            isHuman = !isHuman;
            PlayerManager.Instance.GetPlayerData(2).IsAI = !isHuman;
            targetImage.sprite = isHuman ? humanSprite : robotSprite;
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}
