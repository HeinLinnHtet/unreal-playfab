using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FriendsManagement : MonoBehaviour
{
    [SerializeField] Text txtFrdList, leaderboarddisplay;
    [SerializeField] TMP_InputField tgtFriend, tgtunfrnd;
    List<FriendInfo> _friends = null;
    enum FriendIdType { PlayFabId, Username, Email, DisplayName };

    ///////////////////////////////// HEIN LINN HTET 243962J  FRIEND /////////////////////////////////
    void DisplayFriends(List<FriendInfo> friendsCache)
    {
        txtFrdList.text = "";
        friendsCache.ForEach(f => {
            Debug.Log(f.FriendPlayFabId + "," + f.TitleDisplayName);
            txtFrdList.text += f.TitleDisplayName + "[" + f.FriendPlayFabId + "]\n";
            if (f.Profile != null) Debug.Log(f.FriendPlayFabId + " - " + f.Profile.DisplayName);
        });
    }
    void DisplayPlayFabError(PlayFabError error)
    {
        Debug.Log(error.GenerateErrorReport());
    }
    void DisplayError(string error) { Debug.LogError(error); }

    public void GetFriends()
    {
        PlayFabClientAPI.GetFriendsList(new GetFriendsListRequest
        {
            // ExternalPlatformFriends = false,
            // XboxToken = null
        }, result => {
            _friends = result.Friends;
            DisplayFriends(_friends); // triggers your UI
        }, DisplayPlayFabError);
    }
    void AddFriend(FriendIdType idType, string friendId)
    {
        var request = new AddFriendRequest();
        switch (idType)
        {
            case FriendIdType.PlayFabId:
                request.FriendPlayFabId = friendId;
                break;
            case FriendIdType.Username:
                request.FriendUsername = friendId;
                break;
            case FriendIdType.Email:
                request.FriendEmail = friendId;
                break;
            case FriendIdType.DisplayName:
                request.FriendTitleDisplayName = friendId;
                break;
        }
        // Execute request and update friends when we are done
        PlayFabClientAPI.AddFriend(request, result => {
            Debug.Log("Friend added successfully!");
        }, DisplayPlayFabError);
    }

    ///////////////////////////////// HEIN LINN HTET 243962J  FRIEND ADD REMOVE /////////////////////////////////
    public void OnAddFriend()
    {
        AddFriend(FriendIdType.DisplayName, tgtFriend.text);
    }

    public void RemoveFriend(FriendInfo friendInfo)
    {
        PlayFabClientAPI.RemoveFriend(new RemoveFriendRequest
        {
            FriendPlayFabId = friendInfo.FriendPlayFabId
        }, result => {
            _friends.Remove(friendInfo);
        }, DisplayPlayFabError);
    }
    public void OnUnFriend()
    {
        RemoveFriend(tgtunfrnd.text);
    }
    void RemoveFriend(string pfid)
    {
        var req = new RemoveFriendRequest
        {
            FriendPlayFabId = pfid
        };
        PlayFabClientAPI.RemoveFriend(req
        , result => {
            Debug.Log("unfriend!");
        }, DisplayPlayFabError);
    }

    public void OnGetFriendLB()
    {
        PlayFabClientAPI.GetFriendLeaderboard(
        new GetFriendLeaderboardRequest { StatisticName = "highscore", MaxResultsCount = 10 },
        r => {
            leaderboarddisplay.text = "--- Friends LB ---\n";
            foreach (var item in r.Leaderboard)
            {
                string onerow = item.Position + 1 + ")" + item.DisplayName + " - " + item.StatValue + "\n";
                Debug.Log(onerow);
                leaderboarddisplay.text += onerow;
            }
        }, DisplayPlayFabError);
    }
}
