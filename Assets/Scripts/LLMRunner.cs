using UnityEngine;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class LLMRunner : MonoBehaviour
{
    public string pythonPath = @"C:\Users\Daniel\AppData\Local\Programs\Python\Python312\python.exe";
    public string modelPath = @"C:\Users\Daniel\Documents\GitHub\EcoBeeLLM\Assets\Plugins\harmonious_caramel_model.pth";
    public string scriptPath = "Assets/Plugins/llm_server.py";

    private Process serverProcess;
    private HttpClient httpClient;
    private bool isInitialized = false;

    async void Start()
    {
        await Initialize();
    }

    void OnDisable()
    {
        StopServer();
    }

    private async Task Initialize()
    {
        StartServer();
        httpClient = new HttpClient();

        await Task.Delay(5000);

        isInitialized = true;
        UnityEngine.Debug.Log("LLMRunner initialized");
    }


    private void StartServer()
    {
        string absoluteModelPath = System.IO.Path.GetFullPath(System.IO.Path.Combine(Application.dataPath, "..", modelPath));
        string absoluteScriptPath = System.IO.Path.GetFullPath(System.IO.Path.Combine(Application.dataPath, "..", scriptPath));

        ProcessStartInfo start = new ProcessStartInfo();
        start.FileName = pythonPath;
        start.Arguments = $"\"{absoluteScriptPath}\" \"{absoluteModelPath}\"";
        start.UseShellExecute = false;
        start.CreateNoWindow = true;
        start.RedirectStandardOutput = true;
        start.RedirectStandardError = true;

        serverProcess = new Process();
        serverProcess.StartInfo = start;
        serverProcess.OutputDataReceived += (sender, e) => UnityEngine.Debug.Log("Server: " + e.Data);
        serverProcess.ErrorDataReceived += (sender, e) => UnityEngine.Debug.LogError("Server Error: " + e.Data);

        serverProcess.Start();
        serverProcess.BeginOutputReadLine();
        serverProcess.BeginErrorReadLine();
    }

    private void StopServer()
    {
        if (serverProcess != null && !serverProcess.HasExited)
        {
            serverProcess.Kill();
            serverProcess.WaitForExit();
            serverProcess.Dispose();
        }
    }

    public async Task<string> RunModel(string prompt)
    {
        if (!isInitialized)
        {
            UnityEngine.Debug.LogWarning("LLMRunner not initialized. Initializing now...");
            await Initialize();
        }

        var content = new StringContent(JsonConvert.SerializeObject(new { prompt }), Encoding.UTF8, "application/json");
        try
        {
            var response = await httpClient.PostAsync("http://localhost:5000/generate", content);
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<GenerationResponse>(responseString);
            return responseObject.output;
        }
        catch (HttpRequestException e)
        {
            UnityEngine.Debug.LogError($"Error communicating with server: {e.Message}");
            return null;
        }
    }
}

public class GenerationResponse
{
    public string output { get; set; }
}