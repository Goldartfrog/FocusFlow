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

public class Interaction : MonoBehaviour
{
    protected GazeData gazeData;
    protected Dictionary<string, (string, string)> ObjRsp;

    protected Vector3 GazeDirectionSum;
    protected Vector3 GazeDirectionMean;
    protected Queue<Vector3> PrevGaze;

    protected float DepthSum;
    protected float DepthMean;    
    protected Queue<float> PrevDepth;

    public int ValidGazeSteps;

    protected int blinkBuffer;
    public int blinkThres;

    public float ActivateDepthFactor;
    public float MaintainDepthFactor;

    protected List<string> Frames;
    public GameObject Guidance_1;
    public GameObject Guidance_2;
    public GameObject Panel;
    protected string PanelName;
    protected string CurrHitFrame;
    protected int CurrHitHold;
    public int HitHoldValue;

    public int frameLayer;
    public int panelLayer;

    public bool UseGuidance_1;
    public bool UseGuidance_2;
    public bool UseGuidance_3;
    
    protected float time;
    protected string folderPath;
    protected string participantId;

    public void GetGazeParameter(GazeData GazeData_)
    {
        gazeData = GazeData_;
    }

    public GazeData GetGazeData() { return gazeData; }

    public bool ObjectDetection(int layer, out RaycastHit hit)
    {
        /* Collision detection */
        bool isHit = false;
        int layermask = 1 << layer;
        if (Physics.Raycast(gazeData.GazeOriginCombined, gazeData.GazeDirectionCombined, out hit, float.MaxValue, layermask))
        {
            // Panel
            if (layer == 6)
            {
                if (DepthMean > 0.1)
                {
                    if (DepthMean <= hit.distance * (1 + ActivateDepthFactor))
                    {
                        if (hit.transform.gameObject.GetComponent<UIObject>().status == 0)
                        {
                            isHit = true;
                        }
                    }
                    if (DepthMean <= hit.distance * 1 + MaintainDepthFactor)
                    {
                        if (hit.transform.gameObject.GetComponent<UIObject>().status == 2)
                        {
                            isHit = true;
                        }
                    }
                }
                //if ((hit.transform.gameObject.GetComponent<UIObject>().status == 0 && 
                //    DepthMean <= hit.distance * (1 + ActivateDepthFactor) && 
                //    DepthMean > 0.1) || 
                //    (hit.transform.gameObject.GetComponent<UIObject>().status == 2 && 
                //    DepthMean <= hit.distance * (1 + MaintainDepthFactor) && 
                //    DepthMean > 0.1))
                //{
                //    isHit = true;
                //}
            }
            // Frames
            else if (layer == 7)
            {
                isHit = true;
            }
        }
        return isHit;
    }

    public void PatternMatching(RaycastHit currHit, bool isPanelHit, bool isFrameHit)
    {
        ObjRsp.Clear();
        int CurrStatus;
        string HitName = "";
        if (isPanelHit || isFrameHit)
        {
            HitName = currHit.transform.gameObject.name;
        }
        
        if (isPanelHit && CurrHitFrame != "")
        {
            ObjRsp.Add(CurrHitFrame, ("Activate", ""));
            ObjRsp.Add(PanelName, ("Activate", CurrHitFrame));
        }
        else 
        {
            CurrStatus = Panel.GetComponent<UIObject>().status;
            if (CurrStatus != 0 && (blinkBuffer == 0 || blinkBuffer > blinkThres))
            {
                ObjRsp.Add(PanelName, ("Deactivate", ""));
            }
            
            // For each interactive object
            foreach (string FrameName in Frames)
            {
                CurrStatus = GameObject.Find(FrameName).GetComponent<UIObject>().status;
                if (FrameName == HitName && CurrStatus == 0)
                {
                    ObjRsp.Add(FrameName, ("Hover", ""));
                    CurrHitHold = HitHoldValue;
                }
                else if (FrameName != HitName && CurrStatus != 0 && (blinkBuffer == 0 || blinkBuffer > blinkThres))
                {
                    if (CurrHitFrame == FrameName && CurrHitHold > 0)
                    {
                        CurrHitHold--;                        
                    }
                    else
                    {
                        ObjRsp.Add(FrameName, ("Deactivate", ""));
                        if (CurrHitFrame == FrameName)
                        {
                            CurrHitFrame = "";
                        }
                    }
                }
            }
        }
        if (UseGuidance_1)
        {
            if (CurrHitFrame != "" && !isPanelHit)
            {
                ObjRsp.Add("Guidance_1", ("Activate", ""));
            }
            else
            {
                if (CurrHitFrame == "" && CurrHitHold == 0 || isPanelHit)
                {
                    ObjRsp.Add("Guidance_1", ("Deactivate", ""));
                }
            }
        }
        if (UseGuidance_2)
        {
            if (CurrHitFrame != "" && !isPanelHit)
            {
                ObjRsp.Add("Guidance_2", ("Activate", ""));
            }
            else
            {
                if (CurrHitFrame == "" && CurrHitHold == 0 || isPanelHit)
                {
                    ObjRsp.Add("Guidance_2", ("Deactivate", ""));
                }
            }
        }
        if (UseGuidance_3)
        {
            Color new_color = Guidance_1.GetComponent<MeshRenderer>().material.color;
            if (DepthMean <= 2.0f && DepthMean >= 0.6f*(1 + ActivateDepthFactor))
            {
                new_color.a = 0.8f*(2.0f - DepthMean) / (2.0f - 0.6f*(1 + ActivateDepthFactor));
            }
            else if (DepthMean < 0.6*(1 + ActivateDepthFactor))
            {
                new_color.a = 0.8f;
            }
            else{
                new_color.a = 0.0f;
            }
            Guidance_1.GetComponent<MeshRenderer>().material.color = new_color;
        }
    }

    public bool UpdateGaze()
    {
        if (gazeData.GazeDirectionCombined.magnitude == 0)
        {
            blinkBuffer++;
            if (blinkBuffer < blinkThres)
            {
                return false;
            }
        }
        else
        {
            blinkBuffer = 0;
        }
        if (gazeData.Depth < 0 || gazeData.Depth > 10)
        {
            return false;
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

        if (GameObject.Find("TestControl").GetComponent<TestControl>().WriteFlag)
        {
            using (StreamWriter sw = File.AppendText(System.IO.Path.Combine(folderPath, "depth_change.txt")))
            {
                sw.WriteLine("{0}, {1}, {2}", time, gazeData.Depth, DepthMean);
            }
        }
        return true;
    }

    // Start is called before the first frame update
    public void Start()
    {
        gazeData = new GazeData();
        ObjRsp = new Dictionary<string, (string, string)>();
        GazeDirectionSum = new Vector3(0.0f, 0.0f, 0.0f);
        GazeDirectionMean = new Vector3(0.0f, 0.0f, 0.0f);
        PrevGaze = new Queue<Vector3>();
        DepthSum = 0.0f;
        DepthMean = 0.0f;
        PrevDepth = new Queue<float>();
        blinkBuffer = 0;
        Frames = new List<String>();
        Transform WallObjs = GameObject.Find("Walls").transform;
        foreach (Transform wall in WallObjs)
        {
            if (wall.gameObject.activeSelf)
            {
                foreach (Transform frames in wall)
                {
                    foreach (Transform frame in frames)
                    {
                        foreach (Transform child in frame)
                        {
                            if (child.gameObject.name.Contains("Frame"))
                            {
                                Frames.Add(child.gameObject.name);
                            }
                        }
                    }
                }
            }
        }
        participantId = File.ReadAllText(System.IO.Path.Combine("Assets/Data", "participantID.txt"), Encoding.UTF8);
        PanelName = Panel.gameObject.name;
        CurrHitFrame = "";
        CurrHitHold = 0;
        time = 0.0f;        
        string folderName = SceneManager.GetActiveScene().name;
        folderPath = System.IO.Path.Combine("Assets/Data", folderName, participantId);
    }

    // Update is called once per frame
    public void Update()
    {
        time += Time.deltaTime;
        bool update = UpdateGaze();
        if(update)
        {
            RaycastHit hit;
            bool isPanelHit = false;
            bool isFrameHit = false;
            isPanelHit = ObjectDetection(panelLayer, out hit);
            if (!isPanelHit)
            {
                isFrameHit = ObjectDetection(frameLayer, out hit);
                if (isFrameHit)
                {
                    CurrHitFrame = hit.transform.gameObject.name;
                }
            }

            PatternMatching(hit, isPanelHit, isFrameHit);
            GameObject.Find("Interaction").SendMessage("GetObjRspList", ObjRsp);
        }
    }
}
