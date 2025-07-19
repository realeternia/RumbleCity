using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DiceGroup : MonoBehaviour
{
    public Camera cam;
    public GameObject[] diceObject;
    public TMP_Text resultText;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    // Method Roll the Dice
    public void RollTheDice(Action<List<int>> callback = null)
    {
        if (diceObject != null)
        {
            foreach (GameObject dice in diceObject)
            {
                if (dice.GetComponent<Rigidbody>() != null)
                {
                    Rigidbody rb = dice.GetComponent<Rigidbody>();
                    
                    // 生成 x-z 方向的随机向量并归一化
                    Vector2 randomXZ = UnityEngine.Random.insideUnitCircle.normalized;
                    Vector3 randomForceDirection = new Vector3(randomXZ.x, 0.7f, randomXZ.y).normalized;
                    
                    rb.AddTorque(Vector3.Cross(randomForceDirection, Vector3.up) * 3000, ForceMode.Impulse);
                    rb.AddForce(randomForceDirection * 100 * rb.mass, ForceMode.Impulse);
                }
            }
        }

        StartCoroutine(GetDiceCount(callback));
    }

    IEnumerator GetDiceCount(Action<List<int>> callback = null)
    {
        resultText.text = "";
        yield return new WaitForSeconds(0.5f);
        List<int> results = new List<int>();
        foreach (GameObject dice in diceObject)
        {
            if (dice != null)
            {
                Rigidbody rb = dice.GetComponent<Rigidbody>();
                int diceValue = 0;
                // 等待骰子停止旋转并且不移动
                while (true)
                {
                    if (rb.angularVelocity.sqrMagnitude <= 0.1f && rb.velocity.sqrMagnitude <= 0.1f)
                    {                
                        diceValue = GetDiceValue(dice);
                        if (diceValue == 0)
                        {
                            rb.AddForce(Vector3.up * 20 * rb.mass, ForceMode.Impulse); //给一个向上的力
                        }
                        else
                        {
                            break;
                        }
                    }

                    yield return new WaitForSeconds(0.1f);
                }



                Debug.Log("骰子 " + dice.name + " 的点数是: " + diceValue);
                results.Add(diceValue);

                //todo 
            }
        }
        resultText.text = "点数: " + string.Join(", ", results);
        callback?.Invoke(results);

    }

    private int GetDiceValue(GameObject dice)
    {
        Vector3 rot;
        rot = dice.transform.rotation.eulerAngles;
        int diceValue = 0;
        if (distanceInRange((int)rot.x, 90, 30))
        {
            diceValue = 6; //6
        }
        else if (distanceInRange((int)rot.x, 270, 30))
        {
            diceValue = 1; //1
        }
        else if (distanceInRange((int)rot.x, -180, 25) && distanceInRange((int)rot.z, -180, 25))
        {
            diceValue = 4; //4
        }
        else if (distanceInRange((int)rot.x, -180, 25) && distanceInRange((int)rot.z, -90, 25))
        {
            diceValue = 2; //2
        }
        else if (distanceInRange((int)rot.x, -180, 25) && distanceInRange((int)rot.z, -270, 25))
        {
            diceValue = 5; //5
        }
        else if (distanceInRange((int)rot.x, -180, 25) && distanceInRange((int)rot.z, 0, 25))
        {
            diceValue = 3; //3
        }
        else if (distanceInRange((int)rot.x, 0, 25) && distanceInRange((int)rot.z, 0, 25))
        {
            diceValue = 4; //4
        }
        else if (distanceInRange((int)rot.x, 0, 25) && distanceInRange((int)rot.z, 90, 25))
        {
            diceValue = 5; //5
        }
        else if (distanceInRange((int)rot.x, 0, 25) && distanceInRange((int)rot.z, -90, 25))
        {
            diceValue = 2; //2
        }
        else if (distanceInRange((int)rot.x, 0, 25) && distanceInRange((int)rot.z, 180, 25))
        {
            diceValue = 3; //3
        }
        return diceValue;

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
