using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowGazeIntercept : MonoBehaviour
{
     public InteractionEyeTracker EyePos;
     public GameObject pointLightRef;
     public Vector3 currIntersectionPoint;

     public Queue<Vector3> posList = new Queue<Vector3>();
     public int maxSizeList;
     private Vector3 runningTotal = new Vector3(0,0,0);
     private bool isColliding;
     public GameObject cameraRef;

     public GameObject planeRef;
    
    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(Smoother());
    }

    // Update is called once per frame
    void Update()
    {
        //pointLightRef.transform.position = new Vector3(EyePos.gazeLocation.x, EyePos.gazeLocation.y, planeRef.transform.position.z);
        //this.transform.position = EyePos.gazeLocation;
        //EyePos.worldPosition gets the user's location in the world.
        Ray ray = new Ray(cameraRef.transform.position, EyePos.gazeLocation);
        //Debug.DrawRay(cameraRef.transform.position, EyePos.gazeLocation);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100f)) { 
            if (hit.collider.gameObject == planeRef) {
                pointLightRef.transform.position = hit.point;
                Debug.Log("Sphere location: " + pointLightRef.transform.position + " intersect point: " + hit.point);
            //     currIntersectionPoint = hit.point;
            //     if (posList.Count != 0) {
            //         pointLightRef.transform.position = runningTotal / posList.Count;
            //     }
            //     isColliding = true;
            // } else {
            //     isColliding = false;
            }
            
        } else {
            isColliding = false;
        }
        
    }

    public IEnumerator Smoother() {
        yield return new WaitForSeconds(0.05f);
        Debug.Log("coroutine is running");
        if (isColliding) {
            Debug.Log("got to is colliding");
            runningTotal += currIntersectionPoint;
            posList.Enqueue(currIntersectionPoint);
             Vector3 off = new Vector3(0, 0, 0);
            if (posList.Count > maxSizeList)
            {
                off = posList.Dequeue();
            }
            runningTotal -= off;
        }
        StartCoroutine(Smoother());
    }
}
