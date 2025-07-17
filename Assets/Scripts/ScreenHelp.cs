using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_ANDROID && !UNITY_EDITOR

public class ScreenHelp : MonoBehaviour
{
    AndroidJavaObject activity;
    AndroidJavaObject window;

    void Awake()
    {
        activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
        window = activity.Call<AndroidJavaObject>("getWindow");
    }

    // Start is called before the first frame update
    void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Screen.orientation = ScreenOrientation.LandscapeLeft;

        
        window.Call("addFlags", 128);
    }

    void onDestroy()
    {
        window.Call("clearFlags", 128);
    }
}

#endif