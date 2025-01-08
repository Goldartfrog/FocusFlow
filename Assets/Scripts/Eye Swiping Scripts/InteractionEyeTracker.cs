using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static KeyboardTextSystem;

public class InteractionEyeTracker : MonoBehaviour
{

    public Vector3 worldPosition;
    public Vector3 gazeLocation;
    public Vector3 smoothGaze;
    public Vector3 gazeDirection;
    public Vector3 gazeOrigin;
    public float gazeDepth;
    GazeData gazeData;
    [SerializeField] private int smoothLength = 10;
    private Queue<Vector3> gazeHistory;
    private Vector3 runningTotal = new Vector3(0, 0, 0);


    public float smoothDepth;
    private 
    // Start is called before the first frame update

    void Start()
    {
        gazeHistory = new Queue<Vector3>();

    }
    public void GetGazeParameter(GazeData GazeData_)
    {
        gazeData = GazeData_;
        worldPosition = gazeData.GazeOriginCombined;
        //Debug.Log(worldPosition);
        gazeLocation = gazeData.GazeOriginCombined + gazeData.Depth * gazeData.GazeDirectionCombined;
        gazeDirection = gazeData.GazeDirectionCombined;
        gazeOrigin = gazeData.GazeOriginCombined;
        gazeDepth = gazeData.Depth;
        smoothGaze = SmoothGaze();
        // smoothDepth = SmoothDepth();
    }

    private Vector3 SmoothGaze()
    {
        runningTotal += gazeLocation;
        gazeHistory.Enqueue(gazeLocation);
        Vector3 off = new Vector3(0, 0, 0);
        if (gazeHistory.Count > smoothLength)
        {
            off = gazeHistory.Dequeue(); 
        }
        runningTotal -= off;
        return runningTotal / gazeHistory.Count;
    }

    // private Vector3 SmoothDepth() {

    // }
}
