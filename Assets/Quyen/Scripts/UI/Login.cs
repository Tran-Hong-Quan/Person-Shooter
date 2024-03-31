using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class Login : MonoBehaviour
{
    private static Login instance;
    public static Login Instance => instance;

    //private readonly string ULI = "http://localhost/UnityData/LogIn.php";
    private readonly string ULI = "https://tempquan.000webhostapp.com/Login.php";

    [SerializeField] private UIAnimation signInBoard;
    [SerializeField] private UIAnimation accountBoard;

    [SerializeField] private TMP_InputField username;
    [SerializeField] private TMP_InputField password;

    public TMP_InputField Username => username;

    private void Awake()
    {
        instance = this;
    }

    public void SignIn()
    {
        StartCoroutine(LoginUser());
    }

    IEnumerator LoginUser()
    {
        WWWForm form = new();
        form.AddField("userName", this.username.text);
        form.AddField("userPass", this.password.text);

        using UnityWebRequest www = UnityWebRequest.Post(ULI, form);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(www.error);
        }
        else
        {
            string mess = www.downloadHandler.text;
            if (mess.Equals("Success"))
            {
                signInBoard.Hide();
                Account.Instance.Information();
                accountBoard.Show();
            }
        }
    }
}
