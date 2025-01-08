using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using TMPro;
using UnityEngine;

public class GazeDataGathering : MonoBehaviour
{
    protected GazeData gazeData;

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

    private int blinkBuffer;
    [SerializeField] protected int blinkThres = 20;
    [SerializeField] protected float nearThres = 0.2f;
    [SerializeField] protected float farThres = 10;
    [SerializeField] protected float interactionThreshold = 5;

    [SerializeField] private int GazeWindowSize = 20;

    private string folderPath;

    private DateTime currentDate;

    void Start()
    {
        currentDate = DateTime.Now;
        folderPath = System.IO.Path.Combine("Assets/Data", "GazeDataGathering");
        gazeData = new GazeData();
        Coroutine fun = StartCoroutine(GazeTest());
        
    }

    void Update()
    {
        bool update = UpdateGaze();
        //if (update)
        //{
        //    RaycastHit hit;
        //    //Physics.Raycast(gazeData.GazeOriginCombined, gazeData.GazeDirectionCombined, out hit, float.MaxValue, layerMask);
        //    Physics.Raycast(gazeData.GazeOriginCombined, GazeDirectionMean, out hit, float.MaxValue, layerMask);

        //    if (hit.transform != null)
        //    {
        //        previousHit = hit.transform;
        //    }

        //}

    }

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

    private void Smoothing()
    {

        using (StreamWriter sw = File.AppendText(System.IO.Path.Combine(folderPath, "GazeData" + currentDate.ToString("yyyy-MM-dd-HH-mm") + ".txt")))
        {
            sw.WriteLine("{0}, {1}, {2}", Time.time, gazeData.Depth, DepthMean);
        }

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

    [SerializeField] private TextMeshProUGUI near;
    [SerializeField] private TextMeshProUGUI far;

    IEnumerator GazeTest()
    {
        yield return new WaitForSeconds(5);
        for (int i = 0; i < 5; i++)
        {
            far.color = Color.white;
            near.color = Color.yellow;
            yield return new WaitForSeconds(2);
            far.color = Color.yellow;
            near.color = Color.white;
            yield return new WaitForSeconds(2);

        }        
    }
}
