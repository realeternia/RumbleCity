using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenScaler : MonoBehaviour
{
    public GameObject Canvas;
    // Start is called before the first frame update
    void Start()
    {
        // 获取设备当前宽度和高度
        int width = Screen.width;
        int height = Screen.height;
        int screenWidth = Mathf.Max(width, height);
        int screenHeight = Mathf.Min(width, height);
        // 获取 CanvasScaler 组件
        CanvasScaler canvasScaler = Canvas.GetComponent<CanvasScaler>();
        if (canvasScaler != null)
        {
            Debug.Log("ScreenScaler " + screenWidth + " * " + screenHeight);
          //  canvasScaler.referenceResolution = new Vector2(screenWidth, screenHeight);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
