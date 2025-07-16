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
        Text.color = Color.gray;
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
        if(!string.IsNullOrEmpty(cityName))
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

        // 调用TrayController.RemoveSoldier获取cube
        List<GameObject> cubes = TrayController.Instance.RemoveSoldier(side, count);

        foreach (GameObject cube in cubes)
        {            
            // 移除 Rigidbody 组件
            Rigidbody rb = cube.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.useGravity = false;
                rb.isKinematic = true;
            }
            
            // 简单排列 cube 方块，避免重叠
            if (!soldierCounts.ContainsKey(side))
            {
                soldierCounts[side] = 0;
            }
            float angle = 2 * Mathf.PI * soldierTotal / 12;
            soldierCounts[side]++;
            soldierTotal++;
            float radius = bounds.extents.x * 0.8f; // 使用 Cylinder 半径的 80% 作为排列半径
            Vector3 offset = new Vector3(Mathf.Cos(angle), 1, Mathf.Sin(angle)) * radius;
            cube.transform.position = spawnPosition + offset;
            cube.transform.rotation = Quaternion.Euler(Vector3.zero);

            // 让 cube 方块作为当前对象的子对象
            cube.transform.SetParent(cylinder.transform);
            cube.transform.localScale = new Vector3(0.2f, 0.5f, 0.2f);
        }

        // 根据士兵数量设置Text颜色
        int side1Count = soldierCounts.ContainsKey(1) ? soldierCounts[1] : 0;
        int side2Count = soldierCounts.ContainsKey(2) ? soldierCounts[2] : 0;

        if (side1Count > side2Count)
        {
            Text.color = Color.blue;
        }
        else if (side2Count > side1Count)
        {
            Text.color = Color.red;
        }
        else
        {
            Text.color = Color.gray;
        }
    }
}
