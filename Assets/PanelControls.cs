using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelControls : MonoBehaviour
{

    //SAM's Added variables:
    private bool isLookingAtKeyboard; //Determines if we can gaze-actuate
    [SerializeField]
    private GameObject panelRef;
    [SerializeField]
    private Material transparentMat;
    [SerializeField]
    private Material focusedMat;
    private bool panelActive;
    private bool waitingOnConfirm;

    [SerializeField]
    private KeyboardTextSystemIntroduction keyboardRef;

    [SerializeField]
    private EnterScriptTutorial ESTRef;
    //public InteractionEyeTracker eyeTrackerRef;

    public EyeDepthTracker eyeTrackerRef;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public void Update()
    {
        //CheckIfLookingAtKeyboard();
        // Debug.Log(depth1);
        // //if (isLookingAtKeyboard) {
        // Debug.Log(depth1);
        float currDepth = eyeTrackerRef.smoothedGazeDepth;
        Debug.Log("Curr depth: " + currDepth);
        if (currDepth < 0.5f) {
            Debug.Log("HERE");
            panelActive = true;
            panelRef.GetComponent<Renderer>().material = focusedMat;
            if (!waitingOnConfirm) {
                waitingOnConfirm = true;
                StartCoroutine(ConfirmSpace());
            }
        } else {
            panelActive = false;
            panelRef.GetComponent<Renderer>().material = transparentMat;
        }
        //}
        IEnumerator ConfirmSpace() {

            yield return new WaitForSeconds(1.5f);
            if (panelActive) {
                //call space
                Debug.Log("calling space");
                keyboardRef.RecieveEnter();
                ESTRef.TogglePressedAccessor();
            }
            waitingOnConfirm = false;
            yield return null;
        }    
    }


}
