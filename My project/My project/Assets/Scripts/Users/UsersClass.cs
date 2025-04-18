using System.Collections.Generic;

[System.Serializable]
public class UserProfile
{
    public string username;
    public int level;
    public int freePlayCounter;
    public int noteRecognitionCounter;
    public List<string> achievements;
    public int points;

    public int preferredPianoSize;
    public string preferredScreenSize;
    public float volume = 1f;

    public int ProfileINDX;
}



[System.Serializable]
public class UserDataWrapper
{
    public List<UserProfile> users = new List<UserProfile>();
}
