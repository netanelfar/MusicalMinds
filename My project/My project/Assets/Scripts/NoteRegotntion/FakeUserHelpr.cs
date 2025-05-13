using UnityEngine;

public class FakeUserHelpr : MonoBehaviour
{
    void Start()
    {
        if (CurrentUserManager.CurrentUser == null)
        {
            Debug.LogWarning("No user connected — creating a fake test user.");

            UserProfile fakeUser = new UserProfile
            {
                username = "TestUser",
                level = 1,
                points = 0,
                preferredPianoSize = 25,
                preferredScreenSize = "Medium",
                // Add any default or required fields here
            };

            CurrentUserManager.SetCurrentUser(fakeUser);
        }
    }
}
