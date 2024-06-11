using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using System.Text;
using Random = System.Random;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class ResponseShop : MonoBehaviour
{    
    private Dictionary<string, (string, string)> ObjRspList;

    private float time;
    private string folderPath;

    private string participantId;

    public bool isGuidance_1;
    public bool isGuidance_2;
    public bool isGuidance_3;
    public bool isSwitchAllowed;
    public GameObject Guidance_1;
    public GameObject Guidance_2;

    public void GetObjRspList(Dictionary<string, (string, string)> ObjRsp)
    {
        ObjRspList = ObjRsp;
    }

    public virtual void Executor()
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
                    if (!obj.GetComponent<UIObject>().isLocked)
                    {
                        obj.SendMessage("Hover");
                    }
                    break;
                case "Activate":
                    if (!obj.GetComponent<UIObject>().isLocked)
                    {
                        obj.SendMessage("Activate", action.Item2);
                    }
                    break;
                case "Deactivate":
                    if (!obj.GetComponent<UIObject>().isLocked)
                    {
                        obj.SendMessage("Deactivate");
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
                GameObject.Find("Interaction").GetComponent<InteractionShop>().UseGuidance_1 = false;
                Guidance_1.SetActive(false);
            }
            else{
                isGuidance_1 = true;
                GameObject.Find("Interaction").GetComponent<InteractionShop>().UseGuidance_1 = true;
                Guidance_1.SetActive(true);
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
                GameObject.Find("Interaction").GetComponent<InteractionShop>().UseGuidance_2 = false;
                Guidance_2.SetActive(false);
            }
            else{
                isGuidance_2 = true;
                GameObject.Find("Interaction").GetComponent<InteractionShop>().UseGuidance_2 = true;
                Guidance_2.SetActive(true);
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
                GameObject.Find("Interaction").GetComponent<InteractionShop>().UseGuidance_3 = false;
                Guidance_1.SetActive(false);
            }
            else{
                isGuidance_3 = true;
                GameObject.Find("Interaction").GetComponent<InteractionShop>().UseGuidance_3 = true;
                Guidance_1.SetActive(true);
            }
        }
    }

    // Start is called before the first frame update
    public void Start()
    {
        time = 0;
        participantId = File.ReadAllText(System.IO.Path.Combine("Assets/Data", "participantID.txt"), Encoding.UTF8);
        string folderName = SceneManager.GetActiveScene().name;
        folderPath = System.IO.Path.Combine("Assets/Data", folderName, participantId);
    }

    // Update is called once per frame
    public void Update()
    {
        time += Time.deltaTime;
        Executor();
    }
}
