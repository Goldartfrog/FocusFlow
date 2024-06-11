using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShoppingPanel : UIObject
{
    float DefaultTransparency;
    float ActivatedTransparency;
    private string DisplayProductName;

    public GameObject ProductObj;
    public float y_clothes;
    public float y_trousers;
    public float y_hat;
    public float y_tie;
    public float y_backpack;
    public Vector3 scale_clothes;
    public Vector3 scale_trousers;
    public Vector3 scale_hat;
    public Vector3 scale_tie;
    public Vector3 scale_backpack;

    /*    public GameObject guidance;*/

    public override void ChangeColor(GameObject obj, Color targetColor)
    {
        obj.GetComponent<Image>().color = targetColor;
    }

    public override IEnumerator ChangeTransparency(float targetTP, float changeTime)
    {
        isLocked = true;

        int stepsNum = (int)(changeTime / stepTime);
        Color currPanelColor = this.GetComponent<Image>().color;
        // Color currImageColor = this.GetComponent<Image>().color;
        float currPanelTP = currPanelColor.a;
        // float currImageTP = currImageColor.a;
        float stepPanel = (targetTP - currPanelTP) / stepsNum;
        // float stepImage = (targetTP - currImageTP) / stepsNum;

        for (int i = 0; i < stepsNum; i += 1)
        {
            currPanelTP += stepPanel;
            currPanelColor.a = currPanelTP;
            // currImageTP += stepImage;
            // currImageColor.a = currImageTP;
            ChangeColor(gameObject, currPanelColor);
            // ChangeColor(ProductObj, currImageColor);
            yield return new WaitForSeconds(stepTime);
        }

        isLocked = false;
    }

    public override void Hover()
    {
        
    }

    public override void Activate(string ProductName)
    {
        isLocked = true;

        StartCoroutine(ChangeTransparency(ActivatedTransparency, 0.2f));

        ProductObj.SetActive(true);
        DisplayProductName = ProductName;
        string category = DisplayProductName.Split('_')[0];
        if(category == "clothes")
        {
            if (DisplayProductName.Split('_')[1] == "1" || DisplayProductName.Split('_')[1] == "2" || DisplayProductName.Split('_')[1] == "3")
            {
                Vector3 pos = ProductObj.GetComponent<RectTransform>().anchoredPosition3D;
                pos.y = y_trousers;
                ProductObj.GetComponent<RectTransform>().anchoredPosition3D = pos;
                ProductObj.GetComponent<RectTransform>().localScale = scale_trousers;
            }
            else
            {
                Vector3 pos = ProductObj.GetComponent<RectTransform>().anchoredPosition3D;
                pos.y = y_clothes;
                ProductObj.GetComponent<RectTransform>().anchoredPosition3D = pos;
                ProductObj.GetComponent<RectTransform>().localScale = scale_clothes;
            }
        }
        else if(category == "hat")
        {
            Vector3 pos = ProductObj.GetComponent<RectTransform>().anchoredPosition3D;
            pos.y = y_hat;
            ProductObj.GetComponent<RectTransform>().anchoredPosition3D = pos;
            ProductObj.GetComponent<RectTransform>().localScale = scale_hat;
        }
        else if(category == "tie")
        {
            Vector3 pos = ProductObj.GetComponent<RectTransform>().anchoredPosition3D;
            pos.y = y_tie;
            ProductObj.GetComponent<RectTransform>().anchoredPosition3D = pos;
            ProductObj.GetComponent<RectTransform>().localScale = scale_tie;
        }
        else if(category == "Backpack")
        {
            Vector3 pos = ProductObj.GetComponent<RectTransform>().anchoredPosition3D;
            pos.y = y_backpack;
            ProductObj.GetComponent<RectTransform>().anchoredPosition3D = pos;
            ProductObj.GetComponent<RectTransform>().localScale = scale_backpack;
        }
        GameObject HitProductObj = GameObject.Find(DisplayProductName);
        GetComponent<Image>().material = HitProductObj.GetComponent<Product>().mat;
        ProductObj.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", HitProductObj.GetComponent<MeshRenderer>().material.mainTexture);
        ProductObj.GetComponent<MeshFilter>().mesh = HitProductObj.GetComponent<MeshFilter>().mesh;

        status = 2;
    }

    public override void Deactivate()
    {
        isLocked = true;
        
        DisplayProductName = "";
        StartCoroutine(ChangeTransparency(DefaultTransparency, 0.2f));

        ProductObj.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", null);
        ProductObj.GetComponent<MeshFilter>().mesh = null;
        ProductObj.SetActive(false);

        status = 0;
    }

    // Start is called before the first frame update
    public new void Start()
    {
        // Base Start()
        status = 0;
        stepTime = 0.02f;
        defaultColor = this.GetComponent<Image>().color;
        isLocked = false;

        DefaultTransparency = defaultColor.a;
        ActivatedTransparency = 0.9f;
        DisplayProductName = "";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
