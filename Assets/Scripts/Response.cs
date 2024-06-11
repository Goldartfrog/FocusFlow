using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using System.Text;
using Random = System.Random;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class Response : MonoBehaviour
{
    protected float globalTime;
    protected string folderPath;
    protected Dictionary<string, (string, string)> ObjRspList;

    protected string participantId;

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

    // Start is called before the first frame update
    public void Start()
    {
        globalTime = 0;
        participantId = File.ReadAllText(System.IO.Path.Combine("Assets/Data", "participantID.txt"), Encoding.UTF8);
        string folderName = SceneManager.GetActiveScene().name;
        folderPath = System.IO.Path.Combine("Assets/Data", folderName, participantId);
        ObjRspList = new Dictionary<string, (string, string)>();
    }

    // Update is called once per frame
    public void Update()
    {
        globalTime += Time.deltaTime;
        Executor();
    }
}
