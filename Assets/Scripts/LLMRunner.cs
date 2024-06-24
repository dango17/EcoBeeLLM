using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

public class LLMRunner : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(SendText("Hello"));
    }

    IEnumerator SendText(string text)
    {
        string url = "http://localhost:5000/predict";
        var json = JsonUtility.ToJson(new { text = text });
        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.error == null)
        {
            Debug.Log("Response: " + request.downloadHandler.text);
        }
        else
        {
            Debug.Log("Error: " + request.error);
        }
    }
}