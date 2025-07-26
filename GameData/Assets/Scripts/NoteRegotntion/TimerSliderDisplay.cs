using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// Visual countdown timer using UI slider.
public class TimerSliderDisplay : MonoBehaviour
{
    public Slider timerSlider;
    public float maxTime = 6f;
    private float currentTime;
    private bool isRunning = false;
    private bool isFilling = false;
    private bool isPaused = false;

    public System.Action OnTimerFinished;

    // Updates countdown.
    void Update()
    {
        if (!isRunning || isFilling || isPaused) return;

        currentTime -= Time.deltaTime;
        timerSlider.value = currentTime; // direct assignment

        if (currentTime <= 0f)
        {
            currentTime = 0f;
            isRunning = false;
            OnTimerFinished?.Invoke();
            Debug.Log("[TIMER] Timer finished!");
        }
    }

    // Starts countdown timer from max time.
    public void StartTimer()
    {
        currentTime = maxTime;
        timerSlider.maxValue = maxTime;
        timerSlider.value = maxTime;
        isRunning = true;
        isFilling = false;
        isPaused = false;
    }

    // Stops the countdown timer.
    public void StopTimer()
    {
        isRunning = false;
    }

    // Pauses countdown without resetting.
    public void PauseTimer()
    {
        isPaused = true;
    }

    // Resumes paused countdown.
    public void ResumeTimer()
    {
        isPaused = false;
    }

    // Triggers fill animation coroutine.
    public void PlayFillAnimation(float fillDuration = 1f)
    {
        StartCoroutine(FillSlider(fillDuration));
    }

    // Animates slider filling.
    private IEnumerator FillSlider(float duration)
    {
        isRunning = false;
        isFilling = true;
        float t = 0f;
        float startVal = timerSlider.value;

        while (t < duration)
        {
            t += Time.deltaTime;
            timerSlider.value = Mathf.Lerp(startVal, maxTime, t / duration);
            yield return null;
        }

        timerSlider.value = maxTime;
        isFilling = false;
    }


}
