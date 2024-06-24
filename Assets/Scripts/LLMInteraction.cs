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

    public void Update()
    {
        // Check if the input field is active and the user presses the Enter key
        if (inputField.isFocused && inputField.text != "" && Input.GetKeyDown(KeyCode.Space))
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

        if (jsonData == "{}")
        {
            Debug.Log("JSON data is empty after serialization.");
            yield break; // Exit the coroutine if JSON serialization fails
        }

        using (UnityWebRequest www = UnityWebRequest.Put(url, jsonData))
        {
            www.method = UnityWebRequest.kHttpVerbPOST;
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
                responseText.text = www.downloadHandler.text; // Display the response
            }
        }
    }
}
