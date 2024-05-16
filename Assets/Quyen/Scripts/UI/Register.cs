using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class Register : MonoBehaviour
{
    private readonly string ULI = "https://tempquan.000webhostapp.com/Register.php";
    
    [SerializeField] private TMP_InputField username;
    [SerializeField] private TMP_InputField email;
    [SerializeField] private TMP_InputField password;

    [SerializeField] private TMP_Text usernameMess;
    [SerializeField] private TMP_Text emailMess;
    [SerializeField] private TMP_Text passwordMess;

    [SerializeField] UIAnimation UIAnimation;
    public bool check = true;

    private void Awake()
    {
        UIAnimation = GetComponent<UIAnimation>();
    }

    public void SignUp()
    {
        MainMenu.instance.loadingUI.gameObject.SetActive(true);
        StartCoroutine(RegisterUser());
        Invoke(nameof(DisableLoading), 0.8f);
    }

    IEnumerator RegisterUser()
    {
        CheckInfo(username.text, password.text, email.text);

        if (check)
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
                string mess = www.downloadHandler.text;
                Debug.Log(mess);
                if (mess.Equals("Username already exists"))
                {
                    usernameMess.SetText("Tài khoản đã tồn tại");
                    usernameMess.enabled = true;
                } else if (mess.Equals("Nickname already exists"))
                {
                    emailMess.SetText("Email đã được sử dụng");
                    emailMess.enabled = true;
                } else if (mess.Equals("Success"))
                {
                    MainMenu.instance.loadingUI.gameObject.SetActive(true);
                    UIAnimation.Hide();
                    Account.Instance.Information(username.text);
                }
            }
        }
    }

    private void CheckInfo(string username, string password, string email)
    {
        IsValidUsername(username);
        IsStrongPassword(password);
        IsValidEmail(email);
    } 
        

    public void IsValidUsername(string username)
    {
        // Điều kiện cho tên tài khoản:
        // không chứa ký tự đặc biệt và độ dài từ 4 đến 20 ký tự
        string pattern = @"^[a-zA-Z0-9]{4,20}$";
        check = Regex.IsMatch(username, pattern);
        if (!check)
        {
            usernameMess.SetText("Tài khoản dài 4 - 20 ký tự, " +
                "không chứa ký tự đặc biệt");
            usernameMess.enabled = true;
        }
        else
        {
            usernameMess.enabled = false;
        }
    }

    public void IsStrongPassword(string password)
    {
        // Kiểm tra chiều dài của mật khẩu
        if (password.Length < 8)
        {
            check = false;
            passwordMess.SetText("Mật khẩu dài từ 8 ký tự");
            passwordMess.enabled = true;
            return;
        }
        else 
        { 
            passwordMess.enabled = false; 
        }

        // Kiểm tra xem mật khẩu có chứa ít nhất một chữ cái viết thường
        // một chữ cái viết hoa và một số không
        check = password.Any(char.IsLower) 
            && password.Any(char.IsUpper) 
            && password.Any(char.IsDigit);
        if (!check)
        {
            passwordMess.SetText("Mật khẩu chứa ký tự hoa, " +
                "ký tự thường và số");
            passwordMess.enabled = true;
        }
        else
        {
            passwordMess.enabled = false;
        }
    }

    public void IsValidEmail(string email)
    {
        // Kiểm tra định dạng email bằng Regex
        string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        check = Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
        if (!check)
        {
            emailMess.SetText("Nhập đúng email");
            emailMess.enabled = true;
        }
        else
        {
            emailMess.enabled = false;
        }
    }

    private void DisableLoading()
    {
        MainMenu.instance.loadingUI.gameObject.SetActive(false);
    }
}
