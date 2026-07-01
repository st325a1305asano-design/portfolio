using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enums : MonoBehaviour
{
    public enum SceneName
    {
        Title = 0,
        StageSelect = 1,
        Tutorial = 2,
        Stage1 = 3,
        Stage2 = 4
    }

    public enum SE
    {
        hitVase = 0,
        hitCharacter = 1,
        hitMirror = 2
    }

    public enum GameState
    {
        Playing = 0,
        Stop = 1
    }

    public enum ActionMaps
    {
        UI,
        PlayerAction,
        Create //Ç¢ÇÈÇ©Ç»ÅH
    }
}