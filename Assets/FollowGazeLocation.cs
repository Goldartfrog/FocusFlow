using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowGazeLocation : MonoBehaviour
{
    
     public InteractionEyeTracker EyePos;
     public GameObject particleRef;
     public Vector3 currLocation;

     public Queue<Vector3> posList = new Queue<Vector3>();
     public int maxSizeList;
     private Vector3 runningTotal = new Vector3(0,0,0);
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Smoother());
    }

    // Update is called once per frame
    void Update()
    {
        //this.transform.position = EyePos.gazeLocation;
        //EyePos.worldPosition gets the user's location in the world.lliding = false;
        if (posList.Count != 0) {
            particleRef.transform.position = runningTotal / posList.Count;
        }
    }

    public IEnumerator Smoother() {
        yield return new WaitForSeconds(0.05f);
 
            runningTotal += EyePos.gazeLocation;
            posList.Enqueue(EyePos.gazeLocation);
             Vector3 off = new Vector3(0, 0, 0);
            if (posList.Count > maxSizeList)
            {
                off = posList.Dequeue();
            }
            runningTotal -= off;

        StartCoroutine(Smoother());
    }
}
