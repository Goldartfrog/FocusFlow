
//========= Copyright 2018, HTC Corporation. All rights reserved. ===========
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEditor;


namespace ViveSR
{
    namespace anipal
    {
        namespace Eye
        {
            public class EyeTracker : MonoBehaviour
            {
                private GazeData gazeData = new GazeData();
                [SerializeField] private LineRenderer CombinedGazeRayRenderer;

                private static EyeData_v2 eyeData = new EyeData_v2();
                private bool eye_callback_registered = false;

                private float time;
                private string folderPath;

                public bool useProjection;
                public bool useGazeInteraction;
                public bool useLineRenderer;
                
                private string participantId;

                private void Start()
                {
                    time = 0;
                    participantId = File.ReadAllText(System.IO.Path.Combine("Assets/Data", "participantID.txt"), Encoding.UTF8);
                    /* Build the folder for data recording (clear if exists) */
                    string folderName = SceneManager.GetActiveScene().name;
                    folderPath = System.IO.Path.Combine("Assets/Data", folderName, participantId);
                    if (System.IO.Directory.Exists(folderPath))
                    {
                        //Debug.Log("CHANGE THE PARTICIPANTID!!!!!!!!!!!!!!!!!!");
                        //EditorApplication.ExitPlaymode();

                        System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(folderPath);
                        di.Delete(true);
                    }
                    System.IO.Directory.CreateDirectory(folderPath);

                    /* Make sure eye-tracking is enabled */
                    if (!SRanipal_Eye_Framework.Instance.EnableEye)
                    {
                        enabled = false;
                        return;
                    }

                    /* Make sure the gaze ray renderer is initialized */
                    Assert.IsNotNull(CombinedGazeRayRenderer);

                    
                }

                private void Update()
                {
                    /* Update the time */
                    time += Time.deltaTime;

                    /* Return directly if eye-tracking is not working */
                    if (SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.WORKING &&
                        SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.NOT_SUPPORT) return;
                    
                    /* Update eye data callback status */
                    if (SRanipal_Eye_Framework.Instance.EnableEyeDataCallback == true && eye_callback_registered == false)
                    {
                        SRanipal_Eye_v2.WrapperRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye_v2.CallbackBasic)EyeCallback));
                        eye_callback_registered = true;
                    }
                    else if (SRanipal_Eye_Framework.Instance.EnableEyeDataCallback == false && eye_callback_registered == true)
                    {
                        SRanipal_Eye_v2.WrapperUnRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye_v2.CallbackBasic)EyeCallback));
                        eye_callback_registered = false;
                    }

                    /* Initialize the gaze data */
                    Vector3 GazeOriginCombinedLocal = Vector3.zero, GazeOriginLeftLocal = Vector3.zero, GazeOriginRightLocal = Vector3.zero;
                    Vector3 GazeDirectionCombinedLocal = Vector3.forward, GazeDirectionLeftLocal = Vector3.forward, GazeDirectionRightLocal = Vector3.forward;

                    /* Get gaze parameters, default: 0-vector */
                    if (eye_callback_registered)
                    {
                        bool CombinedGazeAccess = SRanipal_Eye_v2.GetGazeRay(GazeIndex.COMBINE, out GazeOriginCombinedLocal, out GazeDirectionCombinedLocal, eyeData);
                        bool LeftGazeAccess = SRanipal_Eye_v2.GetGazeRay(GazeIndex.LEFT, out GazeOriginLeftLocal, out GazeDirectionLeftLocal, eyeData);
                        bool RightGazeAccess = SRanipal_Eye_v2.GetGazeRay(GazeIndex.RIGHT, out GazeOriginRightLocal, out GazeDirectionRightLocal, eyeData);
                        if(!CombinedGazeAccess && !LeftGazeAccess && !RightGazeAccess)
                        {
                            // Debug.Log("Not Tracking Eye Movement!");
                            // Set default gaze origin and direction as (0, 0, 0) if there is no valid gaze
                            GazeOriginCombinedLocal = Vector3.zero;
                            GazeOriginLeftLocal = Vector3.zero;
                            GazeOriginRightLocal = Vector3.zero;
                            GazeDirectionCombinedLocal = Vector3.zero;
                            GazeDirectionLeftLocal = Vector3.zero;
                            GazeDirectionRightLocal = Vector3.zero;
                        }
                    }
                    else
                    {
                        bool CombinedGazeAccess = SRanipal_Eye_v2.GetGazeRay(GazeIndex.COMBINE, out GazeOriginCombinedLocal, out GazeDirectionCombinedLocal);
                        bool LeftGazeAccess = SRanipal_Eye_v2.GetGazeRay(GazeIndex.LEFT, out GazeOriginLeftLocal, out GazeDirectionLeftLocal);
                        bool RightGazeAccess = SRanipal_Eye_v2.GetGazeRay(GazeIndex.RIGHT, out GazeOriginRightLocal, out GazeDirectionRightLocal);
                        if (!CombinedGazeAccess && !LeftGazeAccess && !RightGazeAccess)
                        {
                            // Debug.Log("Not Tracking Eye Movement!");
                            // Set default gaze origin and direction as (0, 0, 0) if there is no valid gaze
                            GazeOriginCombinedLocal = Vector3.zero;
                            GazeOriginLeftLocal = Vector3.zero;
                            GazeOriginRightLocal = Vector3.zero;
                            GazeDirectionCombinedLocal = Vector3.zero;
                            GazeDirectionLeftLocal = Vector3.zero;
                            GazeDirectionRightLocal = Vector3.zero;
                        }
                    }

                    /* Initialization */
                    /*Vector3 HeadOrigin = Camera.main.transform.position;*/
                    Vector3 HeadOrigin = Camera.main.transform.position - Camera.main.transform.up * 0.02f;
                    Vector3 HeadDirection = Camera.main.transform.TransformDirection(Vector3.forward);
                    Vector3 GazeDirection = HeadDirection;

                    // ------------Depth implementation (Projection)------------

                    /* Project to xz-plane */
                    Vector3 GazeOriginCombinedProjXZLocal = new Vector3(GazeOriginCombinedLocal.x, 0, GazeOriginCombinedLocal.z);
                    Vector3 GazeOriginLeftProjXZLocal = new Vector3(GazeOriginLeftLocal.x, 0, GazeOriginLeftLocal.z);
                    Vector3 GazeOriginRightProjXZLocal = new Vector3(GazeOriginRightLocal.x, 0, GazeOriginRightLocal.z);
                    Vector3 GazeDirectionCombinedProjXZLocal = Vector3.ProjectOnPlane(GazeDirectionCombinedLocal, Vector3.up);
                    Vector3 GazeDirectionLeftProjXZLocal = Vector3.ProjectOnPlane(GazeDirectionLeftLocal, Vector3.up);
                    Vector3 GazeDirectionRightProjXZLocal = Vector3.ProjectOnPlane(GazeDirectionRightLocal, Vector3.up);
                    float GazeAngleYLeft = Vector3.Angle(GazeDirectionLeftLocal, GazeDirectionLeftProjXZLocal);
                    float GazeAngleYRight = Vector3.Angle(GazeDirectionRightLocal, GazeDirectionRightProjXZLocal);
                    float GazeAngleYCombined = Vector3.Angle(GazeDirectionCombinedLocal, GazeDirectionCombinedProjXZLocal);

                    /* Project to yz-plane */
                    Vector3 GazeDirectionCombinedProjYZLocal = Vector3.ProjectOnPlane(GazeDirectionCombinedLocal, Vector3.right);
                    Vector3 GazeDirectionLeftProjYZLocal = Vector3.ProjectOnPlane(GazeDirectionLeftLocal, Vector3.right);
                    Vector3 GazeDirectionRightProjYZLocal = Vector3.ProjectOnPlane(GazeDirectionRightLocal, Vector3.right);
                    float GazeAngleXLeft = Vector3.Angle(GazeDirectionLeftLocal, GazeDirectionLeftProjYZLocal);
                    float GazeAngleXRight = Vector3.Angle(GazeDirectionRightLocal, GazeDirectionRightProjYZLocal);
                    float GazeAngleXCombined = Vector3.Angle(GazeDirectionCombinedLocal, GazeDirectionCombinedProjYZLocal);

                    /* Convert to world coordinates */
                    Vector3 GazeOriginCombined = Camera.main.transform.TransformPoint(GazeOriginCombinedLocal);
                    Vector3 GazeOriginLeft = Camera.main.transform.TransformPoint(GazeOriginLeftLocal);
                    Vector3 GazeOriginRight = Camera.main.transform.TransformPoint(GazeOriginRightLocal);
                    Vector3 GazeDirectionCombined = Camera.main.transform.TransformDirection(GazeDirectionCombinedLocal);
                    Vector3 GazeDirectionLeft = Camera.main.transform.TransformDirection(GazeDirectionLeftLocal);
                    Vector3 GazeDirectionRight = Camera.main.transform.TransformDirection(GazeDirectionRightLocal);
                    Vector3 GazeDirectionCombinedProj = Camera.main.transform.TransformDirection(GazeDirectionCombinedProjXZLocal);
                    Vector3 GazeDirectionLeftProj = Camera.main.transform.TransformDirection(GazeDirectionLeftProjXZLocal);
                    Vector3 GazeDirectionRightProj = Camera.main.transform.TransformDirection(GazeDirectionRightProjXZLocal);
                    

                    /* Find the intersection */
                    float x_1 = GazeOriginLeftProjXZLocal.x, z_1 = GazeOriginLeftProjXZLocal.z;
                    float x_2 = GazeOriginRightProjXZLocal.x, z_2 = GazeOriginRightProjXZLocal.z;
                    float k_1 = 0.0f, k_2 = 0.0f, b_1 = 0.0f, b_2 = 0.0f, x = 0.0f, z = 0.0f;
                    float ratio = 1f;
                    if (GazeDirectionLeftProjXZLocal.x != 0 && GazeDirectionRightProjXZLocal.x != 0)
                    {
                        k_1 = GazeDirectionLeftProjXZLocal.z / GazeDirectionLeftProjXZLocal.x;
                        k_2 = GazeDirectionRightProjXZLocal.z / GazeDirectionRightProjXZLocal.x;
                        b_1 = z_1 - k_1 * x_1;
                        b_2 = z_2 - k_2 * x_2;
                        x = (b_2 - b_1) / (k_1 - k_2);
                        z = (k_1 * b_2 - k_2 * b_1) / (k_1 - k_2);
                        ratio = (z - z_1) / GazeDirectionLeftProjXZLocal.z;
                    }
                    Vector3 GazeCrossProjLocal = new Vector3(x, 0, z);
                    Vector3 GazeCrossProj = Camera.main.transform.TransformDirection(GazeCrossProjLocal).normalized;
                    float depth1 = GazeCrossProjLocal.z;

                    // ------------Depth implementation (Vector)------------

                    Vector3 p1 = GazeOriginLeftLocal, p2 = p1 + GazeDirectionLeftLocal, p3 = GazeOriginRightLocal, p4 = p3 + GazeDirectionRightLocal;
                    Vector3 r = GazeDirectionLeftLocal, s = GazeDirectionRightLocal, q = p1 - p3;
                    float t = (Vector3.Dot(q, s) * Vector3.Dot(s, r) - Vector3.Dot(q, r) * Vector3.Dot(s, s)) / (Vector3.Dot(r, r) * Vector3.Dot(s, s) - Vector3.Dot(s, r) * Vector3.Dot(s, r));
                    float v = (Vector3.Dot(q, s) + t * Vector3.Dot(s, r)) / Vector3.Dot(s, s);
                    Vector3 pa = t * r, pb = v * s, pc = (pa + pb) / 2;
                    float depth2 = pc.z;

                    // /* Record all data */
                    // bool flag = GameObject.Find("TestControl").GetComponent<TestControl>().WriteFlag;
                    // if (flag)
                    // {
                    //     using (StreamWriter sw = File.AppendText(System.IO.Path.Combine(folderPath, "gazedata.txt")))
                    //     {
                    //         sw.WriteLine("{0}, {1}", time, flag);
                    //         sw.WriteLine("'HeadOrigin':{0}, {1}, {2}", HeadOrigin.x, HeadOrigin.y, HeadOrigin.z);
                    //         sw.WriteLine("'HeadDirection':{0}, {1}, {2}", HeadDirection.x, HeadDirection.y, HeadDirection.z);

                    //         sw.WriteLine("'GazeOriginCombinedLocal':{0}, {1}, {2}", GazeOriginCombinedLocal.x, GazeOriginCombinedLocal.y, GazeOriginCombinedLocal.z);
                    //         sw.WriteLine("'GazeOriginCombined':{0}, {1}, {2}", GazeOriginCombined.x, GazeOriginCombined.y, GazeOriginCombined.z);
                    //         sw.WriteLine("'GazeOriginLeftLocal':{0}, {1}, {2}", GazeOriginLeftLocal.x, GazeOriginLeftLocal.y, GazeOriginLeftLocal.z);
                    //         sw.WriteLine("'GazeOriginLeft':{0}, {1}, {2}", GazeOriginLeft.x, GazeOriginLeft.y, GazeOriginLeft.z);
                    //         sw.WriteLine("'GazeOriginRightLocal':{0}, {1}, {2}", GazeOriginRightLocal.x, GazeOriginRightLocal.y, GazeOriginRightLocal.z);
                    //         sw.WriteLine("'GazeOriginRight':{0}, {1}, {2}", GazeOriginRight.x, GazeOriginRight.y, GazeOriginRight.z);

                    //         sw.WriteLine("'GazeDirectionCombinedLocal':{0}, {1}, {2}, {3}", GazeDirectionCombinedLocal.x, GazeDirectionCombinedLocal.y, GazeDirectionCombinedLocal.z, GazeDirectionCombinedLocal.magnitude);
                    //         sw.WriteLine("'GazeDirectionCombined':{0}, {1}, {2}, {3}", GazeDirectionCombined.x, GazeDirectionCombined.y, GazeDirectionCombined.z, GazeDirectionCombined.magnitude);
                    //         sw.WriteLine("'GazeDirectionLeftLocal':{0}, {1}, {2}, {3}", GazeDirectionLeftLocal.x, GazeDirectionLeftLocal.y, GazeDirectionLeftLocal.z, GazeDirectionLeftLocal.magnitude);
                    //         sw.WriteLine("'GazeDirectionLeft':{0}, {1}, {2}, {3}", GazeDirectionLeft.x, GazeDirectionLeft.y, GazeDirectionLeft.z, GazeDirectionLeft.magnitude);
                    //         sw.WriteLine("'GazeDirectionRightLocal':{0}, {1}, {2}, {3}", GazeDirectionRightLocal.x, GazeDirectionRightLocal.y, GazeDirectionRightLocal.z, GazeDirectionRightLocal.magnitude);
                    //         sw.WriteLine("'GazeDirectionRight':{0}, {1}, {2}, {3}", GazeDirectionRight.x, GazeDirectionRight.y, GazeDirectionRight.z, GazeDirectionRight.magnitude);

                    //         sw.WriteLine("'GazeOriginCombinedProjLocal':{0}, {1}, {2}", GazeOriginCombinedProjXZLocal.x, GazeOriginCombinedProjXZLocal.y, GazeOriginCombinedProjXZLocal.z);
                    //         sw.WriteLine("'GazeOriginLeftProjLocal':{0}, {1}, {2}", GazeOriginLeftProjXZLocal.x, GazeOriginLeftProjXZLocal.y, GazeOriginLeftProjXZLocal.z);
                    //         sw.WriteLine("'GazeOriginRightProjLocal':{0}, {1}, {2}", GazeOriginRightProjXZLocal.x, GazeOriginRightProjXZLocal.y, GazeOriginRightProjXZLocal.z);

                    //         sw.WriteLine("'GazeDirectionCombinedProjLocal':{0}, {1}, {2}, {3}", GazeDirectionCombinedProjXZLocal.x, GazeDirectionCombinedProjXZLocal.y, GazeDirectionCombinedProjXZLocal.z, GazeDirectionCombinedProjXZLocal.magnitude);
                    //         sw.WriteLine("'GazeDirectionLeftProjLocal':{0}, {1}, {2}, {3}", GazeDirectionLeftProjXZLocal.x, GazeDirectionLeftProjXZLocal.y, GazeDirectionLeftProjXZLocal.z, GazeDirectionLeftProjXZLocal.magnitude);
                    //         sw.WriteLine("'GazeDirectionRightProjLocal':{0}, {1}, {2}, {3}", GazeDirectionRightProjXZLocal.x, GazeDirectionRightProjXZLocal.y, GazeDirectionRightProjXZLocal.z, GazeDirectionRightProjXZLocal.magnitude);
                    //         sw.WriteLine("'GazeCrossProjLocal':{0}, {1}, {2}, {3}", GazeCrossProjLocal.x, GazeCrossProjLocal.y, GazeCrossProjLocal.z, GazeCrossProjLocal.magnitude);
                    //         sw.WriteLine("'GazeCrossProj':{0}, {1}, {2}, {3}", GazeCrossProj.x, GazeCrossProj.y, GazeCrossProj.z, GazeCrossProj.magnitude);
                    //         sw.WriteLine("'k_1':{0}, 'k_2':{1}, 'b_1':{2}, 'b_2':{3}, 'depth1':{4}, 'depth2':{5}", k_1, k_2, b_1, b_2, depth1, depth2);
                    //         sw.WriteLine("'GazeAngleYCombined':{0}, 'GazeAngleYLeft':{1}, 'GazeAngleYRight':{2}, ", GazeAngleYCombined, GazeAngleYLeft, GazeAngleYRight);
                    //         sw.WriteLine("'GazeAngleXCombined':{0}, 'GazeAngleXLeft':{1}, 'GazeAngleXRight':{2}, ", GazeAngleXCombined, GazeAngleXLeft, GazeAngleXRight);
                    //         sw.WriteLine("'left x ratio':{0}', 'left z ratio':{1}, 'comb x ratio':{2}', 'comb z ratio':{3}", (x - x_1) / GazeDirectionLeftProjXZLocal.x, (z - z_1) / GazeDirectionLeftProjXZLocal.z, GazeCrossProjLocal.x / GazeDirectionCombinedProjXZLocal.x, GazeCrossProjLocal.z / GazeDirectionCombinedProjXZLocal.z);
                    //         sw.WriteLine("");
                    //     }
                    // }

                    /* Use gaze or not */
                    if(useGazeInteraction)
                    {
                        GazeDirection = GazeDirectionCombined;
                    }

                    /* Draw the gaze ray */
                    if(useLineRenderer)
                    {
                        if(ratio > 0){
                            CombinedGazeRayRenderer.enabled = true;
                            CombinedGazeRayRenderer.SetPosition(0, HeadOrigin);
                            CombinedGazeRayRenderer.SetPosition(1, GazeOriginCombined + GazeDirectionCombined * ratio);
                        }
                    }
                    else{
                        this.transform.GetComponent<LineRenderer>().enabled = false;
                    }

                    /* Send gaze parameters */
                    gazeData.HeadOrigin = HeadOrigin;
                    gazeData.HeadDirection = HeadDirection;
                    gazeData.GazeOriginCombined = GazeOriginCombined;
                    gazeData.GazeDirectionCombined = GazeDirectionCombined;
                    gazeData.GazeDirectionLeft = GazeDirectionLeft;
                    gazeData.GazeDirectionRight = GazeDirectionRight;
                    if (useProjection)
                    {
                        gazeData.Depth = depth1;
                        gazeData.Depth2 = depth2;
                    }
                    else
                    {
                        gazeData.Depth = depth2;
                        gazeData.Depth2 = depth1;
                    }
                    gazeData.GazeAngleX = GazeAngleXCombined;
                    gazeData.GazeAngleY = GazeAngleYCombined;

                    GameObject.Find("Interaction").SendMessage("GetGazeParameter", gazeData);
                }

                private void Release()
                {
                    if (eye_callback_registered == true)
                    {
                        SRanipal_Eye_v2.WrapperUnRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye_v2.CallbackBasic)EyeCallback));
                        eye_callback_registered = false;
                    }
                }
                private static void EyeCallback(ref EyeData_v2 eye_data)
                {
                    eyeData = eye_data;
                }
            }
        }
    }
}
