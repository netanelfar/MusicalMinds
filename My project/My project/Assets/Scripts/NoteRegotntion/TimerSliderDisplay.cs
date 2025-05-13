using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TimerSliderDisplay : MonoBehaviour
{
    public Slider timerSlider;
    public float maxTime = 6f;
    private float currentTime;
    private bool isRunning = false;
    private bool isFilling = false;
    private bool isPaused = false; // 

    public System.Action OnTimerFinished; // callback for when time runs out

    public void StartTimer()
    {
        currentTime = maxTime;
        timerSlider.maxValue = maxTime;
        timerSlider.value = maxTime;
        isRunning = true;
        isFilling = false;
        isPaused = false; //
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    public void PauseTimer() // NEW
    {
        isPaused = true;
    }

    public void ResumeTimer() //  NEW
    {
        isPaused = false;
    }

    public void PlayFillAnimation(float fillDuration = 1f)
    {
        StartCoroutine(FillSlider(fillDuration));
    }

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

    void Update()
    {
        //if (!isRunning || isFilling) return;
        if (!isRunning || isFilling || isPaused) return; // FREEZE when paused


        currentTime -= Time.deltaTime;
        timerSlider.value = Mathf.Lerp(timerSlider.value, currentTime, Time.deltaTime * 10f);

        if (currentTime <= 0f)
        {
            isRunning = false;
            currentTime = 0f;
            OnTimerFinished?.Invoke();
        }
    }
}
