using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private LevelParameters levelParameters;

    public LevelParameters LevelParameters => levelParameters;
}
