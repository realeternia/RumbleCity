using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // 引入 TextMeshPro 命名空间

public class SceneController : MonoBehaviour
{
    public static SceneController Instance{ private set; get; }

    public GameObject[] Cities;
    public Material[] CitySkins; // 新增 CitySkins 数组
    private bool[,] connectivityMap = new bool[15, 15]; // 保存城市连通关系的二维数组
    private List<GameObject> roadCubes = new List<GameObject>();

    public Button RollButton;
    public Button[] CheckButtons;
    public GameObject DiceGObj;

    private PlayerData pRed = new PlayerData();
    private PlayerData pBlue = new PlayerData();
    private int swordLeft = 4;

    public TMPro.TMP_Text RedText;
    public TMPro.TMP_Text BlueText;

    void Start()
    {
        Instance = this;
        Debug.Log("cityNames " + Cities.Length + " " + CitySkins.Length);
        if (Cities.Length == 11 && CitySkins.Length == 11)
        {
            // 创建 CitySkins 的副本用于打乱顺序
            Material[] shuffledSkins = new Material[CitySkins.Length];
            System.Array.Copy(CitySkins, shuffledSkins, CitySkins.Length);

            // 打乱 shuffledSkins 顺序以确保不重复分配
            for (int i = shuffledSkins.Length - 1; i > 0; i--) {
                int j = Random.Range(0, i + 1);
                Material temp = shuffledSkins[i];
                shuffledSkins[i] = shuffledSkins[j];
                shuffledSkins[j] = temp;
            }

            // 获取 DataManager 实例
            List<string> cityNames = DataManager.Instance.GetRandomCityNames(11);

            for (int i = 0; i < Cities.Length; i++) {
                if (Cities[i] != null) {
                    MeshRenderer meshRenderer = Cities[i].GetComponent<MeshRenderer>();
                    if (meshRenderer != null) {
                        meshRenderer.material = shuffledSkins[i];
                    }
                    CityControllerNew cityController = Cities[i].GetComponent<CityControllerNew>();
                    if (cityController != null) {
                        // 查找当前使用的材质在原始 CitySkins 中的索引
                        for (int j = 0; j < CitySkins.Length; j++) {
                            if (shuffledSkins[i] == CitySkins[j]) {
                                if (i < cityNames.Count) {
                                    cityController.Init(j + 2, cityNames[i]);
                                } else {
                                    cityController.Init(j + 2, "");
                                }
                                break;
                            }
                        }
                    }
                }
            }
        }

        // 初始化连通关系数组
        for (int i = 0; i < 13; i++)
        {
            for (int j = 0; j < 13; j++)
            {
                connectivityMap[i, j] = false;
            }
        }

        InitRoad();
        RollButton.onClick.AddListener(OnRollButtonClick);
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
        if (cityA < 0 || cityA > 10 || cityB < 0 || cityB > 10)
        {
            return false;
        }
        return connectivityMap[cityA, cityB];
    }
    // Update is called once per frame
    void Update()
    {
    }

    private void OnRollButtonClick()
    {
        DiceGroup diceGroup = DiceGObj.GetComponent<DiceGroup>();
        if (diceGroup != null)
        {
            RollButton.gameObject.SetActive(false);
            diceGroup.RollTheDice((diceResults) => {
                Debug.Log("骰子滚动完成回调触发");
                if (diceResults.Contains(0))
                {
                    RollButton.gameObject.SetActive(true);
                }
                else
                {
                    List<(int, int)> paramPairs = new List<(int, int)>();
                    if (diceResults.Count >= 3)
                    {
                        paramPairs.Add((diceResults[0] + diceResults[1], diceResults[2]));
                        paramPairs.Add((diceResults[0] + diceResults[2], diceResults[1]));
                        paramPairs.Add((diceResults[1] + diceResults[2], diceResults[0]));
                    }

                    HashSet<(int, int)> usedParams = new HashSet<(int, int)>();
                    for (int i = 0; i < Mathf.Min(CheckButtons.Length, paramPairs.Count); i++)
                    {
                        Button button = CheckButtons[i];
                        if (button != null)
                        {
                            var paramPair = paramPairs[i];
                            if (usedParams.Contains(paramPair))
                            {
                                button.gameObject.SetActive(false);
                            }
                            else
                            {
                                button.gameObject.SetActive(true);
                                ActionWork actionWork = button.GetComponent<ActionWork>();
                                if (actionWork != null)
                                {
                                    actionWork.SetData(paramPair.Item1, paramPair.Item2);
                                }
                                usedParams.Add(paramPair);
                            }
                        }
                    }
                }
            });
        }
    }

    public void RoundEnd()
    {
        CheckTurn(1);
    }

    private void RollDiceAndProcessResults(DiceGroup diceGroup)
    {
        diceGroup.RollTheDice((diceResults) => {
            if (diceResults.Contains(0))
            {
                RollDiceAndProcessResults(diceGroup);
            }
            else
            {
                // 随机选择2个数相加作为cityId，另一个数作为count
                if (diceResults.Count >= 3)
                {
                    List<int> indices = new List<int> { 0, 1, 2 };
                    int randomIndex1 = indices[Random.Range(0, indices.Count)];
                    indices.Remove(randomIndex1);
                    int randomIndex2 = indices[Random.Range(0, indices.Count)];
                    indices.Remove(randomIndex2);
                    int cityId = diceResults[randomIndex1] + diceResults[randomIndex2];
                    int count = diceResults[indices[0]];

                    AddSoldier(cityId, 2, (count + 1) / 2);

                    CheckTurn(0);
                }
            }
        });
    }

    private void CheckTurn(int turn)
    {
        if(pRed.Sword > 0 && pBlue.Sword > 0)
        {
            StartCoroutine(CalculateCityScores());
            return;
        }

        if (turn == 0)
        {
            if(TrayController.Instance.GetSoldierLeft(turn + 1) == 0)
            {
                if (pBlue.Sword == 0)
                {
                    pBlue.Sword = swordLeft;
                    swordLeft--;
                }
                CheckTurn(1);
            }
            else
            {
                RollButton.gameObject.SetActive(true);
            }
        }
        else
        {
            // 隐藏所有CheckButtons
            foreach (var button in CheckButtons)
            {
                if (button != null)
                {
                    button.gameObject.SetActive(false);
                }
            }

            if(TrayController.Instance.GetSoldierLeft(turn + 1) == 0)
            {                
                if (pRed.Sword == 0)
                {
                    pRed.Sword = swordLeft;
                    swordLeft--;
                }
                CheckTurn(0);
            }
            else
            {
                DiceGroup diceGroup = DiceGObj.GetComponent<DiceGroup>();
                if (diceGroup != null)
                { //ai进行中
                    RollDiceAndProcessResults(diceGroup);
                }
            }            

        }
    }

    public void AddSoldier(int cityId, int side, int count)
    {
        foreach (var city in Cities)
        {
            if (city.GetComponent<CityControllerNew>().CityID == cityId)
            {
                city.GetComponent<CityControllerNew>().AddSoldier(side, count);
                break;
            }
        }
    }

    // 静态变量记录上次播放路径和 clip
    string lastPath = "";
    AudioClip lastClip = null;

    public void PlaySound(string path)
    {
        AudioSource audioSource = gameObject.GetComponent<AudioSource>();
        if (lastPath != path)
        {
            lastPath = path;
            lastClip = Resources.Load<AudioClip>(path);
            if (lastClip != null)
            {
                audioSource.clip = lastClip;
            }
        }

        if (audioSource.clip != null)
        {
            audioSource.Stop();
            audioSource.Play();
        }
    }

    private CityControllerNew FindCityById(int cityId)
    {
        foreach (var cityObj in Cities)
        {
            if (cityObj != null)
            {
                var cityController = cityObj.GetComponent<CityControllerNew>();
                if (cityController != null && cityController.CityID == cityId)
                {
                    return cityController;
                }
            }
        }
        return null;
    }

    private IEnumerator CalculateCityScores()
    {
        RedText.gameObject.SetActive(true);
        BlueText.gameObject.SetActive(true);

        RedText.text = "红方: " + pRed.Sword;
        BlueText.text = "蓝方: " + pBlue.Sword;

        for (int cityId = 2; cityId <= 12; cityId++)
        {
            var cityController = FindCityById(cityId);
             // 获取士兵数量
            int blueSoldiers = cityController.soldierCounts.ContainsKey(1) ? cityController.soldierCounts[1] : 0;
            int redSoldiers = cityController.soldierCounts.ContainsKey(2) ? cityController.soldierCounts[2] : 0;

            if(redSoldiers == 0 && blueSoldiers == 0)
            {
                cityController.FlashAndShrink(0);
                yield return new WaitForSeconds(2f);
                continue;
            }

            int winSide = 0;
            if (redSoldiers < blueSoldiers)
            {
                pBlue.Mark += cityController.CityID;
                if(redSoldiers > 0)
                    pRed.Mark += cityController.CityID / 2;
                winSide = 1;
            }
            else if (blueSoldiers < redSoldiers)
            {
                pRed.Mark += cityController.CityID;
                if(blueSoldiers > 0)
                    pBlue.Mark += cityController.CityID / 2;
                winSide = 2;
            }
            else
            {
                // 平局，比较 Sword 数量
                if (pRed.Sword > pBlue.Sword)
                {
                    pRed.Mark += cityController.CityID;
                    if(blueSoldiers > 0)
                        pBlue.Mark += cityController.CityID / 2;
                    winSide = 2;
                }
                else if (pBlue.Sword > pRed.Sword)
                {
                    pBlue.Mark += cityController.CityID;
                    if(redSoldiers > 0)
                        pRed.Mark += cityController.CityID / 2;
                    winSide = 1;
                }
                // 若 Sword 也相同则不加分
            }
            cityController.FlashAndShrink(winSide);
            SceneController.Instance.PlaySound("Sounds/sword");
            // 遍历联通的城市，派遣援军
            for (int otherCityId = 2; otherCityId <= 12; otherCityId++)
            {
                if (otherCityId > cityId && IsDirectlyConnected(cityId - 2, otherCityId - 2))
                {
                    Debug.Log($"城市{cityId}和城市{otherCityId}直接联通");
                    var targetCity = FindCityById(otherCityId);
                    int otherBlueSoldiers = targetCity.soldierCounts.ContainsKey(1) ? targetCity.soldierCounts[1] : 0;
                    int otherRedSoldiers = targetCity.soldierCounts.ContainsKey(2) ? targetCity.soldierCounts[2] : 0;
                    
                    if (otherBlueSoldiers > 0 && otherRedSoldiers > 0)
                    {
                        if(winSide == 1)
                            targetCity.AddSoldierHelp(cityController.GetCenterPos(), 1, 2);
                        else
                            targetCity.AddSoldierHelp(cityController.GetCenterPos(), 2, 2);
                    }
                }
            }

            // 更新显示
            RedText.text = "红方: " + pRed.Sword + "+" + pRed.Mark;
            BlueText.text = "蓝方: " + pBlue.Sword + "+" + pBlue.Mark;
            
            yield return new WaitForSeconds(3f);
        }
    }

}
