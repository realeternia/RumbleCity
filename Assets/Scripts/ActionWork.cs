using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionWork : MonoBehaviour
{
    public int DestId;
    public int ManCount;
    
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetData(int did, int count)
    {
        DestId = did;
        ManCount = count;

        transform.Find("Text (TMP)").GetComponent<TMPro.TMP_Text>().text = ((ManCount+1) / 2).ToString() + "士兵->" + DestId.ToString();
        
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(() => {
                SceneController.Instance.AddSoldier(DestId, 1, (ManCount+1) / 2);
                SceneController.Instance.RoundEnd();
            });
        }
    }
}
