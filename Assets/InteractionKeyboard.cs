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

    protected Vector3 GazeDirectionSum = new Vector3(0.0f, 0.0f, 0.0f);
    protected Vector3 GazeDirectionMean = new Vector3(0.0f, 0.0f, 0.0f);
    protected Queue<Vector3> PrevGaze = new Queue<Vector3>();

    protected float DepthSum = 0.0f;
    protected float DepthMean = 0.0f;
    protected Queue<float> PrevDepth = new Queue<float>();

    protected int GazeWindowSize = 20;

    [SerializeField] private GameObject blueSphere;
    [SerializeField] private GameObject redSphere;
    protected List<GameObject> gazeObjects = new List<GameObject>();
    [SerializeField] private GameObject keyButtons;
    private int layerMask = 1 << 15;

    private float timeOnKey = 0.0f;
    [SerializeField] private float pressTime = 1.0f;
    private Transform previousHit;

    [SerializeField] AudioSource typeSound;
    [SerializeField] private GameObject typingField;


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
        /* Record current gaze */
        PrevGaze.Enqueue(gazeData.GazeDirectionCombined);
        GazeDirectionSum += gazeData.GazeDirectionCombined;
        if (PrevGaze.Count > GazeWindowSize)
        {
            Vector3 eraseDirection = PrevGaze.Dequeue();
            GazeDirectionSum -= eraseDirection;
        }
        GazeDirectionMean = GazeDirectionSum.normalized;

        PrevDepth.Enqueue(gazeData.Depth);
        DepthSum += gazeData.Depth;
        if (PrevDepth.Count > GazeWindowSize)
        {
            float eraseDepth = PrevDepth.Dequeue();
            DepthSum -= eraseDepth;
        }
        DepthMean = DepthSum / PrevDepth.Count;

        return true;
    }

    // Start is called before the first frame update
    void Start()
    {
        gazeData = new GazeData();
        string keys = "QWERTYUIOPASDFGHJKLZXCVBNM";
        for (int i = 0; i < keyButtons.transform.childCount; i++)
        {
            Transform child = keyButtons.transform.GetChild(i);
            child.GetComponent<Keyboard>().SetKey(keys[i]);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        bool update = UpdateGaze();
        if (update)
        {
            RaycastHit hit;
            Physics.Raycast(gazeData.GazeOriginCombined, gazeData.GazeDirectionCombined, out hit, float.MaxValue, layerMask);


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

            previousHit = hit.transform;




            //if (gazeData.Depth > interactionThreshold)
            //{
            //    gazeObjects.Add(Instantiate(redSphere, hit.point, Quaternion.identity));
            //}
            //else
            //{
            //    gazeObjects.Add(Instantiate(blueSphere, hit.point, Quaternion.identity));
            //}
            //Coroutine myCoroutine;
            //if (hit.transform != null)
            //{
            //    myCoroutine = StartCoroutine(ChangeColor(hit.transform));
            //}



            if (gazeObjects.Count > howMany)
            {
                Destroy(gazeObjects[0]);
                gazeObjects.RemoveAt(0);
            }
        }

    }

    private void TypeKey(Transform transform)
    {
        char k = transform.GetComponent<Keyboard>().GetKey();
        typeSound.Play();
        typingField.GetComponent<TextMeshProUGUI>().text += k;

    }

}
