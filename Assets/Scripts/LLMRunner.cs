using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LLMRunner : MonoBehaviour
{
    // Set these paths in the Unity Inspector
    public string pythonPath = @"C:\Users\Daniel\AppData\Local\Programs\Python\Python312\python.exe"; // Path to your Python executable
    public string modelPath = @"C:\Users\\Daniel\\Documents\GitHub\EcoBeeLLM\Assets\Plugins\harmonious_caramel_model.pth"; // Path to your saved model
    public string scriptPath = "Assets/Plugins/run_model.py"; // Path to your Python script

    public void RunModel(string prompt)
    {
        // Convert relative paths to absolute paths
        string absoluteModelPath = Path.GetFullPath(Path.Combine(Application.dataPath, "..", modelPath));
        string absoluteScriptPath = Path.GetFullPath(Path.Combine(Application.dataPath, "..", scriptPath));

        ProcessStartInfo start = new ProcessStartInfo();
        start.FileName = pythonPath;
        start.Arguments = $"\"{absoluteScriptPath}\" \"{absoluteModelPath}\" \"{prompt}\"";
        start.UseShellExecute = false;
        start.RedirectStandardOutput = true;
        start.RedirectStandardError = true;

        UnityEngine.Debug.Log($"Running command: {start.FileName} {start.Arguments}");

        try
        {
            using (Process process = Process.Start(start))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    string result = reader.ReadToEnd();
                    UnityEngine.Debug.Log($"Output: {result}");
                }
                using (StreamReader reader = process.StandardError)
                {
                    string errors = reader.ReadToEnd();
                    if (!string.IsNullOrEmpty(errors))
                    {
                        UnityEngine.Debug.LogError($"Errors: {errors}");
                    }
                }
            }
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError($"Error starting process: {e.Message}");
        }
    }
}
