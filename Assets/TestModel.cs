using UnityEngine;
using System.Threading.Tasks;

public class TestModel : MonoBehaviour
{
    public LLMRunner modelRunner;

    async void Start()
    {
        if (modelRunner == null)
        {
            modelRunner = FindObjectOfType<LLMRunner>();
            if (modelRunner == null)
            {
                Debug.LogError("LLMRunner not found in the scene. Please add it to a GameObject.");
                return;
            }
        }

        // Wait a bit to ensure LLMRunner has time to initialize
        await Task.Delay(6000);

        string prompt = "For me, progress means turning forests into cities.";
        string response = await modelRunner.RunModel(prompt);
        Debug.Log($"LLM Response: {response}");
    }
}
