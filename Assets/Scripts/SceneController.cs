using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // 引入 TextMeshPro 命名空间

public class SceneController : MonoBehaviour
{
    public GameObject[] Cities;
    public Material[] CitySkins; // 新增 CitySkins 数组
    private bool[,] connectivityMap = new bool[13, 13]; // 保存城市连通关系的二维数组
    private List<GameObject> roadCubes = new List<GameObject>();

    void Start()
    {
        if (Cities.Length == 11 && CitySkins.Length == 11)
        {
            // 创建 CitySkins 的副本用于打乱顺序
            Material[] shuffledSkins = new Material[CitySkins.Length];
            System.Array.Copy(CitySkins, shuffledSkins, CitySkins.Length);

            // 打乱 shuffledSkins 顺序以确保不重复分配
            for (int i = shuffledSkins.Length - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                Material temp = shuffledSkins[i];
                shuffledSkins[i] = shuffledSkins[j];
                shuffledSkins[j] = temp;
            }

            for (int i = 0; i < Cities.Length; i++)
            {
                if (Cities[i] != null)
                {
                    MeshRenderer meshRenderer = Cities[i].GetComponent<MeshRenderer>();
                    if (meshRenderer != null)
                    {
                        meshRenderer.material = shuffledSkins[i];
                    }
                    CityControllerNew cityController = Cities[i].GetComponent<CityControllerNew>();
                    if (cityController != null)
                    {
                        // 查找当前使用的材质在原始 CitySkins 中的索引
                        for (int j = 0; j < CitySkins.Length; j++)
                        {
                            if (shuffledSkins[i] == CitySkins[j])
                            {
                                cityController.CityID = j + 2;
                                break;
                            }
                        }
                    }
                }
            }
        }

        // 初始化连通关系数组
        for (int i = 0; i < 11; i++)
        {
            for (int j = 0; j < 11; j++)
            {
                connectivityMap[i, j] = false;
            }
        }

        InitRoad();
    }

    private void InitRoad()
    {
        // 查找 RoadRoot 下所有 Cube 节点
        GameObject roadRoot = GameObject.Find("Roads");
        if (roadRoot != null)
        {
            int rdIndx = 1;
            foreach (Transform child in roadRoot.transform)
            {
                if (child.gameObject.name.Contains("Cube"))
                {   
                  //  Debug.Log("init " + child.gameObject.name);
                    List<int> collidedCityIds = new List<int>();
                    Bounds roadBounds = child.GetComponent<Collider>().bounds;

                    foreach (var city in Cities)
                    {
                        Bounds cityBounds = city.GetComponent<Collider>().bounds;
                        if (roadBounds.Intersects(cityBounds))
                        {
                            CityControllerNew cityController = city.GetComponent<CityControllerNew>();
                            if (cityController != null && !collidedCityIds.Contains(cityController.CityID))
                            {
                                collidedCityIds.Add(cityController.CityID);
                                if (collidedCityIds.Count == 2)
                                {
                                    var cityId1 = collidedCityIds[0];
                                    var cityId2 = collidedCityIds[1];
                                    if (cityId1 >= 0 && cityId1 < connectivityMap.GetLength(0) && cityId2 >= 0 && cityId2 < connectivityMap.GetLength(1))
                                    {
                                        connectivityMap[cityId1 - 2, cityId2 - 2] = true;
                                        connectivityMap[cityId2 - 2, cityId1 - 2] = true;
                                        Debug.Log("connet " + rdIndx ++ + " " + cityId1 + " and " + cityId2);
                                    }
                                    collidedCityIds.Clear();
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public void UpdateConnectivity(int cityId1, int cityId2)
    {

    }

    // 判定 cityA 和 cityB 是否直接连通的函数
    private bool IsDirectlyConnected(int cityA, int cityB)
    {
        if (cityA < 0 || cityA >= 10 || cityB < 0 || cityB >= 10)
        {
            return false;
        }
        return connectivityMap[cityA, cityB];
    }
    // Update is called once per frame
    void Update()
    {
    }
}
