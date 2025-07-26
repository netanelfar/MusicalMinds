using UnityEngine;

public class FakeUserHelpr : MonoBehaviour
{
    void Start()
    {
        if (UserManager.CurrentUser == null)
        {
            Debug.LogWarning("No user connected — creating a fake test user.");

            UserProfile fakeUser = new UserProfile
            {
                username = "TestUser",
                level = 1,
                points = 0,
                preferredPianoSize = 5,
                preferredScreenSize = "Medium",
                NoteRecognitionDifficulty = 1,
                showNoteRecHints = 1
            };

            UserManager.SetCurrentUser(fakeUser);
        }
    }
}
