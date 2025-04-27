using TMPro;
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
        if (PointsNumber != null && UserManager.CurrentUser != null)
        {
            PointsNumber.text = UserManager.CurrentUser.points.ToString();
        }
    }
}
