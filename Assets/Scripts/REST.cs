using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

public class REST : MonoBehaviour
{
    public void signUpRequest()
    {
        try
        {
            string url = "http://54.93.126.7/signUp.php";

            var request = UnityWebRequest.Post(url, "");
            StartCoroutine(signUpResponse(request));
        }
        catch (Exception e) { Debug.Log("ERROR : " + e.Message); }
    }

    private IEnumerator signUpResponse(UnityWebRequest req)
    {
        yield return req.SendWebRequest();
        if (req.result == UnityWebRequest.Result.ConnectionError)
            Debug.Log("Network error has occured: " + req.GetResponseHeader(""));
        else
            Debug.Log("Success " + req.downloadHandler.text);
        byte[] results = req.downloadHandler.data;
        Debug.Log("Second Success");
        // Some code after success
    }
}
