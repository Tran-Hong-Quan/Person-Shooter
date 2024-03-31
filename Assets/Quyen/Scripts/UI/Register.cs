using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class Register : MonoBehaviour
{
    private readonly string ULI = "http://localhost/UnityData/Register.php";
    
    [SerializeField] private TMP_InputField username;
    [SerializeField] private TMP_InputField email;
    [SerializeField] private TMP_InputField password;

    public void SignUp()
    {
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

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(www.error);
        }
        else
        {
            // todo
            Debug.Log(www.downloadHandler.text);
        }
    }
}
