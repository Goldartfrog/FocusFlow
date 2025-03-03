using System.Collections;
using System.Collections.Generic;
//using System.Numerics;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class DwellBehavior : MonoBehaviour
{
    
    //The point of the dwell indicator is to show the user what an effective glance at a letter would entail based on duration the user is looking at a letter.

    public float duration;
    public GameObject cursorRef;
    private Queue<Vector3> mostRecentPositions = new Queue<Vector3>();
    public float diffThreshold; //The distance difference between the most recently read cursor position and the average of the most recent position at which to stop displaying dwell.
    public Light lightRef;

    private Vector3 runningTotal = new Vector3(0,0,0);

    private Vector3 currAvg;
    private bool playingAnimation;
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(MonitorCursor());
    }

    // Update is called once per frame
    void Update()
    {
        //Check for interrupt and activate DwellAnimation
        this.transform.position = cursorRef.transform.position;
       // Debug.Log("current position: " + cursorRef.transform.position + " curr average: " + currAvg);
        //Debug.Log("Current distance: " + Vector3.Distance(currAvg, cursorRef.transform.position));
        if (Vector3.Distance(currAvg, cursorRef.transform.position) > 0.2f) {
            //Halt animation
            //Await restart
            lightRef.range = 0f;
            lightRef.intensity = 0f;
        } else {
            playingAnimation = true;
            StartCoroutine(DwellAnimation());
        }
    }

    public IEnumerator MonitorCursor() {
        yield return new WaitForSeconds(0.1f);
        Vector3 toAdd = cursorRef.transform.position;
        runningTotal += toAdd;
        mostRecentPositions.Enqueue(toAdd);
        Vector3 takeOff = new Vector3(0,0,0);
        if (mostRecentPositions.Count > 4) {
            takeOff = mostRecentPositions.Dequeue();
            runningTotal -= takeOff;
        }
        

        currAvg = runningTotal/4;
        StartCoroutine(MonitorCursor());
    }

    public IEnumerator DwellAnimation() {
        yield return new WaitForSeconds(0.05f);
        if (lightRef.range < 0.2f && lightRef.intensity < 100f) {
            lightRef.range += 0.01f;
            lightRef.intensity += 5f;
            StartCoroutine(DwellAnimation());
        }
    }
}
