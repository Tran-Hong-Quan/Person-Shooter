using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class Login : MonoBehaviour
{
    private static Login instance;
    public static Login Instance => instance;

    private readonly string ULI = "https://tempquan.000webhostapp.com/LogIn.php";


    [SerializeField] private TMP_InputField username;
    [SerializeField] private TMP_InputField password;

    private UIAnimation UIAnimation;
    [SerializeField] LoadingUI loadingUI;

    public TMP_InputField Username => username;

    private void Awake()
    {
        instance = this;
        UIAnimation = GetComponent<UIAnimation>();
    }

    public void SignIn()
    {
        MainMenu.instance.loadingUI.gameObject.SetActive(true);
        StartCoroutine(LoginUser());
    }

    IEnumerator LoginUser()
    {
        WWWForm form = new();
        form.AddField("userName", this.username.text);
        form.AddField("userPass", this.password.text);

        using UnityWebRequest www = UnityWebRequest.Post(ULI, form);
        yield return www.SendWebRequest();
        MainMenu.instance.loadingUI.gameObject.SetActive(false);
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(www.error);
        }
        else
        {
            string mess = www.downloadHandler.text;
            if (mess.Equals("Success"))
            {
                UIAnimation.Hide();
                Account.Instance.Information(username.text);
            }
        }
        www.Dispose();
    }
}
