using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// Manages settings panel navigation.
public class SettingsUIController : MonoBehaviour
{
    [Header("Main UI Panels")]
    public GameObject SettingsPNL;

    [Header("Option Sub-Panels")]
    public GameObject HelpPNL;
    public GameObject UserPNL;
    public GameObject NewUserPNL;
    public GameObject PianoSizePNL;

    [Header("UI Components")]
    public UserUI userUI;
    public GameObject XBTN;
    public GameObject EditBTN;

    [Header("Components")]
    private ProfilePictureCarouselLoader profilePicLoader;

    // Initialize profile picture loader.
    private void Awake()
    {
        profilePicLoader = GetComponent<ProfilePictureCarouselLoader>();
    }

    // Toggle main settings panel.
    public void ToggleSettingsPanel()
    {
        bool isActive = SettingsPNL.activeSelf;
        if (isActive)
        {
            // close everything
            CloseOptions();
        }
        else
        {
            SettingsPNL.SetActive(true);
            ShowUsersOptions();
        }
    }

    // Show piano size options.
    public void ShowPianoSizeOptions()
    {
        HelpPNL.SetActive(false);
        UserPNL.SetActive(false);
        NewUserPNL.SetActive(false);
        PianoSizePNL.SetActive(true);
    }

    // Show help panal.
    public void ShowHelpPNL()
    {
        PianoSizePNL.SetActive(false);
        HelpPNL.SetActive(true);
        UserPNL.SetActive(false);
        NewUserPNL.SetActive(false);
    }

    // Show user selection options.
    public void ShowUsersOptions()
    {
        PianoSizePNL.SetActive(false);
        HelpPNL.SetActive(false);
        UserPNL.SetActive(true );
        NewUserPNL.SetActive(false);
    }

    // Open new user creation panel.
    public void ShowNewUsersPNL()
    {
        SettingsPNL.SetActive(true);
        HelpPNL.SetActive(false);
        PianoSizePNL.SetActive(false);
        UserPNL.SetActive(false);
        NewUserPNL.SetActive(true);
        EditBTN.SetActive(enabled);// do anything?
        userUI.ResetFormState();            
        userUI.OpenCreateUserPanel();
    }

    // Switch to edit mode for current user.
    public void OnEditButtonClicked()
    {
        UserPNL.SetActive(false);
        userUI.EditCurrentUser();
    }

    // Switch to edit mode for current user.
    public void OpenEditProfile(UserProfile userToEdit)
    {
        HelpPNL.SetActive(false);
        UserPNL.SetActive(false);
        PianoSizePNL.SetActive(false);

        // Use the same panel for editing
        NewUserPNL.SetActive(true);
        userUI.OpenCreateUserPanel(userToEdit);

        profilePicLoader.SetupProfilePictureButtons();
    }

    // Reset to default setting view.
    public void CloseOptions()
    {
        UserPNL.SetActive(true);
        HelpPNL.SetActive(false);
        NewUserPNL.SetActive(false);
        SettingsPNL.SetActive(false);
        PianoSizePNL.SetActive(false);


        // Reset user creation form.
        if (userUI != null)
        {
            userUI.ResetFormState();
        }
    }

    // Update piano size.
    public void SelectPianoSize(int size)
    {
        UserManager.UpdatePianoSize(size);
        Debug.Log("Piano size set to: "+size);
        CloseOptions();
    }

}
