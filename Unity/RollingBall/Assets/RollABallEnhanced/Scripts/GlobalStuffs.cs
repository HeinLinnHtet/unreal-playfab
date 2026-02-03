using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;
public static class GlobalStuffs {
    public static string username="GuestPlayer";
    public static int xp=0;
    public static int cash=0;
    public static int level=1;

    
}



//https://docs.unity3d.com/ScriptReference/Networking.UnityWebRequest.Get.html

//create classes and data structure to match the JSON structure
[Serializable]
class OneScore {
    public string username;
    public int score;    
}
[Serializable]
class ScoreList {
    public List<OneScore> scores=new List<OneScore>();
}



