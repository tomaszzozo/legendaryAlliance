using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using TMPro;

public class REST : MonoBehaviour
{
    public GameObject usernameInput;
    public GameObject passwordInput;
    public GameObject repeatPasswordInput;

    private string _username;

    public void SignUp()
    {
        _username = usernameInput.GetComponent<TMP_InputField>().text;
        string password = passwordInput.GetComponent<TMP_InputField>().text;
        string repeatPassword = repeatPasswordInput.GetComponent<TMP_InputField>().text;

        string verificationResult = DataValidator.ValidateSignUpData(_username, password, repeatPassword);
        if (!verificationResult.Equals(DataValidator.StatusOk))
        {
            MessageBoxFactory.ShowAlertDialog(verificationResult, gameObject);
            return;
        }

        StartCoroutine(SignURequest($"http://54.93.126.7/signUp.php?username={_username}&password={password}"));
    }

    public void SignIn()
    {
        _username = usernameInput.GetComponent<TMP_InputField>().text;
        string password = passwordInput.GetComponent<TMP_InputField>().text;

        StartCoroutine(SignInRequest($"http://54.93.126.7/signIn.php?username={_username}&password={password}"));
    }

    private IEnumerator SignURequest(string uri)
    {
        using UnityWebRequest webRequest = UnityWebRequest.Get(uri);
        // Request and wait for the desired page.
        yield return webRequest.SendWebRequest();

        switch (webRequest.result)
        {
            case UnityWebRequest.Result.ConnectionError:
            case UnityWebRequest.Result.DataProcessingError:
                MessageBoxFactory.ShowAlertDialog("Error: " + webRequest.error, gameObject);
                break;
            case UnityWebRequest.Result.ProtocolError:
                MessageBoxFactory.ShowAlertDialog(webRequest.downloadHandler.text, gameObject);
                break;
            case UnityWebRequest.Result.Success:
                SharedVariables.SetUsername(_username);
                SceneManager.LoadScene("SceneLoggedInMenu");
                break;
        }
    }

    private IEnumerator SignInRequest(string uri)
    {
        using UnityWebRequest webRequest = UnityWebRequest.Get(uri);
        // Request and wait for the desired page.
        yield return webRequest.SendWebRequest();

        switch (webRequest.result)
        {
            case UnityWebRequest.Result.ConnectionError:
            case UnityWebRequest.Result.DataProcessingError:
                MessageBoxFactory.ShowAlertDialog("Unexpected error: " + webRequest.error, gameObject);
                break;
            case UnityWebRequest.Result.ProtocolError:
                MessageBoxFactory.ShowAlertDialog(webRequest.downloadHandler.text, gameObject);
                break;
            case UnityWebRequest.Result.Success:
                SharedVariables.SetUsername(_username);
                SceneManager.LoadScene("SceneLoggedInMenu");
                break;
        }
    }
}
