using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Product : UIObject
{
    public Vector3 changeHoverSize = new Vector3(0.1f, 0.1f, 0.1f);
    public Vector3 changeActiveSize = new Vector3(0.4f, 0.4f, 0.4f);
    public Vector3 hoverSize;
    public Vector3 activeSize;
    public Vector3 deactiveSize;
    public Material mat;

    /*public GameObject guidance;*/
    public GameObject Panel;

    public override void Hover()
    {
        /*preHoverColor = this.GetComponent<MeshRenderer>().material.color;
        Color targetColor = this.GetComponent<MeshRenderer>().material.color * hoverColorFactor;
        ChangeColor(targetColor);*/
        status = 1;
        StartCoroutine(ChangeSize(hoverSize, 0.1f));
    }

    public override void Activate(string ProductName)
    {
        /*Color targetColor = defaultColor;
        ChangeColor(targetColor);
        StartCoroutine(ChangeSize(activeSize, 0.1f));*/
        status = 2;
    }

    public override void Deactivate()
    {
        status = 0;
        StartCoroutine(ChangeSize(deactiveSize, 0.1f));
    }

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        hoverSize = this.transform.localScale + changeHoverSize;
        activeSize = this.transform.localScale + changeActiveSize;
        deactiveSize = this.transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
