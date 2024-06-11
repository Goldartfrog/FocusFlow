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

public class InteractionShopBaseline : MonoBehaviour
{
    private GazeData gazeData;
    private Dictionary<string, (string, string)> ObjRsp;

    private Vector3 GazeDirectionSum;
    private Vector3 GazeDirectionMean;
    private Queue<Vector3> PrevGaze;
    private float DepthSum;
    private float DepthMean;
    private Queue<float> PrevDepth;

    public int ValidGazeSteps;

    private int blinkBuffer;
    public int blinkThres;

    private List<string> Products;
    public GameObject Guidance;
    public GameObject Panel;
    private string PanelName;
    private string CurrHitProductName;
    private int CurrHitHold;
    public int HitHoldValue;

    private float time;
    private string folderPath;

    public int productLayer;
    public int panelLayer;
    public float ActivateDepthFactor;
    public float MaintainDepthFactor;
    public bool UseGuidance;
    private string participantId;

    private float hitTime;
    public float hitTimeThres;

    public void GetGazeParameter(GazeData GazeData_)
    {
        gazeData = GazeData_;
    }

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
                if (hit.transform.gameObject.GetComponent<UIObject>().status == 2)
                {
                    isHit = true;
                }
            }
            // Products
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
        
        if (isPanelHit || hitTime >= hitTimeThres)
        {
            ObjRsp.Add(CurrHitProductName, ("Activate", ""));
            ObjRsp.Add(PanelName, ("Activate", CurrHitProductName));
        }
        else 
        {
            CurrStatus = Panel.GetComponent<UIObject>().status;
            if (CurrStatus != 0 && (blinkBuffer == 0 || blinkBuffer > blinkThres))
            {
                ObjRsp.Add(PanelName, ("Deactivate", ""));
            }
            
            // For each interactive object
            foreach (string ProductName in Products)
            {
                CurrStatus = GameObject.Find(ProductName).GetComponent<UIObject>().status;
                if (ProductName == HitName && CurrStatus == 0)
                {
                    ObjRsp.Add(ProductName, ("Hover", ""));
                    CurrHitHold = HitHoldValue;
                }
                else if (ProductName != HitName && CurrStatus != 0 && (blinkBuffer == 0 || blinkBuffer > blinkThres))
                {
                    if (CurrHitProductName == ProductName && CurrHitHold > 0)
                    {
                        CurrHitHold--;                        
                    }
                    else
                    {
                        ObjRsp.Add(ProductName, ("Deactivate", ""));
                        if (CurrHitProductName == ProductName)
                        {
                            CurrHitProductName = "";
                        }
                    }
                }
            }
        }
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
        Products = new List<string>();
        PanelName = Panel.gameObject.name;
        CurrHitProductName = "";
        CurrHitHold = 0;

        time = 0.0f;
        hitTime = 0;

        participantId = File.ReadAllText(System.IO.Path.Combine("Assets/Data", "participantID.txt"), Encoding.UTF8);
        string folderName = SceneManager.GetActiveScene().name;
        folderPath = System.IO.Path.Combine("Assets/Data", folderName, participantId);

        Transform Clothes = GameObject.Find("clothes").transform;
        Transform Clothes_Props = GameObject.Find("clothes_props").transform;
        Transform Backpacks = GameObject.Find("backpacks").transform;
        foreach (Transform product in Clothes)
        {
            if (product.gameObject.activeSelf)
            {
                Products.Add(product.gameObject.name);
            }
        }
        foreach (Transform product in Clothes_Props)
        {
            if (product.gameObject.activeSelf)
            {
                Products.Add(product.gameObject.name);
            }
        }
        foreach (Transform product in Backpacks)
        {
            if (product.gameObject.activeSelf)
            {
                Products.Add(product.gameObject.name);
            }
        }
    }

    // Update is called once per frame
    public void Update()
    {
        time += Time.deltaTime;
        hitTime += Time.deltaTime;

        UpdateGaze();

        RaycastHit hit;
        bool isPanelHit = false;
        bool isFrameHit = false;
        isPanelHit = ObjectDetection(panelLayer, out hit);
        if (!isPanelHit)
        {
            isFrameHit = ObjectDetection(productLayer, out hit);
            if (isFrameHit)
            {
                if (CurrHitProductName != hit.transform.gameObject.name)
                    {
                        hitTime = 0;
                        CurrHitProductName = hit.transform.gameObject.name;
                    }
                CurrHitProductName = hit.transform.gameObject.name;
            }
            else{
                hitTime = 0;
                CurrHitProductName = "";
            }
        }

        PatternMatching(hit, isPanelHit, isFrameHit);
        GameObject.Find("Interaction").SendMessage("GetObjRspList", ObjRsp);
    }
}
