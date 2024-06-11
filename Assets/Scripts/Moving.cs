using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Moving : MonoBehaviour
{                          
    public float AX;
    public float AY;
    public float AZ;
    public float fZ;
    public float phiZ;
    float time;

    // Start is called before the first frame update
    void Start()
    {
        time = 0;
        this.transform.position = new Vector3(AX, AY, AZ * Mathf.Cos(10 * Mathf.PI * fZ * time + phiZ) + AZ + 0.3f);
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameObject.Find("TestControl").GetComponent<TestControlDetection>().StartFlag)
        {
            return;
        }
        time += Time.deltaTime;
        Vector3 currPos = this.transform.position, nextPos;
        nextPos.x = AX;
        nextPos.y = AY;
        nextPos.z = AZ * Mathf.Cos(Mathf.PI * fZ * time + phiZ) + AZ + 0.3f;
        this.transform.Translate(nextPos - currPos);
    }
}
