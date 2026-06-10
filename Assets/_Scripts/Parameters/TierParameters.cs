using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.Parameters
{
    [CreateAssetMenu(fileName = "LevelParameters", menuName = "LevelParameters", order = 0)]
    public class TierParameters : ScriptableObject
    {
        [Range(1, 3)]
        public int minLadder;
        [Range(1, 3)]
        public int maxLadder;
        public Material tierMaterial;

        [Header("Collectables")]
        public float collectableSpawnChance;
        public List<Collectable> collectables;
    }
}

