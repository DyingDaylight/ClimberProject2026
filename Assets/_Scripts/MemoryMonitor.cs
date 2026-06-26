using System;
using UnityEngine;
using TMPro;
using Unity.Notifications.Android; // Use TextMeshPro
using UnityEngine.Profiling;

public class MemoryMonitor : MonoBehaviour {
    TMP_Text memoryText;

    private void Start()
    {
        memoryText = GetComponent<TMP_Text>();
    }

    void Update() {
        // Get total allocated memory in Megabytes
        long totalMemory = Profiler.GetTotalAllocatedMemoryLong(); 
        double mbMemory = totalMemory / (1024.0 * 1024.0);

        memoryText.text = $"Total Allocated Memory: {mbMemory:F2} MB";
    }
}