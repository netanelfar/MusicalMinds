using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Stores gamemode across the game.
public static class GameSettings
{
    // Possible Gamemodes.
    public enum GameMode
    {
        FreePlay,
        SingleNoteRecognition,
        MelodyPlay
    }
    
    public static GameMode CurrentGameMode; 
    
}