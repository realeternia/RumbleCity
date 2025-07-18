using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class DataManager
{
    public static DataManager Instance{ private set; get; }

    private List<string> cityNames = new List<string>();

    static DataManager()
    {
        Instance = new DataManager();
        Instance.Init();
    }

    // Start is called before the first frame update
    private void Init()
    {
        try
        {
            TextAsset textAsset = Resources.Load<TextAsset>("Datas/cityname");
            if (textAsset != null)
            {
                string[] lines = textAsset.text.Split(new [] {"\r\n", "\r", "\n"}, System.StringSplitOptions.RemoveEmptyEntries);
                foreach (string line in lines)
                {
                    string[] words = line.Split('\t');
                    cityNames.AddRange(words);
                }
                Debug.Log("成功读取城市名称文件，共读取 " + cityNames.Count + " 个名称。");
            }
            else
            {
                Debug.LogError("未找到 citiname.txt 文件，请检查路径。");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("读取城市名称文件时出错：" + e.Message);
        }
    }

    /// <summary>
    /// 获取 n 个不重复的随机城市名字
    /// </summary>
    /// <param name="n">需要获取的名字数量</param>
    /// <returns>包含 n 个不重复随机城市名字的列表</returns>
    public List<string> GetRandomCityNames(int n)
    {
        List<string> result = new List<string>();
        List<string> availableNames = new List<string>(cityNames);
        
        n = Mathf.Min(n, availableNames.Count);
        
        for (int i = 0; i < n; i++)
        {
            int randomIndex = Random.Range(0, availableNames.Count);
            result.Add(availableNames[randomIndex]);
            availableNames.RemoveAt(randomIndex);
        }
        
        return result;
    }
}
