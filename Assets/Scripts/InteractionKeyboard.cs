using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;

public class InteractionKeyboard : MonoBehaviour
{
    protected GazeData gazeData;



    protected int blinkBuffer;
    [SerializeField] protected int blinkThres = 20;
    [SerializeField] protected float nearThres = 0.2f;
    [SerializeField] protected float farThres = 10;
    [SerializeField] protected float interactionThreshold = 5;
    [SerializeField] protected int howMany = 10;

    /* smoothing variables */
    protected Vector3 GazeDirectionSum = new Vector3(0.0f, 0.0f, 0.0f);
    protected Vector3 GazeDirectionMean = new Vector3(0.0f, 0.0f, 0.0f);
    protected Queue<Vector3> PrevGaze = new Queue<Vector3>();

    protected float DepthSum = 0.0f;
    protected float DepthMean = 0.0f;
    protected Queue<float> PrevDepth = new Queue<float>();

    private Vector3 GazeOriginSum = new Vector3(0.0f, 0.0f, 0.0f);
    private Vector3 GazeOriginMean = new Vector3(0.0f, 0.0f, 0.0f);
    private Queue<Vector3> PrevGazeOrigin = new Queue<Vector3>();

    protected float OriginSum = 0.0f;
    protected float OriginMean = 0.0f;
    protected Queue<float> PrevOrigin = new Queue<float>();

    [SerializeField] private int GazeWindowSize = 20;

    /* spawning spheres on gaze variables */
    [SerializeField] private GameObject blueSphere;
    [SerializeField] private GameObject redSphere;
    protected List<GameObject> gazeObjects = new List<GameObject>();

    /* keyboard variables*/
    [SerializeField] private GameObject keyButtons;
    private int layerMask = 1 << 15;
    private float timeOnKey = 0.0f;
    [SerializeField] private float pressTime = 1.0f;
    private Transform previousHit;

    private float bufferTime = 0.0f;
    [SerializeField] private float maxBufferTime = 0.4f;

    [SerializeField] AudioSource typeSound;
    [SerializeField] private GameObject typingField;

    [SerializeField] private GameObject halfGazeObject;

    [SerializeField] private GameObject vrCamera;
    private Vector3 cameraPos;

    [SerializeField] private GameObject grabkey;
    [SerializeField] private GameObject grabline;
    [SerializeField] private GameObject grabPosition;

    [SerializeField] private float scaleDist = 0.8f;
    private List<GameObject> NearKeys = new List<GameObject>();
    [SerializeField] private Transform NearKeyHolder;

    [SerializeField] private float DownOffsetDist = 0.01f;
    [SerializeField] private float LeftOffsetDist = 0.01f;

    private string folderPath;


    public void GetGazeParameter(GazeData GazeData_)
    {
        gazeData = GazeData_;
    }

    /* Check to see if the gaze is in valid range before updating */
    private bool UpdateGaze()
    {
        if (gazeData.GazeDirectionCombined.magnitude == 0) /* if the user is blinking or the gaze is messed up, don't record */
        {
            blinkBuffer++;
            if (blinkBuffer < blinkThres)
            {
                return false;
            }
        } else
        {
            blinkBuffer = 0;
        }

        if (gazeData.Depth < nearThres || gazeData.Depth > farThres) /* if the user is looking "too far" or "too near" don't record */
        {
            return false;
        }
        Smoothing();

        return true;
    }

    private void Smoothing()
    {

        using (StreamWriter sw = File.AppendText(System.IO.Path.Combine(folderPath, "testRecordDepth.txt")))
        {
            sw.WriteLine("{0}, {1}", Time.time, gazeData.Depth);
        }
        /* Record current gaze */
        PrevGaze.Enqueue(gazeData.GazeDirectionCombined);
        GazeDirectionSum += gazeData.GazeDirectionCombined;
        if (PrevGaze.Count > GazeWindowSize)
        {
            Vector3 eraseDirection = PrevGaze.Dequeue();
            GazeDirectionSum -= eraseDirection;
        }
        GazeDirectionMean = GazeDirectionSum.normalized;
        /* Record depth */
        PrevDepth.Enqueue(gazeData.Depth);
        DepthSum += gazeData.Depth;
        if (PrevDepth.Count > GazeWindowSize)
        {
            float eraseDepth = PrevDepth.Dequeue();
            DepthSum -= eraseDepth;
        }
        DepthMean = DepthSum / PrevDepth.Count;
        /* record and smooth gaze origin */
        PrevGazeOrigin.Enqueue(gazeData.GazeOriginCombined);
        GazeOriginSum += gazeData.GazeOriginCombined;
        if (PrevGazeOrigin.Count > GazeWindowSize)
        {
            Vector3 eraseDirection = PrevGazeOrigin.Dequeue();
            GazeOriginSum -= eraseDirection;
        }
        GazeOriginMean = GazeOriginSum / PrevGazeOrigin.Count;


    }

    // Start is called before the first frame update
    void Start()
    {

        folderPath = System.IO.Path.Combine("Assets/Data", "AidanTestingPullKeyboard");

        gazeData = new GazeData();
        MakeKeyboard();
        SpawnHalfGazeKeyboard();
        cameraPos = vrCamera.transform.position;
        
    }

    // Update is called once per frame
    void Update()
    {
        bool update = UpdateGaze();
        if (update)
        {
            RaycastHit hit;
            //Physics.Raycast(gazeData.GazeOriginCombined, gazeData.GazeDirectionCombined, out hit, float.MaxValue, layerMask);
            Physics.Raycast(gazeData.GazeOriginCombined, GazeDirectionMean, out hit, float.MaxValue, layerMask);
            
            //DualTimeTyping(hit);
            //MoveHalfGazeObject(hit);
            HalfGazeKeyboard();
            PullGazeDwellTyping(hit);

            previousHit = hit.transform;
        }

    }

    private void MakeKeyboard ()
    {
        string keys = "QWERTYUIOPASDFGHJKLZXCVBNM";
        for (int i = 0; i < keyButtons.transform.childCount; i++)
        {
            Transform child = keyButtons.transform.GetChild(i);
            child.GetComponent<Keyboard>().SetKey("" + keys[i]);
        }
    }

    private void TypeKey(Transform transform)
    {
        string k = transform.GetComponent<Keyboard>().GetKey();
        typeSound.Play();
        string t = typingField.GetComponent<TextMeshProUGUI>().text;
        if (k == "back")
        {
            typingField.GetComponent<TextMeshProUGUI>().text = t.Remove(t.Length - 1);
        }
        else
        {
            typingField.GetComponent<TextMeshProUGUI>().text += k;
        }
        

    }

    private void SpawnGazeSpheres(RaycastHit hit)
    {
        if (gazeData.Depth > interactionThreshold)
        {
            gazeObjects.Add(Instantiate(redSphere, hit.point, Quaternion.identity));
        }
        else
        {
            gazeObjects.Add(Instantiate(blueSphere, hit.point, Quaternion.identity));
        }
        if (gazeObjects.Count > howMany)
        {
            Destroy(gazeObjects[0]);
            gazeObjects.RemoveAt(0);
        }
    }

    private void DualTimeTyping(RaycastHit hit)
    {
        if (hit.transform == previousHit)
        {
            timeOnKey += Time.deltaTime;
            if (hit.transform != null)
            {
                hit.transform.GetComponent<MeshRenderer>().material.color = Color.gray;
            }

        }
        else
        {
            timeOnKey = 0;
            if (previousHit != null)
            {
                previousHit.GetComponent<MeshRenderer>().material.color = Color.white;
            }

        }
        if (timeOnKey > pressTime)
        {
            TypeKey(hit.transform);
            timeOnKey = 0;
        }
    }

    private void PullGazeDwellTyping(RaycastHit hit)
    {
        if (hit.transform == previousHit)
        {
            if (hit.transform != null)
            {
                hit.transform.GetComponent<MeshRenderer>().material.color = Color.gray;
            }

        }
        else
        {
            if (previousHit != null)
            {
                previousHit.GetComponent<MeshRenderer>().material.color = Color.white;
            }

        }
        float thresholdDistance = hit.distance / 5;
        if (DepthMean < interactionThreshold && gazeData.Depth < interactionThreshold)
        {
            bufferTime += Time.deltaTime;
            if (bufferTime > maxBufferTime)
            {
                TypeKey(hit.transform);
                bufferTime = 0;
            }
            
            
        }
        if (DepthMean > interactionThreshold)
        {
            bufferTime = 0;
        }
    }

    private void PullGazeTyping()
    {

    }

    private void MoveHalfGazeObject(RaycastHit hit)
    {
        if (hit.transform != null)
        {
            halfGazeObject.transform.position = ((hit.transform.position - gazeData.GazeOriginCombined) / 10) + gazeData.GazeOriginCombined;
            //halfGazeObject.transform.position = ((hit.transform.position - gazeData.GazeOriginCombined) / 10) + gazeData.GazeOriginCombined;
        }
        
    }

    private void HalfGazeKeyboard()
    {

        Vector3 referencePosition = GazeOriginMean;
        
        for (int i = 0; i < NearKeyHolder.childCount; i++)
        {
            Transform NearKey = NearKeyHolder.GetChild(i);
            Transform BackKey = keyButtons.transform.GetChild(i);
            //var pos = child.position - grabPosition.transform.position;
            //var pos = (child.position - referencePosition).normalized * scaleDist;
            var pos = (BackKey.position - referencePosition) / scaleDist;
            //NearKeys[i].gameObject.transform.position = pos.normalized * scaleDist + grabPosition.transform.position;
            var targetPos = pos + referencePosition + Vector3.down * DownOffsetDist + Vector3.forward * LeftOffsetDist;
            NearKey.position = Vector3.MoveTowards(NearKey.position, targetPos, 0.05f);
        }
        //grabline.transform.position = (grabkey.transform.position - grabPosition.transform.position).normalized * scaleDist + grabPosition.transform.position;
    }

    private void SpawnHalfGazeKeyboard()
    {
        string keys = "QWERTYUIOPASDFGHJKLZXCVBNM";
        for (int i = 0; i < keyButtons.transform.childCount; i++)
        {
            Transform child = keyButtons.transform.GetChild(i);
            var pos = child.position - grabPosition.transform.position;
            GameObject obj = Instantiate(grabline, pos, grabline.transform.rotation, NearKeyHolder);
            obj.GetComponent<Keyboard>().SetKey(keys[i] + "");
            //obj.transform.GetChild(0).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = keys[i] + "";
            NearKeys.Add(obj);
        }
    }
}
