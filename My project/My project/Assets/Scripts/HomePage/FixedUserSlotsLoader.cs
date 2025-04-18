using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class FixedUserSlotsLoader : MonoBehaviour
{
    [Header("Fixed Buttons")]
    public List<Button> userButtons; 

    [Header("Profile Pictures")]
    public List<Sprite> profilePictures;

    private List<UserProfile> users;

    void Start()
    {
        users = UserDataManager.LoadUsers();
        SetupUserButtons();
    }

    private void SetupUserButtons()
    {
        for (int i = 0; i < userButtons.Count; i++)
        {
            Button button = userButtons[i];

            TMP_Text nameText = button.transform.Find("UserNameTXT")?.GetComponent<TMP_Text>();
            Image profileImage = button.transform.Find("UserIMG")?.GetComponent<Image>();

            if (i < users.Count)
            {
                UserProfile user = users[i];

                if (nameText != null)
                    nameText.text = user.username;

                if (profileImage != null && user.ProfileINDX >= 0 && user.ProfileINDX < profilePictures.Count)
                    profileImage.sprite = profilePictures[user.ProfileINDX];

                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => OnUserSelected(user));
                button.interactable = true;
            }
            else
            {
                if (nameText != null)
                    nameText.text = "Empty Slot";

                if (profileImage != null)
                    profileImage.sprite = null; // Clear image if no user

                button.onClick.RemoveAllListeners();
                button.interactable = false;
            }
        }
    }


    private void OnUserSelected(UserProfile user)
    {
        UserManager.SetCurrentUser(user);
        Debug.Log("Switched to user: " + user.username);
    }

    public void ReloadUserButtons()
    {
        users = UserDataManager.LoadUsers(); // reload fresh users list
        SetupUserButtons();
    }

}
