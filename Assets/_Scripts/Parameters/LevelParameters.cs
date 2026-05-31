using UnityEngine;

namespace _Scripts.Parameters
{
    [System.Serializable]
    public class LevelParameters
    {
        public string levelName;
        public int numOfTiers;
    
        [Header("Tier Parameters Reference")]
        public TierParameters tierParameters;
    }
}