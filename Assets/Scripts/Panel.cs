using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Panel : UIObject
{
    float DefaultTransparency;
    float HoverTransparency;
    public GameObject ContentObj;
    public GameObject GlassObj;
    public string DisplayFrameName;

    /*    public GameObject guidance;*/

    public override IEnumerator ChangeTransparency(float targetTP, float changeTime)
    {
        isLocked = true;

        int stepsNum = (int)(changeTime / stepTime);
        Color currFrameColor = this.GetComponent<MeshRenderer>().material.color;
        Color currImageColor = this.GetComponent<MeshRenderer>().material.color;
        float currFrameTP = currFrameColor.a;
        float currImageTP = currImageColor.a;
        float stepFrame = (targetTP - currFrameTP) / stepsNum;
        float stepImage = (targetTP - currImageTP) / stepsNum;

        for (int i = 0; i < stepsNum; i += 1)
        {
            currFrameTP += stepFrame;
            currFrameColor.a = currFrameTP;
            currImageTP += stepImage;
            currImageColor.a = currImageTP;
            ChangeColor(gameObject, currFrameColor);
            ChangeColor(ContentObj, currImageColor);
            yield return new WaitForSeconds(stepTime);
        }

        isLocked = false;
    }

    public override void Hover()
    {
        
    }

    public override void Activate(string FrameName)
    {
        isLocked = true;

        GameObject HitContentObj = ContentObj;
        DisplayFrameName = FrameName;
        Transform HitFrameParent = GameObject.Find(FrameName).transform.parent;
        foreach(Transform child in HitFrameParent)
        {
            if (child.gameObject.name == "Image")
            {
                HitContentObj = child.gameObject;
                break;
            }
        }
        ContentObj.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", HitContentObj.GetComponent<MeshRenderer>().material.mainTexture);

        StartCoroutine(ChangeTransparency(HoverTransparency, 0.1f));
        status = 2;
    }

    public override void Deactivate()
    {
        isLocked = true;
        DisplayFrameName = "";
        StartCoroutine(ChangeTransparency(DefaultTransparency, 0.1f));
        ContentObj.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", null);

        status = 0;
    }

/*    public void RaiseGuidance()
    {
        guidance.SetActive(true);
        Debug.Log(guidance.activeSelf);
    }

    public void DropGuidance()
    {
        guidance.SetActive(false);
        Debug.Log(guidance.activeSelf);
    }*/

    // Start is called before the first frame update
    public new void Start()
    {
        base.Start();
        DefaultTransparency = defaultColor.a;
        HoverTransparency = 0.9f;
        DisplayFrameName = "";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
