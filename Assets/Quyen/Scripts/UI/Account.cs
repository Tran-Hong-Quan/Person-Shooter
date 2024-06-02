using HongQuan;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class Account : MonoBehaviour
{
    private static Account instance;
    public static Account Instance => instance;

    private readonly string ULI = "https://tempquan.000webhostapp.com/Infor.php";

    [SerializeField] private AccountInformation[] accountInformation;

    [SerializeField] TMP_Text userNameTMP;
    [SerializeField] TMP_Text inforTMP;

    [SerializeField] UIAnimation accountInforBoard;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        //var t = JsonHelper.ToJsonArray(accountInformation);
        //Debug.Log(t);
        //accountInformation = JsonHelper.FromJsonArray<AccountInformation>(t);
    }

    public void Information(string userName)
    {
        MainMenu.instance.loadingUI.gameObject.SetActive(true);
        StartCoroutine(ShowInformation(userName));
    }

    public void SignOut()
    {
        PlayerPrefs.SetString("UserName", "");
    }

    IEnumerator ShowInformation(string userName)
    {
        WWWForm form = new();
        form.AddField("userName", userName);

        using UnityWebRequest www = UnityWebRequest.Post(ULI, form);
        yield return www.SendWebRequest();
        MainMenu.instance.loadingUI.gameObject.SetActive(false);
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(www.error);
        }
        else
        {
            accountInforBoard.Show();
            string jsonArrayString = www.downloadHandler.text;
            //Debug.Log(jsonArrayString);
            accountInformation = JsonHelper.FromJsonArray<AccountInformation>(jsonArrayString);
            UpdateInfor();
        }
    }

    private void UpdateInfor()
    {
        string inforText = "Created Date: "+accountInformation[0].date_created + "<br><br>" +
            "Highest Score: " + accountInformation[0].high_score + "<br><br>" +
            "Email: " + accountInformation[0].email;
        string userNameText = accountInformation[0].user_name;
        PlayerPrefs.SetString("Username", accountInformation[0].user_name);
        userNameTMP.text = userNameText;
        inforTMP.text = inforText;

        PlayerPrefs.SetString("UserName", userNameText);
    }

    private void OnDestroy()
    {
        instance = null;
    }

}

[System.Serializable]
public struct AccountInformation
{
    public string user_name;
    public int high_score;
    public string email;
    public string date_created;
}
