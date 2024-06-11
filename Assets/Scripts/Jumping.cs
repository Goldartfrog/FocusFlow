using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Jumping : MonoBehaviour
{
    public float objDepth;
    public float panelDepth;
    public float maxIter;
    public float stageTime;

    float switchTime;
    float waitTime;
    int iter;
    int status;
    float baseRadius;
    Vector3 panelScale;
    Vector3 objScale;
    Vector3 panelPos;
    Vector3 objPos;

    public int GetStatus()
    {
        return status;
    }

    // Start is called before the first frame update
    void Start()
    {
        switchTime = 0f;
        waitTime = 0f;
        iter = 1;
        status = 0;
        baseRadius = 0.2f;

        panelScale = new Vector3(baseRadius, baseRadius, 0.05f);
        /*objScale = new Vector3(baseRadius * objDepth / panelDepth, baseRadius * objDepth / panelDepth, 0.05f);*/
        objScale = panelScale;
        panelPos = new Vector3(0, 1.1f, panelDepth);
        objPos = new Vector3(0, 1.1f, objDepth);

        this.transform.localScale = objScale;
        this.transform.position = objPos;
        Color c = this.GetComponent<MeshRenderer>().material.color;
        c.a = 0.3f;
        this.GetComponent<MeshRenderer>().material.color = c;
    }

    // Update is called once per frame
    void Update()
    {
        if (iter > maxIter)
        {
            EditorApplication.ExitPlaymode();
            return;
        }
        if (!GameObject.Find("Control").GetComponent<CalibrationControl>().StartFlag)
        {
            return;
        }

        if(status == 0)
        {
            waitTime += Time.deltaTime;
        }
        else
        {
            switchTime += Time.deltaTime;
        }

        if(switchTime >= stageTime)
        {
            if(status == 1)
            {
                status = 2;
                this.transform.localScale = panelScale;
                this.transform.position = panelPos;
                switchTime = 0f;
            }
            else
            {
                iter++;
                status = 1;
                this.transform.localScale = objScale;
                this.transform.position = objPos;
                switchTime = 0f;
            }
        }
        else if(waitTime >= 3f)
        {
            status = 1;
            waitTime = 0f;
            Color c = this.GetComponent<MeshRenderer>().material.color;
            c.a = 1f;
            this.GetComponent<MeshRenderer>().material.color = c;
        }
    }
}
