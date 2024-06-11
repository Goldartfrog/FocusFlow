using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GazeData
{
    public bool LeftGaze;
    public bool RightGaze;
    public Vector3 HeadOrigin;
    public Vector3 HeadDirection;
    public Vector3 GazeOriginCombined;
    public Vector3 GazeDirectionCombined;
    public Vector3 GazeDirectionLeft;
    public Vector3 GazeDirectionRight;
    public Vector3 GazeOriginLeftProjXZLocal;
    public Vector3 GazeOriginRightProjXZLocal;
    public Vector3 GazeDirectionLeftProjXZLocal;
    public Vector3 GazeDirectionRightProjXZLocal;
    public float Depth;
    public float Depth2;
    public float GazeAngleX;
    public float GazeAngleY;
    
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
