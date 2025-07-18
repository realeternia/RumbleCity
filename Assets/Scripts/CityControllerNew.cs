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
    public Dictionary<int, int> soldierCounts = new Dictionary<int, int>();

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
        for(int side = 1; side <= PlayerManager.Instance.playerCount; side++)
            soldierCounts[side] = 0;
        // AddSoldierHelp(Vector3.zero, 1, 1);
        // AddSoldierHelp(Vector3.zero, 2, 1);
        // AddSoldierHelp(Vector3.zero, 3, 1);
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

        if(cubes.Count == 0)
            return;

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
            Vector3 offset = new Vector3(Mathf.Cos(angle), 0.3f, Mathf.Sin(angle)) * radius;
            cube.transform.position = spawnPosition + offset;
            cube.transform.rotation = Quaternion.Euler(Vector3.zero);

            // 让 cube 方块作为当前对象的子对象
            cube.transform.SetParent(cylinder.transform);
            cube.transform.localScale = new Vector3(0.2f, 0.5f, 0.2f);
        }

        // 根据士兵数量设置Text颜色
        int maxCount = 0;
        List<int> maxSides = new List<int>();
        
        foreach (var pair in soldierCounts)
        {
            if (pair.Value > maxCount)
            {
                maxCount = pair.Value;
                maxSides.Clear();
                maxSides.Add(pair.Key);
            }
            else if (pair.Value == maxCount)
            {
                maxSides.Add(pair.Key);
            }
        }

        if (maxSides.Count == 1)
        {
            Text.color = PlayerManager.Instance.GetPlayerData(maxSides[0]).Color;
        }
        else
        {
            Text.color = Color.gray;
        }

        foreach(var cube in cubes)
        {
            StartCoroutine(ShrinkObj(cube, 2f));
        }

        StartCoroutine(FlashObj(cylinder, Text.color, 2f));
        SceneController.Instance.PlaySound("Sounds/wood");
    }

    public Vector3 GetCenterPos()
    {
        GameObject cylinder = gameObject;
        MeshRenderer cylinderRenderer = cylinder.GetComponent<MeshRenderer>();

        // 计算 Cylinder 上平面的位置
        Bounds bounds = cylinderRenderer.bounds;
        return bounds.center + Vector3.up * bounds.extents.y;  
    }
    
    public void AddSoldierHelp(Vector3 pos, int side, int count)
    {
        // 获取当前对象（假设为 Cylinder）的信息
        GameObject cylinder = gameObject;
        MeshRenderer cylinderRenderer = cylinder.GetComponent<MeshRenderer>();
        if (cylinderRenderer == null) return;

        // 计算 Cylinder 上平面的位置
        Bounds bounds = cylinderRenderer.bounds;
        Vector3 spawnPosition = bounds.center + Vector3.up * bounds.extents.y;

        if(pos != Vector3.zero)
            spawnPosition = pos;

        for (int i = 0; i < count; i++)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.localScale = new Vector3(10, 3, 10);
            cube.layer = LayerMask.NameToLayer("Board");
            cube.GetComponent<MeshRenderer>().material = Resources.Load<Material>("Materials/Side" + side);

            float angle = 2 * Mathf.PI * soldierTotal / 12;
            soldierCounts[side]++;
            soldierTotal++;
            float radius = bounds.extents.x * 0.8f; // 使用 Cylinder 半径的 80% 作为排列半径
            Vector3 offset = new Vector3(Mathf.Cos(angle), 0.3f, Mathf.Sin(angle)) * radius;
            Vector3 targetPosition = bounds.center + Vector3.up * bounds.extents.y + offset;
            
            cube.transform.position = spawnPosition;
            
            StartCoroutine(MoveCube(cube, targetPosition, 1.5f));
            cube.transform.rotation = Quaternion.Euler(Vector3.zero);

            IEnumerator MoveCube(GameObject cube, Vector3 targetPosition, float duration)
            {
                Vector3 startPosition = cube.transform.position;
                float elapsedTime = 0f;
                
                while (elapsedTime < duration)
                {
                    cube.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }
                
                cube.transform.position = targetPosition;
            }

            // 让 cube 方块作为当前对象的子对象
            cube.transform.SetParent(cylinder.transform);
        }
    }

    public void FlashAndShrinkCity(int side)
    {
        gameObject.GetComponent<MeshRenderer>().material.SetFloat("_BlendMode", 0);
        if (side != 0)
        {
            Text.color = PlayerManager.Instance.GetPlayerData(side).Color;
        }
        else
        {
            Text.color = Color.gray;
        }
        GameObject cylinder = gameObject;
        StartCoroutine(FlashObj(cylinder, Text.color, 2f));
    }

    IEnumerator FlashObj(GameObject target, Color color, float duration)
    {
        float elapsed = 0f;
        Color originalColor = target.GetComponent<MeshRenderer>().material.color;
        target.GetComponent<MeshRenderer>().material.SetFloat("_BlendMode", 0.5f);

        while (elapsed < duration)
        {
            float t = Mathf.PingPong(elapsed * 4f, 1f);
            target.GetComponent<MeshRenderer>().material.color = Color.Lerp(originalColor, color, t);
            
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        target.GetComponent<MeshRenderer>().material.SetFloat("_BlendMode", 0);
        target.GetComponent<MeshRenderer>().material.color = originalColor;
    }

    IEnumerator ShrinkObj(GameObject target, float duration)
    {
        float elapsed = 0f;
        Vector3 originalScale = target.transform.localScale;
        while (elapsed < duration)
        {
            float t = Mathf.PingPong(elapsed * 4f, 1f);
            target.transform.localScale = originalScale * (0.9f + 0.3f * t);
            
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        target.transform.localScale = originalScale;
    }    

    public bool NoSoldier()
    {
        foreach(var sold in soldierCounts)
        {
            if(sold.Value > 0)
            {
                return false;
            }
        }
        return true;
    }
}
