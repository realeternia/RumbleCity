using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CityControllerNew : MonoBehaviour, IPointerClickHandler
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SendABC(string msg)
    {    Debug.Log("Scene Obj SendABC " + msg);
    }

    public void OnPointerClick(PointerEventData eventData)
    {    
        Debug.Log("Scene Obj OnPointerClick");
    }
}
