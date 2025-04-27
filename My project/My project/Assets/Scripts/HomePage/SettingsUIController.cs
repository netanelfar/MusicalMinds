using UnityEngine;
using UnityEngine.UI;

public class SettingsUIController : MonoBehaviour
{
    public GameObject MainsettingsPNL; //Main manue options pannel
    public GameObject OptionsPNL; // Option main view
    public GameObject pianoSizeOptions;
    public GameObject screenSizeOptions;
    public GameObject UserPNL;
    public GameObject NewUserPNL;
    public UserProfileFormUI newUserCreator;
    public GameObject XBTN;
    public GameObject SettingsPNL;
    public GameObject EditBTN;
    private ProfilePictureCarouselLoader profilePicLoader;



    /* public void CloseAllSetiingesPNL ()
     {
         OptionsPNL.SetActive(false);
         pianoSizeOptions.SetActive(false);
         screenSizeOptions.SetActive(false);
         UserPNL.SetActive(false);
         NewUserPNL.SetActive(false);
     }
    */

    private void Awake()
    {
        profilePicLoader = GetComponent<ProfilePictureCarouselLoader>();
    }
    public void SetUIInteractable(bool isEnabled)
    {
        XBTN.SetActive(isEnabled);
       
    }



    public void ToggleSettingsPanel()
    {
        bool isActive = SettingsPNL.activeSelf;

        if (isActive)
        {
            // If it's open → close everything
            CloseOptions();
        }
        else
        {
            // If it's closed → open everything
            SettingsPNL.SetActive(true);
            MainsettingsPNL.SetActive(true);
            OptionsPNL.SetActive(true);
        }
    }



    public void ShowPianoSizeOptions()
    {
        OptionsPNL.SetActive(true);
        pianoSizeOptions.SetActive(true);
        screenSizeOptions.SetActive(false);
        UserPNL.SetActive(false);
        NewUserPNL.SetActive(false);

    }

    public void ShowScreenSizeOptions()
    {
        OptionsPNL.SetActive(true);
        pianoSizeOptions.SetActive(false);
        screenSizeOptions.SetActive(true);
        UserPNL.SetActive(false);
        NewUserPNL.SetActive(false);


    }

    public void ShowUsersOptions()
    {
        OptionsPNL.SetActive(true);
        pianoSizeOptions.SetActive(false);
        screenSizeOptions.SetActive(false);
        UserPNL.SetActive(true );
        NewUserPNL.SetActive(false);

    }

    public void ShowNewUsersPNL()
    {
        SettingsPNL.SetActive(true);
        OptionsPNL.SetActive(true);
        pianoSizeOptions.SetActive(false);
        screenSizeOptions.SetActive(false);
        UserPNL.SetActive(false);
        NewUserPNL.SetActive(true);
        //
        EditBTN.SetActive(enabled);// do anything?
                                   //
        profilePicLoader.SetupProfilePictureButtons();


    }
    public void OnEditButtonClicked()
    {
        if (UserManager.CurrentUser != null)
        {
            OpenEditProfile(UserManager.CurrentUser);
        }
    }

    public void OpenEditProfile(UserProfile userToEdit)
    {
        OptionsPNL.SetActive(true);
        pianoSizeOptions.SetActive(false);
        screenSizeOptions.SetActive(false);
        UserPNL.SetActive(false);



        // Use the same panel for editing
        NewUserPNL.SetActive(true);

        // Pass user to NewUserCreator (fill fields)
        newUserCreator.OpenCreateUserPanel(userToEdit);

        profilePicLoader.SetupProfilePictureButtons();

    }




    public void CloseOptions()
    {

        OptionsPNL.SetActive(false);
        pianoSizeOptions.SetActive(false);
        screenSizeOptions.SetActive(false);
        UserPNL.SetActive(false);
        NewUserPNL.SetActive(false);
        MainsettingsPNL.SetActive(false);
        SettingsPNL.SetActive(false);
    }


    public void SelectPianoSize25()
    {
        UserEditorService.UpdatePianoSize(25);
        Debug.Log("Piano size set to 25 keys");
    }

    public void SelectPianoSize88()
    {
        UserEditorService.UpdatePianoSize(88);
        Debug.Log("Piano size set to 88 keys");
    }






}
