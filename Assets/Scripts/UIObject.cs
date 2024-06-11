using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIObject : MonoBehaviour
{
    public int status;
    public float stepTime;
    public Color defaultColor;

    public bool isLocked;

    public virtual void ChangeColor(GameObject obj, Color targetColor)
    {
        obj.GetComponent<MeshRenderer>().material.color = targetColor;
    }

    public IEnumerator ChangeSize(Vector3 targetSize, float changeTime)
    {
        isLocked = true;

        int stepsNum = (int)(changeTime / stepTime);
        Vector3 currSize = this.transform.localScale;
        Vector3 step = (targetSize - currSize) / stepsNum;

        for (int i = 0; i < stepsNum; i += 1)
        {
            this.transform.localScale += step;
            yield return new WaitForSeconds(stepTime);
        }

        isLocked = false;
    }
    
    public IEnumerator ChangePosition(Vector3 targetPos, float changeTime)
    {
        isLocked = true;

        int stepsNum = (int)(changeTime / stepTime);
        Vector3 currPos = this.transform.position;
        Vector3 step = (targetPos - currPos) / stepsNum;

        for (int i = 0; i < stepsNum; i += 1)
        {
            this.transform.position += step;
            yield return new WaitForSeconds(stepTime);
        }

        isLocked = false;
    }

    public virtual IEnumerator ChangeTransparency(float targetTP, float changeTime)
    {
        isLocked = true;

        int stepsNum = (int)(changeTime / stepTime);
        Color currColor = this.GetComponent<MeshRenderer>().material.color;
        float currTP = currColor.a;
        float step = (targetTP - currTP) / stepsNum;

        for (int i = 0; i < stepsNum; i += 1)
        {
            currTP += step;
            currColor.a = currTP;
            ChangeColor(gameObject, currColor);
            yield return new WaitForSeconds(stepTime);
        }

        isLocked = false;
    }

    public void RecoverColor()
    {
        ChangeColor(gameObject, defaultColor);
    }

    public abstract void Hover();

    public abstract void Activate(string FrameName);

    public abstract void Deactivate();

    // Start is called before the first frame update
    public virtual void Start()
    {
        status = 0;
        stepTime = 0.02f;
        defaultColor = this.GetComponent<MeshRenderer>().material.color;
        isLocked = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
