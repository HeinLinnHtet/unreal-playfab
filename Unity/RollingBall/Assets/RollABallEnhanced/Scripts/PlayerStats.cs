using UnityEngine;

[System.Serializable]
public class PlayerStats
{
    public string username;
    public int xp;
    public int level;
    public int cash;

    public static PlayerStats CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<PlayerStats>(jsonString);
    }

    // Given JSON input:
    // {"name":"Dr Charles","lives":3,"health":0.8}
    // this example will return a PlayerInfo object with
    // name == "Dr Charles", lives == 3, and health == 0.8f.
}

//https://docs.unity3d.com/ScriptReference/JsonUtility.FromJson.html
