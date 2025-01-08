using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using ViveSR.anipal.Eye;
using static KeyboardTextSystem;

public class EyeDepthTracker : MonoBehaviour
{

    public Vector3 worldPosition;
    public Vector3 gazeLocation;
    public Vector3 smoothGaze;
    public Vector3 gazeDirection;
    public Vector3 gazeOrigin;
    public float gazeDepth;
    GazeData gazeData;
    [SerializeField] private int smoothLength = 10;
    private Queue<float> gazeDepthHistory;
    private float runningTotal = 0f;

    public EyeTracker eyeTrackerRef;
    public float smoothedGazeDepth;// ACCESS THIS FOR SMOOTHED DEPTH;

    public Transform headOriginRef;
    public Vector3 headOriginRefPos;

    // Start is called before the first frame update

    void Start()
    {
        gazeDepthHistory = new Queue<float>();


    }

    void Update() {
        headOriginRefPos = headOriginRef.transform.position;
    }


    public void FixedUpdate() {
        SmoothDepth();
    }
    private void SmoothDepth() {
        //TODO: Figure out how to get this from gazeData
        //gazeLocation = eyeTrackerRef.GazeOriginCombined + gazeData.Depth * gazeData.GazeDirectionCombined;
        float depth = Vector3.Distance(headOriginRefPos, gazeLocation);
        runningTotal += depth;
        gazeDepthHistory.Enqueue(depth);
        if (gazeDepthHistory.Count > smoothLength) {
            runningTotal -= gazeDepthHistory.Dequeue();
        }

        smoothedGazeDepth = runningTotal / smoothLength;

    }
}
