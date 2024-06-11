using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using System.Text;
using System;
using Random = System.Random;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEngine.UI;

public class ResponseTutorial : Response
{
    // Random numbers
    private Random rand;
    private int prevRand;
    private int currRand;
    // Name of current target frame
    private string targetFrameName;
    // Whether the target has been activated
    private bool isActivated;
    private bool isFirstActivated;
    // Index for iteration and round
    private int iterIndex;
    private int roundIndex;
    // Iteration time
    private float iterTime;

    // Countdown timer for iteration switch
    public float timer;
    // Pre-defined time interval between iterations
    public float iterInterval;
    // Frame index base number
    public int frameIndexBase;
    // Total number of frames
    public int frameNum;
    // The number of fixed learning iterations
    public int fixLearnIter;
    // Maximum number of iterations
    public int maxIter;
    // Whether it is using guidance
    public bool isGuidance_1;
    public bool isGuidance_2;
    public bool isGuidance_3;
    // Whether the user can switch to guidance
    public bool isSwitchAllowed;
    // Whether it is final test
    public bool isFinalTest;
    // Gameobject for Guidance
    public GameObject Guidance_1;
    public GameObject Guidance_2;
    // Gameobject for DisplayText
    public GameObject DisplayText;

    public void RandomlyChooseOne()
    {
        prevRand = currRand;
        while(currRand == prevRand)
        {
            currRand = rand.Next(frameNum);
        }
        targetFrameName = "Frame_" + (currRand+frameIndexBase).ToString();
        GameObject.Find(targetFrameName).GetComponent<MeshRenderer>().material.color = new Color(1f, 0.5f, 0.5f, 1f);
        if(isFinalTest)
        {
            timer = iterInterval;
        }
        else{
            timer = 3;
        }
        iterTime = 0;
        if(GameObject.Find(targetFrameName).GetComponent<UIObject>().status == 1)
        {
            using (StreamWriter sw = File.AppendText(System.IO.Path.Combine(folderPath, "interaction_record.txt")))
            {
                sw.WriteLine("{0}, {1}, {2}, {3}, {4}, {5}, {6}", globalTime, isGuidance_1, isGuidance_2, isGuidance_3, iterIndex, iterTime, "Hover");
            }
        }
        isActivated = false;
    }

    public new void Executor()
    {
        if (ObjRspList is null)
        {
            return;
        }
        string objName;
        (string, string) action;
        foreach (KeyValuePair<string, (string, string)> ele in ObjRspList)
        {
            objName = ele.Key;
            action = ele.Value;
            GameObject obj = GameObject.Find(objName);
            switch (action.Item1)
            {
                case "Hover":
                    if (!obj.GetComponent<UIObject>().isLocked && obj.GetComponent<UIObject>().status != 1)
                    {
                        obj.SendMessage("Hover");

                        if (GameObject.Find("TestControl").GetComponent<TestControl>().WriteFlag)
                        {
                            if (objName.Contains("Frame"))
                            {
                                if (objName == targetFrameName)
                                {
                                    using (StreamWriter sw = File.AppendText(System.IO.Path.Combine(folderPath, "interaction_record.txt")))
                                    {
                                        sw.WriteLine("{0}, {1}, {2}, {3}, {4}, {5}, {6}", globalTime, isGuidance_1, isGuidance_2, isGuidance_3, iterIndex, iterTime, "Hover");
                                    }
                                }
                            }
                        }
                    }
                    break;
                case "Activate":
                    if (!obj.GetComponent<UIObject>().isLocked && obj.GetComponent<UIObject>().status != 2)
                    {
                        obj.SendMessage("Activate", action.Item2);

                        if (GameObject.Find("TestControl").GetComponent<TestControl>().WriteFlag)
                        {
                            if (objName.Contains("Panel"))
                            {
                                if (action.Item2 != targetFrameName)
                                {
                                    using (StreamWriter sw = File.AppendText(System.IO.Path.Combine(folderPath, "interaction_record.txt")))
                                    {
                                        sw.WriteLine("{0}, {1}, {2}, {3}, {4}, {5}, {6}", globalTime, isGuidance_1, isGuidance_2, isGuidance_3, iterIndex, iterTime, "False");
                                    }
                                }
                                else
                                {
                                    if(!isActivated)
                                    {
                                        isFirstActivated = true;
                                    }
                                    isActivated = true;
                                    
                                    using (StreamWriter sw = File.AppendText(System.IO.Path.Combine(folderPath, "interaction_record.txt")))
                                    {
                                        sw.WriteLine("{0}, {1}, {2}, {3}, {4}, {5}, {6}", globalTime, isGuidance_1, isGuidance_2, isGuidance_3, iterIndex, iterTime, "True");
                                    }
                                }
                            }
                        }
                    }

                    break;
                case "Deactivate":
                    if (!obj.GetComponent<UIObject>().isLocked && obj.GetComponent<UIObject>().status != 0)
                    {
                        obj.SendMessage("Deactivate");
                        if (objName.Contains("Panel"))
                        {
                            if(isFirstActivated)
                            {
                                if(isFinalTest)
                                {
                                    timer = Math.Max(timer, 3);
                                }
                                GameObject.Find(targetFrameName).GetComponent<MeshRenderer>().material.color = Color.white;
                                isFirstActivated = false;
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
        }
    }

    public void GuidanceSwitch1()
    {
        if(isSwitchAllowed)
        {
            if(isGuidance_2 || isGuidance_3)
            {
                Debug.Log("Please remove other guidances!!!!!");
                return;
            }

            if(isGuidance_1)
            {
                isGuidance_1 = false;
                GameObject.Find("Interaction").GetComponent<Interaction>().UseGuidance_1 = false;
                Guidance_1.SetActive(false);
            }
            else{
                isGuidance_1 = true;
                GameObject.Find("Interaction").GetComponent<Interaction>().UseGuidance_1 = true;
                Guidance_1.SetActive(true);
            }
                
            if(targetFrameName != "" && GameObject.Find(targetFrameName).GetComponent<UIObject>().status == 1)
            {
                using (StreamWriter sw = File.AppendText(System.IO.Path.Combine(folderPath, "interaction_record.txt")))
                {
                    sw.WriteLine("{0}, {1}, {2}, {3}, {4}, {5}, {6}", globalTime, isGuidance_1, isGuidance_2, isGuidance_3, iterIndex, iterTime, "Hover");
                }
            }
        }
    }

    public void GuidanceSwitch2()
    {
        if(isSwitchAllowed)
        {
            if(isGuidance_1 || isGuidance_3)
            {
                Debug.Log("Please remove other guidances!!!!!");
                return;
            }

            if(isGuidance_2)
            {
                isGuidance_2 = false;
                GameObject.Find("Interaction").GetComponent<Interaction>().UseGuidance_2 = false;
                Guidance_2.SetActive(false);
            }
            else{
                isGuidance_2 = true;
                GameObject.Find("Interaction").GetComponent<Interaction>().UseGuidance_2 = true;
                Guidance_2.SetActive(true);
            }
                
            if(targetFrameName != "" && GameObject.Find(targetFrameName).GetComponent<UIObject>().status == 1)
            {
                using (StreamWriter sw = File.AppendText(System.IO.Path.Combine(folderPath, "interaction_record.txt")))
                {
                    sw.WriteLine("{0}, {1}, {2}, {3}, {4}, {5}, {6}", globalTime, isGuidance_1, isGuidance_2, isGuidance_3, iterIndex, iterTime, "Hover");
                } 
            }
        }
    }

    public void GuidanceSwitch3()
    {
        if(isSwitchAllowed)
        {
            if(isGuidance_1 || isGuidance_2)
            {
                Debug.Log("Please remove other guidances!!!!!");
                return;
            }

            if(isGuidance_3)
            {
                isGuidance_3 = false;
                GameObject.Find("Interaction").GetComponent<Interaction>().UseGuidance_3 = false;
                Guidance_1.SetActive(false);
            }
            else{
                isGuidance_3 = true;
                GameObject.Find("Interaction").GetComponent<Interaction>().UseGuidance_3 = true;
                Guidance_1.SetActive(true);
            }
            
            if(targetFrameName != "" && GameObject.Find(targetFrameName).GetComponent<UIObject>().status == 1)
            {
                using (StreamWriter sw = File.AppendText(System.IO.Path.Combine(folderPath, "interaction_record.txt")))
                {
                    sw.WriteLine("{0}, {1}, {2}, {3}, {4}, {5}, {6}", globalTime, isGuidance_1, isGuidance_2, isGuidance_3, iterIndex, iterTime, "Hover");
                }
            }
        }
    }

    // Start is called before the first frame update
    public new void Start()
    {
        base.Start();
        rand = new Random();
        prevRand = -1;
        currRand = -1;
        targetFrameName = "";
        isActivated = true;
        isFirstActivated = false;
        iterIndex = 0;
        iterTime = 0.0f;
        using (StreamWriter sw = File.AppendText(System.IO.Path.Combine(folderPath, "interaction_record.txt")))
        {
            sw.WriteLine("Participant Id:{0}", participantId);
        }
    }

    public new void Update()
    {
        globalTime += Time.deltaTime;
        iterTime += Time.deltaTime;
        Executor();

        if (!GameObject.Find("TestControl").GetComponent<TestControl>().StartFlag)
        {
            return;
        }

        if((isActivated && !isFirstActivated) || isFinalTest)
        {
            timer -= Time.deltaTime;
        }

        if (timer <= 0)
        {
            if(isFinalTest)
            {
                if(iterIndex != 0)
                {
                    GameObject.Find(targetFrameName).GetComponent<MeshRenderer>().material.color = Color.white;
                    if (!isActivated)
                    {
                        using (StreamWriter sw = File.AppendText(System.IO.Path.Combine(folderPath, "interaction_record.txt")))
                        {
                            sw.WriteLine("{0}, {1}, {2}, {3}, {4}, {5}, {6}", globalTime, isGuidance_1, isGuidance_2, isGuidance_3, iterIndex, iterTime, "False");
                        }
                    }
                }
            }
            iterIndex++;

            if (iterIndex <= fixLearnIter)
            {
                isGuidance_1 = true;
                isGuidance_2 = false;
                isGuidance_3 = false;
                isSwitchAllowed = false;
            }
            else if(iterIndex <= maxIter)
            {
                if(iterIndex == fixLearnIter + 1 && !isFinalTest)
                {
                    isGuidance_1 = false;
                    isGuidance_2 = true;
                    isGuidance_3 = false;
                }
                isSwitchAllowed = true;
            }
            else{
                if(!isFinalTest)
                {
                    SceneManager.LoadScene("Test");
                }
                else{
                    EditorApplication.ExitPlaymode();
                }
                return;
            }

            if(isGuidance_1)
            {
                GameObject.Find("Interaction").GetComponent<Interaction>().UseGuidance_1 = true;
                Guidance_1.SetActive(true);
            }
            else{
                GameObject.Find("Interaction").GetComponent<Interaction>().UseGuidance_1 = false;
                Guidance_1.SetActive(false);
            }
            if(isGuidance_2)
            {
                GameObject.Find("Interaction").GetComponent<Interaction>().UseGuidance_2 = true;
                Guidance_2.SetActive(true);
            }
            else{
                GameObject.Find("Interaction").GetComponent<Interaction>().UseGuidance_2 = false;
                Guidance_2.SetActive(false);
            }
            if(isGuidance_3)
            {
                GameObject.Find("Interaction").GetComponent<Interaction>().UseGuidance_3 = true;
                Guidance_1.SetActive(true);
            }
            else{
                GameObject.Find("Interaction").GetComponent<Interaction>().UseGuidance_3 = false;
                if(!isGuidance_1)
                {
                    Guidance_1.SetActive(false);
                }
            }

            RandomlyChooseOne();
        }
    }
}
