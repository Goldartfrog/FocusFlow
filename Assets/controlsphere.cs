using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using UnityEditor;
using UnityEngine;

public class controlsphere : MonoBehaviour
{
    [SerializeField] private GameObject interactionObject;
    [SerializeField] private GameObject vrCamera;
    [SerializeField] private int windowSize;
    [SerializeField] private float speed = 10;
    [SerializeField] private float nearLimit = 1;
    [SerializeField] private float farLimit = 15;
    private Interaction interaction;
    private GazeData gazeData;
    private Transform cameraTransform;
    private Queue<float> gazeDistances;
    private Queue<Vector3> gazeDirections;
    // Start is called before the first frame update
    void Start()
    {
        interaction = interactionObject.GetComponent<Interaction>();
        gazeData = interaction.GetGazeData();
        cameraTransform = vrCamera.GetComponent<Transform>();
        gazeDistances = new Queue<float>();
        gazeDirections = new Queue<Vector3>();
    }

    // Update is called once per frame
    void Update()
    {
        gazeData = interaction.GetGazeData();
        if (gazeData.Depth > nearLimit && gazeData.Depth < farLimit)
        {
            gazeDistances.Enqueue(gazeData.Depth);
            gazeDirections.Enqueue(gazeData.GazeDirectionCombined);
            if (gazeDistances.Count > windowSize)
            {
                gazeDistances.Dequeue();
                gazeDirections.Dequeue();
            }
        }
        
        //gazeDistances.Add(gazeData.Depth);
        
        //Debug.Log(gazeData.Depth);
        Vector3 averageGaze = new Vector3(0, 0, 0);
        foreach (var direction in gazeDirections)
        {
            averageGaze += direction;
        }
        averageGaze.Normalize();
        var step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, gazeData.GazeOriginCombined + averageGaze * gazeDistances.Average(), step);
        //transform.position = gazeData.GazeOriginCombined + (gazeDistances.Average() * averageGaze);
        
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("hit something");
        if (collision.gameObject.name == "testcube")
        {
            Debug.Log("hit the plane");
        }
    }
}
