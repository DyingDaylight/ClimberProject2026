using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.Parameters
{
    [CreateAssetMenu(fileName = "LevelManager", menuName = "LevelManager", order = 0)]
    public class TowerParameterManager : ScriptableObject
    {
        [Header("Difficulty Progression")]
        [SerializeField] private List<LevelParameters> levelParameters;

        private int currentLevelIndex = 0;
        private int tiersGenerated = 0;
        
        public string CurrentLevelName => CurrentLevelParameters?.levelName ?? "Unknown";

        private void Awake()
        {
            Debug.Log("Awake: " + currentLevelIndex);
        }

        public TierParameters CurrentTierParameters => 
            levelParameters.Count > 0 && currentLevelIndex < levelParameters.Count 
                ? levelParameters[currentLevelIndex].tierParameters 
                : null;
    
        public LevelParameters CurrentLevelParameters =>
            levelParameters.Count > 0 && currentLevelIndex < levelParameters.Count 
                ? levelParameters[currentLevelIndex] 
                : null;

        public bool IncrementTiers()
        {
            Debug.Log("IncrementTiers" + currentLevelIndex);
            if (CurrentLevelParameters == null) return false;
        
            tiersGenerated++;

            if (tiersGenerated >= CurrentLevelParameters.numOfTiers)
                return AdvanceToNextLevel();
        
            return false;
        }

        public bool AdvanceToNextLevel()
        {
            Debug.Log("AdvanceToNextLevel " + currentLevelIndex);
            if (currentLevelIndex + 1 < levelParameters.Count)
            {
                currentLevelIndex++;
                tiersGenerated = 0;
                Debug.Log($"Difficulty advanced to: {CurrentLevelName}");
                return true;
            }    
        
            tiersGenerated = 0;
            return false;
        }
        
        public void ResetDifficulty()
        {
            currentLevelIndex = 0;
            tiersGenerated = 0;
        }
    }
}
