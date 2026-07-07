using System.Collections.Generic;
using _Scripts;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private ScoreState scoreState;

    private void Start()
    {
        GameRandom.SetSeed(101);
        scoreState.ResetScore();
    }
}
