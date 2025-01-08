using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.EventSystems;

public class InteractionPushKeybaord : MonoBehaviour
{
    protected GazeData gazeData;



    protected int blinkBuffer;
    [SerializeField] protected int blinkThres = 20;
    [SerializeField] protected float nearThres = 0.2f;
    [SerializeField] protected float farThres = 10;
    [SerializeField] protected float interactionThreshold = 5;

    /* smoothing variables */
    protected Vector3 GazeDirectionSum = new Vector3(0.0f, 0.0f, 0.0f);
    protected Vector3 GazeDirectionMean = new Vector3(0.0f, 0.0f, 0.0f);
    protected Queue<Vector3> PrevGaze = new Queue<Vector3>();

    [SerializeField] private int speedRecordSize;
    private Queue<float> depths = new Queue<float>();

    protected float DepthSum = 0.0f;
    protected float DepthMean = 0.0f;
    protected Queue<float> PrevDepth = new Queue<float>();

    private Vector3 GazeOriginSum = new Vector3(0.0f, 0.0f, 0.0f);
    private Vector3 GazeOriginMean = new Vector3(0.0f, 0.0f, 0.0f);
    private Queue<Vector3> PrevGazeOrigin = new Queue<Vector3>();

    protected float OriginSum = 0.0f;
    protected float OriginMean = 0.0f;
    protected Queue<float> PrevOrigin = new Queue<float>();

    [SerializeField] private int GazeWindowSize = 20;

    /* keyboard variables*/
    [SerializeField] private GameObject keyButtons;
    private int layerMask = 1 << 15;
    private Transform previousHit;

    [SerializeField] AudioSource typeSound;
    [SerializeField] private GameObject typingField;

    [SerializeField] private GameObject vrCamera;
    private Vector3 cameraPos;

    [SerializeField] private GameObject grabkey;
    [SerializeField] private GameObject grabline;
    [SerializeField] private GameObject grabPosition;

    [SerializeField] private GameObject typingInterface;
    [SerializeField] private GameObject calibrationObject;
    private bool calibrated = false;

    [SerializeField] private TesterController testerController;
    private string folderPath;

    private bool keyPressed;

    public void GetGazeParameter(GazeData GazeData_)
    {
        gazeData = GazeData_;
    }

    /* Check to see if the gaze is in valid range before updating */
    private bool UpdateGaze()
    {
        if (gazeData.GazeDirectionCombined.magnitude == 0) /* if the user is blinking or the gaze is messed up, don't record */
        {
            blinkBuffer++;
            //if (blinkBuffer < blinkThres)
            //{
                return false;
            //}
        }
        else
        {
            blinkBuffer = 0;
        }

        if (gazeData.Depth < nearThres || gazeData.Depth > farThres) /* if the user is looking "too far" or "too near" don't record */
        {
           return false;
        }
        Smoothing();

        return true;
    }

    private float oldDepth = 0;
    private float newDepth = 0;
    private List<float> depthList = new List<float>();
    private DateTime currentDate;
    private bool intentToPush = false;
    private void Smoothing()
    {

        depths.Enqueue(gazeData.Depth);
        if (depths.Count > 2 * speedRecordSize)
        {
            depths.Dequeue();
        }
        depthList = depths.ToList();
        oldDepth = 0;
        newDepth = 0;
        if (depthList.Count == speedRecordSize * 2)
        {
            for (int i = 0; i < speedRecordSize; i++)
            {
                oldDepth += depthList[i];
            }
            for (int i = speedRecordSize; i < depths.Count; i++)
            {
                newDepth += depthList[i];
            }
            oldDepth = oldDepth / speedRecordSize;
            newDepth = newDepth / speedRecordSize;
        }
        intentToPush = false;
        if (Input.GetButton("Jump"))
        {
            intentToPush = true;
        } 

        using (StreamWriter sw = File.AppendText(System.IO.Path.Combine(folderPath, "PushKeyboardDepth" + currentDate.ToString("yyyy-MM-dd-HH-mm") + ".txt")))
        {
            sw.WriteLine("{0}, {1}, {2}, {3}, {4}, {5}", Time.time, gazeData.Depth, DepthMean, newDepth - oldDepth, keyPressed, intentToPush);
        }
        //if (newDepth - oldDepth > 5)
        //{
        //    Debug.Log(oldDepth + " " + newDepth);
        //}


        /* Record current gaze */
        PrevGaze.Enqueue(gazeData.GazeDirectionCombined);
        GazeDirectionSum += gazeData.GazeDirectionCombined;
        if (PrevGaze.Count > GazeWindowSize)
        {
            Vector3 eraseDirection = PrevGaze.Dequeue();
            GazeDirectionSum -= eraseDirection;
        }
        GazeDirectionMean = GazeDirectionSum.normalized;
        /* Record depth */
        PrevDepth.Enqueue(gazeData.Depth);
        DepthSum += gazeData.Depth;
        if (PrevDepth.Count > GazeWindowSize)
        {
            float eraseDepth = PrevDepth.Dequeue();
            DepthSum -= eraseDepth;
        }
        DepthMean = DepthSum / PrevDepth.Count;
        /* record and smooth gaze origin */
        PrevGazeOrigin.Enqueue(gazeData.GazeOriginCombined);
        GazeOriginSum += gazeData.GazeOriginCombined;
        if (PrevGazeOrigin.Count > GazeWindowSize)
        {
            Vector3 eraseDirection = PrevGazeOrigin.Dequeue();
            GazeOriginSum -= eraseDirection;
        }
        GazeOriginMean = GazeOriginSum / PrevGazeOrigin.Count;


    }

    
    // Start is called before the first frame update
    void Start()
    {
        folderPath = System.IO.Path.Combine("Assets/Data", "AidanTestingPushKeyboard");
        currentDate = DateTime.Now;
        gazeData = new GazeData();
        MakeKeyboard();
        cameraPos = vrCamera.transform.position;
        

    }

    // Update is called once per frame
    void Update()
    {
        bool update = UpdateGaze();
        if (update)
        {
            RaycastHit hit;
            //Physics.Raycast(gazeData.GazeOriginCombined, gazeData.GazeDirectionCombined, out hit, float.MaxValue, layerMask);
            Physics.Raycast(gazeData.GazeOriginCombined, GazeDirectionMean, out hit, float.MaxValue, layerMask);
            PushGazeTyping(hit);

            if (hit.transform != null)
            {
                previousHit = hit.transform;
            }
            
        }
        keyPressed = false;
        

    }

    private float originalDepth = 0;

    private void MakeKeyboard()
    {
        string keys = "QWERTYUIOPASDFGHJKLZXCVBNM";
        for (int i = 0; i < keyButtons.transform.childCount; i++)
        {
            Transform child = keyButtons.transform.GetChild(i);
            child.GetComponent<Keyboard>().SetKey("" + keys[i]);
            child.GetComponent<MeshRenderer>().material.color = new Color(0.9f, 0.9f, 0.9f, 0.8f);
            originalDepth = child.transform.position.x;
        }
    }

    private string TypeKey(Transform transform)
    {
        keyPressed = true;
        string k = transform.GetComponent<Keyboard>().GetKey();
        typeSound.Play();
        string t = typingField.GetComponent<TextMeshProUGUI>().text;
        if (k == "calibrate")
        {
            StartCoroutine(CalibrateGazeDepth());
        }
        else if (calibrated || mode != Modes.PushDistance)
        {
            if (k == "back")
            {
                if (t.Length > 0)
                {
                    typingField.GetComponent<TextMeshProUGUI>().text = t.Substring(0, t.Length - 1);
                }
            }
            else
            {
                typingField.GetComponent<TextMeshProUGUI>().text += k;
            }
        }
        return k;


    }

    private float currentPushTime = 0f;
    private float pushDuration = 1.0f;
    private bool isGazeOnButton = false;
    private Vector3 originalPosition;
    [SerializeField] private float keyMoveDepth = 0.2f;
    [SerializeField] private float pushFrom = 1.0f;
    [SerializeField] private float pushTo = 3.0f;
    [SerializeField] private float coolDownTime = 1f;
    [SerializeField] private float pushSpeedThreshold = 5f;
    private float coolDownTimer = 0f;
    private bool onCooldown = false;
    private float storeDepth = 0;

    private enum Modes { DualTime, PushDistance, PushSpeed }
    [SerializeField] private Modes mode;
    [SerializeField] private Vector3 colliderScale;


    private void PushGazeTyping(RaycastHit hit)
    {
        if (hit.transform != null)
        {

            //Debug.Log("Hit something");
            if (hit.transform != previousHit)
            {
                //Debug.Log("Resetting button");
                ResetButton();
                storeDepth = DepthMean;
            }
            if (!isGazeOnButton)
            {
                //Debug.Log("starting to hit button");
                isGazeOnButton = true;
                currentPushTime = 0f;
                originalPosition = hit.transform.position;
                previousHit = hit.transform;
                hit.transform.GetComponent<BoxCollider>().size = colliderScale;
                //hit.transform.GetComponent<MeshRenderer>().material.color = new Color(0.8f, 0.8f, 0.8f, 0.5f);
                //hit.transform.GetComponent<MeshRenderer>().material.color.a = 0.5f;
            }
            /* uses dual time to push button */
            float pushProgress = 0;
            if (mode == Modes.DualTime)
            {
                currentPushTime += Time.deltaTime;
                pushProgress = Mathf.Clamp01(currentPushTime / pushDuration);
            }

            if (mode == Modes.PushDistance)
            {
                float depthChange = storeDepth - DepthMean;
                pushProgress = Mathf.Clamp01((DepthMean - pushFrom) / (pushTo - pushFrom));
            }

            if (mode == Modes.PushSpeed)
            {
                float depthChange = newDepth - oldDepth;
                pushProgress = Mathf.Clamp01(depthChange / pushSpeedThreshold);
                //Debug.Log(newDepth + " " + oldDepth + " " + depthChange + " " + pushProgress);
                //writer.WriteLine(newDepth + " " + oldDepth + " " + pushProgress + Time.time);
                //if (Mathf.Abs(depthChange) > pushSpeedThreshold)
                //{
                //    Debug.Log("it did the thing");
                //}
            }


            //Debug.Log(pushProgress);

            hit.transform.position = originalPosition - hit.transform.right * pushProgress * keyMoveDepth;

            if (pushProgress >= 0.8f)
            {
                if (!onCooldown)
                {
                    string k = TypeKey(previousHit);
                    testerController.WordProgress(k);
                    ResetButton();
                    onCooldown = true;
                }

                coolDownTimer += Time.deltaTime;
                if (coolDownTimer > coolDownTime)
                {
                    coolDownTimer = 0f;
                    onCooldown = false;
                }
            }

        }
        else
        {

            ResetButton();
        }
    }


    private void ResetButton()
    {
        isGazeOnButton = false;
        currentPushTime = 0.0f;
        if (previousHit != null)
        {
            previousHit.transform.position = originalPosition;
            previousHit.transform.GetComponent<BoxCollider>().size = new Vector3(1, 1, 1);
            //previousHit.transform.position = new Vector3(originalDepth, previousHit.transform.position.y, previousHit.transform.position.z);
            //previousHit.transform.GetComponent<MeshRenderer>().material.color = Color.white;
        }
        

    }


    private List<float> nearDepths = new List<float>();
    private List<float> farDepths = new List<float>();
    [SerializeField] private float nearCalibrate = 1f;
    [SerializeField] private float farCalibrate = 4f;

    private IEnumerator CalibrateGazeDepth()
    {
        List<float> nearDepths = new List<float>();
        List<float> farDepths = new List<float>();
        calibrationObject.SetActive(true);
        typingInterface.SetActive(false);
        yield return new WaitForSeconds(1f);
        Vector3 nearPosition = vrCamera.transform.position + new Vector3(-nearCalibrate, 0, 0);
        yield return StartCoroutine(CalibratePosition(nearPosition, nearDepths));

        yield return new WaitForSeconds(1f);

        Vector3 farPosition = vrCamera.transform.position + new Vector3(-farCalibrate, 0, 0);
        yield return StartCoroutine(CalibratePosition(farPosition, farDepths));

        typingInterface.SetActive(true);
        calibrationObject.SetActive(false);
        pushFrom = nearDepths.Average();
        pushTo = farDepths.Average();
        Debug.Log($"Calibrated near Depth: {pushFrom}, Far Depth: {pushTo}");
        if (pushFrom < 0 || pushTo < 0 || pushTo < pushFrom)
        {
            StartCoroutine(CalibrateGazeDepth());
        }
        typingInterface.transform.position = new Vector3(-pushFrom, typingInterface.transform.position.y, typingInterface.transform.position.z);
        calibrated = true;
    }

    private float calibrationDuration = 5f;
    private float samplesPerSecond = 10;

    private IEnumerator CalibratePosition(Vector3 position, List<float> depthList)
    {
        calibrationObject.transform.position = position;
        float timePassed = 0f;
        while (timePassed < calibrationDuration)
        {
            depthList.Add(gazeData.Depth);
            timePassed += 1f / samplesPerSecond;
            yield return new WaitForSeconds(1f / samplesPerSecond);
        }
    }

    // read input based on "speed" of gaze depth change.
    // take last 10 depths, if average of oldest 5 - average of newest 5 is over some (negative) threshold, then gaze has been pushed far away

}
