using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvObject : UIObject
{
    public Vector3 changeHoverSize = new Vector3(0.1f, 0.1f, 0f);
    public Vector3 changeActiveSize = new Vector3(0.4f, 0.4f, 0f);
    public Vector3 hoverSize;
    public Vector3 activeSize;
    public Vector3 deactiveSize;

    float DefaultImageTransparency;
    float HoverImageTransparency;

    /*public GameObject guidance;*/
    public GameObject Panel;
    public GameObject Image;

    public override IEnumerator ChangeTransparency(float targetTP, float changeTime)
    {
        isLocked = true;

        int stepsNum = (int)(changeTime / stepTime);
        Color currColor = Image.GetComponent<MeshRenderer>().material.color;
        float currTP = currColor.a;
        float step = (targetTP - currTP) / stepsNum;

        for (int i = 0; i < stepsNum; i += 1)
        {
            currTP += step;
            currColor.a = currTP;
            ChangeColor(Image, currColor);
            yield return new WaitForSeconds(stepTime);
        }

        isLocked = false;
    }

/*    public void SelectedForTutorial()
    {
        Color red = Color.red;
        ChangeColor(Image, red);
    }

    public void ExitFromTutorial()
    {
        Color recover = defaultColor;
        recover.a = Image.GetComponent<MeshRenderer>().material.color.a;
        ChangeColor(Image, recover);
    }*/

    public override void Hover()
    {
        /*preHoverColor = this.GetComponent<MeshRenderer>().material.color;
        Color targetColor = this.GetComponent<MeshRenderer>().material.color * hoverColorFactor;
        ChangeColor(targetColor);*/
        status = 1;
        StartCoroutine(ChangeTransparency(HoverImageTransparency, 0.1f));
    }

    public override void Activate(string FrameName)
    {
        /*Color targetColor = defaultColor;
        ChangeColor(targetColor);
        StartCoroutine(ChangeSize(activeSize, 0.1f));*/
        status = 2;
    }

    public override void Deactivate()
    {
/*        if (status == 1)
        {
            ChangeColor(preHoverColor);
        }
        else
        {
            ChangeColor(defaultColor);
        }
        StartCoroutine(ChangeSize(deactiveSize, 0.1f));*/
        status = 0;
        StartCoroutine(ChangeTransparency(DefaultImageTransparency, 0.1f));
        // Remove Highlight
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
    public override void Start()
    {
        base.Start();
        hoverSize = this.transform.localScale + changeHoverSize;
        activeSize = this.transform.localScale + changeActiveSize;
        deactiveSize = this.transform.localScale;
        DefaultImageTransparency = Image.GetComponent<MeshRenderer>().material.color.a;
        HoverImageTransparency = 0.6f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
