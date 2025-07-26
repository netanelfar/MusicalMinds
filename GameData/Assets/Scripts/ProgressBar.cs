using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Updates the progress bar display with current and total values.
public class ProgressBar : MonoBehaviour
{
    [Header("UI References")]
    public Slider progressSlider;
    public TextMeshProUGUI progressText;

    // Update progress bar.
    public void UpdateProgress(int currentProg, int totalProg)
    {
        if (totalProg <= 0) return;

        // Calculate progress
        float progress = (float)currentProg / (float)totalProg;
        progress = Mathf.Clamp01(progress);

        if (progressSlider != null)
        {
            progressSlider.value = progress;
        }

        if (progressText != null)
        {
            progressText.text = $"{currentProg}/{totalProg}";
        }
    }

    // Reset progress bar.
    public void ResetProgress()
    {
        if (progressSlider != null) progressSlider.value = 0f;
        if (progressText != null) progressText.text = "0/0";

    }
}