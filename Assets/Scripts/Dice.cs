using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dice : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 新增碰撞检测方法
    private static float lastPlayTime = -3f; // 记录上次播放时间，初始化为 -2 保证首次能播放
    private void OnCollisionEnter(Collision collision)
    {
        // 检查碰撞对象的层是否为 Board
        if (collision.gameObject.layer == LayerMask.NameToLayer("Board"))
        {
            if (Time.time - lastPlayTime >= 3f)
            {
                SceneController.Instance.PlaySound("Sounds/dice");
                lastPlayTime = Time.time; // 更新上次播放时间
            }

           
        }
    }
}
