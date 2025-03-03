using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.SceneManagement;
using UnityEngine;
using TMPro;



[System.Serializable]
public class StageThreshold
{
    public float accuracy = 0.75f;
    public float avgSwipeTime = 2000f;
    public int minSuccessfulSwipes = 12;
    public int totalCompletions = 1;
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
    [SerializeField] KeyboardTextSystemIntroduction textSystem;
    [SerializeField] SwipePerformanceTracker performanceTracker;

    // [SerializeField]
    private List<LearningStage> progressionStages = new List<LearningStage>
    {
        new LearningStage {
            stageNumber = 1,
            name = "Basic Patterns",
            activeKeys = new List<string> { "A", "TU", "EF" },
            practiceWords = new List<string> { "A fat ate fat", "Fat ate a tea", "Fate ate a feta", "A tea at a fete" },
            description = "First 3 keys",
            threshold = new StageThreshold { accuracy = 0, avgSwipeTime = 0, minSuccessfulSwipes = 0 }
        },
        new LearningStage {
            stageNumber = 2,
            name = "Common Three-Key Patterns",
            activeKeys = new List<string> { "A", "TU", "EF", "NO", "IJK", "S" },
            practiceWords = new List<string> { "None sees use in feats", "Son sits on soft seats", "Seek safe sites in June", "No one sees it fit", "Sit on sofa at east", "See sis eat fast snake" },
            description = "Add NO, IJK, S",
            threshold = new StageThreshold { accuracy = 0, avgSwipeTime = 0, minSuccessfulSwipes = 0 }
        },
        new LearningStage {
            stageNumber = 3,
            name = "High Frequency Keys",
            activeKeys = new List<string> { "A", "TU", "EF", "NO", "IJK", "S", "GH", "QR", "CD" },
            practiceWords = new List<string> { "The quick ghost finds each jar", "Figs can gather dust quite soon", "Sacred queens judge fair tasks", "Quiet factions cheer at grand feats", "This judge can quash the discount card", "She quotes the true facts" },
            description = "Add GH, QR, CD",
            threshold = new StageThreshold { accuracy = 0, avgSwipeTime = 0, minSuccessfulSwipes = 0 }
        },
        new LearningStage {
            stageNumber = 4,
            name = "Full Circle Patterns",
            activeKeys = new List<string> { "A", "TU", "EF", "NO", "IJK", "S", "GH", "QR", "CD", "L", "M", "VWX" },
            practiceWords = new List<string> { "wolves lurk in the quiet forest", "Vivid colors mix well", "Little fish travel east in cold water", "six mile tracks in forest land", "We must serve salmon at home", "Fox finds silver watches in unused desk" },
            description = "Add L, M, VWX",
            threshold = new StageThreshold { accuracy = 0, avgSwipeTime = 0, minSuccessfulSwipes = 0 }
        },
        new LearningStage {
            stageNumber = 5,
            name = "Complete Layout",
            activeKeys = new List<string> { "A", "TU", "EF", "NO", "IJK", "S", "GH", "QR", "CD", "L", "M", "VWX", "B", "YZ", "P" },
            practiceWords = new List<string> { "Fuzzy pups zip and zoom past the boy", "The zebra plays by the big pool all day", "Busy boys put on fun plays with props", "fly by on a path of pink and blue", "Big blaze as bats zip by.", "bees zip by pink and gold buds" },
            description = "Add B, YZ, P",
            threshold = new StageThreshold { accuracy = 0, avgSwipeTime = 0, minSuccessfulSwipes = 0 }
        }
    };
    private int currentStage = 0;

    public bool ShouldAdvanceStage(SwipePerformanceData performance)
    {
        // Debug.Log($"Stage Progress Check - Stage {currentStage}\n" +
        //           $"Performance: [Accuracy: {performance.accuracy:P0}, Time: {performance.duration:F0}ms, Swipes: {performance.successfulSwipes}]\n" +
        //           $"Required:   [Accuracy: {progressionStages[currentStage].threshold.accuracy:P0}, Time: {progressionStages[currentStage].threshold.avgSwipeTime:F0}ms, Swipes: {progressionStages[currentStage].threshold.minSuccessfulSwipes}]");
        

        if (currentStage >= progressionStages.Count) return false;

        var threshold = progressionStages[currentStage].threshold;
        // return performance.accuracy >= threshold.accuracy &&
        //        performance.duration <= threshold.avgSwipeTime &&
        //        performance.successfulSwipes >= threshold.minSuccessfulSwipes;
        return performance.totalSwipes >= threshold.totalCompletions;
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

    private List<string> currentWordQueue = new List<string>();
    private System.Random random = new System.Random();
    private string lastWord = "";

    private void Start()
    {
        // Change from textSystem.OnCorrectTextEntered to KeyboardTextSystemIntroduction.OnCorrectTextEntered
        KeyboardTextSystemIntroduction.OnCorrectTextEntered += SetNextWord;
        performanceTracker.OnPerformanceUpdated += CheckStageAdvancement;
        RefillWordQueue();
        SetNextWord();
    }

    private void OnDestroy()
    {
        // Change from textSystem.OnCorrectTextEntered to KeyboardTextSystemIntroduction.OnCorrectTextEntered
        KeyboardTextSystemIntroduction.OnCorrectTextEntered -= SetNextWord;
        performanceTracker.OnPerformanceUpdated -= CheckStageAdvancement;
    }

    private void CheckStageAdvancement(SwipePerformanceData performance)
    {
        if (ShouldAdvanceStage(performance))
        {
            AdvanceStage();
            UpdateActiveKeys();
            RefillWordQueue();
            performanceTracker.Reset();  // Reset performance tracking for new stage
            SetNextWord();
            textSystem.StartCoroutine(textSystem.SetupKeyboard());
        }
    }

    private void RefillWordQueue()
    {
        currentWordQueue.Clear();
        var practiceWords = GetCurrentPracticeWords();
        
        // Add each word twice to the queue
        foreach (var word in practiceWords)
        {
            currentWordQueue.Add(word);
            currentWordQueue.Add(word);
        }
        
        // Shuffle the queue
        for (int i = currentWordQueue.Count - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            var temp = currentWordQueue[i];
            currentWordQueue[i] = currentWordQueue[j];
            currentWordQueue[j] = temp;
        }
    }

    public void SetNextWord()
    {
        if (currentWordQueue.Count == 0)
        {
            RefillWordQueue();
        }

        if (currentWordQueue.Count > 0)
        {
            string nextWord = currentWordQueue[0];
            // If the next word is the same as the last word and we have more words available,
            // move it to the end of the queue and get a different word
            if (nextWord == lastWord && currentWordQueue.Count > 1)
            {
                currentWordQueue.RemoveAt(0);
                currentWordQueue.Add(nextWord);
                nextWord = currentWordQueue[0];
            }
            currentWordQueue.RemoveAt(0);
            lastWord = nextWord;
            textSystem.SetTarget(nextWord);
        }
    }

    private void Awake()
    {
        UpdateActiveKeys();
    }



    
}
