using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private ScoreState scoreState;

    private void Start()
    {
        scoreState.ResetScore();
    }
}
