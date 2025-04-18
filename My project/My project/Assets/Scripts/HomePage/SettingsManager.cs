using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public GameObject settingsPNL; //Main options pannel
    public GameObject OptionsPNL;
    public GameObject pianoSizeOptions;
    public GameObject screenSizeOptions;
    public GameObject UserPNL;
    public GameObject NewUserPNL;
    //public Transform contentTransform; // ScrollView Content
    //public GameObject userButtonPrefab; // ChooseUserBTN prefab


    public void ToggleSettingsPanel()
    {
        bool isActive = settingsPNL.activeSelf;
        settingsPNL.SetActive(!isActive);
        OptionsPNL.SetActive(!isActive);

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
        OptionsPNL.SetActive(true);
        pianoSizeOptions.SetActive(false);
        screenSizeOptions.SetActive(false);
        UserPNL.SetActive(false);
        NewUserPNL.SetActive(true);

    }

    /*
    private void PopulateUserCarousel()
    {
        // Clear old buttons first (optional but cleaner)
        foreach (Transform child in contentTransform)
        {
            Destroy(child.gameObject);
        }

        // Load all users
        var allUsers = UserDataManager.LoadUsers();

        foreach (var user in allUsers)
        {
            GameObject buttonObj = Instantiate(userButtonPrefab, contentTransform);
            buttonObj.transform.localScale = Vector3.one;

            var userText = buttonObj.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if (userText != null)
            {
                userText.text = user.username;
            }
        }
    }*/



    // Piano size  options
    public void SelectPianoSize25()
    {
        UpdateCurrentUserPianoSize(25);
        Debug.Log("Piano size set to 25 keys");
    }

    public void SelectPianoSize88()
    {
        UpdateCurrentUserPianoSize(88);
        Debug.Log("Piano size set to 88 keys");
    }

    private void UpdateCurrentUserPianoSize(int size)
    {
        if (UserManager.CurrentUser != null)
        {
            UserManager.CurrentUser.preferredPianoSize = size;

            var allUsers = UserDataManager.LoadUsers();

            foreach (var user in allUsers)
            {
                if (user.username == UserManager.CurrentUser.username)
                {
                    user.preferredPianoSize = size;
                    break;
                }
            }

            UserDataManager.SaveUsers(allUsers);
        }
        Debug.Log(JsonUtility.ToJson(UserManager.CurrentUser, true));

    }





}
