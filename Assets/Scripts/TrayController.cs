using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrayController : MonoBehaviour
{   
    public static TrayController Instance{ private set; get; }
    public GameObject Tray;
    private Dictionary<int, int> sideCubeCounts = new Dictionary<int, int>();

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        // 初始化各side的方块数量为18
        for (int side = 1; side <= 2; side++)
        {
            sideCubeCounts[side] = 18;
        }

        for (int side = 1; side <= 2; side++)
        {
            for (int i = 0; i < 18; i++)
            {
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.localScale = new Vector3(10, 3, 10);
                cube.layer = LayerMask.NameToLayer("Board");
                cube.GetComponent<MeshRenderer>().material = Resources.Load<Material>("Materials/Side" + side);
                
                // 在tray上方10位置，x-z做一点随机
                Vector3 trayPosition = Tray != null ? Tray.transform.position : Vector3.zero;
                float randomX = Random.Range(-30f, 30f);
                float randomZ = Random.Range(-30f, 30f);
                float randomY = Random.Range(50f, 150f);
                cube.transform.position = new Vector3(trayPosition.x + randomX, trayPosition.y + randomY, trayPosition.z + randomZ);
                
                // 开启物理
                Rigidbody rb = cube.AddComponent<Rigidbody>();
                rb.useGravity = true;

                // 让 cube 方块作为当前对象的子对象
                cube.transform.SetParent(Tray.transform);   
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
    
    }

    public List<GameObject> RemoveSoldier(int side, int n)
    {
        List<GameObject> removedCubes = new List<GameObject>();
        
        // 如果n大于剩余数量，将n设置为剩余数量
        int availableCount = sideCubeCounts[side];
        n = Mathf.Min(n, availableCount);

        // 获取Tray下对应side的cube
        int foundCount = 0;
        foreach (Transform child in Tray.transform)
        {
            if (foundCount >= n)
            {
                break;
            }

            MeshRenderer renderer = child.GetComponent<MeshRenderer>();
            if (renderer != null && renderer.material.name.StartsWith("Side" + side))
            {
                removedCubes.Add(child.gameObject);
                foundCount++;
            }
        }

        // 更新sideCubeCounts
        sideCubeCounts[side] -= foundCount;

        return removedCubes;
    }

    public int GetSoldierLeft(int side)
    {
        return sideCubeCounts[side];
    }
}
