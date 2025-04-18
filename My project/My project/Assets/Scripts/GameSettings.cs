using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class GameSettings
{
    public enum GameMode
    {
        FreePlay,
        SingleNoteRecognition,
        MelodyReplay
        // Add 
    }



    public static GameMode CurrentGameMode;


}