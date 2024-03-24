using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Login : MonoBehaviour
{
    private readonly string ULI = "http://localhost/UnityData/GetUser.php";
    private string username;
    private string password;

    [Header("Message")]
    [SerializeField] private Text usernameMess;
    [SerializeField] private Text passwordMess;

    private void Start()
    {
        usernameMess.enabled = false;
        passwordMess.enabled = false;

        usernameMess.text = "Username is not exists";
        passwordMess.text = "Password is not correct";
    }

    public void OnClick()
    {
        StartCoroutine(LoginUser());
    }

    IEnumerator LoginUser()
    {
        WWWForm form = new();
        form.AddField("loginUser", this.username);
        form.AddField("loginPass", this.password);

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
            if (mess.Equals("Success"))
            {
                LoadMainMap();
            }
            usernameMess.enabled = usernameMess.text.Equals(mess);
            passwordMess.enabled = passwordMess.text.Equals(mess);
        }
    }

    public void LoginUsername(string username)
    {
        this.username = username;
    }

    public void LoginPassword(string password)
    {
        this.password = password;
    }

    public void LoadMainMap()
    {
        SceneManager.LoadScene("MainMap");
    }

    public void ButtonBack()
    {
        SceneManager.LoadScene("Menu");
    }
}
