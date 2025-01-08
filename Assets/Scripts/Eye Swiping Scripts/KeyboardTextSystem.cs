using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Xml;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Windows;
using Vector3 = UnityEngine.Vector3;
using Vector2 = UnityEngine.Vector2;
using UnityEngine.Networking;
using System.Text;
using System.Threading;
using static UnityEngine.EventSystems.EventTrigger;
using System.Linq.Expressions;

public class KeyboardTextSystem : MonoBehaviour
{
    public string currTextInput;
    public TextMeshPro textInputRef;
    public string targetText = "starting!!";
    public int targetTextLength;
    public TextMeshPro targetTextRef;
    public bool completed = false;
    public KeyboardExperimentManager manager;
    public FlashingScript blinker;
    public TextMeshPro TargetCopy;
    public TextMeshPro CurrCopy;
    public bool beganWord = false;


    private bool started = false;
    private bool inputtingName = false;
    private int currWord = 0;
    private List<String> topWords = new List<string>();
    private bool usingSpecial = false;
    private string lastInput = "";
    private NgramModel probGenerator = new NgramModel("Assets/probs.json");
    private LevenshteinModel wordGenerator = new LevenshteinModel("Assets/vocab.json", .5,1,1);
    private string[] alphabet = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
    private string[] randomWords =
    {
        "Enter anything you want",
        "DESPITE",
        "MINUTE",
        "REASON",
        "FOREIGN",
        "GREATER",
        "ANYBODY",
        "TEACHER",
        "FUTURE",
        "PERFECT",
        "TROUBLE",
        "BALANCE",
        "CAPABLE",
        "EXACTLY",
        "ANOTHER",
        "BELIEVE",
        "CONTROL",
        "CONNECT",
        "MESSAGE",
        "MISTAKE",
        "NETWORK",
        "PROCESS",
        "PROJECT",
        "SUPPORT",
        "THOUGHT",
        "HISTORY",
        "IMPROVE",
        "MEETING",
        "SECTION",
        "STATION",
        "THEREFORE",
        "REMEMBER",
        "Finished! Stop typing"
    };
    private string[] randomSentences =
    {
        "Enter your name",
        "THE QUICK BROWN FOX JUMPED OVER THE LAZY DOG ",
        "A FASTER TRADER CAN SELL AT A HIGHER PRICE ",
        "WHATEVER YOU ARE BE A GOOD ONE ",
        "THE CHANGE YOU WISH TO SEE IN THE WORLD",
        "YOU ONLY FAIL IF YOU NEVER TRY ",
        "WHY DO YOU PLAY VIDEO GAMES EVERY DAY  ",
        "THE GLASS BREAKING WAS A MISTAKE TODAY ",
        "BUILD A NETWORK FOR A GREATER MEETING ",
        "DESPITE READING THE MESSAGE HE MADE HISTORY ",
        "JOHNNY SUPPORTS THEREFORE HE IS AMAZING ",
        "Finished! Stop typing"
    };
    private List<int> numberResults = new List<int>();


    private Dictionary<string, double> probabilities = new Dictionary<string, double>();
    public Dictionary<string, Vector3> RegularKeyboard = new Dictionary<string, Vector3>();
    public Dictionary<string, Vector3> SpecialKeyboard = new Dictionary<string, Vector3>();
    private List<Vector3> ValueList = new List<Vector3>();
    private Dictionary<string, int> PosDict = new Dictionary<string, int>();

    private SectionPositions sectionPositions;


    private List<Vector3> gazePoints;
    private float currWordTime;
    private string fileId;
    [SerializeField] private AudioSource buttonSound;
    [SerializeField] private AudioSource finishedWordSound;
    [SerializeField] private AudioSource deleteSound;
    private int phraseNum = 0;
    [SerializeField] private int numberOfPhrases = 10;
    private string path = @"C:\Users\joaolmbc\Desktop\Softkeyboard\gaze-collection-circle-{0}.txt";
    private SuperLogger logger;


    private List<string> phrasesToUse;
    public enum keyboardType
    {
        circle,
        circleAlphabetical,
        circleOuterRing,
        circleSmallerCloser,
        circle26,
        general
    }
    public enum KeyboardLayout
    {
        qwerty,
        circle
    }
    [SerializeField] private keyboardType currentKeyboard;
    [SerializeField] private KeyboardLayout keyboardLayout;
    void Awake()
    {
        gazePoints = new List<Vector3>();
        fileId = Guid.NewGuid().ToString().Substring(0, 8);
        
        //randomWords = randomSentences;
    }

    // Start is called before the first frame update
    void Start()
    {
        
        logger = FindObjectOfType<SuperLogger>();
        sectionPositions = FindObjectOfType<SectionPositions>();
        if (currentKeyboard == keyboardType.general)
        {
            Debug.Log("sending setup");
            StartCoroutine(SetupKeyboard());
        }
        targetText = "starting!!!";
        currTextInput = "";
        currWordTime = 0.0f;
        textInputRef.text = currTextInput;
        //GenerateText();
        phrasesToUse = GetRandomPhrases(numberOfPhrases);
    }

    private void Update()
    {
        CurrCopy.text = textInputRef.text;
        TargetCopy.text = targetTextRef.text;
        OutputData.UpdateTime();
        if (started || inputtingName)
        {
            var pos = manager.GetGazePoint();
            currWordTime += Time.deltaTime;
            gazePoints.Add(new Vector3(pos.x, pos.y, currWordTime));
        }

        if (currTextInput.Trim() == targetText)
        {
            //GenerateText();
            NextPhrase();
            finishedWordSound.Play();
            currTextInput = "";
            textInputRef.text = currTextInput;
            started = false;
            //lastInput = "";
            //manager.nextWord();
            gazePoints.Clear();
            currWordTime = 0.0f;
            currWord = 0;
        }
    }

    public static List<string> GetRandomPhrases(int numlines)
    {
        List<string> lines = new List<string>();

        try
        {
            lines = new List<string>(System.IO.File.ReadAllLines("Assets/AidanTestResources/phrases2.txt"));
            for (int i = 0; i < lines.Count; i++)
            {
                lines[i] = lines[i].ToUpper();
            }
            for (int i = 0; i < lines.Count; i++)
            {
                int randomIndex = UnityEngine.Random.Range(i, lines.Count);
                (lines[i], lines[randomIndex]) = (lines[randomIndex], lines[i]);
            }
            return lines.GetRange(0, Mathf.Min(numlines, lines.Count));
        }
        catch (Exception e)
        {
            Debug.LogError($"Error reading text file: {e.Message}");
            return lines;
        }
    }

    private bool hasBegun = false;
    private void Begin()
    {
        if (hasBegun)
        {
            return;
        }
        hasBegun = true;
        targetText = phrasesToUse[0];
        targetTextRef.text = targetText;
        currTextInput = "";
        textInputRef.text = "";

    }


    public void RecieveSuggestion(string input)
    {
        input = input.ToUpper();
        //started = true;
        if (string.IsNullOrEmpty(input))
        {
            return;
        }
        DeleteLastWord();
        OutputData.WriteSuggestionAccepted(string.Format(path, fileId), input);
        currTextInput += " " + input;

        //blinker.inputted();
        buttonSound.Play();


        textInputRef.text = currTextInput;

    }
    private void DeleteLastWord()
    {
        string[] words = currTextInput.Split(' ');
        if (words.Length > 0)
        {
            Array.Resize(ref words, words.Length - 1);
            currTextInput = string.Join(" ", words);
        }
        textInputRef.text = currTextInput;
        Begin();
    }
    public void RecieveDelete()
    {
        blinker.deleted();

        //currTextInput = "";

        DeleteLastWord();
        OutputData.WriteDeletePressed(string.Format(path, fileId));
        //string[] words = currTextInput.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        //Array.Resize(ref words, words.Length - 1);
        //currTextInput = string.Join(" ", words);
        //currTextInput += " ";

        //textInputRef.text = currTextInput;
        deleteSound.Play();
        gazePoints.Clear();
        currWordTime = 0.0f;
        if (currWord > 0) { currWord -= 1; }
        
        //started = false;
        inputtingName = false;
    }


    private void NextPhrase()
    {
        phraseNum++;
        if (phraseNum < numberOfPhrases)
        {
            targetText = phrasesToUse[phraseNum];
            targetTextRef.text = phrasesToUse[phraseNum];
        } else
        {
            Finished();
        }
    }

    private void Finished()
    {
        targetTextRef.text = "Finished! Nice Job!";
    }


    public string GetCurrentWord()
    {
        string[] words = targetText.Split(' ');
        string ou = "";
        try
        {
            ou = words[currWord];
        }
        catch
        {

        }
        return ou;
    }

    // Called by EnterScript when spacebar is pressed
    public void RecieveEnter()
    {
        Debug.Log("Recieved enter");
        
        //blinker.inputted();
        //buttonSound.Play();

        if (started)
        {
            started = false;
            StartCoroutine(runPrediction());
            logger.StartEntry();
            string[] words = targetText.Split(' ');
            OutputData.WriteGazePoints(gazePoints, words[currWord], string.Format(path, fileId), topWords);
            //if (currWord > 1)
            //{
            //    
            //}
            
            gazePoints.Clear();
            currWordTime = 0.0f;
        }
        else
        {
            logger.StopEntry();
            started = true;
            manager.enable_timer();
            //manager.nextWord();
        }
    }

    [SerializeField] private GameObject topBound;
    [SerializeField] private GameObject bottomBound;
    [SerializeField] private GameObject leftBound;
    [SerializeField] private GameObject rightBound;
    [SerializeField] private GameObject manualCenter;

    private IEnumerator SetupKeyboard()
    {
        string keyboard = sectionPositions.GetPositions2D();
        //hardcoded values for now
        KeyboardInfo keyboardInfo = new KeyboardInfo();
        if (keyboardLayout == KeyboardLayout.circle)
        {
            Vector2 cent = new Vector2(manualCenter.transform.position.x, manualCenter.transform.position.y);
            keyboardInfo = new KeyboardInfo { keyboard = keyboard, center = cent, inner_radius = 0.323f, outer_radius = 0.5f, k = 1, shape = "circle" };
        } else if (keyboardLayout == KeyboardLayout.qwerty)
        {
            Vector2 top_bound = new Vector2(topBound.transform.position.x, topBound.transform.position.y);
            Vector2 bottom_bound = new Vector2(bottomBound.transform.position.x, bottomBound.transform.position.y);
            Vector2 left_bound = new Vector2(leftBound.transform.position.x, leftBound.transform.position.y);
            Vector2 right_bound = new Vector2(rightBound.transform.position.x, rightBound.transform.position.y);
            keyboardInfo = new KeyboardInfo
            {
                keyboard = keyboard,
                center = new Vector2(0, 0.221f),
                inner_radius = 0.001f,
                outer_radius = 100f,
                k = 3,
                shape = "rectangle",
                right_bound = right_bound,
                left_bound = left_bound,
                top_bound = top_bound,
                bottom_bound = bottom_bound
            };
        }

        string json = JsonUtility.ToJson(keyboardInfo);
        logger.WriteSetup(json);
        byte[] jsonBytes = Encoding.UTF8.GetBytes(json);

        using UnityWebRequest request = new UnityWebRequest("http://localhost:5000/setup", "POST");
        request.uploadHandler = new UploadHandlerRaw(jsonBytes);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(request.error);
        }
        else
        {

        }
    }

    private IEnumerator runPrediction()
    {
        float r = 0;
        Vector2 cent = new Vector2(0.0f, 0.0f);
        string extra = "";
        float or = 0;
        switch (currentKeyboard)
        {
            case keyboardType.circle:
                r = 0.891277f;
                cent = new Vector2(0.0f, 0.424f);
                extra = "circle";
                break;
            case keyboardType.circleAlphabetical:
                r = 0.75f;
                cent = new Vector2(0.0f, 0.525f);
                extra = "circleAlpha";
                break;
            case keyboardType.circleOuterRing:
                r = 0.75f;
                or = 1.2f;
                cent = new Vector2(0.0f, 0.525f);
                extra = "circleOuter";
                break;
            case keyboardType.circleSmallerCloser:
                r = 0.295f;
                or = 0.475f;
                cent = new Vector2(0.0f, 0.221f);
                extra = "circleSmaller";
                break;
            case keyboardType.circle26:
                r = 0.31f;
                or = 0.46f;
                cent = new Vector2(0.0f, 0.215f);
                extra = "circle26";
                break;
            case keyboardType.general:
                r = 0.31f;
                or = 0.46f;
                cent = new Vector2(0.0f, 0.215f);
                extra = "general";
                break;


        }
        string[] words = targetText.Split(' ');
        string[] bigramWords = new string[2];
        if (words.Length >= 2)
        {
            bigramWords[0] = words[words.Length - 2];
            bigramWords[1] = words[words.Length - 1];
        } else
        {
            bigramWords[0] = "<s>";
            bigramWords[1] = "<s>";
        }
        GazeData gazeData = new GazeData { gaze_points = gazePoints, radius = r, outer_radius = or, center = cent, context = bigramWords };

        string json = JsonUtility.ToJson(gazeData);
        Debug.Log("Gaze Points: " + gazePoints);
        Debug.Log("Serialized JSON: " + json);
        //UnityWebRequest wr = new UnityWebRequest("http://localhost:5000/circle");
        //WWWForm form = new WWWForm();
        //form.AddField("json", json);
        byte[] jsonBytes = Encoding.UTF8.GetBytes(json);
        
        using (UnityWebRequest request = new UnityWebRequest("http://localhost:5000/" + extra, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(jsonBytes);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(request.error);
            }
            else
            {
                var response = JsonUtility.FromJson<Response>(request.downloadHandler.text);
                topWords.Clear();
                gazePoints.Clear();
                topWords.AddRange(response.top_words);
                Debug.Log("Top words updated.");
                Debug.Log(topWords.ToString());

                currTextInput += " " + topWords[0].ToUpper();
                // currTextInput += topWords[0].ToUpper() + " ";
                textInputRef.text = currTextInput;
                currWord += 1;
            }
        }

    }

    [System.Serializable]
    public class GazeData
    {
        public List<Vector3> gaze_points;
        public float radius;
        public Vector2 center;
        public float outer_radius;
        public string[] context;
    }

    [System.Serializable]
    public class KeyboardInfo
    {
        public string keyboard;
        public Vector2 center;
        public float inner_radius;
        public float outer_radius;
        public int k;
        public string shape;
        public Vector2 left_bound;
        public Vector2 right_bound;
        public Vector2 top_bound;
        public Vector2 bottom_bound;
    }
        

    [System.Serializable]
    public class Response
    {
        public List<string> top_words;
    }

    public List<string> GiveTopWords()
    {
        return topWords;
    }

}
