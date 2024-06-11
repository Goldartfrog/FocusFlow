using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEditor;

public class InteractionDetection : MonoBehaviour
{
    private GazeData gazeData;

    private Vector3 GazeDirectionSum;
    private Vector3 GazeDirectionMean;
    private Queue<Vector3> PrevGaze;

    private float DepthSum;
    private float DepthMean;    
    private Queue<float> PrevDepth;

    protected int blinkBuffer;
    public int blinkThres;
    public int ValidGazeSteps;

    private string folderPath;
    private float time;
    private string participantId;

    public void GetGazeParameter(GazeData GazeData_)
    {
        gazeData = GazeData_;
    }

    public void UpdateGaze()
    {
        if (gazeData.GazeDirectionCombined.magnitude == 0)
        {
            blinkBuffer++;
            if (blinkBuffer < blinkThres)
            {
                return;
            }
        }
        else
        {
            blinkBuffer = 0;
        }
        if (gazeData.Depth < 0 || gazeData.Depth > 10)
        {
            return;
        }
        /* Record current gaze */
        PrevGaze.Enqueue(gazeData.GazeDirectionCombined);
        GazeDirectionSum += gazeData.GazeDirectionCombined;
        if (PrevGaze.Count > ValidGazeSteps)
        {
            Vector3 eraseDirection = PrevGaze.Dequeue();
            GazeDirectionSum -= eraseDirection;
        }
        GazeDirectionMean = GazeDirectionSum.normalized;

        PrevDepth.Enqueue(gazeData.Depth);
        DepthSum += gazeData.Depth;
        if (PrevDepth.Count > ValidGazeSteps)
        {
            float eraseDepth = PrevDepth.Dequeue();
            DepthSum -= eraseDepth;
        }
        DepthMean = DepthSum / PrevDepth.Count;

        if (GameObject.Find("TestControl").GetComponent<TestControlDetection>().WriteFlag)
        {
            using (StreamWriter sw = File.AppendText(System.IO.Path.Combine(folderPath, ("depth_" + SceneManager.GetActiveScene().name + ".txt"))))
            {
                sw.WriteLine("{0}, {1}, {2}, {3}", time, gazeData.Depth, DepthMean, GameObject.Find("Sphere").transform.position.z - Camera.main.transform.position.z);
            }
        }
    }

    // Start is called before the first frame update
    public void Start()
    {
        gazeData = new GazeData();
        GazeDirectionSum = new Vector3(0.0f, 0.0f, 0.0f);
        GazeDirectionMean = new Vector3(0.0f, 0.0f, 0.0f);
        PrevGaze = new Queue<Vector3>();
        DepthSum = 0.0f;
        DepthMean = 0.0f;
        PrevDepth = new Queue<float>();
        blinkBuffer = 0;

        participantId = File.ReadAllText(System.IO.Path.Combine("Assets/Data", "participantID.txt"), Encoding.UTF8);
        string folderName = SceneManager.GetActiveScene().name;
        folderPath = System.IO.Path.Combine("Assets/Data", folderName, participantId);
        time = 0.0f;
        
    }

    // Update is called once per frame
    public void Update()
    {
        if (!GameObject.Find("TestControl").GetComponent<TestControlDetection>().StartFlag)
        {
            return;
        }

        time += Time.deltaTime;
        UpdateGaze();
    }
}
