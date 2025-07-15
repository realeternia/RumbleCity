using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CityControllerNew : MonoBehaviour, IPointerClickHandler
{
    // Start is called before the first frame update
    public int CityID;
    public TMPro.TMP_Text Text;
    private int soldierTotal;
    private Dictionary<int, int> soldierCounts = new Dictionary<int, int>();
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Init(int cityId, string cityName)
    {
        CityID = cityId;
        soldierTotal = 0;
        soldierCounts.Clear();
        Text.text = cityName;
    }

    public void SendABC(string msg)
    {    Debug.Log("Scene Obj SendABC " + msg);
    }

    public void OnPointerClick(PointerEventData eventData)
    {    
        Debug.Log("Scene Obj OnPointerClick");
    }

    public void AddSoldier(int side, int count)
    {
        // 获取当前对象（假设为 Cylinder）的信息
        GameObject cylinder = gameObject;
        MeshRenderer cylinderRenderer = cylinder.GetComponent<MeshRenderer>();
        if (cylinderRenderer == null) return;

        // 计算 Cylinder 上平面的位置
        Bounds bounds = cylinderRenderer.bounds;
        Vector3 spawnPosition = bounds.center + Vector3.up * bounds.extents.y;

        // 循环创建 count 个 cube 方块
        for (int i = 0; i < count; i++)
        {
            // 创建一个新的 cube 方块
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.localScale = new Vector3(10, 10, 10);
            cube.GetComponent<MeshRenderer>().material = Resources.Load<Material>("Materials/Side" + side);
            
            // 简单排列 cube 方块，避免重叠
            if (!soldierCounts.ContainsKey(side))
            {
                soldierCounts[side] = 0;
            }
            float angle = 2 * Mathf.PI * (i + soldierTotal) / 12;
            soldierCounts[side]++;
            soldierTotal++;
            float radius = bounds.extents.x * 0.8f; // 使用 Cylinder 半径的 80% 作为排列半径
            Vector3 offset = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
            cube.transform.position = spawnPosition + offset;
            
            // 让 cube 方块作为当前对象的子对象
            cube.transform.SetParent(cylinder.transform);
        }
    }
}
