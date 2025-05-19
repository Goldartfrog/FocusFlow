using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetMovement : MonoBehaviour
{
    
    public GameObject planeRef;
    public float angularSpeed = 50f;
    public float linearSpeed = 5f;

    public Vector3 CurrOrigin;

    public bool isExpanding;
    public bool isMoving;
    private int currStep;
    private int currStage;
    private bool movementEnabled;
    public int NumRotationsPerStep;
    private int currNumRotations;

    private Vector3 planePos;

    public DataCollector dataCollectorRef;

    private bool canIncrementRotations;
    public List<Vector3> PosList = new List<Vector3>();

    // Start is called before the first frame update
    void Start()
    {
        this.transform.position = planeRef.transform.position + new Vector3(0.25f,0,0);
        CurrOrigin = this.transform.position;
        canIncrementRotations = false;
        currStep = 0;
        movementEnabled = false;
        //StartCoroutine(ExperimentTimer());
    }

    // // Update is called once per frame
    // void Update()
    // {
        
    // }

    public void StartMovement() {
        dataCollectorRef.ResumePolling();
        movementEnabled = true;
        Debug.Log(currStep);
        CurrOrigin = this.transform.position;
        planePos = planeRef.transform.position;
        //Called by DataCollector at the same time it starts polling.
    }

    public void FixedUpdate() {
        //The goal is to have the dot move at a consistent velocity regardless of radius adjust angular speed based on radius
        //for now radii are 0.25 0.5 0.75
        if (movementEnabled) {
            if (currStep == 0) {
                angularSpeed = linearSpeed / 0.25f;
                float angleDegreesPerFrame = angularSpeed * Mathf.Rad2Deg;
                transform.RotateAround(planePos, Vector3.forward, angleDegreesPerFrame);
            } else if (currStep == 1) {
                angularSpeed = linearSpeed / 0.5f;
                float angleDegreesPerFrame = angularSpeed * Mathf.Rad2Deg;
                transform.RotateAround(planePos, Vector3.forward, angleDegreesPerFrame);
            } else {
                angularSpeed = linearSpeed / 0.75f;
                float angleDegreesPerFrame = angularSpeed * Mathf.Rad2Deg;
                transform.RotateAround(planePos, Vector3.forward, angleDegreesPerFrame);
            }
        }
        //Handles checking if target is near endpoint
        if (Vector3.Distance(this.transform.position, CurrOrigin) <= 0.05f && canIncrementRotations && movementEnabled) {
            if (currNumRotations < NumRotationsPerStep - 1) {
                currNumRotations += 1;
                canIncrementRotations = false;
            } else {
                movementEnabled = false;
                this.transform.position = CurrOrigin;
                if (currStep < 2) {
                    PrepareNextStep();
                } else {
                    FinishStage();
                }
            }
        } else {
            if (Vector3.Distance(this.transform.position, CurrOrigin) > 0.075f)
                canIncrementRotations = true;
        }

    }

    // public IEnumerator ExperimentTimer() {
    //     yield return new WaitForSeconds(10f);
    //     isExpanding = false;
    //     Debug.Log("Now shrinking");
    //     yield return null;
    // }

    public void PrepareNextStep() {
        dataCollectorRef.RecordStep(currStep);

        //dataCollectorRef.PausePolling();
        this.transform.position += new Vector3(0.25f, 0, 0);
        currStep += 1;
        CurrOrigin = this.transform.position;
        currNumRotations = 0;
        canIncrementRotations = false;
        StartCoroutine(PauseToFocus());
    }

    public IEnumerator PauseToFocus() {
        yield return new WaitForSeconds(2f);
        dataCollectorRef.ResumePolling();
        movementEnabled = true;
    }

    public void FinishStage() {
        dataCollectorRef.RecordStep(currStep);
        //dataCollectorRef.PausePolling();
        dataCollectorRef.EndStage();
        movementEnabled = false;
    }

    public void ResetForNextStage() {
        this.transform.position = planeRef.transform.position + new Vector3(0.25f,0,0);
        currStep = 0;
        currNumRotations = 0;
        movementEnabled = false;
        canIncrementRotations = false;
    }
}
