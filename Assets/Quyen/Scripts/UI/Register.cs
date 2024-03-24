using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Register : MonoBehaviour
{
    private readonly string ULI = "http://localhost/UnityData/RegisterUser.php";
    private string username;
    private string password1;
    private string password2;
    private string nickname;

    [Header("Message")]
    [SerializeField] private Text usernameMess;
    [SerializeField] private Text nicknameMess;
    [SerializeField] private Text passwordMess;
    [SerializeField] private Text signinMess;

    private void Start()
    {
        usernameMess.enabled = false;
        nicknameMess.enabled = false;
        passwordMess.enabled = false;
        signinMess.enabled = false;

        usernameMess.text = "Username already exists";
        nicknameMess.text = "Nickname already exists";
        passwordMess.text = "Password is not correct";
        signinMess.text = "Success";
    }

    public void Onclick()
    {
        StartCoroutine(RegisterUser());
    }

    IEnumerator RegisterUser()
    {
        passwordMess.enabled = !password1.Equals(password2);

        WWWForm form = new();
        form.AddField("loginUser", this.username);
        form.AddField("loginPass", this.password1);
        form.AddField("loginNickName", this.nickname);

        using UnityWebRequest www = UnityWebRequest.Post(ULI, form);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(www.error);
        }
        else
        {
            string mess = www.downloadHandler.text;
            /// enable message
            usernameMess.enabled = usernameMess.text.Equals(mess);
            nicknameMess.enabled = nicknameMess.text.Equals(mess);
            signinMess.enabled = signinMess.text.Equals(mess);
        }
    }

    public void RegisterUsername(string username)
    {
        this.username = username;
    }

    public void RegisterPassword1(string password1)
    {
        this.password1 = password1;
    }

    public void RegisterPassword2(string password2)
    {
        this.password2 = password2;
    }

    public void RegisterNickname(string nickname)
    {
        this.nickname = nickname;
    }

    public void ButtonBack()
    {
        SceneManager.LoadScene("Menu");
    }
}
