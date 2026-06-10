using UnityEngine;
using TMPro;

public class ScoreView : MonoBehaviour
{
    [SerializeField] private ScoreState scoreState;
    [SerializeField] private TMP_Text scoreText;

    private void OnEnable()
    {
        scoreState.OnChanged += UpdateText;
        UpdateText(scoreState.Value);
    }

    private void OnDisable()
    {
        scoreState.OnChanged -= UpdateText;
    }

    private void UpdateText(int value)
    {
        scoreText.text = $"Score: {value}";
    }
}
