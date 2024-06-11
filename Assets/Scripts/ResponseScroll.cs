using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using Random = System.Random;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class ResponseScroll : MonoBehaviour
{    
    public Dictionary<string, (string, float)> ObjRspList;

    public string folderPath;
    public string tester_index;
    public string test_index;

    public float globalTime;

    public void GetObjRspList(Dictionary<string, (string, float)> ObjRsp)
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
        (string, float) action;
        foreach (KeyValuePair<string, (string, float)> ele in ObjRspList)
        {
            objName = ele.Key;
            action = ele.Value;
            GameObject obj = GameObject.Find(objName);
            // Debug.Log(objName + action.Item1 + action.Item2);
            switch (action.Item1)
            {
                case "Up":
                    obj.SendMessage("Up", action.Item2);
                    break;
                case "Down":
                    obj.SendMessage("Down", action.Item2);
                    break;
                case "Stop":
                    obj.SendMessage("Stop", action.Item2);
                    Debug.Log("Stop!!!!!");
                    break;
                default:
                    break;
            }
        }
    }

    // Start is called before the first frame update
    public void Start()
    {
        string folderName = SceneManager.GetActiveScene().name;
        folderPath = System.IO.Path.Combine("Assets/Data", folderName, tester_index, test_index);
        globalTime = 0;
        // Debug.Log("Response Start");
    }

    // Update is called once per frame
    public void Update()
    {
        globalTime += Time.deltaTime;
        Executor();
        // Debug.Log("Response Update");
    }
}
