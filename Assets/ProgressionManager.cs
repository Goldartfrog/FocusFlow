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
    public int totalCompletions = 5;
    public int duration = 480; // 8 minutes
    //NOTE: change progression indicator with this variable.
}

[System.Serializable]
public class LearningStage
{
    public int stageNumber;
    public string name;
    public List<string> activeKeys;
    public List<string> practicePhrases;
    public string description;
    public StageThreshold threshold;
    public List<string> highlightkeys;
    public string prescreen;
}


public class ProgressionManager : MonoBehaviour
{
    [SerializeField] GameObject keyholder;
    [SerializeField] KeyboardTextSystemIntroduction textSystem;
    [SerializeField] SwipePerformanceTracker performanceTracker;
    [SerializeField] SuperLogger logger;
    [SerializeField] ProgressIndicator progressBarRef;
    [SerializeField] SuggestedWordsScriptTutorial suggestedWordsScript;

    // [SerializeField]
    private List<LearningStage> progressionStages = new List<LearningStage>
    {
        new LearningStage {
            stageNumber = 0,
            name = "Basic Patterns",
            activeKeys = new List<string> { "A", "TU", "EF" },
            practicePhrases = new List<string> { "FATE ATE A FAT",
            "EAT FATE AFT",
            "FAT TEA AT AFT",
            "ATE FAT TEA",
            "TEA AT A FATE",
            "AFT ATE FAT" },
            description = "First 3 keys",
            threshold = new StageThreshold { accuracy = 0, avgSwipeTime = 0, minSuccessfulSwipes = 0 },
            highlightkeys = new List<string> { "A", "TU", "EF" }
            /*
            "A FAT EAT FAT", "FAT ATE A TEA", "FATE ATE A FETA", "A TEA AT A FETE",
            "Fate ate a fat",
            "Eat fate aft",
            "Fat tea at aft",
            "Ate fat tea",
            "Tea at a fate",
            "Aft ate fat",
            "Ate a tau tea",
            "Teat a fat ate",
            "Ate fat Teat",
            "eat a fat ute fate"
            */
        },
        new LearningStage {
            stageNumber = 1,
            name = "Common Three-Key Patterns",
            activeKeys = new List<string> { "A", "TU", "EF", "NO", "IJK", "S" },
            practicePhrases = new List<string> { "A FAT FATE",
            "SIT AT FANS",
            "A JOKE FITS",
            "I ATE SEA",
            "INK JOTS FAT",
            "SEA FIT JOKES" },
            description = "Add NO, IJK, S",
            threshold = new StageThreshold { accuracy = 0, avgSwipeTime = 0, minSuccessfulSwipes = 0 },
            highlightkeys = new List<string> { "NO", "IJK", "S" },
            prescreen = "Now we're going to add three new keys. These will be 'NO', 'IJK', and 'S'."
            /*
            "NONE SEES USE IN FEATS", "SON SITS ON SOFT SEATS", "SEEK SAFE SITES IN JUNE", "NO ONE SEES IT FIT", "SIT ON SOFA AT EAST", "SEE SIS EAT FAST SNAKE"
            "A FAT FATE",
            "SIT AT FANS",
            "A JOKE FITS",
            "I ATE SEA",
            "INK JOTS FAT",
            "SEA FIT JOKES",
            "TAKE FUN JOTS",
            "I JOKE AS FAN",
            "FATES JOINS US",
            "FASTEN I SIT"
            */
        },
        new LearningStage {
            stageNumber = 2,
            name = "High Frequency Keys",
            activeKeys = new List<string> { "A", "TU", "EF", "NO", "IJK", "S", "GH", "QR", "CD" },
            practicePhrases = new List<string> { "THE QUICK GHOST", "FIND EACH JAR", "FIGS CAN GATHER", "DUST QUITE SOON", "SACRED QUEENS JUDGE", "THE TRUE FACTS" },
            description = "Add GH, QR, CD",
            threshold = new StageThreshold { accuracy = 0, avgSwipeTime = 0, minSuccessfulSwipes = 0 },
            highlightkeys = new List<string> { "GH", "QR", "CD" },
            prescreen = "Now we're going to add three more keys. These will be 'GH', 'QR', and 'CD'."
            /*
            "CARDS GIFT HOG QUIT SAD",
            "QUAD HOG CRIT SING",
            "FADE CHUG RIG TORN QUO",
            "CART HUD FOG SINK",
            "QUART CINCH DOG FISH RIG",
            "HOST CORDS FAQIR JUG",
            "TRACK CODA FUR SIGN HOC",
            "CHAIN GUARD QUEST FOG ROD"
            */
        },
        new LearningStage {
            stageNumber = 3,
            name = "Full Circle Patterns",
            activeKeys = new List<string> { "A", "TU", "EF", "NO", "IJK", "S", "GH", "QR", "CD", "L", "M", "VWX" },
            practicePhrases = new List<string> { "IN THE QUIET FOREST", "VIVID COLORS MIX WELL", "LITTLE FISH TRAVEL EAST", "IN COLD WATER", "SIX MILE TRACKS", "IN FOREST LAND", "WE MUST SERVE SALMON" },
            description = "Add L, M, VWX",
            threshold = new StageThreshold { accuracy = 0, avgSwipeTime = 0, minSuccessfulSwipes = 0 },
            highlightkeys = new List<string> { "L", "M", "VWX" },
            prescreen = "Now we're going to add three more keys. These will be 'L', 'M', and 'VWX'."
            /*
            "Vial Mow Dux Most"
            "Wind Clam Fox Vex Jolt"
            "Mix Clove Dwarf Sing Hot"
            "Fam Final Scow Trudge Mix"
            "Low Max Victor Judge Wish"
            "Six Calm Vouch Wend Motif"
            "Welt Mild Vixen Scald Jar"
            "Max Low Vouch Dent Swirl"
            */
        },
        new LearningStage {
            stageNumber = 4,
            name = "Complete Layout",
            activeKeys = new List<string> { "A", "TU", "EF", "NO", "IJK", "S", "GH", "QR", "CD", "L", "M", "VWX", "B", "YZ", "P" },
            practicePhrases = new List<string> { "FUZZY PUPS ZIP", "ZOOM PAST THE BOY", "THE ZEBRA PLAYS", "THE BIG POOL ALL DAY", "BUSY BOYS PUT ON FUN", "FLY BY ON A PATH", "OF PINK AND BLUE" },
            description = "Add B, YZ, P",
            threshold = new StageThreshold { accuracy = 0, avgSwipeTime = 0, minSuccessfulSwipes = 0 },
            highlightkeys = new List<string> { "B", "YZ", "P" },
            prescreen = "Now we're going to add three more keys. These will be 'B', 'YZ', and 'P'."
            /*
            "Razor Went Zip"
            "Zap Bow Fly Myth"
            "Bandy Wow Zip Thug"
            "My Zebra Cub Avoids Pink"
            "Comb Plaza Wynd Job Tofu"
            "Bypath Cove Jumps Wiz Fawn"
            "Bow Pyx Hot Jumps Livid"
            */
        },
        new LearningStage {
            stageNumber = 5,
            name = "Final testing",
            activeKeys = new List<string> { "A", "TU", "EF", "NO", "IJK", "S", "GH", "QR", "CD", "L", "M", "VWX", "B", "YZ", "P" },
            practicePhrases = new List<string> {
                "MY FAVORITE PLACE TO VISIT",
                "THREE TWO ONE ZERO BLAST OFF",
                "IF AT FIRST YOU DO NOT SUCCEED",
                "PLEASE PROVIDE YOUR DATE OF BIRTH",
                "WE RUN THE RISK OF FAILURE",
                "LOVE MEANS MANY THINGS",
                "YOU MUST BE GETTING OLD",
                "THE WORLD IS A STAGE",
                "I AGREE WITH YOU",
                "DO NOT SAY ANYTHING" },
            description = "Final testing",
            threshold = new StageThreshold { accuracy = 0, avgSwipeTime = 0, minSuccessfulSwipes = 0 },
            highlightkeys = new List<string> { },
            prescreen = "Please take off the headset and complete the survey that the experimenter will give you. When instructed to put the headset back on, press continue to continue.This will be the final evaluation. Do your best to type 5 phrases correctly."
        }, 
        new LearningStage {
            stageNumber = 2,
            name = "ENDSCREEN",
            activeKeys = new List<string> { },
            practicePhrases = new List<string> { },
            description = "Testing is complete. Thank you for your participation.",
            threshold = new StageThreshold { accuracy = 0, avgSwipeTime = 0, minSuccessfulSwipes = 0 },
            highlightkeys = new List<string> { },
            prescreen = "Testing is complete. Thank you for your participation."
        }
    };

    private List<LearningStage> controlStages = new List<LearningStage> {
        new LearningStage {
            stageNumber = 0,
            name = "Control Stage 1",
            activeKeys = new List<string> { "A", "TU", "EF", "NO", "IJK", "S", "GH", "QR", "CD", "L", "M", "VWX", "B", "YZ", "P" },
            practicePhrases = new List<string> { 
                "FATE ATE A FAT",
            "EAT FATE AFT",
            "FAT TEA AT AFT",
            "ATE FAT TEA",
            "TEA AT A FATE",
            "AFT ATE FAT",
                "A FAT FATE",
            "SIT AT FANS",
            "A JOKE FITS",
            "I ATE SEA",
            "INK JOTS FAT",
            "SEA FIT JOKES",
                "THE QUICK GHOST", "FIND EACH JAR", "FIGS CAN GATHER", "DUST QUITE SOON", "SACRED QUEENS JUDGE", "THE TRUE FACTS",
                "IN THE QUIET FOREST", "VIVID COLORS MIX WELL", "LITTLE FISH TRAVEL EAST", "IN COLD WATER", "SIX MILE TRACKS", "IN FOREST LAND", "WE MUST SERVE SALMON",
                "FUZZY PUPS ZIP", "ZOOM PAST THE BOY", "THE ZEBRA PLAYS", "THE BIG POOL ALL DAY", "BUSY BOYS PUT ON FUN", "FLY BY ON A PATH", "OF PINK AND BLUE"
            },
            description = "Control Group Stage",
            threshold = new StageThreshold { accuracy = 0, avgSwipeTime = 0, minSuccessfulSwipes = 0 },
            highlightkeys = new List<string> { },
            prescreen = "Please take a short rest. When ready, look at the continue button and then type 5 more sentences."
        },
        new LearningStage {
            stageNumber = 1,
            name = "Control Testing",
            activeKeys = new List<string> { "A", "TU", "EF", "NO", "IJK", "S", "GH", "QR", "CD", "L", "M", "VWX", "B", "YZ", "P" },
            practicePhrases = new List<string> {
                "MY FAVORITE PLACE TO VISIT",
                "THREE TWO ONE ZERO BLAST OFF",
                "IF AT FIRST YOU DO NOT SUCCEED",
                "PLEASE PROVIDE YOUR DATE OF BIRTH",
                "WE RUN THE RISK OF FAILURE",
                "LOVE MEANS MANY THINGS",
                "YOU MUST BE GETTING OLD",
                "THE WORLD IS A STAGE",
                "I AGREE WITH YOU",
                "DO NOT SAY ANYTHING" },
            description = "Control Group Testing",
            threshold = new StageThreshold { accuracy = 0, avgSwipeTime = 0, minSuccessfulSwipes = 0 },
            highlightkeys = new List<string> { },
            prescreen = "Please take off the headset and complete the survey that the experimenter will give you. When instructed to put the headset back on, press continue to continue.This will be the final evaluation. Do your best to type 5 phrases correctly."
        },
        new LearningStage {
            stageNumber = 2,
            name = "ENDSCREEN",
            activeKeys = new List<string> { },
            practicePhrases = new List<string> { },
            description = "Testing is complete. Thank you for your participation.",
            threshold = new StageThreshold { accuracy = 0, avgSwipeTime = 0, minSuccessfulSwipes = 0 },
            highlightkeys = new List<string> { },
            prescreen = "Testing is complete. Thank you for your participation."
        }
    };


    [SerializeField]
    private bool isControlGroup = false;

    public int currentStage = 0;

    private List<LearningStage> GetActiveStages()
    {
        return isControlGroup ? controlStages : progressionStages;
    }

    public bool ShouldAdvanceStage(SwipePerformanceData performance)
    {
        // if (currentStage >= GetActiveStages().Count) return false;

        var threshold = GetActiveStages()[GetRightStage()].threshold;
        
        // For experimental group, use normal progression
        return performance.totalSwipes >= threshold.totalCompletions;
    }

    public void AdvanceStage()
    {
        if (currentStage < progressionStages.Count - 1) {
            currentStage++;
        }
    }

    private int GetRightStage() { 
        if (isControlGroup) {
            if (currentStage < 4) {
                return 0;
            } else if (currentStage == 4) {
                return 1;
            } else {
                return 2;
            }
        } else {
            return currentStage;
        }
    }

    public List<string> GetCurrentPracticePhrases()
    {
        return GetActiveStages()[GetRightStage()].practicePhrases;
    }

    public List<string> GetActiveKeys()
    {
        return GetActiveStages()[GetRightStage()].activeKeys;
    }

    public LearningStage GetCurrentStageInfo()
    {
        return GetActiveStages()[GetRightStage()];
    }

    private void UpdateActiveKeys()
    {
        var keys = GetActiveKeys();
        foreach (Transform child in keyholder.transform)
        {
            child.gameObject.SetActive(keys.Contains(child.name));
        }
    }

    private List<string> currentPhraseQueue = new List<string>();
    private System.Random random = new System.Random();
    private string lastPhrase = "";

    private void Start()
    {
        // Change from textSystem.OnCorrectTextEntered to KeyboardTextSystemIntroduction.OnCorrectTextEntered
        KeyboardTextSystemIntroduction.OnCorrectTextEntered += SetNextPhrase;
        performanceTracker.OnPerformanceUpdated += CheckStageAdvancement;
        if (logger == null)
        {
            logger = FindObjectOfType<SuperLogger>();
        }
        if (suggestedWordsScript == null) {
            suggestedWordsScript = GameObject.Find("SuggestedWords").GetComponent<SuggestedWordsScriptTutorial>();
        }



        RefillPhraseQueue();
        SetNextPhrase();
        SetHighlightKeys();
        suggestedWordsScript.ClearText();
        // TODO: Add prescreen showing
    }

    private void OnDestroy()
    {
        // Change from textSystem.OnCorrectTextEntered to KeyboardTextSystemIntroduction.OnCorrectTextEntered
        KeyboardTextSystemIntroduction.OnCorrectTextEntered -= SetNextPhrase;
        performanceTracker.OnPerformanceUpdated -= CheckStageAdvancement;
    }

    private void CheckStageAdvancement(SwipePerformanceData performance)
    {
        Debug.Log("Current Stage: " + currentStage);
        // progressBarRef.ReceiveUpdate();
        if (ShouldAdvanceStage(performance))
        {
            AdvanceStage();
            UpdateActiveKeys();
            RefillPhraseQueue();
            performanceTracker.Reset();  // Reset performance tracking for new stage
            SetNextPhrase();
            textSystem.StartCoroutine(textSystem.SetupKeyboard());
            logger.LogEvent("Stage Completed", GetRightStage().ToString());
            SetHighlightKeys();
            // TODO: Add prescreen showing
            suggestedWordsScript.ClearText();
        }
    }

    private void RefillPhraseQueue()
    {
        currentPhraseQueue.Clear();
        var practicePhrases = GetCurrentPracticePhrases();
        
        // Add each phrase twice to the queue
        foreach (var phrase in practicePhrases)
        {
            currentPhraseQueue.Add(phrase);
            currentPhraseQueue.Add(phrase);
        }
        
        // Shuffle the queue
        for (int i = currentPhraseQueue.Count - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            var temp = currentPhraseQueue[i];
            currentPhraseQueue[i] = currentPhraseQueue[j];
            currentPhraseQueue[j] = temp;
        }
    }

    public void SetNextPhrase()
    {
        if (currentPhraseQueue.Count == 0)
        {
            RefillPhraseQueue();
        }

        if (currentPhraseQueue.Count > 0)
        {
            string nextPhrase = currentPhraseQueue[0];
            // If the next phrase is the same as the last phrase and we have more phrases available,
            // move it to the end of the queue and get a different phrase
            if (nextPhrase == lastPhrase && currentPhraseQueue.Count > 1)
            {
                currentPhraseQueue.RemoveAt(0);
                currentPhraseQueue.Add(nextPhrase);
                nextPhrase = currentPhraseQueue[0];
            }
            currentPhraseQueue.RemoveAt(0);
            lastPhrase = nextPhrase;
            textSystem.SetTarget(nextPhrase);
            logger.LogEvent("New Phrase", nextPhrase);
        }
    }

    private void Awake()
    {
        UpdateActiveKeys();
    }

    private void SetHighlightKeys() {
        var highlightkeys = GetCurrentStageInfo().highlightkeys;
        if (highlightkeys.Count > 0) {  
            foreach (Transform child in keyholder.transform)
            {
                if (highlightkeys.Contains(child.name)) {
                    Debug.Log("Highlighting key: " + child.name);
                    child.gameObject.GetComponent<letterTutorialScript>().SetDefaultColor(new Color(0.4f, 0.7f, 0.04f));
                } else {
                    child.gameObject.GetComponent<letterTutorialScript>().SetDefaultColor(new Color(1, 1, 1));
                }
            }
        }
    }

    public int GetStageTime() {
        return GetActiveStages()[GetRightStage()].threshold.duration;
    }



    
}
