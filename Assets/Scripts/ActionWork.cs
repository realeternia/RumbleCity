using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ActionWork : MonoBehaviour
{
    public int DestId;
    public int ManCount;

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

    public void SetData(int did, int count)
    {
        DestId = did;
        ManCount = count;

        transform.Find("Text (TMP)").GetComponent<TMP_Text>().text = ((ManCount+1) / 2).ToString() + "士兵->" + DestId.ToString();
    }

    private void OnButtonClick()
    {
        if(DestId <= 0)
            return;

        SceneController.Instance.AddSoldier(DestId, 1, (ManCount+1) / 2);
        SceneController.Instance.RoundEnd();
        DestId = 0; // 防止连点
    }
}
