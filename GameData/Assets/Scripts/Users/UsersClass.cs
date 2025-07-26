using System.Collections.Generic;


// Stores all user data including progress, settings, and achievements
[System.Serializable]
public class UserProfile
{
    public string username;
    public int level;
    public int freePlayCounter;
    public int noteRecognitionCounter;
    public int MelodyPlayCounter;
    public List<string> achievements;
    public int points;
    public bool systemPressHasColor = true;
    public int preferredPianoSize;
    public string preferredScreenSize;
    public float volume = 1f;
    public int ProfileINDX; // Index for profile picture.

    // Sound Match game settings.
    public int NoteRecognitionDifficulty; 
    public int showNoteRecHints = 1; // 1 = show hints (default), 0 = no hints

    // Melody Play songs prograss.
    public int littleStarLevel = 0;
    public int littleJonathanLevel = 0;
    public int ninthSymphonyLevel = 0;
}

// Wrapper class for serializing list of user profiles to JSON
[System.Serializable]
public class UserDataWrapper
{
    public List<UserProfile> users = new List<UserProfile>();
}
