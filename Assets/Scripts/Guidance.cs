using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guidance : UIObject
{
    public Color activatedColor;

    public override void Hover()
    {
        
    }

    public override void Activate(string FrameName)
    {
        ChangeColor(gameObject, activatedColor);
        status = 2;
    }

    public override void Deactivate()
    {
        ChangeColor(gameObject, defaultColor);
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
        activatedColor = defaultColor;
        activatedColor.a = 0.8f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
