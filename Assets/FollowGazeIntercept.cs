using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowGazeIntercept : MonoBehaviour
{
     public bool UseHighlighter;
     
     public InteractionEyeTracker EyePos;
     public GameObject pointLightRef;
     public Vector3 currIntersectionPoint;

     public Queue<Vector3> posList = new Queue<Vector3>();
     public int maxSizeList;
     private Vector3 runningTotal = new Vector3(0,0,0);
     private bool isColliding;
     public GameObject cameraRef;

     public GameObject planeRef;

     public bool seeingKeyboard;

     public GameObject Debug1;
     public GameObject Debug2;


     public Vector3 currHitPoint;

    //  public Vector3 originOffset;
    //  public Vector3 StartPos;
    
    // Start is called before the first frame update
    void Start()
    {
        // if (UseHighlighter) {
        //     StartCoroutine(Smoother());
        // }
        Ray ray = new Ray(cameraRef.transform.position, EyePos.gazeLocation);
        //Debug.DrawRay(cameraRef.transform.position, EyePos.gazeLocation, Color.green);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100f)) { 
            if (hit.collider.gameObject == planeRef) {
               currHitPoint = hit.point;

            }
            
        }
        
        //StartPos = cameraRef.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //originOffset = cameraRef.transform.position - StartPos;
        Ray ray = new Ray(cameraRef.transform.position, EyePos.gazeLocation);
        //Debug.DrawRay(cameraRef.transform.position, EyePos.gazeLocation, Color.green);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100f)) { 
            if (hit.collider.gameObject == planeRef) {
               currHitPoint = hit.point;

            }
            
        }
    }

    public IEnumerator Smoother() {
        yield return new WaitForSeconds(0.05f);
        Ray ray = new Ray(cameraRef.transform.position, EyePos.gazeLocation);
        //Debug.DrawRay(cameraRef.transform.position, EyePos.gazeLocation, Color.green);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100f)) { 
            if (hit.collider.gameObject == planeRef) {
                runningTotal += hit.point;
                posList.Enqueue(hit.point);
                Vector3 off = new Vector3(0, 0, 0);
                if (posList.Count > maxSizeList)
                {
                    off = posList.Dequeue();
                }
                runningTotal -= off;

                pointLightRef.transform.position = runningTotal / posList.Count;
                

            }
            
        }
        StartCoroutine(Smoother());
        
    }
}
