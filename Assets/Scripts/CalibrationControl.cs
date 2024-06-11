using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalibrationControl : MonoBehaviour
{
    public bool StartFlag;

    // Start is called before the first frame update
    void Start()
    {
        StartFlag = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartFlag = true;
        }
    }
}
