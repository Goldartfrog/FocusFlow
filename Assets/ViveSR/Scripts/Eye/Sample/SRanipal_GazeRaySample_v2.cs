//========= Copyright 2018, HTC Corporation. All rights reserved. ===========
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;


namespace ViveSR
{
    namespace anipal
    {
        namespace Eye
        {
            public class SRanipal_GazeRaySample_v2 : MonoBehaviour
            {
                public int LengthOfRay = 25;
                [SerializeField] private LineRenderer GazeRayRenderer;
                private static EyeData_v2 eyeData = new EyeData_v2();
                private bool eye_callback_registered = false;

                private void Start()
                {
                    if (!SRanipal_Eye_Framework.Instance.EnableEye)
                    {
                        enabled = false;
                        return;
                    }
                    Assert.IsNotNull(GazeRayRenderer);
                }

                private void Update()
                {
                    if (SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.WORKING &&
                        SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.NOT_SUPPORT) return;

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

                    Vector3 GazeOriginCombinedLocal, GazeDirectionCombinedLocal;

                    if (eye_callback_registered)
                    {
                        
                        if (SRanipal_Eye_v2.GetGazeRay(GazeIndex.COMBINE, out GazeOriginCombinedLocal, out GazeDirectionCombinedLocal, eyeData)) { }
                        else if (SRanipal_Eye_v2.GetGazeRay(GazeIndex.LEFT, out GazeOriginCombinedLocal, out GazeDirectionCombinedLocal, eyeData)) { }
                        else if (SRanipal_Eye_v2.GetGazeRay(GazeIndex.RIGHT, out GazeOriginCombinedLocal, out GazeDirectionCombinedLocal, eyeData)) { }
                        else return;
                    }
                    else
                    {
                        if (SRanipal_Eye_v2.GetGazeRay(GazeIndex.COMBINE, out GazeOriginCombinedLocal, out GazeDirectionCombinedLocal)) { }
                        else if (SRanipal_Eye_v2.GetGazeRay(GazeIndex.LEFT, out GazeOriginCombinedLocal, out GazeDirectionCombinedLocal)) { }
                        else if (SRanipal_Eye_v2.GetGazeRay(GazeIndex.RIGHT, out GazeOriginCombinedLocal, out GazeDirectionCombinedLocal)) { }
                        else return;
                    }

                    Vector3 GazeDirectionCombined = Camera.main.transform.TransformDirection(GazeDirectionCombinedLocal);
                    GazeRayRenderer.SetPosition(0, Camera.main.transform.position - Camera.main.transform.up * 0.05f);
                    GazeRayRenderer.SetPosition(1, Camera.main.transform.position + GazeDirectionCombined * LengthOfRay);

                    // Debug.Log("origin:" + gaze_origin.x + "/" + gaze_origin.y + "/" + gaze_origin.z);
                    // Debug.Log("direction: " + gaze_direction.x + "/" + gaze_direction.y + "/" + gaze_direction.z);
                    // Debug.Log("GazeOriginCombinedLocal:" + GazeOriginCombinedLocal.x + "/" + GazeOriginCombinedLocal.y + "/" + GazeOriginCombinedLocal.z);
                    // Debug.Log("GazeDirectionCombinedLocal:" + GazeDirectionCombinedLocal.x + "/" + GazeDirectionCombinedLocal.y + "/" + GazeDirectionCombinedLocal.z);
                    // Debug.Log("GazeDirectionCombined:" + GazeDirectionCombined.x + "/" + GazeDirectionCombined.y + "/" + GazeDirectionCombined.z);

                    // rohans new code
                    // Bit shift the index of the layer (8) to get a bit mask
                    int layerMask = 1 << 8;
                    // This would cast rays only against colliders in layer 8.
                    // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
                    // layerMask = ~layerMask;

                    RaycastHit hit;
                    // Does the ray intersect any objects excluding the player layer
                    if (Physics.Raycast(Camera.main.transform.position - Camera.main.transform.up * 0.05f, GazeDirectionCombined, out hit, Mathf.Infinity, layerMask))
                    {

                        Debug.DrawRay(Camera.main.transform.position - Camera.main.transform.up * 0.05f, GazeDirectionCombined * hit.distance, Color.yellow, 1);
                        Debug.Log("Did Hit");
                        hit.transform.SendMessage("HitByRay");
                    }
                    else
                    {
                        Debug.DrawRay(Camera.main.transform.position - Camera.main.transform.up * 0.05f, GazeDirectionCombined * 1000, Color.white,1);
                        Debug.Log("Did not Hit");
                    }
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
