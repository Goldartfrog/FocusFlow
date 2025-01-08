using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SuperLogger : MonoBehaviour
{
    private StreamWriter writer;
    private DateTime currentDate;
    private InteractionEyeTracker eyeData;
    private string sceneName;
    private KeyboardTextSystem keyboardSystem;
    private KeyboardTextSystemIntroduction introkeyboardSystem;
    private float timer;
    private char currentLetter = ' ';
    private bool typingPhrase = false;
    private bool insidePhrase = false;
    private void Awake()
    {
        sceneName = SceneManager.GetActiveScene().name;
        currentDate = DateTime.Now;
        string eyeTrackingPath = @"C:\Users\awefel2\Desktop\EyeTrackingData\" + currentDate.ToString("yyyy-MM-dd-HH-mm") + sceneName + ".txt";
        writer = new StreamWriter(eyeTrackingPath);
    }

    void Start()
    {
        
        
        writer.WriteLine(sceneName);
        
        writer.WriteLine("gazeDirection_x, gazeDirection_y, gazeDirection_z, gazeOrigin_x, gazeOrigin_y, gazeOrigin_z, depth, time, current_word, highlighted_letter");

        eyeData = FindObjectOfType<InteractionEyeTracker>();
        keyboardSystem = FindObjectOfType<KeyboardTextSystem>();
        introkeyboardSystem = FindObjectOfType<KeyboardTextSystemIntroduction>();
    }

    // Update is called once per frame
    void Update()
    {
        typingPhrase = introkeyboardSystem.phraseTyping();

        //writer.WriteLine(eyeData.gazeDirection);
        string s = "";
        timer += Time.deltaTime;
        s += VectorComponents(eyeData.gazeDirection);
        s += VectorComponents(eyeData.gazeOrigin);
        s += eyeData.gazeDepth.ToString("F4") + ", ";
        s += timer.ToString("F4");
        if (keyboardSystem != null)
        {
            s += ", " + keyboardSystem.GetCurrentWord();
        }
        if (introkeyboardSystem != null)
        {
            s += ", " + introkeyboardSystem.CorrectTarget();

            if (introkeyboardSystem.CorrectTarget() == "Finished!" && typingPhrase && insidePhrase)
            {
                MarkEnd();
            }

        }

        

        //s += ", " + currentLetter;
        writer.WriteLine(s);
        //WriteVector(eyeData.gazeDirection);
    }

    string VectorComponents(Vector3 v)
    {
        return v.x.ToString("F4") + ", " + v.y.ToString("F4") + ", " + v.z.ToString("F4") + ", ";
    }
    public void StartEntry()
    {
        writer.WriteLine("&Started recording");

        if (typingPhrase && !insidePhrase)
        {
            insidePhrase = true;
            MarkStart();
        }
    }

    public void StopEntry()
    {
        writer.WriteLine("*Stopped recording");
    }
    
    public void SetCurrentLetter(char letter)
    {
        currentLetter = letter;
    }

    public void WriteSetup(string setupString)
    {
        writer.WriteLine(setupString);
    }


    public void WriteDelay(float delay)
    {
        writer.WriteLine("?Waiting for:" + delay);
    }

    public void WriteLetter(char letter, float time)
    {
        writer.WriteLine("!" + letter + "," + time);
    }


    public void TopThree(List<string> topwords)
    {
        string s = ";Top words:";
        for (int i = 0; i < topwords.Count; i++)
        {
            s += topwords[i] + ",";
        }
        writer.WriteLine(s);
    }

    public void AcceptedSuggestion(string suggestion)
    {
        writer.WriteLine("@Acepted:" + suggestion);
    }

    public void Deleted()
    {
        writer.WriteLine("#Deleted Last Word");
    }

    public void Target(string target)
    {
        writer.WriteLine("%New Target:" + target);
    }

    public void NextEntered()
    {
        writer.WriteLine("^Next pressed");
    }

    public void MarkStart()
    {
        writer.WriteLine("`Started Phrase");
    }

    public void MarkEnd()
    {
        writer.WriteLine("+Finished Phrase");
        insidePhrase = false;
    }

}
