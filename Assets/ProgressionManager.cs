using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.SceneManagement;
using UnityEngine;


[System.Serializable]
public class StageThreshold
{
    public float accuracy = 0.75f;
    public float avgSwipeTime = 2000f;
    public int minSuccessfulSwipes = 12;
}

[System.Serializable]
public class LearningStage
{
    public int stageNumber;
    public string name;
    public List<string> activeKeys;
    public List<string> practiceWords;
    public string description;
    public StageThreshold threshold;
}


public class ProgressionManager : MonoBehaviour
{
    [SerializeField] GameObject keyholder;
    [SerializeField]
    private List<LearningStage> progressionStages = new List<LearningStage>
    {
        new LearningStage {
            stageNumber = 1,
            name = "Basic Patterns",
            activeKeys = new List<string> { "A", "TU", "NO" },
            practiceWords = new List<string> { "tat", "to", "at", "an", "oat" },
            description = "Learn basic left-right eye movements across common keys",
            threshold = new StageThreshold { accuracy = 0.75f, avgSwipeTime = 2000f, minSuccessfulSwipes = 12 }
        },
        new LearningStage {
            stageNumber = 2,
            name = "Common Three-Key Patterns",
            activeKeys = new List<string> { "A", "TU", "NO", "S", "IJK", "CD" },
            practiceWords = new List<string> { "is", "in", "it", "sit", "cat", "and" },
            description = "Short words using three key groups",
            threshold = new StageThreshold { accuracy = 0.70f, avgSwipeTime = 1800f, minSuccessfulSwipes = 15 }
        },
        new LearningStage {
            stageNumber = 3,
            name = "High Frequency Keys",
            activeKeys = new List<string> { "A", "TU", "NO", "S", "IJK", "CD", "EF", "GH" },
            practiceWords = new List<string> { "had", "get", "she", "the", "this" },
            description = "Common words using frequently used key groups",
            threshold = new StageThreshold { accuracy = 0.65f, avgSwipeTime = 1600f, minSuccessfulSwipes = 20 }
        },
        new LearningStage {
            stageNumber = 4,
            name = "Full Circle Patterns",
            activeKeys = new List<string> { "A", "TU", "NO", "S", "IJK", "CD", "EF", "GH", "L", "M", "P" },
            practiceWords = new List<string> { "help", "small", "plan", "make" },
            description = "Words requiring circular eye movements",
            threshold = new StageThreshold { accuracy = 0.60f, avgSwipeTime = 1400f, minSuccessfulSwipes = 25 }
        },
        new LearningStage {
            stageNumber = 5,
            name = "Complete Layout",
            activeKeys = new List<string> { "A", "TU", "NO", "S", "IJK", "CD", "EF", "GH", "L", "M", "P", "QR", "VWX", "YZ", "B" },
            practiceWords = new List<string> { "very", "quick", "why", "been", "work" },
            description = "Full keyboard access with all key groups",
            threshold = new StageThreshold { accuracy = 0.55f, avgSwipeTime = 1200f, minSuccessfulSwipes = 30 }
        }
    };
    private int currentStage = 0;

    [System.Serializable]
    public class SwipePerformanceData
    {
        public float accuracy;
        public float duration;
        public int successfulSwipes;
    }

    public bool ShouldAdvanceStage(SwipePerformanceData performance)
    {
        if (currentStage >= progressionStages.Count) return false;

        var threshold = progressionStages[currentStage].threshold;
        return performance.accuracy >= threshold.accuracy &&
               performance.duration <= threshold.avgSwipeTime &&
               performance.successfulSwipes >= threshold.minSuccessfulSwipes;
    }

    public void AdvanceStage()
    {
        if (currentStage < progressionStages.Count - 1)
        {
            currentStage++;
        }
    }

    public List<string> GetCurrentPracticeWords()
    {
        return progressionStages[currentStage].practiceWords;
    }

    public List<string> GetActiveKeys()
    {
        return progressionStages[currentStage].activeKeys;
    }

    public int GetCurrentStage()
    {
        return currentStage;
    }

    public LearningStage GetCurrentStageInfo()
    {
        return progressionStages[currentStage];
    }
    private void UpdateActiveKeys()
    {
        var keys = GetActiveKeys();
        foreach (Transform child in keyholder.transform)
        {
            child.gameObject.SetActive(keys.Contains(child.name));
        }
    }

    private void Awake()
    {
        UpdateActiveKeys();
    }
}
