using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using ViveSR.anipal.Eye;
using static UnityEngine.EventSystems.EventTrigger;


/**
 * What this script does: apparently everything
 * 
 * 
 * 
 */
public class IntroductionManager : MonoBehaviour
{
    public InteractionEyeTracker EyePos;
    public TextMeshPro introTextRef;
    public KeyboardTextSystemIntroduction keyboard;
    public EnterScriptTutorial ring;
    public deleteScriptTutorial delete;

    private string introText;
    private Renderer rend;
    private BoxCollider boxCollider;
    private bool lookingAt = false;
    private float timer = 0;
    public GameObject linePrefab;
    private GameObject currentLine;

    private Coroutine lightUpCoroutine;

    public enum keyboardType
    {
        qwerty,
        ring
    }
    [SerializeField] public keyboardType currType;

    private SuperLogger logger;
    int step = 0;

    //Format: Text to display, draw lines/highlight, set target, light up ring, enable ring, word typing vs full sentence typing

    //Comment out words you dont need

    (string, bool, bool, bool, bool, bool)[] instructions = new (string, bool, bool, bool, bool, bool)[]
    {
        //("To continue through scenes, look at the next button", false, false, true, false, false),
        //("Try to familiarize yourself with the keyboard layout", false, false, true, false, false),
        //("We will now begin teaching letter typing", false, false, true, false, false),
        ("To mark the beginning and end of your words, use the spacebar \"ring\" on the virtual keyboard. Press space to begin, draw out the word, and press space again to end.", false, false, true, false, false),
        ("AND", true, true, true, true, false),
        ("AREA", true, true, true, true, false),
        ("HOLE", true, true, true, true, false),
        ("BRAVE", true, true, true, true, false),
        //("SHIRT", true, true, true, true, false),
        //("COMB", true, true, true, true, false),
        //("DRAFT", true, true, true, true, false), //here
        //("BANANA", true, true, true, true, false),
        //("DRIVING", true, true, true, true, false),
        //("CRISIS", true, true, true, true, false),
        ////("LIMB", true, true, true, true, false),
        //("ELEVEN", true, true, true, true, false),
        ////("HAND", true, true, true, true, false),
        //("GARAGE", true, true, true, true, false),
        //("KNOCK", true, true, true, true, false),
        ////("OTHER", true, true, true, true, false),
        //("START", true, true, true, true, false),
        //("RELAX", true, true, true, true, false),
        //("WORK", true, true, true, true, false),
        //("STOPPED", true, true, true, true, false),
        //("WINDOW", true, true, true, true, false),
        ////("PURPLE", true, true, true, true, false),
        //("MOMENT", true, true, true, true, false),
        //("SHOWER", true, true, true, true, false),
        //("PURSUE", true, true, true, true, false),
        ////("COMPUTER", true, true, true, true, false),
        ////("SEQUEL", true, true, true, true, false),
        ////("ORGANIZE", true, true, true, true, false),
        ////("BEDFRAME", true, true, true, true, false),
        //("Now, please use the ring around the letters instead of the physical spacebar. Gaze at it for a short time to start and stop each word.", false, false, true, false, false),
        ////("EAT", false, true, true, true, false),
        ////("APPLE", false, true, true, true, false),
        ////("FRIEND", false, true, true, true, false),
        //("PLAGUE", false, true, true, true, false),
        //("CANDLE", false, true, true, true, false),
        //("SHINE", false, true, true, true, false),
        ////("PLOT", false, true, true, true, false),
        //("SECURITY", false, true, true, true, false),
        //("MEDICAL", false, true, true, true, false), // here
        //("Now you will be given the ability to type full sentences, and use the ring here as if it were a spacebar", false, false, true, false, false),
        //("A LITTLE TALKING IS NEEDED", true, true, true, true, false),
        //("FALL IS MY FAVORITE SEASON", true, true, true, true, false),
        //("EVERYONE MUST KNOW ALL THE RULES", true, true, true, true, false),
        //("FEBRUARY HAS AN EXTRA DAY", true, true, true, true, false),
        //("BAD DRIVING RESULTS IN A FINE", true, true, true, true, false),
        ("Now, please type these sentences unguided:", false, false, true, false, false),
        //("PLEASE WORK", false, true, true, true, true),
        //("NUMBER TWO", false, true, true, true, true),
        ("THE LEAF DROPS TO THE GROUND", false, true, true, true, true),
        ("THE NIGHT SKY IS BRIGHT AND CLEAR", false, true, true, true, true),
        ("A SMALL STREAM FLOWS UNDER THE BRIDGE", false, true, true, true, true),
        ("GENTLE WAVES LAP AT THE SHORE", false, true, true, true, true),
        ("EVERYONE MUST KNOW ALL THE RULES", false, true, true, true, true),
        //("A CUTE BREEZE MOVES THE TABLE", false, true, true, true, true),
        ("You're done with the testing!", false, false, true, false, false),

    };

    (string, bool, bool, bool, bool, bool)[] instructionsQwerty = new (string, bool, bool, bool, bool, bool)[]
    {
        //("To continue through scenes, look at the next button", false, false, true, false, false),
        //("Try to familiarize yourself with the keyboard layout", false, false, true, false, false),
        //("We will now begin teaching letter typing", false, false, true, false, false),
        ("To mark the beginning and end of your words, use the spacebar on the virtual keyboard. Press space to begin, draw out the word, and press space again to end.", false, false, true, false, false),
        //("AND", true, true, true, true, false),
        //("AREA", true, true, true, true, false),
        //("HOLE", true, true, true, true, false),
        //("BRAVE", true, true, true, true, false),
        //("SHIRT", true, true, true, true, false),
        //("COMB", true, true, true, true, false),
        //("DRAFT", false, true, true, true, false), //here
        //("BANANA", false, true, true, true, false),
        //("DRIVING", false, true, true, true, false),
        //("CRISIS", false, true, true, true, false),
        ////("LIMB", true, true, true, true, false),
        //("ELEVEN", false, true, true, true, false),
        ////("HAND", true, true, true, true, false),
        //("GARAGE", false, true, true, true, false),
        //("KNOCK", false, true, true, true, false),
        ////("OTHER", true, true, true, true, false),
        //("START", false, true, true, true, false),
        //("RELAX", false, true, true, true, false),
        //("WORK", false, true, true, true, false),
        //("STOPPED", false, true, true, true, false),
        //("WINDOW", false, true, true, true, false),
        ////("PURPLE", true, true, true, true, false),
        //("MOMENT", false, true, true, true, false),
        //("SHOWER", false, true, true, true, false),
        //("PURSUE", false, true, true, true, false),
        ////("COMPUTER", true, true, true, true, false),
        ////("SEQUEL", true, true, true, true, false),
        ////("ORGANIZE", true, true, true, true, false),
        ////("BEDFRAME", true, true, true, true, false),
        //("Now, please use the ring around the letters instead of the physical spacebar. Gaze at it for a short time to start and stop each word.", false, false, true, false, false),
        ////("EAT", false, true, true, true, false),
        ////("APPLE", false, true, true, true, false),
        ////("FRIEND", false, true, true, true, false),
        //("PLAGUE", false, true, true, true, false),
        //("CANDLE", false, true, true, true, false),
        //("SHINE", false, true, true, true, false),
        ////("PLOT", false, true, true, true, false),
        //("SECURITY", false, true, true, true, false),
        //("MEDICAL", false, true, true, true, false), // here
        //("Now you will be given the ability to type full sentences, and use the ring here as if it were a spacebar", false, false, true, false, false),
        //("A LITTLE TALKING IS NEEDED", true, true, true, true, false),
        //("FALL IS MY FAVORITE SEASON", true, true, true, true, false),
        //("EVERYONE MUST KNOW ALL THE RULES", true, true, true, true, false),
        //("FEBRUARY HAS AN EXTRA DAY", true, true, true, true, false),
        //("BAD DRIVING RESULTS IN A FINE", true, true, true, true, false),
        ("Now, please type these sentences unguided:", false, false, true, false, false),
        //("PLEASE WORK", false, true, true, true, true),
        //("NUMBER TWO", false, true, true, true, true),
        ("THE LEAF DROPS TO THE GROUND", false, true, true, true, true),
        ("THE NIGHT SKY IS BRIGHT AND CLEAR", false, true, true, true, true),
        ("A SMALL STREAM FLOWS UNDER THE BRIDGE", false, true, true, true, true),
        ("GENTLE WAVES LAP AT THE SHORE", false, true, true, true, true),
        ("EVERYONE MUST KNOW ALL THE RULES", false, true, true, true, true),
        //("A CUTE BREEZE MOVES THE TABLE", false, true, true, true, true),
        ("You're done with the testing!", false, false, true, false, false),

    };

    public Dictionary<string, GameObject> letterGroups = new Dictionary<string, GameObject>();

    private WaitForSeconds wordPause = new WaitForSeconds(1f);
    private float timeoutDuration = 1.2f; //Time for center to not be looked at until hint comes up


    // CHANGE THIS FOR TIME BETWEEN LETTERS

    private TextMeshPro textMeshPro;

    public letterTutorialScript centerBox;

    private Material material;

    private float howGreen = 0f;
    private float longWait = 1.2f;
    private float lineOpacity = 1f;


    private List<float> gazeTimes = new List<float>();
    private List<float> avgWordTimes = new List<float>();

    [SerializeField] private AudioSource buttonSound;

    void Start()
    {
        rend = gameObject.GetComponent<Renderer>();
        material = rend.material;
        boxCollider = gameObject.GetComponent<BoxCollider>();
        logger = FindObjectOfType<SuperLogger>();
        introText = instructions[step].Item1;
        introTextRef.text = introText;

        ring.turnOff();
        ring.disable();

        centerBox.turnOff();
        centerBox.preventChange();
        if (currType == keyboardType.qwerty)
        {
            instructions = instructionsQwerty;
        }

        avgWordTimes.Add(1);
        avgWordTimes.Add(1);
        avgWordTimes.Add(.8f);
        avgWordTimes.Add(.8f);
        avgWordTimes.Add(.8f);
    }

    void Update()
    {
        checkBox();

        //Debug.Log(ring.getOn() + " and " + ring.getAllowed());
    }

    private void checkBox()
    {
        if (CheckingBox(EyePos.worldPosition, EyePos.gazeLocation))
        {
            //rend.enabled = false;
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / 2);
            float currentAlpha = Mathf.Lerp(1f, 0f, t);
            SetAlpha(currentAlpha);
            if (timer > 2 && !lookingAt)
            {
                timer = 0;
                lookingAt = true;
                goNext();
                logger.NextEntered();
            }
        }
        else
        {
            //rend.enabled = true;
            SetAlpha(1f);
            lookingAt = false;
        }
    }

    private void SetAlpha(float alpha)
    {
        Color newColor = material.color;
        newColor.a = alpha;
        material.color = newColor;
    }

    private void goNext()
    {
        step++;

        if (lightUpCoroutine != null)
        {
            StopCoroutine(lightUpCoroutine);
            lightUpCoroutine = null;
        }

        allDisable();
        ClearLine();
        if (buttonSound != null)
        {
            buttonSound.Play();
        }
        ring.turnWhite();
        centerBox.turnOff();
        centerBox.turnWhite();
        keyboard.setInput("");

        avgWordTimes.Add(CalculateAverageGazeTime());
        PrintAverageGazeTime();
        gazeTimes.Clear();
        AdjustDifficultyBasedOnPerformance();

        if (textMeshPro != null)
        {
            textMeshPro.fontStyle &= ~FontStyles.Italic;
            textMeshPro.color = Color.black;
        }

        introText = instructions[step].Item1;
        introTextRef.text = introText;



        if (instructions[step].Item3)
        {
            keyboard.setTarget(instructions[step].Item1);
        }
        else
        {
            keyboard.setTarget("Read intro ->");
        }

        if (instructions[step].Item2)
        {
            lightUpCoroutine = StartCoroutine(LightUpWord(instructions[step].Item1));
        }


        if (instructions[step].Item4)
        {
            ring.turnOn();
        }
        else { ring.turnOff(); }

        if (instructions[step].Item5)
        {
            ring.enable();
            delete.enable();

        }
        else
        {
            ring.disable();
            delete.disable();
        }

        keyboard.setMode(instructions[step].Item6);
    }

    bool CheckingBox(Vector3 userPosition, Vector3 fixationPoint)
    {

        Vector3 direction = (fixationPoint - userPosition).normalized;
        float distance = Vector3.Distance(userPosition, fixationPoint);
        if (direction != Vector3.zero)
        {
            Ray ray = new Ray(userPosition, direction.normalized);
            RaycastHit hit;
            if (boxCollider.Raycast(ray, out hit, Mathf.Infinity))
            {
                return true;
            }
        }
        


        return false;
    }

    IEnumerator LightUpWord(string word)
    {
        keyboard.setTarget(word + " ");
        WaitForSeconds letterPause = new WaitForSeconds(longWait);

        preventChange();

        int iter = 0;
        bool firstPass = true;

        Vector3 previousLetter = new Vector3();

        yield return wordPause;


        while (true)
        {
            string lastLetter = "";

            int letterNum = 0;
            if (iter != 0)
            {
                firstPass = false;
            }
            foreach (char letter in word)
            {
                logger.SetCurrentLetter(letter);
                if (InSameGroup(letter.ToString(), lastLetter))
                {
                    if (currType == keyboardType.ring)
                    {
                        float startTime = Time.time;
                        Vector3 center = ring.getPosition();

                        if (letterNum != 0)
                        {
                            DrawLineBetweenPointsXtra(previousLetter, center);
                        }

                        yield return new WaitUntil(() =>
                                centerBox.LookingAtBox(EyePos.worldPosition, EyePos.gazeLocation) || Time.time - startTime > timeoutDuration);


                        float gazeTime = Time.time - startTime;
                        gazeTimes.Add(gazeTime);

                        if (Time.time - startTime > timeoutDuration)
                        {
                            centerBox.moveTo(center);
                            centerBox.preventChange();
                            centerBox.turnOn();
                            centerBox.turnGreen(howGreen);
                            yield return new WaitUntil(() =>
                                centerBox.LookingAtBox(EyePos.worldPosition, EyePos.gazeLocation));
                        }

                        yield return letterPause;

                        centerBox.turnOff();
                        centerBox.turnWhite();


                        ClearLine();


                        previousLetter = center;
                    } else
                    {
                        continue;
                    }
                    
                }
                foreach (KeyValuePair<string, GameObject> pair in letterGroups)
                {
                    if (pair.Key.Contains(letter.ToString()))
                    {
                        float startTime = Time.time;

                        
                       
                        LightUpEntity(pair.Value);
                        

                        textMeshPro = pair.Value.GetComponentInChildren<TextMeshPro>();
                        if (textMeshPro != null)
                        {
                            textMeshPro.fontStyle |= FontStyles.Italic;
                        }


                        if (letterNum != 0)
                        {
                            DrawLineBetweenPoints(previousLetter, pair.Value.transform.position);
                        }

                        yield return new WaitUntil(() => pair.Value.GetComponent<letterTutorialScript>().LookingAtBox(EyePos.worldPosition, EyePos.gazeLocation));
                        
                        float gazeTime = Time.time - startTime;
                        gazeTimes.Add(gazeTime);

                        logger.WriteLetter(letter, gazeTime);
                        logger.WriteDelay(longWait);
                        yield return letterPause;


                        if (textMeshPro != null)
                        {
                            textMeshPro.fontStyle &= ~FontStyles.Italic;
                        }


                        
                        TurnOffEntity(pair.Value);
                        

                        
                        
                        ClearLine();
                        

                        previousLetter = pair.Value.transform.position;
                        letterNum++;

                        break;
                    }
                    else if (letter.ToString() == " ")
                    {
                        if (currType == keyboardType.ring)
                        {
                            float startTime = Time.time;



                            ring.turnGreen(howGreen);


                            Vector3 closestPointOnLoop = GetClosestPointOnLoop(previousLetter);


                            if (letterNum != 0)
                            {
                                DrawLineBetweenPoints(previousLetter, closestPointOnLoop);
                            }

                            yield return new WaitUntil(() => ring.LookingAtBox() || Time.time - startTime > timeoutDuration);


                            float gazeTime = Time.time - startTime;
                            gazeTimes.Add(gazeTime);


                            if (Time.time - startTime > timeoutDuration)
                            {
                                centerBox.moveTo(closestPointOnLoop);
                                centerBox.preventChange();
                                centerBox.turnOn();
                                centerBox.turnGreen(howGreen);
                                yield return new WaitUntil(() =>
                                    ring.LookingAtBox());
                            }

                            logger.WriteLetter(' ', gazeTime);
                            logger.WriteDelay(longWait);

                            yield return letterPause;

                            centerBox.turnOff();
                            centerBox.turnWhite();


                            ring.turnWhite();


                            ClearLine();


                            previousLetter = closestPointOnLoop;
                            letterNum++;
                            break;
                        } else
                        {
                            float startTime = Time.time;

                            ring.turnGreen(howGreen);

                            Vector3 centerPoint = ring.getPosition();


                            if (letterNum != 0)
                            {
                                DrawLineBetweenPoints(previousLetter, centerPoint);
                            }

                            yield return new WaitUntil(() => ring.LookingAtBox() || Time.time - startTime > timeoutDuration);


                            float gazeTime = Time.time - startTime;
                            gazeTimes.Add(gazeTime);


                            if (Time.time - startTime > timeoutDuration)
                            {
                                centerBox.moveTo(centerPoint);
                                centerBox.preventChange();
                                centerBox.turnOn();
                                centerBox.turnGreen(howGreen);
                                yield return new WaitUntil(() =>
                                    ring.LookingAtBox());
                            }

                            yield return letterPause;

                            centerBox.turnOff();
                            centerBox.turnWhite();


                            ring.turnWhite();


                            ClearLine();


                            previousLetter = centerPoint;
                            letterNum++;
                            break;
                        }
                        
                    }
                }

                lastLetter = letter.ToString();
            }
            iter++;
            logger.WriteDelay(1f);
            yield return wordPause;
        }
    }

    void LightUpEntity(GameObject entity)
    {
        if (entity == null) return;
        var script = entity.GetComponent<letterTutorialScript>();
        if (script != null)
        {
            script.turnGreen(howGreen);
        }
    }

    void TurnOffEntity(GameObject entity)
    {
        if (entity == null) return;
        var script = entity.GetComponent<letterTutorialScript>();
        if (script != null)
        {
            script.turnWhite();
        }
    }

    void preventChange()
    {
        if (letterGroups == null) return;
        foreach (var pair in letterGroups)
        {
            if (pair.Value == null) continue;
            var script = pair.Value.GetComponent<letterTutorialScript>();
            if (script != null)
            {
                script.preventChange();
            }
        }
    }

    void allowChange()
    {
        if (letterGroups == null) return;
        foreach (var pair in letterGroups)
        {
            if (pair.Value == null) continue;
            var script = pair.Value.GetComponent<letterTutorialScript>();
            if (script != null)
            {
                script.allowChange();
            }
        }
    }

    void allDisable()
    {
        if (letterGroups == null) return;
        foreach (var pair in letterGroups)
        {
            if (pair.Value == null) continue;

            TurnOffEntity(pair.Value);

            // Separate null check for TextMeshPro
            var textMeshPro = pair.Value.GetComponentInChildren<TextMeshPro>();
            if (textMeshPro != null)
            {
                textMeshPro.fontStyle &= ~FontStyles.Italic;
            }
        }
        allowChange();
    }

    void ClearLine()
    {
        if (currentLine != null)
        {
            Destroy(currentLine);
            currentLine = null;
        }
    }

    void DrawLineBetween(GameObject startEntity, GameObject endEntity)
    {
        if (currentLine == null)
        {
            currentLine = Instantiate(linePrefab);
        }

        LineRenderer lineRenderer = currentLine.GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, startEntity.transform.position);
        lineRenderer.SetPosition(1, endEntity.transform.position);
    }

    void DrawLineBetweenPoints(Vector3 startEntity, Vector3 endEntity)
    {
        if (currentLine == null)
        {
            currentLine = Instantiate(linePrefab);
        }

        LineRenderer lineRenderer = currentLine.GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, startEntity);
        lineRenderer.SetPosition(1, endEntity);

        Color startColor = lineRenderer.startColor;
        Color endColor = lineRenderer.endColor;
        startColor.a = lineOpacity;
        endColor.a = lineOpacity;
        lineRenderer.startColor = startColor;
        lineRenderer.endColor = endColor;
    }

    void DrawLineBetweenPointsXtra(Vector3 startEntity, Vector3 endEntity)
    {
        if (currentLine == null)
        {
            currentLine = Instantiate(linePrefab);
        }

        LineRenderer lineRenderer = currentLine.GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, startEntity);
        lineRenderer.SetPosition(1, endEntity);

        Color startColor = lineRenderer.startColor;
        Color endColor = lineRenderer.endColor;
        startColor.a = .2f;
        endColor.a = .2f;
        lineRenderer.startColor = startColor;
        lineRenderer.endColor = endColor;
    }

    Vector3 GetClosestPointOnLoop(Vector3 lastBox)
    {
        Vector3 ringCenter = ring.getPosition();
        Vector3 directionToGaze = (lastBox - ringCenter).normalized;
        Vector3 closestPoint = ringCenter + directionToGaze * .6f;

        return closestPoint;
    }

    public bool IsGazeNearRing(Vector3 ringPosition, Vector3 gazeLocation, Vector3 worldPosition)
    {
        Vector3 direction = gazeLocation - worldPosition;

        float t = (2f - EyePos.worldPosition.z) / direction.z;

        Vector3 intersectionPoint = EyePos.worldPosition + t * direction;

        float distance = Vector3.Distance(intersectionPoint, ringPosition);

        return distance <= 0.15f;
    }

    public bool InSameGroup(string currLetter, string lastLetter)
    {
        string lastGrouping = "";

        if (currLetter == " " || lastLetter == " " || lastLetter == "")
        {
            return false;
        }


        foreach (KeyValuePair<string, GameObject> pair in letterGroups)
        {
            if (pair.Key.Contains(lastLetter))
            {
                lastGrouping = pair.Key;
            }
        }

        if (lastGrouping.Contains(currLetter) && currLetter != lastLetter)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public string GetPositions2D()
    {
        if (currType == keyboardType.qwerty)
        {
            letterGroups.Add("A", GameObject.Find("A"));
            letterGroups.Add("B", GameObject.Find("B"));
            letterGroups.Add("C", GameObject.Find("C"));
            letterGroups.Add("D", GameObject.Find("D"));
            letterGroups.Add("E", GameObject.Find("E"));
            letterGroups.Add("F", GameObject.Find("F"));
            letterGroups.Add("G", GameObject.Find("G"));
            letterGroups.Add("H", GameObject.Find("H"));
            letterGroups.Add("I", GameObject.Find("I"));
            letterGroups.Add("J", GameObject.Find("J"));
            letterGroups.Add("K", GameObject.Find("K"));
            letterGroups.Add("L", GameObject.Find("L"));
            letterGroups.Add("M", GameObject.Find("M"));
            letterGroups.Add("N", GameObject.Find("N"));
            letterGroups.Add("O", GameObject.Find("O"));
            letterGroups.Add("P", GameObject.Find("P"));
            letterGroups.Add("Q", GameObject.Find("Q"));
            letterGroups.Add("R", GameObject.Find("R"));
            letterGroups.Add("S", GameObject.Find("S"));
            letterGroups.Add("T", GameObject.Find("T"));
            letterGroups.Add("U", GameObject.Find("U"));
            letterGroups.Add("V", GameObject.Find("V"));
            letterGroups.Add("W", GameObject.Find("W"));
            letterGroups.Add("X", GameObject.Find("X"));
            letterGroups.Add("Y", GameObject.Find("Y"));
            letterGroups.Add("Z", GameObject.Find("Z"));
        }
        else
        {
            letterGroups.Add("A", GameObject.Find("A"));
            letterGroups.Add("B", GameObject.Find("B"));
            letterGroups.Add("CD", GameObject.Find("CD"));
            letterGroups.Add("EF", GameObject.Find("EF"));
            letterGroups.Add("GH", GameObject.Find("GH"));
            letterGroups.Add("IJK", GameObject.Find("IJK"));
            letterGroups.Add("L", GameObject.Find("L"));
            letterGroups.Add("M", GameObject.Find("M"));
            letterGroups.Add("NO", GameObject.Find("NO"));
            letterGroups.Add("P", GameObject.Find("P"));
            letterGroups.Add("QR", GameObject.Find("QR"));
            letterGroups.Add("S", GameObject.Find("S"));
            letterGroups.Add("TU", GameObject.Find("TU"));
            letterGroups.Add("VWX", GameObject.Find("VWX"));
            letterGroups.Add("YZ", GameObject.Find("YZ"));
        }
        

        


        List<string> positions = new List<string>();

        foreach (var kvp in letterGroups)
        {
            string letters = kvp.Key;
            GameObject letterObject = kvp.Value;

            if (letterObject != null)
            {
                Transform transform = letterObject.transform;
                Vector3 position = transform.position;

                string positionString = $"{letters} ({position.x:F4},{position.y:F4})";
                positions.Add(positionString);
            }
        }

        Debug.Log(string.Join("\n", positions));
        return string.Join("\n", positions);
    }


    public float CalculateAverageGazeTime()
    {
        if (gazeTimes.Count == 0)
            return 1f;

        float avg = gazeTimes.Average();

        if (avg > 1.5)
        {
            avg = 1.5f;
        }

        return avg;
    }

    public void PrintAverageGazeTime()
    {
        float averageTime = CalculateAverageGazeTime();
        Debug.Log("Average Gaze Time: " + averageTime);
    }

    public void AdjustDifficultyBasedOnPerformance()
    {
        if (avgWordTimes.Count < 5)
        {
            Debug.Log("Not enough data to adjust difficulty yet.");
            return;
        }

        List<float> lastFive = avgWordTimes.GetRange(Mathf.Max(0, avgWordTimes.Count - 5), Mathf.Min(5, avgWordTimes.Count));
        float recentAverage = lastFive.Average();

        float normalizedTime = Mathf.Clamp01((recentAverage - 0.3f) / 0.7f);

        howGreen = Mathf.Lerp(0f, .85f, 1-normalizedTime);

        longWait = Mathf.Lerp(0.8f, 1.2f, normalizedTime);

        lineOpacity = Mathf.Lerp(-.3f, 1f, normalizedTime-.2f);

        Debug.Log($"Adjusted difficulty: howGreen = {howGreen}, longWait = {longWait}, recentAverage = {recentAverage}, normalizedTime = {normalizedTime}");
    }
}
