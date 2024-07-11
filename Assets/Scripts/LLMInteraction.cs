using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System.Collections;
using System.Collections.Generic;

namespace DSO
{
    public class LLMInteraction : MonoBehaviour
    {
        public string url = "";
        public BarryAnimations barryAnimations;
        public TMP_InputField inputField;
        public TextMeshProUGUI chatLog;
        public float padding = 0.1f;

        private List<string> conversationHistory = new List<string>();
        private Coroutine thinkingAnimation;

        void Start(){
            AppendToChatLog(FormatMessage("Barry Bee: How can I assist your eco needs today?", "Barry Bee"));
        }

        void Update()
        {
            if (inputField.isFocused && inputField.text != "" && Input.GetKeyDown(KeyCode.Insert)){
                string userInput = inputField.text;
                inputField.text = "";
                AppendToChatLog(FormatMessage("User: " + userInput, "User"));
                thinkingAnimation = StartCoroutine(ThinkingAnimation());
                SendRequestToModel(userInput);
            }
        }

        IEnumerator ThinkingAnimation() {
            AppendToChatLog(FormatMessage("Barry Bee: *Thinking*", "Barry Bee"));
            int dotCount = 1;
            while (true)
            {
                string updatedThinkingMessage = FormatMessage("Barry Bee: *Thinking" + new string('.', dotCount) + "*", "Barry Bee");
                conversationHistory[conversationHistory.Count - 1] = updatedThinkingMessage;
                UpdateChatLog();
                dotCount = (dotCount % 3) + 1;
                yield return new WaitForSeconds(0.5f);
            }
        }

        public void SendRequestToModel(string userInput){
            InputData data = new InputData { text = userInput };
            StartCoroutine(PostRequest(data));
        }

        IEnumerator PostRequest(InputData inputData){
            string jsonData = JsonUtility.ToJson(inputData);
            using (UnityWebRequest www = UnityWebRequest.PostWwwForm(url, jsonData))
            {
                www.uploadHandler = new UploadHandlerRaw(new System.Text.UTF8Encoding().GetBytes(jsonData));
                www.uploadHandler.contentType = "application/json";
                www.downloadHandler = new DownloadHandlerBuffer();

                yield return www.SendWebRequest();
                if (www.result != UnityWebRequest.Result.Success){
                    conversationHistory[conversationHistory.Count - 1] = FormatMessage("Barry Bee: Error receiving data.", "Barry Bee");
                }
                else {
                    string responseText = www.downloadHandler.text;
                    ResponseData responseData = JsonUtility.FromJson<ResponseData>(responseText);
                    conversationHistory[conversationHistory.Count - 1] = FormatMessage("Barry Bee: " + responseData.response, "Barry Bee");
                    barryAnimations.StartExcitedBob();
                }

                StopCoroutine(thinkingAnimation);
                UpdateChatLog();
            }
        }


        void AppendToChatLog(string message){
            conversationHistory.Add(message);
            UpdateChatLog();
        }

        void UpdateChatLog(){
            int newlineCount = Mathf.CeilToInt(padding * 10);
            string paddingStr = new string('\n', newlineCount);
            chatLog.text = string.Join(paddingStr, conversationHistory);
        }

        string FormatMessage(string message, string speaker){
            string name = message.Substring(0, message.IndexOf(':') + 1);
            string restOfMessage = message.Substring(message.IndexOf(':') + 1);
            return speaker == "User" ? string.Format("<color=#FF0000>{0}</color>{1}", name, restOfMessage) :
                                       string.Format("<color=#FFFF00>{0}</color>{1}", name, restOfMessage);
        }

        public void OnSendButtonPressed() {
            if (!string.IsNullOrEmpty(inputField.text))
            {
                string userInput = inputField.text;
                inputField.text = "";
                AppendToChatLog(FormatMessage("User: " + userInput, "User"));
                thinkingAnimation = StartCoroutine(ThinkingAnimation());
                SendRequestToModel(userInput);
            }
        }

        [System.Serializable]
        public class InputData{
            public string text;
        }

        [System.Serializable]
        public class ResponseData{
            public string response;
        }
    }
}