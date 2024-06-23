using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestModel : MonoBehaviour
{
    public LLMRunner modelRunner;

    void Start()
    {
        string prompt = "For me, progress means turning forests into cities.";
        modelRunner.RunModel(prompt);
    }
}
