using System.Collections.Generic;
using UnityEngine;

// Data container for song information.
[System.Serializable]
public class SongInfo
{
    public string name;
    public List<List<string>> steps;  // Each step contains a list of note strings.

    // Initialize song.
    public SongInfo(string name, List<List<string>> steps)
    {
        this.name = name;
        this.steps = steps;
    }
}
