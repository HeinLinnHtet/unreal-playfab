using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
public class PFUserMgt : MonoBehaviour //EK: Unity Code to Register new user in playfab
{
    [SerializeField] TMP_Text msgbox;
    [SerializeField] TMP_Text usernamebox;
    [SerializeField] TMP_InputField if_regdisplayname, if_regusername, if_regemail, if_regpassword;
    [SerializeField] TMP_InputField if_logusername, if_logpassword;

    ///////////////////////////////// HEIN LINN HTET 243962J SHOW USERNAME/GUEST ID AFTER LOGIN /////////////////////////////////
    public void Start()
    {
        if (usernamebox != null)
        {
            PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest(), OnGetAccountSuccess, OnError);
        }
    }

    void OnGetAccountSuccess(GetAccountInfoResult result)
    {
        if (string.IsNullOrEmpty(result.AccountInfo.Username))
        {
            usernamebox.text = "Guest: " + result.AccountInfo.CustomIdInfo.CustomId;
        }
        else
        {
            usernamebox.text = "Name: " + result.AccountInfo.TitleInfo.DisplayName;
        }
    }

    ///////////////////////////////// HEIN LINN HTET 243962J  REGISTER /////////////////////////////////
    public void OnBtnRegUser()
    { //reg button click handler
        var reqReq = new RegisterPlayFabUserRequest
        { //create request objct
            Email = if_regemail.text, //object fields
            Password = if_regpassword.text,
            Username = if_regusername.text //no comma
        };
        //execute request by calling reg playfab user api
        PlayFabClientAPI.RegisterPlayFabUser(reqReq, OnRegSucc, OnError);
    }
    void OnRegSucc(RegisterPlayFabUserResult r)
    { //function to handle success
        msgbox.text = "Registration Success! Assigned ID:" + r.PlayFabId;
        SetDisplayName(); //call set display name function
    }
    void OnError(PlayFabError e)
    { //function to handle failure
        msgbox.text = "Error:" + e.GenerateErrorReport();
    }
    public void SetDisplayName()
    { //set display name button click handler
        var reqReq = new UpdateUserTitleDisplayNameRequest
        { //create request object
            DisplayName = if_regdisplayname.text //object field
        };
        //execute request by calling update user title display name api
        PlayFabClientAPI.UpdateUserTitleDisplayName(reqReq, OnSetDispNameSucc, OnError);
    }
    void OnSetDispNameSucc(UpdateUserTitleDisplayNameResult r)
    { //function to handle success
        msgbox.text += "\nDisplay Name Set!";
    }

    ///////////////////////////////// HEIN LINN HTET 243962J  LOGIN /////////////////////////////////
    public void OnBtnLogin()
    {
        string loginInput = if_logusername.text;

        if (checkEmail(loginInput))
        {
            var emailReq = new LoginWithEmailAddressRequest
            {
                Email = loginInput,
                Password = if_logpassword.text
            };
            PlayFabClientAPI.LoginWithEmailAddress(emailReq, OnLoginSucc, OnError);
        }
        else
        {
            var usernameReq = new LoginWithPlayFabRequest
            {
                Username = loginInput,
                Password = if_logpassword.text
            };
            PlayFabClientAPI.LoginWithPlayFab(usernameReq, OnLoginSucc, OnError);
        }
    }
    bool checkEmail(string input)
    {
        if (!input.Contains("@")) return false;

        string[] parts = input.Split('@');
        if (parts.Length != 2) return false;

        return parts[1].Contains(".");
    }
    void OnLoginSucc(LoginResult r)
    {
        msgbox.text = "Login Success! Last login time:" + r.LastLoginTime;
        SceneManager.LoadScene("LoggedInScene");
    }

    ///////////////////////////////// HEIN LINN HTET 243962J  LOGOUT /////////////////////////////////
    public void OnBtnLogout()
    {
        PlayFabClientAPI.ForgetAllCredentials();
        SceneManager.LoadScene("SampleScene");
    }

    ///////////////////////////////// HEIN LINN HTET 243962J  PW VISIBILITY /////////////////////////////////
    public void ToggleRegPassword()
    {
        if_regpassword.contentType = (if_regpassword.contentType == TMP_InputField.ContentType.Password)
            ? TMP_InputField.ContentType.Standard
            : TMP_InputField.ContentType.Password;
        if_regpassword.ForceLabelUpdate();
    }

    public void ToggleLogPassword()
    {
        if_logpassword.contentType = (if_logpassword.contentType == TMP_InputField.ContentType.Password)
            ? TMP_InputField.ContentType.Standard
            : TMP_InputField.ContentType.Password;
        if_logpassword.ForceLabelUpdate();
    }

    ///////////////////////////////// HEIN LINN HTET 243962J  RECOVER PW /////////////////////////////////
    public void RecoverPassword()
    {
        string loginInput = if_logusername.text;

        if (!checkEmail(loginInput))
        {
            msgbox.text = "Error: Password recovery only works with email addresses. Please enter your email.";
            return;
        }

        var reqReq = new SendAccountRecoveryEmailRequest
        {
            Email = loginInput,
            TitleId = PlayFabSettings.TitleId
        };

        PlayFabClientAPI.SendAccountRecoveryEmail(reqReq, OnRecoverySucc, OnError);
    }

    void OnRecoverySucc(SendAccountRecoveryEmailResult r)
    {
        msgbox.text = "Password reset email sent!";
    }

    ///////////////////////////////// HEIN LINN HTET 243962J  GUEST LOGIN /////////////////////////////////

    public void GuestLogin()
    {
        string customId = IDgetter();

        var guestReq = new LoginWithCustomIDRequest
        {
            CustomId = customId,
            CreateAccount = true
        };

        PlayFabClientAPI.LoginWithCustomID(guestReq, OnLoginSucc, OnError);
    }

    string IDgetter()
    {
        string guestId = PlayerPrefs.GetString("GuestID", "");

        if (string.IsNullOrEmpty(guestId))
        {
            guestId = RandomidGenerator();
            PlayerPrefs.SetString("GuestID", guestId);
            PlayerPrefs.Save();
        }
        return guestId;
    }

    string RandomidGenerator()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        char[] id = new char[8];

        for (int i = 0; i < 8; i++)
        {
            id[i] = chars[Random.Range(0, chars.Length)];
        }

        return new string(id);
    }
}

