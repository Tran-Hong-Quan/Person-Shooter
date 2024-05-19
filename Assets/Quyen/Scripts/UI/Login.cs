using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class Login : MonoBehaviour
{
    private readonly string ULI = "https://tempquan.000webhostapp.com/LogIn.php";

    [SerializeField] private TMP_InputField username;
    [SerializeField] private TMP_InputField password;
    public TMP_Text message;

    private UIAnimation UIAnimation;
    [SerializeField] LoadingUI loadingUI;

    private void Awake()
    {
        UIAnimation = GetComponent<UIAnimation>();
    }

    public void SignIn()
    {
        MainMenu.instance.loadingUI.gameObject.SetActive(true);
        StartCoroutine(LoginUser());
        Invoke(nameof(DisableLoading), 0.8f);
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
                MainMenu.instance.loadingUI.gameObject.SetActive(true);
                UIAnimation.Hide();
                Account.Instance.Information(username.text);
            }
            else
            {
                message.SetText("Tài khoản hoặc mật khẩu không đúng");
                message.enabled = true;
            }
        }
        www.Dispose();
    }

    private void DisableLoading()
    {
        MainMenu.instance.loadingUI.gameObject.SetActive(false);
    }
}
