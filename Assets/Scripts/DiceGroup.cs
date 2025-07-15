using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DiceGroup : MonoBehaviour
{
    public Camera cam;
    public GameObject[] diceObject;
    public AudioClip collisionSound; // 新增音效资源引用
    private Vector3 initPos;
    private float initXpose;
    public TMP_Text resultText;

    // Start is called before the first frame update
    void Start()
    {
        // 为每个骰子添加 AudioSource 组件
        foreach (GameObject dice in diceObject)
        {
            if (dice != null)
            {
                AudioSource audioSource = dice.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
                audioSource.clip = collisionSound;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
             Debug.Log("GetMouseButtonDown");
            // initial click to roll a dice
            initPos = Input.mousePosition;
            
            // return x component of dice from screen to view point
            initXpose = cam.ScreenToViewportPoint(Input.mousePosition).x;
        }
        
        // current position of mouse
        Vector3 currentPos = Input.mousePosition;
        
        if (Input.GetMouseButtonUp(0))
        {
            Debug.Log("GetMouseButtonUp");
            Vector3 newPos = cam.ScreenToWorldPoint(currentPos);
            initPos = cam.ScreenToWorldPoint(initPos);
            
            // Method use to roll the dice
            RollTheDice(newPos);
            // use identify face value on dice
            StartCoroutine(GetDiceCount());
        }
    }

    // Method Roll the Dice
    void RollTheDice(Vector3 lastPos)
    {
        if (diceObject != null)
        {
            foreach (GameObject dice in diceObject)
            {
                if (dice != null && dice.GetComponent<Rigidbody>() != null)
                {
                    Debug.Log("RollTheDice");
                    Rigidbody rb = dice.GetComponent<Rigidbody>();
                    
                    // 生成 x-z 方向的随机向量并归一化
                    Vector2 randomXZ = UnityEngine.Random.insideUnitCircle.normalized;
                    Vector3 randomForceDirection = new Vector3(randomXZ.x, 0.7f, randomXZ.y).normalized;
                    
                    rb.AddTorque(Vector3.Cross(randomForceDirection, Vector3.up) * 1500, ForceMode.Impulse);
                    rb.AddForce(randomForceDirection * 50 * rb.mass, ForceMode.Impulse);
                }
            }
        }
    }

    IEnumerator GetDiceCount()
    {   
        resultText.text = "";
        yield return new WaitForSeconds(0.5f);
        List<int> results = new List<int>();
        foreach (GameObject dice in diceObject)
        {
            if (dice != null)
            {
                Rigidbody rb = dice.GetComponent<Rigidbody>();
                
                // 等待骰子停止旋转并且不移动
                while (rb != null && (rb.angularVelocity.sqrMagnitude > 0.1f || rb.velocity.sqrMagnitude > 0.1f))
                {
                    yield return new WaitForFixedUpdate();
                }
                
                // 根据骰子旋转方向判定点数，此处简单模拟，实际需根据需求实现
                Vector3 rot = dice.transform.rotation.eulerAngles;
                var diceValue = 0;
                if (distanceInRange((int)rot.x, 90, 15)) {
                    diceValue = 6; //6
                } else if (distanceInRange((int)rot.x, 270, 15)) {
                    diceValue = 1; //1
                } else if ((distanceInRange((int)rot.x, -180, 15)) && distanceInRange((int)rot.z, -180, 15) ) {
                    diceValue = 4; //4
                } else if ((distanceInRange((int)rot.x, -180, 15)) && distanceInRange((int)rot.z, -90, 15) ) {
                    diceValue = 2; //2
                } else if (distanceInRange((int)rot.x, -180, 15) && distanceInRange((int)rot.z, -270, 15) ) {
                    diceValue = 5; //5
               } else if (distanceInRange((int)rot.x, -180, 15) && distanceInRange((int)rot.z, 0, 15) ) {
                    diceValue = 3; //3
                } else if (distanceInRange((int)rot.x, 0, 15) && distanceInRange((int)rot.z, 0, 15) ) {
                    diceValue = 4; //4
                } else if (distanceInRange((int)rot.x, 0, 15) && distanceInRange((int)rot.z, 90, 15) ) {
                    diceValue = 5; //5
                } else if (distanceInRange((int)rot.x, 0, 15) && distanceInRange((int)rot.z, -90, 15) ) {
                    diceValue = 2; //2
               } else if (distanceInRange((int)rot.x, 0, 15) && distanceInRange((int)rot.z, 180, 15) ) {
                    diceValue = 3; //3
                }                
                
                Debug.Log("骰子 " + dice.name + " 的点数是: " + diceValue + " x:" + rot.x + " z:"+ rot.z);
                results.Add(diceValue);
            }
        }
        resultText.text = "点数: " + string.Join(", ", results);
    }

    private bool distanceInRange(int rot, int target, int dis)
    {
        // 处理角度在 0-360 范围内
        rot = (rot % 360 + 360) % 360;
        target = (target % 360 + 360) % 360;
        
        // 计算两个角度的最小差值
        int diff = Math.Abs(rot - target);
        diff = Math.Min(diff, 360 - diff);
        
        return diff <= dis;
    }

}
