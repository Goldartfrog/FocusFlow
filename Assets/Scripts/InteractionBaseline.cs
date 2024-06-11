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

public class InteractionBaseline : Interaction
{
    private float hitTime;
    public float hitTimeThres;

    public new bool ObjectDetection(int layer, out RaycastHit hit)
    {
        /* Collision detection */
        bool isHit = false;
        int layermask = 1 << layer;
        if (Physics.Raycast(gazeData.GazeOriginCombined, gazeData.GazeDirectionCombined, out hit, float.MaxValue, layermask))
        {
            // Panel
            if (layer == 6)
            {
                if (hit.transform.gameObject.GetComponent<UIObject>().status == 2)
                {
                    isHit = true;
                }
            }
            // Frames
            else if (layer == 7)
            {
                isHit = true;
            }
        }
        return isHit;
    }

    public new void PatternMatching(RaycastHit currHit, bool isPanelHit, bool isFrameHit)
    {
        ObjRsp.Clear();
        int CurrStatus;
        string HitName_ = "";
        if (isPanelHit || isFrameHit)
        {
            HitName_ = currHit.transform.gameObject.name;
        }

        if ((isPanelHit || hitTime >= hitTimeThres))
        {
            ObjRsp.Add(CurrHitFrame, ("Activate", ""));
            ObjRsp.Add(PanelName, ("Activate", CurrHitFrame));
        }
        else
        {
            CurrStatus = Panel.GetComponent<UIObject>().status;
            if (CurrStatus != 0 && (blinkBuffer == 0 || blinkBuffer > blinkThres))
            {
                // Debug.Log(isPanelHit + " " + CurrHitFrame + " " + CurrStatus + " " + blinkBuffer);
                ObjRsp.Add(PanelName, ("Deactivate", ""));
            }

            // For each interactive object
            foreach (string FrameName in Frames)
            {
                CurrStatus = GameObject.Find(FrameName).GetComponent<UIObject>().status;
                if (FrameName == HitName_ && CurrStatus == 0)
                {
                    ObjRsp.Add(FrameName, ("Hover", ""));
                    CurrHitHold = HitHoldValue;
                }
                else if (FrameName != HitName_ && CurrStatus != 0 && (blinkBuffer == 0 || blinkBuffer > blinkThres))
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
    }

    public new void Start()
    {
        base.Start();
        hitTime = 0;
        using (StreamWriter sw = File.AppendText(System.IO.Path.Combine(folderPath, "interaction_record.txt")))
        {
            sw.WriteLine("Dwell time threshold:{0}", hitTimeThres);
        }
    }

    public new void Update()
    {
        time += Time.deltaTime;
        hitTime += Time.deltaTime;

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
                    if (CurrHitFrame != hit.transform.gameObject.name)
                    {
                        hitTime = 0;
                        CurrHitFrame = hit.transform.gameObject.name;
                    }
                }
                else
                {
                    hitTime = 0;
                    CurrHitFrame = "";
                }
            }

            PatternMatching(hit, isPanelHit, isFrameHit);
            GameObject.Find("Interaction").SendMessage("GetObjRspList", ObjRsp);
        }
    }
}
