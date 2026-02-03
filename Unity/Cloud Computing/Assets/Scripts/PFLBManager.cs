using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
public class PFLBManager : MonoBehaviour
{
    [SerializeField] TMP_InputField if_score;
    [SerializeField] TMP_Text t_msg, t_leaderboard;

    [SerializeField] TMP_Text XPDisplay;
    [SerializeField] TMP_InputField XPInput;
    public void OnSubmitScore()
    {
        var updStatReq = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate
                {
                    StatisticName="highscore",
                    Value=int.Parse(if_score.text)
                }
            }
        };
        try
        {
            PlayFabClientAPI.UpdatePlayerStatistics(updStatReq, UpdStatSucc, OnError);
        }
        catch (System.Exception e)
        {
            t_msg.text = "Exception:" + e.Message;
        }
    }
    void UpdStatSucc(UpdatePlayerStatisticsResult result)
    { //on success
        t_msg.text = "Score [" + if_score.text + "] Submitted!";
    }
    void OnError(PlayFabError error)
    { //if unsuccessful
        t_msg.text = "Error Submitting Score!" + error.GenerateErrorReport();
    }

    public void OnGetLeaderboard()
    { //to get leaderboard
        var getLBReq = new GetLeaderboardRequest
        { //get leaderboard request
            StatisticName = "highscore", //leaderboard name
            StartPosition = 0, //from top
            MaxResultsCount = 10 //top 10
        };
        try
        {
            PlayFabClientAPI.GetLeaderboard(getLBReq, GetLBSucc, OnError);
        }
        catch (System.Exception e)
        {
            t_msg.text = "Exception:" + e.Message;
        }
    }
    void GetLBSucc(GetLeaderboardResult result)
    { //on success
        t_msg.text = "Leaderboard Retrieved!";
        t_leaderboard.text = "--- Leaderboard ---\n";
        foreach (var item in result.Leaderboard)
        {
            t_leaderboard.text += item.Position + 1 + ")" + item.DisplayName + " - " + item.StatValue + "\n";
        }
    }

    public void ClientGetTitleData()
    { //using arrow function method to reduce code
        PlayFabClientAPI.GetTitleData(new GetTitleDataRequest(),
        result => {
            if (result.Data == null || !result.Data.ContainsKey("MOTD")) Debug.Log("No MOTD found");
            else Debug.Log("MOTD: " + result.Data["MOTD"]);
        },
        error => {
            Debug.Log("Got error getting titleData:");
            Debug.Log(error.GenerateErrorReport());
        }
        );
    }
    public void SetUserData()
    {
        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>()
            {
                {"XP", XPInput.text.ToString() }
            }
        },
        result => Debug.Log("Successfully updated user data"),
        error =>
        {
            Debug.Log("Got error setting user data XP");
            Debug.Log(error.GenerateErrorReport());
        });
    }
    public void GetUserData()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest()
        {

        },
        result =>
        {
            Debug.Log("Got user data:");
            if (result.Data == null || !result.Data.ContainsKey("XP")) Debug.Log("No XP");
            else
            {
                    Debug.Log("XP: " + result.Data["XP"].Value);
                    XPDisplay.text = "XP:" + result.Data["XP"].Value;
            }
        },
        (error)=>
        {
            Debug.Log("Got error retrieving user data:");
            Debug.Log(error.GenerateErrorReport());
        }
        );
    }

    public void OnGetAroundMeLeaderboard()
    {
        PlayFabClientAPI.GetLeaderboardAroundPlayer(
            new GetLeaderboardAroundPlayerRequest
            {
                StatisticName = "highscore",
                MaxResultsCount = 5
            },
            result => 
            {
                t_leaderboard.text = "--- Around Me ---\n";

                foreach (var item in result.Leaderboard)
                {
                    string onerow = (item.Position + 1) + ")" + item.DisplayName + " - " + item.StatValue + "\n";
                    t_leaderboard.text += onerow;
                }
            },
            OnError
        );
    }
}