using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq; // 引入 TextMeshPro 命名空间

public class SceneController : MonoBehaviour
{
    public static SceneController Instance{ private set; get; }

    public GameObject[] Cities;
    public Material[] CitySkins; // 新增 CitySkins 数组
    private bool[,] connectivityMap = new bool[15, 15]; // 保存城市连通关系的二维数组

    public Button RollButton;
    public Button[] CheckButtons;
    public GameObject DiceGObj;

    private int swordLeft = 4;

    public TMPro.TMP_Text[] FinalMarkText;

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
        CheckTurn();
    }

    private void AIRollDiceAndProcessResults(int side, DiceGroup diceGroup)
    {
        diceGroup.RollTheDice((diceResults) => {
            if (diceResults.Contains(0))
            {
                AIRollDiceAndProcessResults(side, diceGroup);
            }
            else
            {
                var playerData = PlayerManager.Instance.GetPlayerData(side);

                // 随机选择2个数相加作为cityId，另一个数作为count
                if (diceResults.Count >= 3 && (Random.Range(1, 100) > playerData.GreedRate))
                {
                    Debug.Log("try random");

                    List<int> indices = new List<int> { 0, 1, 2 };
                    int randomIndex1 = indices[Random.Range(0, indices.Count)];
                    indices.Remove(randomIndex1);
                    int randomIndex2 = indices[Random.Range(0, indices.Count)];
                    indices.Remove(randomIndex2);
                    int cityId = diceResults[randomIndex1] + diceResults[randomIndex2];
                    int count = diceResults[indices[0]];

                    AddSoldier(cityId, side, (count + 1) / 2);
                }
                else
                {
                    var mark1 = TryCalculateMarkTotal(side, diceResults[0] + diceResults[1], diceResults[2]);
                    var mark2 = TryCalculateMarkTotal(side, diceResults[0] + diceResults[2], diceResults[1]);
                    var mark3 = TryCalculateMarkTotal(side, diceResults[2] + diceResults[1], diceResults[0]);
                    Debug.Log("try" + diceResults[0] + diceResults[1] + "=" + mark1);
                    Debug.Log("try" + diceResults[0] + diceResults[2] + "=" + mark2);
                    Debug.Log("try" + diceResults[1] + diceResults[2] + "=" + mark3);

                    if (mark1 > mark2 && mark1 >= mark3)
                    {
                        AddSoldier(diceResults[0] + diceResults[1], side, (diceResults[2] + 1) / 2);
                    }
                    else if (mark2 > mark1 && mark2 >= mark3)
                    {
                        AddSoldier(diceResults[0] + diceResults[2], side, (diceResults[1] + 1) / 2);
                    }
                    else if (mark3 > mark1 && mark3 >= mark2)
                    {
                        AddSoldier(diceResults[1] + diceResults[2], side, (diceResults[0] + 1) / 2);
                    }
                    else
                    {
                        // 计算三种组合的 cityId
                        int cityId1 = diceResults[0] + diceResults[1];
                        int cityId2 = diceResults[0] + diceResults[2];
                        int cityId3 = diceResults[1] + diceResults[2];

                        // 找出最小的 cityId
                        int minCityId = Mathf.Min(cityId1, Mathf.Min(cityId2, cityId3));

                        // 根据最小的 cityId 确定对应的 count
                        int count;
                        if (minCityId == cityId1)
                        {
                            count = diceResults[2];
                        }
                        else if (minCityId == cityId2)
                        {
                            count = diceResults[1];
                        }
                        else
                        {
                            count = diceResults[0];
                        }

                        // 调用 AddSoldier 方法
                        AddSoldier(minCityId, side, (count + 1) / 2);
                    }
                }
                CheckTurn();
            }
        });
    }

    private int TryCalculateMarkTotal(int side, int tryCityId, int tryCityCount)
    {
        int mark = 0;
        Dictionary<int, Dictionary<int, int>> helpCcount = new Dictionary<int, Dictionary<int, int>>(); // city / side / help
        for (int i = 2; i <= 12; i++)
        {
            helpCcount[i] = new Dictionary<int, int>();
            for (int j = 1; j <= PlayerManager.Instance.playerCount; j++)
                helpCcount[i][j] = 0;
        }
        helpCcount[tryCityId][side] = tryCityCount;
        for (int cityId = 2; cityId <= 12; cityId++)
        {
            var cityController = FindCityById(cityId);
            var sortedPlayers = new List<KeyValuePair<int, PlayerData>>();
            foreach (var kv in cityController.soldierCounts)
            {
                if (kv.Value > 0 || helpCcount[cityId][kv.Key] > 0)
                {
                    PlayerData playerData = PlayerManager.Instance.GetPlayerData(kv.Key);
                    sortedPlayers.Add(new KeyValuePair<int, PlayerData>(kv.Key, playerData));
                }
            }

            if(sortedPlayers.Count == 0)
                continue;

            sortedPlayers = sortedPlayers.OrderByDescending(kv => cityController.soldierCounts[kv.Key] + helpCcount[cityId][kv.Key])
                                       .ThenByDescending(kv => kv.Value.Sword)
                                       .ToList();

            if (sortedPlayers.Count >= 1 && sortedPlayers[0].Key == side)
            {
                mark += cityId;
            }
            else if (sortedPlayers.Count >= 2 && sortedPlayers[1].Key == side)
            {
                mark += cityId / 2;
            }

            int winSide = sortedPlayers[0].Key;
            for (int otherCityId = 2; otherCityId <= 12; otherCityId++)
            {
                if (otherCityId > cityId && IsDirectlyConnected(cityId - 2, otherCityId - 2))
                {
                    var targetCity = FindCityById(otherCityId);
                    int winSideSoldiers = targetCity.soldierCounts.ContainsKey(winSide) ? targetCity.soldierCounts[winSide] : 0;
                    int nonZeroCount = 0;
                    foreach (var count in targetCity.soldierCounts.Values)
                    {
                        if (count > 0)
                            nonZeroCount++;
                    }
                    
                    if (nonZeroCount > 1 && winSideSoldiers > 0)
                        helpCcount[otherCityId][winSide] += 2;
                }
            }
        }
        return mark;
    }

    private void CheckTurn()
    {
        if(PlayerManager.Instance.IsAllFinish())
        {
            StartCoroutine(CalculateCityScores());
            return;
        }

        int turn = PlayerManager.Instance.GetTurn();
        var nowPlayer = PlayerManager.Instance.GetPlayerData(turn + 1);

        if(TrayController.Instance.GetSoldierLeft(turn + 1) == 0)
        {
            if (nowPlayer.Sword == 0)
            {
                nowPlayer.Sword = swordLeft;
                swordLeft--;
            }
            
        }

        // 隐藏所有CheckButtons
        foreach (var button in CheckButtons)
        {
            if (button != null)
            {
                button.gameObject.SetActive(false);
            }
        }

        PlayerManager.Instance.NextTurn();
        turn = PlayerManager.Instance.GetTurn();
        nowPlayer = PlayerManager.Instance.GetPlayerData(turn + 1);

        if(TrayController.Instance.GetSoldierLeft(turn + 1) > 0)
        {
            if(nowPlayer.IsAI)
            {
                DiceGroup diceGroup = DiceGObj.GetComponent<DiceGroup>();
                AIRollDiceAndProcessResults(turn + 1, diceGroup);

            }
            else
            {
                RollButton.gameObject.SetActive(true);
            }
        }
        else
        {
            CheckTurn();
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
        for (int i = 0; i < FinalMarkText.Length; i++)
        {
            FinalMarkText[i].gameObject.SetActive(true);
            var player = PlayerManager.Instance.GetPlayerData(i + 1);

            FinalMarkText[i].text = player.Name + " 得分: " + player.Sword;
            FinalMarkText[i].color = player.Color;
        }

        for (int cityId = 2; cityId <= 12; cityId++)
        {
            var cityController = FindCityById(cityId);

            if(cityController.NoSoldier())
            {
                cityController.FlashAndShrink(0);
                yield return new WaitForSeconds(2f);
                continue;
            }

            int winSide = 0;
        // 过滤掉 count 为 0 的项并排序
            var sortedPlayers = new List<KeyValuePair<int, PlayerData>>();
            foreach (var kv in cityController.soldierCounts)
            {
                if (kv.Value > 0)
                {
                    PlayerData playerData = PlayerManager.Instance.GetPlayerData(kv.Key);
                    sortedPlayers.Add(new KeyValuePair<int, PlayerData>(kv.Key, playerData));
                }
            }

            sortedPlayers = sortedPlayers.OrderByDescending(kv => cityController.soldierCounts[kv.Key])
                                       .ThenByDescending(kv => kv.Value.Sword)
                                       .ToList();

            foreach (var entry in sortedPlayers)
            {
                Debug.Log($"After Key: {entry.Key}, Name: {entry.Value.Name}, Value: {entry.Value.Mark}");
            }

            if (sortedPlayers.Count >= 1)
            {
                sortedPlayers[0].Value.Mark += cityController.CityID;
                winSide = sortedPlayers[0].Key;
            }
            Debug.Log("winSide " + winSide);
            if (sortedPlayers.Count >= 2)
                sortedPlayers[1].Value.Mark += cityController.CityID / 2;

            cityController.FlashAndShrink(winSide);
            SceneController.Instance.PlaySound("Sounds/sword");
            // 遍历联通的城市，派遣援军
            for (int otherCityId = 2; otherCityId <= 12; otherCityId++)
            {
                if (otherCityId > cityId && IsDirectlyConnected(cityId - 2, otherCityId - 2))
                {
                    Debug.Log($"城市{cityId}和城市{otherCityId}直接联通");
                    var targetCity = FindCityById(otherCityId);
                    int winSideSoldiers = targetCity.soldierCounts.ContainsKey(winSide) ? targetCity.soldierCounts[winSide] : 0;
                    int nonZeroCount = 0;
                    foreach (var count in targetCity.soldierCounts.Values)
                    {
                        if (count > 0)
                            nonZeroCount++;
                    }
                    
                    if (nonZeroCount > 1 && winSideSoldiers > 0)
                        targetCity.AddSoldierHelp(cityController.GetCenterPos(), winSide, 2); //派遣2个志愿士兵
                }
            }

            for (int i = 0; i < FinalMarkText.Length; i++)
            {
                var player = PlayerManager.Instance.GetPlayerData(i + 1);
                FinalMarkText[i].text = player.Name + " 得分: " + player.Sword + "+" + player.Mark;
            }
            
            yield return new WaitForSeconds(3f);
        }
    }

}
                 