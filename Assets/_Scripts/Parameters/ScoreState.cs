using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Score State")]
public class ScoreState : ScriptableObject
{
    public int Value { get; private set; }

    public event Action<int> OnChanged;

    public void ResetScore()
    {
        Value = 0;
        OnChanged?.Invoke(Value);
    }

    public void Add(int amount)
    {
        Value += amount;
        OnChanged?.Invoke(Value);
    }
}