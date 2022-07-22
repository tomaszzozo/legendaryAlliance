using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System;
using TMPro;

public class REST : MonoBehaviour
{
    public GameObject usernameInput;
    public GameObject passwordInput;
    public GameObject repeatPasswordInput;

    public void SignUpRequest()
    {
        string username = usernameInput.GetComponent<TMP_InputField>().text;
        string password = passwordInput.GetComponent<TMP_InputField>().text;
        string repeatPassword = repeatPasswordInput.GetComponent<TMP_InputField>().text;

        string verificationResult = DataValidator.ValidateSignUpData(username, password, repeatPassword);
        if (!verificationResult.Equals(DataValidator.statusOk))
        {
            MessageBoxFactory.ShowAlertDialog(verificationResult, gameObject);
            return;
        }

        try
        {
            WWWForm form = new();
            form.AddField("username", username);
            form.AddField("password", password);

            string url = "http://54.93.126.7/signUp.php";

            var request = UnityWebRequest.Post(url, form);
            StartCoroutine(SignUpResponse(request));
        }
        catch (Exception e) { MessageBoxFactory.ShowAlertDialog("ERROR : " + e.Message, gameObject); }
    }

    public void SignInRequest()
    {
        string username = usernameInput.GetComponent<TMP_InputField>().text;
        string password = passwordInput.GetComponent<TMP_InputField>().text;

        try
        {
            string url = $"http://54.93.126.7/signIn.php?username={username}&password={password}";

            var request = UnityWebRequest.Get(url);
            StartCoroutine(SignInResponse(request));
        }
        catch (Exception e) { MessageBoxFactory.ShowAlertDialog("ERROR : " + e.Message, gameObject); }
    }

    private IEnumerator SignUpResponse(UnityWebRequest req)
    {
        yield return req.SendWebRequest();
        if (req.result == UnityWebRequest.Result.ConnectionError)
            MessageBoxFactory.ShowAlertDialog("Network error has occured: " + req.GetResponseHeader(""), gameObject);
        else if (req.downloadHandler.text.Contains("already"))
        {
            MessageBoxFactory.ShowAlertDialog("User already exists!", gameObject);
        }
        else MessageBoxFactory.ShowAlertDialog("Success " + req.downloadHandler.text, gameObject);
    }

    private IEnumerator SignInResponse(UnityWebRequest req)
    {
        yield return req.SendWebRequest();
        if (req.result == UnityWebRequest.Result.ConnectionError)
            MessageBoxFactory.ShowAlertDialog("Network error has occured: " + req.GetResponseHeader(""), gameObject);
        else if (req.downloadHandler.text.Contains("Ok"))
        {
            MessageBoxFactory.ShowAlertDialog("Successfully logged in.", gameObject);
        }
        else MessageBoxFactory.ShowAlertDialog("Incorrect username and password combination.", gameObject);
    }
}
