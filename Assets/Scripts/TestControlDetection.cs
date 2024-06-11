using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class TestControlDetection : MonoBehaviour
{
    private float time;
    private string folderPath;
    
    public float maxTime;
    public bool WriteFlag;
    public bool StartFlag;
    private string participantId;

    // Start is called before the first frame update
    void Start()
    {
        participantId = File.ReadAllText(System.IO.Path.Combine("Assets/Data", "participantID.txt"), Encoding.UTF8);
        string folderName = SceneManager.GetActiveScene().name;
        folderPath = System.IO.Path.Combine("Assets/Data", folderName, participantId);
    }

    // Update is called once per frame
    void Update()
    {
        if(StartFlag)
        {
            time += Time.deltaTime;
        }

        if(time >= maxTime){
            int nextSceneNum = (Convert.ToInt32(SceneManager.GetActiveScene().name[1].ToString())) + 1;

            if(nextSceneNum == 5){
                EditorApplication.ExitPlaymode();
            }
            else{
                SceneManager.LoadScene("S" + nextSceneNum.ToString());
            }
            return;
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            GameObject.Find("Eye Tracker").GetComponent<ViveSR.anipal.Eye.EyeTracker>().useLineRenderer = !GameObject.Find("Eye Tracker").GetComponent<ViveSR.anipal.Eye.EyeTracker>().useLineRenderer;
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            WriteFlag = true;
            StartFlag = true;
        }
    }
}
