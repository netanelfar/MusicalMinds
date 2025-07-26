using UnityEngine;
using UnityEngine.UI;
using TMPro;

// UI button representing a step in song map.
public class StepButton : MonoBehaviour
{
    public TextMeshProUGUI stepLabel;
    public Image backgroundImage;
    public Sprite completedSprite;
    public Sprite lockedSprite;
    public Button button;
    public Image stepIcon; // Lock/check image.
    public Sprite iconCheck;
    public Sprite iconLock;

    // Configures button appearance based on step state.
    public void Setup(int stepNumber, bool isUnlocked, bool isCurrent, System.Action onClickAction)
    {
        stepLabel.text = stepNumber.ToString();

        if (isUnlocked)
        {
            backgroundImage.sprite = completedSprite;
            button.interactable = true;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => onClickAction());
            
            if (isCurrent)
            {
                stepIcon.enabled = false; 
                stepLabel.color = Color.yellow; 
            }
            else
            {
                stepIcon.enabled = true;
                stepIcon.sprite = iconCheck;
            }
        }
        else
        {
            backgroundImage.sprite = lockedSprite;
            stepIcon.enabled = true;
            stepIcon.sprite = iconLock;
            button.interactable = false;
        }
    }
}

