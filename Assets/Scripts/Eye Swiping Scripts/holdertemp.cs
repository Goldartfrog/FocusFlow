using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class holdertemp : MonoBehaviour
{
    [SerializeField] private InteractionEyeTracker eyeData;
    private int layerMask;
    private Vector3 lasthit = Vector3.zero;
    // Start is called before the first frame update
    void Start()
    {
        layerMask = 1 << 15;
        string st = "";
        foreach (Transform child in gameObject.transform.GetComponentInChildren<Transform>())
        {
            st += "(" + child.position.x.ToString("F4") + "," + child.position.y.ToString("F4") + ")";
        }
        Debug.Log(st);
    }

    // Update is called once per frame
    void Update()
    {
        //RaycastHit hit;
        //Physics.Raycast(eyeData.gazeOrigin, eyeData.smoothGaze - eyeData.gazeOrigin, out hit, float.MaxValue, layerMask);

        ////Debug.Log(hit.point);
        //if (hit.point.magnitude > 0.2)
        //{
        //    lasthit = hit.point;
        //    transform.position = Vector3.MoveTowards(transform.position, hit.point, 0.1f);
        //}
        //else
        //{
        //    transform.position = Vector3.MoveTowards(transform.position, lasthit, 0.1f);
        //}


    }
}
