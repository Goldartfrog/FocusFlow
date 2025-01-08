using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class KeyboardExperimentManager : MonoBehaviour
{
    public InteractionEyeTracker EyePos;

    private float total_time = 0;
    private bool counting = false;
    private string UserName = "";
    private bool UsingStandard = false;
    private int characters = 0;
    StreamWriter dataOutput;
    private int currWord = -1;

    private List<string> managerWords = new List<string>();
    private int managerLen;

    private string pathToTXT = @"C:\Users\joaolmbc\Desktop\Softkeyboard\ExperimentDocJohnny4.txt";
    private string eyeTrackingPath = @"C:\Users\joaolmbc\Desktop\Softkeyboard\EyeTrackingJohnny4.txt";

    private bool pause = true;
    private DateTime currentDate;

    // Start is called before the first frame update
    void Start()
    {
        currentDate = DateTime.Now;
        eyeTrackingPath = @"C:\Users\awefel2\Desktop\EyeTrackingData\ProjectedEyePositions" + currentDate.ToString("yyyy-MM-dd-HH-mm") + ".txt";
        dataOutput = new StreamWriter(eyeTrackingPath);
        dataOutput.WriteLine("Begin data from special keyboard");
    }

    public void swapUsing() { UsingStandard = !UsingStandard; }

    // Update is called once per frame
    void Update()
    {
        if (counting)
        {
            total_time += Time.deltaTime;

            var output = GetGazePoint();
            dataOutput.WriteLine(output.ToString("F4"));
        }
    }

    public void enable_timer()
    {
        if (pause == true)
        {
            pause = false;
            Debug.Log("Unpaused");
        }
        counting = true;
    }

    public void pause_timer()
    {
        Debug.Log("Paused Timer");
        pause = true;
        counting = false;
    }

    //public void get_name(string name)
    //{
    //    UserName = name;
    //}

    public void disable_timer()
    {
        counting = false;
        Debug.Log("Total time taken was: " +  total_time);
        Debug.Log("Total characters: " + characters);
        Debug.Log("WPM is: " + characters / 5 / total_time * 60);
        writeResults();
    }

    //public void reset_timer()
    //{
    //    counting = false;
    //    total_time = 0;
    //    Debug.Log("Restarted!");

    //    dataOutput.Close();
    //    dataOutput = new StreamWriter(eyeTrackingPath, true);
    //}


    private void writeResults()
    {
        //dataOutput.WriteLine("Total time taken was: " + total_time);
        //dataOutput.Close();

        //using (StreamWriter writer = new StreamWriter(pathToTXT, true))
        //{
            //writer.WriteLine("User: " + UserName);
        if (UsingStandard)
        {
            dataOutput.WriteLine("Using Standard Keyboard");
        }
        else
        {
            dataOutput.WriteLine("Using Unique Keyboard");
        }
        dataOutput.WriteLine("Total time taken was: " + total_time);
        dataOutput.WriteLine("Total characters: " + characters);
        dataOutput.WriteLine("WPM is: " + characters / 5 / total_time * 60);
        dataOutput.WriteLine("");
        dataOutput.WriteLine("---------------------------");
        dataOutput.WriteLine("");
        dataOutput.Close();
        //}
    }

    public void LoadCharacters(string[] words, List<int> randomized, int len)
    {
        managerLen = len;

        for (int i = 0; i < len; i++)
        {
            int index = randomized[i];
            characters += words[index].Length;
            managerWords.Add(words[index]);
            characters += 1;   // for the space between words
            Debug.Log(words[index]);
        }

        //Debug.Log("TOTAL LENGTH IS: " + characters);
    }

    public Vector2 GetGazePoint()
    {
        return FindIntersection(EyePos.worldPosition, EyePos.gazeLocation);
    }

    public static Vector2 FindIntersection(Vector3 userPos, Vector3 fixationPT)
    {
        Vector3 direction = fixationPT - userPos;

        float zPlane = 2;
        if (direction.z == 0)
        {
            return Vector2.zero;
        }

        float t = (zPlane - userPos.z) / direction.z;

        Vector3 intersectionPoint = userPos + t * direction;

        return new Vector2(intersectionPoint.x, intersectionPoint.y);
    }

    //public void nextWord()
    //{
    //    if (currWord < managerWords.Count)
    //    {
    //        dataOutput.WriteLine();
    //        currWord++;
    //        dataOutput.WriteLine(managerWords[currWord]);
    //    }
    //}
}
