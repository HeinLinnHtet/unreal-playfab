using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.SceneManagement;
// Include the namespace required to use Unity UI
using UnityEngine.UI;
using UnityEngine.Windows;

public class PlayerController : MonoBehaviour {
	
	// Create public variables for player speed, and for the Text UI game objects
	public float speed;
	public Text countText;
	public Text winText;
	public Text timerText;
	public TextMeshProUGUI TMPUGUI_playerStats;
	public float countDownTimer=0;
	// Create private references to the rigidbody component on the player, and the count of pick up objects picked up so far
	private Rigidbody rb;
	private int count;
	bool isRunning=true;
	public GameObject winPanel;
	[SerializeField] InputField pname;
	[SerializeField] Text txtsb;
	[SerializeField] AudioSource audsrc;
	[SerializeField] TextMeshProUGUI TMPUGUI_LastScore;

    private string gameVersion;
    private string gameName;
    [SerializeField] TMP_Text gameIntro;

    // At the start of the game..
    void Start ()
	{
		// Assign the Rigidbody component to our private rb variable
		rb = GetComponent<Rigidbody>();

		// Set the count to zero 
		count = 0;

		// Run the SetCountText function to update the UI (see below)
		SetCountText ();

		// Set the text property of our Win Text UI to an empty string, making the 'You Win' (game over message) blank
		winText.text = "";
		winPanel.SetActive(false);

		RefreshPlayerStats();
		speed=GlobalStuffs.level*5+5;

        GetUserData();
        GetTitleData();
    }
    void RefreshPlayerStats()
    {
        TMPUGUI_playerStats.text = "Player Stats\nName:" + GlobalStuffs.username + "\nLevel:" + GlobalStuffs.level + "\nXP:" + GlobalStuffs.xp + "\nCoins:" + GlobalStuffs.cash;
    }

    // Each physics step..
    void FixedUpdate ()
	{
		if(!isRunning)return;
		// Set some local float variables equal to the value of our Horizontal and Vertical Inputs
		float moveHorizontal = UnityEngine.Input.GetAxis ("Horizontal");
		float moveVertical = UnityEngine.Input.GetAxis ("Vertical");

		// Create a Vector3 variable, and assign X and Z to feature our horizontal and vertical float variables above
		Vector3 movement = new Vector3 (moveHorizontal, 0.0f, moveVertical);

		// Add a physical force to our Player rigidbody using our 'movement' Vector3 above, 
		// multiplying it by 'speed' - our public player speed that appears in the inspector
		rb.AddForce (movement * speed);
	}

    void Update(){
		if(!isRunning)return;
            countDownTimer-=Time.deltaTime;
            SetTimerText();
        
    }

	// When this game object intersects a collider with 'is trigger' checked, 
	// store a reference to that collider in a variable named 'other'..
	void OnTriggerEnter(Collider other) 
	{
		// ..and if the game object we intersect has the tag 'Pick Up' assigned to it..
		if (other.gameObject.CompareTag ("Pick Up"))
		{
			// Make the other game object (the pick up) inactive, to make it disappear
			other.gameObject.SetActive (false);

			// Add one to the score variable 'count'
			count = count + 1;
			GlobalStuffs.xp+=1;
			if(GlobalStuffs.xp>GlobalStuffs.level*10){
				GlobalStuffs.level++;
				GlobalStuffs.xp=0;
			}

			// Run the 'SetCountText()' function (see below)
			SetCountText ();
			audsrc.Play();
			RefreshPlayerStats();
		}
		if (other.gameObject.CompareTag ("Coin"))
		{
			// Make the other game object (the pick up) inactive, to make it disappear
			other.gameObject.SetActive (false);

			// Add one to the score variable 'count'
			GlobalStuffs.cash+=5;

			// Run the 'SetCountText()' function (see below)
			//SetCountText ();
			audsrc.Play();
			RefreshPlayerStats();
		}
	}

	// Create a standalone function that can update the 'countText' UI and check if the required amount to win has been achieved
	void SetCountText()
	{
		// Update the text field of our 'countText' variable
		countText.text = "Score: " + count.ToString ();

		// Check if our 'count' is equal to or exceeded 12
		if (count >= 12) 
		{
			// Set the text value of our 'winText'
			winText.text = "You Win!";
			GameOver();
		}
	}
	void SetTimerText(){
        timerText.text="Time Left:"+countDownTimer.ToString("0.0");
		if(countDownTimer<=0){
			winText.text = "Time over!";
			GameOver();
		}
    }
	void GameOver(){
		OnSubmitScore();
        SetUserData();
        isRunning =false;
		winPanel.SetActive(true);
		GlobalStuffs.xp+=count;
		rb.linearVelocity=Vector3.zero;
		rb.angularVelocity=Vector3.zero;
		TMPUGUI_LastScore.text="LastScore:"+count;
        //StartCoroutine(GlobalStuffs.DoSendScore(GlobalStuffs.username, count));
    }

    ///////////////////////////////// HEIN LINN HTET 243962J  LEADERBOARD /////////////////////////////////
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
            //t_msg.text = "Exception:" + e.Message;
        }
    }

    void GetLBSucc(GetLeaderboardResult result)
    { //on success
        //t_msg.text = "Leaderboard Retrieved!";
        txtsb.text = "--- Leaderboard ---\n";
        foreach (var item in result.Leaderboard)
        { //iterate through each leaderboard item
            txtsb.text += item.Position + 1 + ". " + item.DisplayName + " : " + item.StatValue + "\n";
        }
    }

    void OnError(PlayFabError error)
    { //if unsuccessful
        //t_msg.text = "Error Submitting Score!" + error.GenerateErrorReport();
    }

    public void Replay(){
		//StartCoroutine(GlobalStuffs.DoSendScore(pname.text,count));
		//winPanel.SetActive(false);
		//Restart();
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		
	}
    public void OnDeleteUser()
    {

    }


    // public void Restart(){

    // 	StartCoroutine(GlobalStuffs.GetScoreBoard(txtsb));
    // 	isRunning=true;
    // 	count=0;
    // 	countDownTimer=10;
    // 	SetCountText();
    // 	SetTimerText();
    // 	transform.position=new Vector3(0f,0.5f,0f);
    // 	winText.text="";
    // 	 SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    // }

    ///////////////////////////////// HEIN LINN HTET 243962J  CLEAR SCORE /////////////////////////////////
    public void ClearScores()
    {
        var updStatReq = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
        {
            new StatisticUpdate
            {
                StatisticName = "highscore",
                Value = 0
            }
        }
        };

        PlayFabClientAPI.UpdatePlayerStatistics(updStatReq,
            result =>
            {
                Debug.Log("Highscore cleared!");
            },
            error =>
            {
                Debug.Log("Error clearing highscore: " + error.GenerateErrorReport());
            }
        );
    }

    public void BacktoMain()
    {
        GlobalStuffs.level = 1;
        GlobalStuffs.xp = 0;
        GlobalStuffs.cash = 0;
        GlobalStuffs.username = "";
        SceneManager.LoadScene("LoginScene");
	}

    ///////////////////////////////// HEIN LINN HTET 243962J  LEADERBOARD /////////////////////////////////
    public void OnSubmitScore()
    {
        var updStatReq = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate
                {
                    StatisticName="highscore",
                    Value=count
                }
            }
        };
        try
        {
            PlayFabClientAPI.UpdatePlayerStatistics(updStatReq, UpdStatSucc, OnError);
        }
        catch (System.Exception e)
        {
            //t_msg.text = "Exception:" + e.Message;
        }
    }
    void UpdStatSucc(UpdatePlayerStatisticsResult result)
    { //on success
        //t_msg.text = "Score [" + if_score.text + "] Submitted!";
    }

    ///////////////////////////////// HEIN LINN HTET 243962J  USER DATA /////////////////////////////////
    public void SetUserData()
    {
        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>()
        {
            {"level", GlobalStuffs.level.ToString()},
            {"xp", GlobalStuffs.xp.ToString()}
        }
        },
        result => Debug.Log("Updated user data"),
        error =>
        {
            Debug.Log("Error setting user data: " + error.GenerateErrorReport());
        });

        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(),
        result =>
        {
            int currentCoins = result.VirtualCurrency.ContainsKey("CN") ? result.VirtualCurrency["CN"] : 0;
            int coinDifference = GlobalStuffs.cash - currentCoins;

            if (coinDifference > 0)
            {
                PlayFabClientAPI.AddUserVirtualCurrency(new AddUserVirtualCurrencyRequest()
                {
                    VirtualCurrency = "CN",
                    Amount = coinDifference
                },
                r => Debug.Log("Added " + coinDifference + " coins"),
                OnError);
            }
            else if (coinDifference < 0)
            {
                PlayFabClientAPI.SubtractUserVirtualCurrency(new SubtractUserVirtualCurrencyRequest()
                {
                    VirtualCurrency = "CN",
                    Amount = -coinDifference
                },
                r => Debug.Log("Subtracted " + (-coinDifference) + " coins"),
                OnError);
            }
            else
            {
                Debug.Log("Coins unchanged");
            }
        },
        OnError);
    }
    ///////////////////////////////// HEIN LINN HTET 243962J  USER DATA /////////////////////////////////
    public void GetUserData()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(),
        result =>
        {
            // Get Level
            if (result.Data == null || !result.Data.ContainsKey("level"))
            {
                Debug.Log("No level data");
                GlobalStuffs.level = 1;
            }
            else
            {
                Debug.Log("Level: " + result.Data["level"].Value);
                GlobalStuffs.level = int.Parse(result.Data["level"].Value);
            }

            if (result.Data == null || !result.Data.ContainsKey("xp"))
            {
                Debug.Log("No xp data");
                GlobalStuffs.xp = 0;
            }
            else
            {
                Debug.Log("XP: " + result.Data["xp"].Value);
                GlobalStuffs.xp = int.Parse(result.Data["xp"].Value);
            }

            RefreshPlayerStats();
            speed = GlobalStuffs.level * 5 + 5;
        },
        error =>
        {
            Debug.Log("Error retrieving user data");
            Debug.Log(error.GenerateErrorReport());
        });

        GetVirtualCurrency();
    }
    public void GetVirtualCurrency()
    {
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(),
        result =>
        {
            if (result.VirtualCurrency.ContainsKey("CN"))
            {
                GlobalStuffs.cash = result.VirtualCurrency["CN"];
                Debug.Log("Coins: " + GlobalStuffs.cash);
            }
            else
            {
                GlobalStuffs.cash = 0;
                Debug.Log("No coins found");
            }

            RefreshPlayerStats();
        },
        error =>
        {
            Debug.Log("Error retrieving coins: " + error.GenerateErrorReport());
            GlobalStuffs.cash = 0;
            RefreshPlayerStats();
        });
    }

    ///////////////////////////////// HEIN LINN HTET 243962J  TITLE DATA /////////////////////////////////
    public void GetTitleData()
    {
        PlayFabClientAPI.GetTitleData(new GetTitleDataRequest(),
        result =>
        {
            if (result.Data == null || result.Data.Count == 0)
            {
                Debug.Log("No title data found");
            }
            else
            {
                if (result.Data.ContainsKey("GameVersion"))
                {
                    gameVersion = result.Data["GameVersion"];
                }

                if (result.Data.ContainsKey("GameName"))
                {
                    gameName =  result.Data["GameName"];
                }
            }

            gameIntro.text = gameName + " - " + gameVersion;
        },
        error =>
        {
            Debug.Log("Error retrieving title data:");
            Debug.Log(error.GenerateErrorReport());
        });
    }
}