using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class Register : MonoBehaviour
{
    private readonly string ULI = "https://tempquan.000webhostapp.com/Register.php";
    
    [SerializeField] private TMP_InputField username;
    [SerializeField] private TMP_InputField email;
    [SerializeField] private TMP_InputField password;

    [SerializeField] UIAnimation UIAnimation;

    private void Awake()
    {
        UIAnimation = GetComponent<UIAnimation>();
    }

    public void SignUp()
    {
        MainMenu.instance.loadingUI.gameObject.SetActive(true);
        StartCoroutine(RegisterUser());
    }

    IEnumerator RegisterUser()
    {
        WWWForm form = new();
        form.AddField("userName", this.username.text);
        form.AddField("userPass", this.password.text);
        form.AddField("email", this.email.text);

        using UnityWebRequest www = UnityWebRequest.Post(ULI, form);
        yield return www.SendWebRequest();
        MainMenu.instance.loadingUI.gameObject.SetActive(false);

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(www.error);
        }
        else
        {
            Debug.Log(www.downloadHandler.text);
            UIAnimation.Hide();
            Account.Instance.Information(username.text);
        }
    }
}
