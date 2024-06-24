using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class LLMInteraction : MonoBehaviour
{
    public string url = "http://127.0.0.1:5000/predict";
    public TMP_InputField inputField; // Assuming you have an InputField where users type their queries
    public TextMeshProUGUI responseText; // UI Text to display responses

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

    public void Update()
    {
        // Check if the input field is active and the user presses the Enter key
        if (inputField.isFocused && inputField.text != "" && Input.GetKeyDown(KeyCode.Insert))
        {
            SendRequestToModel();
            inputField.text = ""; // Optionally clear the field after sending
            Debug.Log("Sent Prompt!");
        }
    }

    // This method could be called by pressing a button
    public void SendRequestToModel()
    {
        InputData data = new InputData { text = inputField.text };
        StartCoroutine(PostRequest(data));
    }

    IEnumerator PostRequest(InputData inputData)
    {
        string jsonData = JsonUtility.ToJson(inputData);
        Debug.Log("Sending JSON: " + jsonData);

        using (UnityWebRequest www = UnityWebRequest.PostWwwForm(url, jsonData))
        {
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData);
            www.uploadHandler = new UploadHandlerRaw(jsonToSend);
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
                else if (responseText != null)
                {
                    responseText.text = responseData.response;
                    Debug.Log("Response set on UI: " + responseData.response);
                }

            }
        }
    }
}
