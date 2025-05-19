using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class DataCollector : MonoBehaviour
{

    private string directoryPath = "C:\\Users\\awefel2\\Desktop\\EyeTrackingTestingData\\test.txt";
    public GameObject planeRef;
    public GameObject targetRef;

    private int currStage;

    public List<Vector3> groundTruthPoslist = new List<Vector3>();
    public List<Vector3> gazeIntersectionList = new List<Vector3>();

    public FollowGazeIntercept gazeInterceptRef;

    private Coroutine currPollCoroutine;
    private bool firstTime;
    private bool shouldPoll;
    public GameObject center;
    
    // Start is called before the first frame update
    void Start()
    {
        firstTime = true;
        shouldPoll = false;
        currStage = 0;
        StartCoroutine(PauseBeforeStart());
        using (StreamWriter writer = new StreamWriter(directoryPath)) {
            writer.WriteLine("STAGE 0");
        }

    }

    // Update is called once per frame
    void Update()
    {
      if (shouldPoll) {
        if (gazeInterceptRef.currHitPoint != new Vector3(0,0,0)) {
            groundTruthPoslist.Add(targetRef.transform.position);
            gazeIntersectionList.Add(gazeInterceptRef.currHitPoint);
        }
      }  
    }

    public void StartStage() {
        targetRef.transform.GetComponent<TargetMovement>().StartMovement();
        shouldPoll = true;
        //currPollCoroutine = StartCoroutine(PollPositions());
    }

    public IEnumerator PauseBeforeStart() {
        if (firstTime) {
            yield return new WaitForSeconds(20f);
            firstTime = false;
        } else {
            yield return new WaitForSeconds(5f);
        }
        
        StartStage();
    }

    // public IEnumerator PollPositions() {
    //     if (shouldPoll) {
    //         yield return new WaitForSeconds(0.005f);
    //         if (gazeInterceptRef.currHitPoint != new Vector3(0,0,0)) {
    //             groundTruthPoslist.Add(targetRef.transform.position);
    //             gazeIntersectionList.Add(gazeInterceptRef.currHitPoint);
    //         }
            
    //         currPollCoroutine = StartCoroutine(PollPositions());
    //     }
        
    // }

    // public void PausePolling() {
    //     shouldPoll = false;
    // }

    public void ResumePolling() {
        shouldPoll = true;
       //StartCoroutine(PollPositions());
    }

    public void RecordStep(int step) {
        shouldPoll = false;
        string header = "";
        if (step == 0) {
            header = "INNER-CIRCLE";
        } else if (step == 1) {
            header = "MIDDLE-CIRCLE";
        } else {
            header = "OUTER-CIRCLE";
        }

        using (StreamWriter writer = new StreamWriter(directoryPath, true)) {
            writer.WriteLine(header);
            writer.WriteLine("Ground Truth");
            foreach (var item in groundTruthPoslist) {
                string formatted = string.Format("{0:F6},{1:F6},{2:F6}", item.x, item.y, item.z);
                writer.WriteLine(string.Join(",", formatted));
            }
            writer.WriteLine("Eyetracking Info");
            foreach (var item in gazeIntersectionList) {
                string formatted = string.Format("{0:F6},{1:F6},{2:F6}", item.x, item.y, item.z);
                writer.WriteLine(string.Join(",", formatted));
            }
        }
        groundTruthPoslist.Clear();
        gazeIntersectionList.Clear();
    }

    public void EndStage() {

        if (currStage < 2) {
            currStage += 1;
            string title = "STAGE " + currStage.ToString();
            using (StreamWriter writer = new StreamWriter(directoryPath, true)) {
                writer.WriteLine(title);
            }
            planeRef.transform.position += new Vector3(0,0,0.5f);
            center.transform.position += new Vector3(0,0,0.5f);
            targetRef.GetComponent<TargetMovement>().ResetForNextStage();

            StartCoroutine(PauseBeforeStart());
        }
    }
}
