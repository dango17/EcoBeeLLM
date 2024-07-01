using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class LLMInteraction : MonoBehaviour
{
    public string url = "http://127.0.0.1:5000/predict";
    public TMP_InputField inputField;
    public TextMeshProUGUI chatLog;
    public float padding = 0.1f;

    [System.Serializable]
    public class InputData
    {
        public string text;
    }

    [System.Serializable]
    public class ResponseData
    {
        public string response;
    }

    private List<string> conversationHistory = new List<string>();

    void Start()
    {
        string initialGreeting = "Barry Bee: How can I assist your eco needs today?";
        AppendToChatLog(initialGreeting);
    }

    void Update()
    {
        if (inputField.isFocused && inputField.text != "" && Input.GetKeyDown(KeyCode.Insert))
        {
            string userInput = inputField.text;
            inputField.text = "";
            SendRequestToModel(userInput);
            AppendToChatLog("User: " + userInput);
        }
    }

    void AppendToChatLog(string message)
    {
        string name = message.Substring(0, message.IndexOf(':') + 1);
        string restOfMessage = message.Substring(message.IndexOf(':') + 1);

        if (message.StartsWith("User:"))
        {
            message = string.Format("<color=#FF0000>{0}</color>{1}", name, restOfMessage);
        }
        else if (message.StartsWith("Barry Bee:"))
        {
            message = string.Format("<color=#FFFF00>{0}</color>{1}", name, restOfMessage);
        }

        conversationHistory.Add(message);
        int newlineCount = Mathf.CeilToInt(padding * 10);
        string paddingStr = new string('\n', newlineCount);
        chatLog.text = string.Join(paddingStr, conversationHistory.ToArray());
    }

    public void SendRequestToModel(string userInput)
    {
        InputData data = new InputData { text = userInput };
        StartCoroutine(PostRequest(data));
    }

    IEnumerator PostRequest(InputData inputData)
    {
        string jsonData = JsonUtility.ToJson(inputData);
        Debug.Log("Sending JSON: " + jsonData);

        using (UnityWebRequest www = UnityWebRequest.PostWwwForm(url, jsonData))
        {
            www.uploadHandler = new UploadHandlerRaw(new System.Text.UTF8Encoding().GetBytes(jsonData));
            www.uploadHandler.contentType = "application/json";
            www.downloadHandler = new DownloadHandlerBuffer();

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("Error: " + www.error);
            }
            else
            {
                Debug.Log("Received: " + www.downloadHandler.text);
                ResponseData responseData = JsonUtility.FromJson<ResponseData>(www.downloadHandler.text);
                if (responseData == null)
                {
                    Debug.Log("Failed to parse JSON response.");
                }
                else if (chatLog != null)
                {
                    AppendToChatLog("Barry Bee: " + responseData.response);
                    Debug.Log("Response set on UI: " + responseData.response);
                }
            }
        }
    }
}