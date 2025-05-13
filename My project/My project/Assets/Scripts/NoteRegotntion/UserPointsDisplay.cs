using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class UserPointsDisplay : MonoBehaviour
{
    public TextMeshProUGUI PointsNumber; // Drag your Text UI here

    void Start()
    {
        UpdatePointsText(); // Set initial points
    }

    void Update()
    {
        UpdatePointsText(); // Keep updating live every frame
    }

    void UpdatePointsText()
    {
        if (PointsNumber != null && CurrentUserManager.CurrentUser != null)
        {
            PointsNumber.text = CurrentUserManager.CurrentUser.points.ToString();
        }
    }

    

    
}
