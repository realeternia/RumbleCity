using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ActionWork : MonoBehaviour
{
    public int DestId;
    public int ManCount;

    private int sideId;

    void Start()
    {
        var button = GetComponent<Button>();
        if (button != null) {
            button.onClick.AddListener(OnButtonClick);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetData(int cityId, int side, int count)
    {
        DestId = cityId;
        ManCount = count;
        sideId = side;

        transform.Find("Text (TMP)").GetComponent<TMP_Text>().text = "+" + ((ManCount+1) / 2).ToString();
    }

    private void OnButtonClick()
    {
        if(DestId <= 0)
            return;

        SceneController.Instance.AddSoldier(DestId, sideId, (ManCount+1) / 2);
        SceneController.Instance.RoundEnd();
        DestId = 0; // 防止连点
    }
}
