using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEditor;

public class InteractionWalls : MonoBehaviour
{
    private GazeData gazeData = new GazeData();

    private Vector3 GazeDirectionSum;
    private Vector3 GazeDirectionMean;
    private Queue<Vector3> PrevGaze;

    private float DepthSum;
    private float DepthMean;
    private Queue<float> PrevDepth;

    public int ValidGazeSteps;

    private int blinkBuffer;
    public int blinkThres;

    public GameObject Panel;
    private string PanelName;

    private Dictionary<string, (string, float)> ObjRsp;

    private float time;
    private string folderPath;

    public float safeDepthRange;
    public float operationDepthRange;
    public float maxSpeed;
    public string tester_index;
    public string test_index;

    public int numDepths = 10;
    public List<float> depths = new List<float>();

    public void GetGazeParameter(GazeData GazeData_)
    {
        gazeData = GazeData_;
    }

    public void PatternMatching()
    {
        ObjRsp.Clear();

        float firstAvgDepth = 0f;
        float secondAvgDepth = 0f;
        for (int i = 0; i < numDepths/2; i++)
        {
            firstAvgDepth += depths[i];
            secondAvgDepth += depths[i + numDepths / 2];
        }

        Debug.Log("FirstAvgDepth: " + firstAvgDepth);
        Debug.Log("SecondAvgDepth: " + secondAvgDepth);

        if (firstAvgDepth > secondAvgDepth)
        {
            Debug.Log("Going out!");
            ObjRsp.Add(PanelName, ("Out", 0f));
        } 
        else if (firstAvgDepth < secondAvgDepth)
        {
            Debug.Log("Going in!");
            ObjRsp.Add(PanelName, ("In", 0f));
        }
        //if(gazeData.Depth == 0 || DepthMean == 0){
        //    ObjRsp.Add(PanelName, ("Stop", 0));
        //    return;
        //}
        //Debug.Log("Panel, Camera, DepthMean: " + Panel.transform.position.z + ", " + Camera.main.transform.position.z + ", " + DepthMean);

        //float depth_diff = DepthMean - (Panel.transform.position.z - Camera.main.transform.position.z);
        //Debug.Log("Depth diff: " + depth_diff);
        //if (Math.Abs(depth_diff) >= safeDepthRange)
        //{
        //    if(depth_diff < 0)
        //    {
        //        ObjRsp.Add(PanelName, ("Down", Math.Min((Math.Abs(depth_diff) - safeDepthRange) / (operationDepthRange - safeDepthRange), 1) * -maxSpeed));
        //    }
        //    else{
        //        ObjRsp.Add(PanelName, ("Up", Math.Min((Math.Abs(depth_diff) - safeDepthRange) / (operationDepthRange - safeDepthRange), 1) * maxSpeed));
        //    }
        //}
        //else{
        //    ObjRsp.Add(PanelName, ("Stop", 0));
        //}
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
            else
            {
                PrevGaze.Clear();
                GazeDirectionSum = new Vector3(0.0f, 0.0f, 0.0f);
                GazeDirectionMean = new Vector3(0.0f, 0.0f, 0.0f);
                PrevDepth.Clear();
                DepthSum = 0.0f;
                DepthMean = 0.0f;
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
    }

    // Start is called before the first frame update
    public void Start()
    {
        GazeDirectionSum = new Vector3(0.0f, 0.0f, 0.0f);
        GazeDirectionMean = new Vector3(0.0f, 0.0f, 0.0f);
        PrevGaze = new Queue<Vector3>();

        DepthSum = 0.0f;
        DepthMean = 0.0f;
        PrevDepth = new Queue<float>();
        blinkBuffer = 0;

        PanelName = Panel.gameObject.name;

        time = 0.0f;
        ObjRsp = new Dictionary<string, (string, float)>();

        string folderName = SceneManager.GetActiveScene().name;
        folderPath = System.IO.Path.Combine("Assets/Data", folderName, tester_index, test_index);

        using (StreamWriter sw = File.AppendText(System.IO.Path.Combine(folderPath, "interaction_record.txt")))
        {
            sw.WriteLine("Tester index:{0}, Test index:{1}", tester_index, test_index);
        }
    }

    // Update is called once per frame
    public void Update()
    {
        time += Time.deltaTime;

        UpdateGaze();

        //depths.Add(Mathf.Clamp(DepthMean, 0.5f, 2.5f));
        depths.Add(DepthMean);

        if (depths.Count == numDepths)
        {
            PatternMatching();
            depths.RemoveAt(0);
        }

        GameObject.Find("Interaction").SendMessage("GetObjRspList", ObjRsp);
    }
}
