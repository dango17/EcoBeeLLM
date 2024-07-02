using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DSO
{
    public class BarryAnimations : MonoBehaviour
    {
        public float normalAmplitude = 0.5f;  
        public float normalFrequency = 1f;  
        public float excitedAmplitude = 1.0f; 
        public float excitedFrequency = 2.0f; 
        public float transitionDuration = 2.0f; 

        private Vector3 startPosition;
        private Coroutine currentAnimation;

        void Start()
        {
            startPosition = transform.position;
            StartNormalBop();
        }

        void StartNormalBop()
        {
            if (currentAnimation != null)
                StopCoroutine(currentAnimation);
            currentAnimation = StartCoroutine(BopAnimation(normalAmplitude, normalFrequency));
        }

        IEnumerator BopAnimation(float targetAmplitude, float targetFrequency)
        {
            float startTime = Time.time;
            float currentAmplitude = targetAmplitude; // Initialize with target to avoid NaN issues
            float currentFrequency = targetFrequency; // Start with target frequency for consistent results

            // Ensure currentAmplitude is safely calculated
            float sineValue = Mathf.Sin(Time.time * currentFrequency);
            if (Mathf.Abs(sineValue) > 0.0001) // Check to avoid division by a very small number
                currentAmplitude = (transform.position.y - startPosition.y) / sineValue;

            while (Time.time - startTime < transitionDuration)
            {
                currentAmplitude = Mathf.Lerp(currentAmplitude, targetAmplitude, (Time.time - startTime) / transitionDuration);
                currentFrequency = Mathf.Lerp(currentFrequency, targetFrequency, (Time.time - startTime) / transitionDuration);
                float newY = startPosition.y + Mathf.Sin(Time.time * currentFrequency) * currentAmplitude;
                transform.position = new Vector3(startPosition.x, newY, startPosition.z);
                yield return null;
            }

            // Continuously apply the bop with target settings after the transition
            while (true)
            {
                float newY = startPosition.y + Mathf.Sin(Time.time * targetFrequency) * targetAmplitude;
                transform.position = new Vector3(startPosition.x, newY, startPosition.z);
                yield return null;
            }
        }


        public void StartExcitedBob()
        {
            if (currentAnimation != null)
                StopCoroutine(currentAnimation);
            currentAnimation = StartCoroutine(BopAnimation(excitedAmplitude, excitedFrequency));
            Invoke("StartNormalBop", 2);
        }
    }
}