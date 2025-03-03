using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReplayLogger : MonoBehaviour
{
    private StreamWriter writer;
    private DateTime currentDate;
    private InteractionEyeTracker eyeData;
    private string sceneName;
    private KeyboardTextSystem keyboardSystem;
    private KeyboardTextSystemIntroduction introkeyboardSystem;
    private float startTime;

    // Start is called before the first frame update
    void Awake()
    {
        sceneName = SceneManager.GetActiveScene().name;
        currentDate = DateTime.Now;
        string eyeTrackingPath = @"C:\Users\jnt4\Desktop\EyeTrackingData\EyeReplay\" + currentDate.ToString("yyyy-MM-dd-HH-mm") + sceneName + ".csv";
        writer = new StreamWriter(eyeTrackingPath);
        startTime = Time.time;
    }

    private void Start()
    {
        writer.WriteLine("gazePositionX, gazePositionY, gazePositionZ, time");


        eyeData = FindObjectOfType<InteractionEyeTracker>();
    }

    // Update is called once per frame
    void Update()
    {
        float elapsedTime = Time.time - startTime; 

        writer.WriteLine(eyeData.gazeLocation.x + "," + eyeData.gazeLocation.y + "," + eyeData.gazeLocation.z, elapsedTime);
    }

    void OnDestroy()
    {
        writer.Close();
    }
}
