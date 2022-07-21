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
        string username = usernameInput.GetComponent<TextMeshProUGUI>().text;
        string password = passwordInput.GetComponent<TextMeshProUGUI>().text;
        string repeatPassword = repeatPasswordInput.GetComponent<TextMeshProUGUI>().text;

        // TODO: add data validation

        try
        {
            WWWForm form = new();
            form.AddField("username", username);
            form.AddField("password", password);

            string url = "http://54.93.126.7/signUp.php";

            var request = UnityWebRequest.Post(url, form);
            StartCoroutine(SignUpResponse(request));
        }
        catch (Exception e) { Debug.Log("ERROR : " + e.Message); }
    }

    private IEnumerator SignUpResponse(UnityWebRequest req)
    {
        yield return req.SendWebRequest();
        if (req.result == UnityWebRequest.Result.ConnectionError)
            Debug.Log("Network error has occured: " + req.GetResponseHeader(""));
        else if (req.downloadHandler.text.Contains("already"))
        {
            Debug.LogError("User already exists!");
        }
        else
            Debug.Log("Success " + req.downloadHandler.text);
    }
}
