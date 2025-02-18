using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using Valve.VR;
using ViveSR.anipal.Eye;
using static KeyboardTextSystem;

public class KeyboardTextSystemIntroduction : MonoBehaviour
{
    public string currTextInput;
    public TextMeshPro textInputRef;
    public string targetText;
    public int targetTextLength;
    public TextMeshPro targetTextRef;
    public bool completed = false;
    public FlashingScriptIntroduction blinker;
    // public KeyboardExperimentManager keyboardManager;

    public bool beganWord = false;

    private List<string> wordsToType;


    private string lastTarget = "";



    private bool started = false;
    private bool inputtingName = false;
    private int currWord = 0;
    private List<String> topWords = new List<string>();
    private string lastInput = "";
    // private LevenshteinModel wordGenerator = new LevenshteinModel("Assets/vocab.json", .5, 1, 1);

    public InteractionEyeTracker EyePos;
    private List<Vector3> gazePoints;
    private float currWordTime;
    private string fileId;
    private SuperLogger logger;
    [SerializeField] private GameObject keyHolder;

    [SerializeField] private AudioSource buttonSound;
    [SerializeField] private AudioSource finishedWordSound;
    [SerializeField] private AudioSource deleteSound;

    private enum keyboardMode
    {
        singleWord,
        sentences
    }
    [SerializeField] private keyboardMode currMode;



    //public enum keyboardType
    //{
    //    circle,
    //    circleAlphabetical,
    //    circleOuterRing,
    //    general
    //}
    [SerializeField] private keyboardType currentKeyboard;

    public delegate void CorrectTextEnteredHandler();
    public static event CorrectTextEnteredHandler OnCorrectTextEntered;

    [SerializeField] private SwipePerformanceTracker performanceTracker;

    void Awake()
    {
        gazePoints = new List<Vector3>();
        fileId = Guid.NewGuid().ToString().Substring(0, 8);
        logger = FindObjectOfType<SuperLogger>();
        //randomWords = randomSentences;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (keyHolder == null)
        {
            keyHolder = GameObject.Find("keyHolder");
        }
        
        currTextInput = "";
        
        textInputRef.text = currTextInput;
        targetText = "Read intro ->";
        targetTextRef.text = targetText;
        currWordTime = 0.0f;

        OnCorrectTextEntered += PlayCorrectWordSound;

        if (currentKeyboard == keyboardType.general)
        {
            Debug.Log("sending setup");
            StartCoroutine(SetupKeyboard());
        }

        //FillListWithRandomNumbers(numberResults, 5, 1, 10);
        //updatePosDict();
    }

    public Vector2 GetGazePoint()
    {
        Vector3 userPos = EyePos.worldPosition;
        Vector3 fixationPT = EyePos.gazeLocation;
        Vector3 direction = fixationPT - userPos;

        float zPlane = 2;
        if (direction.z == 0)
        {
            return Vector2.zero;
        }

        float t = (zPlane - userPos.z) / direction.z;

        Vector3 intersectionPoint = userPos + t * direction;

        return new Vector2(intersectionPoint.x, intersectionPoint.y);
    }

    private void Update()
    {
        if (started || inputtingName)
        {
            var pos = GetGazePoint();
            currWordTime += Time.deltaTime;
            gazePoints.Add(new Vector3(pos.x, pos.y, currWordTime));
        }

        if (currTextInput.Equals(targetText, StringComparison.OrdinalIgnoreCase))
        {
            OnCorrectTextEntered?.Invoke();
            if (performanceTracker != null)
            {
                performanceTracker.RecordSwipe(true, currWordTime);
            }

            started = false;
            lastInput = "";
            gazePoints.Clear();
            currWordTime = 0.0f;
        }

        //Debug.Log(CorrectTarget());
    }

    public void RecieveSuggestion(string input)
    {
        input = input.ToUpper();

        if (string.IsNullOrEmpty(input)) return;
        //Debug.Log("Should be happening");
        if (currMode == keyboardMode.singleWord)
        {
            currTextInput = input;
        } else
        {
            string[] words = currTextInput.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            words[words.Length - 1] = input;
            currTextInput = string.Join(" ", words);
        }
        buttonSound.Play();

        logger.AcceptedSuggestion(input);

        // update the last word

        //currTextInput += " ";

        //blinker.inputted();

        lastInput = input[input.Length - 1].ToString();

        if (currTextInput.Equals(targetText, StringComparison.OrdinalIgnoreCase))
        {
            OnCorrectTextEntered?.Invoke();
            //Debug.Log("entrei");
            started = false;
            lastInput = "";
            gazePoints.Clear();
            currWordTime = 0.0f;
        }

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
        
    }

    public void RecieveDelete()
    {
        if (currWord > 0)
        {
            currWord -= 1;
        } else
        {
            logger.Deleted();
        }
        
        DeleteLastWord();
        deleteSound.Play();
        gazePoints.Clear();
    }



    public void RecieveEnter()
    {
        //Debug.Log("Recieved enter");

        //blinker.inputted();
        //Debug.Log("HERE!!");
        //buttonSound.Play();

        if (started)
        {
            StartCoroutine(runPrediction());
            string path = @"C:\Users\joaolmbc\Desktop\Softkeyboard\gaze-collection-circle-{0}.txt";
            string[] words = targetText.Split(' ');
            logger.StopEntry();
            //Debug.Log(words);
            //OutputData.Write(gazePoints, words[currWord], string.Format(path, fileId));
            /* if (currWord > 1)
             {
                 OutputData.Write(gazePoints, randomWords[numberResults[currWord - 2]], string.Format(path, fileId));
             }*/
            started = false;
            gazePoints.Clear();
            currWordTime = 0.0f;
        }
        else
        {
            started = true;
            // keyboardManager.enable_timer();
            logger.StartEntry();
            //manager.nextWord();
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
            case keyboardType.general:
                r = 0.31f;
                or = 0.46f;
                cent = new Vector2(0.0f, 0.215f);
                extra = "general";
                break;

        }
        string[] con = currTextInput.Split(' ');
        GazeData gazeData = new GazeData { gaze_points = gazePoints, radius = r, outer_radius = or, center = cent, context = con };

        string json = JsonUtility.ToJson(gazeData);

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
                topWords.AddRange(response.top_words);
                //Debug.Log("Top words updated.");
                if (topWords.Count > 0)
                {
                    if (currMode == keyboardMode.sentences)
                    {
                        currTextInput = currTextInput + " " + topWords[0].ToUpper();
                        textInputRef.text = currTextInput;
                    }
                    else
                    {
                        currTextInput = topWords[0].ToUpper();
                        textInputRef.text = currTextInput;
                    }
                    logger.TopThree(topWords);


                    currWord += 1;
                } else
                {
                    // Did not return any words
                    // TODO: log that no words were returned
                }
                


                
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
    public class Response
    {
        public List<string> top_words;
    }

    public List<string> GiveTopWords()
    {
        return topWords;
    }


    public void SetTarget(string target)
    {
        targetText = target;
        targetTextRef.text = target;
        currTextInput = "";
        textInputRef.text = "";
    }

    public void setInput(string input)
    {
        currTextInput = input;
        textInputRef.text = input;
    }
    [SerializeField] private GameObject centerReference;
    [SerializeField] private GameObject topBound;
    [SerializeField] private GameObject bottomBound;
    [SerializeField] private GameObject leftBound;
    [SerializeField] private GameObject rightBound;
    [SerializeField] private KeyboardLayout keyboardLayout;
    
    public IEnumerator SetupKeyboard()
    {
        string keyboardString = GetKeyboardPositions();
        
        KeyboardInfo keyboardInfo = new KeyboardInfo();
        if (keyboardLayout == KeyboardLayout.circle)
        {
            Vector2 centerPoint = new Vector2(centerReference.transform.position.x, centerReference.transform.position.y);
            keyboardInfo = new KeyboardInfo { keyboard = keyboardString, center = centerPoint, inner_radius = 0.323f, outer_radius = 0.5f, k = 1, shape = "circle" };
        }
        else if (keyboardLayout == KeyboardLayout.qwerty)
        {
            Vector2 top_bound = new Vector2(topBound.transform.position.x, topBound.transform.position.y);
            Vector2 bottom_bound = new Vector2(bottomBound.transform.position.x, bottomBound.transform.position.y);
            Vector2 left_bound = new Vector2(leftBound.transform.position.x, leftBound.transform.position.y);
            Vector2 right_bound = new Vector2(rightBound.transform.position.x, rightBound.transform.position.y);
            keyboardInfo = new KeyboardInfo
            {
                keyboard = keyboardString,
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

    private string GetKeyboardPositions()
    {
        List<string> positions = new List<string>();
        
        foreach (Transform child in keyHolder.transform)
        {
            Vector3 position = child.position;
            string letters = child.name;  // Assumes the GameObject name matches the letter/group name
            string positionString = $"{letters} ({position.x:F4},{position.y:F4})";
            positions.Add(positionString);
        }
        
        return string.Join("\n", positions);
    }

    public void setMode(bool input)
    {
        if (input)
        {
            currMode = keyboardMode.sentences;
        } else
        {
            currMode = keyboardMode.singleWord;
        }
    }

    public string GetTargetText()
    {
        return targetTextRef.text;
    }


    public string CorrectTarget()
    {
        if (currMode == keyboardMode.singleWord)
        {
            var outp = GetTargetText();
            if (lastTarget != outp) {
                lastTarget = outp;
                logger.Target(lastTarget);
            }
            return outp;
        }
        else
        {
            string targ = targetTextRef.text.TrimStart();
            string inp = textInputRef.text.TrimStart();
            string[] targWords = targ.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string[] inpWords = inp.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            int minLength = Math.Min(targWords.Length, inpWords.Length);

            for (int i = 0; i < minLength; i++)
            {
                if (targWords[i] != inpWords[i])
                {

                    if (lastTarget != targWords[i])
                    {
                        lastTarget = targWords[i];
                        logger.Target(lastTarget);
                    }
                    return targWords[i];
                }
            }

            if (targWords.Length > inpWords.Length)
            {

                if (lastTarget != targWords[inpWords.Length])
                {
                    lastTarget = targWords[inpWords.Length];
                    logger.Target(lastTarget);
                }
                return targWords[inpWords.Length];
            }

            return "Finished!";
        }
    }

    public bool phraseTyping()
    {
        if (currMode == keyboardMode.sentences)
        {
            return true;
        } else
        {
            return false;
        }


    }

    private void PlayCorrectWordSound()
    {
        finishedWordSound.Play();
    }
}
