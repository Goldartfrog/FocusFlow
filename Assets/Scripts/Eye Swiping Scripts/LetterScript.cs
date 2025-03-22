using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class LetterScript : MonoBehaviour
{
    // Start is called before the first frame update
    public InteractionEyeTracker EyePos;
    private Renderer rend;
    private Material material;
    private BoxCollider boxCollider;
    private float lingerTime = 0.1f;
    private float timer = 0f;
    private float lastLookTime;
    private bool isBeingLooked;
    private float colorSpeed = 3f;
    private Color defaultColor = new Color(1f, 1f, 1f);
    private Color highlightColor = new Color(0.5f, 0.9f, 0.9f);

    void Start()
    {
        //keyboard.CollectPositions(gameObject.name, gameObject.transform.position);
        rend = gameObject.GetComponent<Renderer>();
        boxCollider = GetComponent<BoxCollider>();
        if (rend != null)
        {
            material = rend.material;
        }
    }

    private void Update()
    {
        ChangeColor();
    }

    private void ChangeColor()
    {
        if (LookingAtBox())
        {
            isBeingLooked = true;
            lastLookTime = Time.time;
        } else
        {
            isBeingLooked = false;
        }
        Color targetColor;
        if (isBeingLooked || Time.time - lastLookTime < lingerTime)
        {
            targetColor = highlightColor;
        }
        else
        {
            targetColor = defaultColor;
        }
        Debug.Log("Target color: " + targetColor);
        material.color = Color.Lerp(material.color, targetColor, Time.deltaTime * colorSpeed);
    }

    private bool LookingAtBox()
    {
        try
        {
            Vector3 fixationPoint = EyePos.worldPosition;
            Vector3 userPosition = EyePos.gazeLocation;
            Vector3 direction = (fixationPoint - userPosition);
            if (direction != Vector3.zero)
            {
                float distance = Vector3.Distance(userPosition, fixationPoint);
                Ray ray = new Ray(userPosition, direction.normalized);
                if (boxCollider.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
                {
                    return true;
                }
            }
        }
        catch
        {

        }
        return false;
    }

    public void SetDefaultColor(Color color)
    {
        defaultColor = color;
    }

    //// Update is called once per frame
    //void Update()
    //{
    //    UpdateOn();
    //    PickPos();

    //    gameObject.transform.localScale = reset;
    //    //gameObject.transform.localScale *= keyboard.giveSize(gameObject.name);

    //    SetVars();



    //    if (LookingAtBox(EyePos.worldPosition, EyePos.gazeLocation) && ((rend.enabled == true) || (PartiallyOn)))
    //    {
    //        // Change the color of the key if you are looking at it
    //        //rend.material.color = new Color(0.0f, 0.0f, 0.0f);
    //    } else
    //    {
    //        rend.material.color = new Color(0.4f, 0.4f, 0.4f);
    //    }


    //    if(neverEnter)
    //    {
    //        //rend.enabled = false;
    //        timer += Time.deltaTime;

    //        if (timer > timeToInput && !justEntered)
    //        {
    //            timer = 0;
    //            //keyboard.RecieveInput(gameObject.name);
    //            justEntered = true;
    //        }
    //    } else 
    //    { 
    //        timer = 0;
    //        justEntered = false;
    //    }
    //}

    //bool LookingAtBox(Vector3 userPosition, Vector3 fixationPoint)
    //{

    //    Vector3 direction = (fixationPoint - userPosition).normalized;
    //    float distance = Vector3.Distance(userPosition, fixationPoint);
    //    Ray ray = new Ray(userPosition, direction);

    //    if (rend.bounds.IntersectRay(ray))
    //    {
    //        return true;
    //    }

    //    return false;
    //}

    void CheckPos()
    {
        //var newPos = keyboard.giveUpdatedPositions(gameObject.name);

        //if (newPos == gameObject.transform.position)
        //{
        //    return;
        //}
        //else
        //{
        //    gameObject.transform.position = newPos;
        //}
    }

    //void UpdateOn()
    //{
    //    var percentage = keyboard.givePercentage(gameObject.name);
       
    //    if (percentage < .05) { TotallyOn = false;  } else { TotallyOn = true; }
    //    if (percentage > 0)   { PartiallyOn = true; } else { PartiallyOn = false; }
    //}

    //void MakeOpaque()
    //{
    //    var trans = 0.00f;
    //    var col = rend.material.color;
    //    col.a = trans;
    //    rend.material.color = col;
    //}

    //void PriorityPos()
    //{
    //    var newPos = keyboard.PriorityPosition(gameObject.name);

    //    gameObject.transform.position = newPos;
    //}

    //void PickPos()
    //{
    //    if (MovingKeyboard) { PriorityPos();} else { CheckPos();}        
    //}

    //void SetVars()
    //{
    //    if (keyboard.GetCurrWord() == 1) { rend.enabled = true; } else { rend.enabled = TotallyOn; }

    //    if (MovingKeyboard) 
    //    { 
    //        timeToInput = .8f; 
    //    } else {
    //        if (keyboard.beganWord && TotallyOn)
    //        {
    //            timeToInput = .35f;
    //        }
    //        else if (keyboard.beganWord && PartiallyOn)
    //        {
    //            timeToInput = .8f;
    //        }
    //        else
    //        {
    //            timeToInput = .8f;
    //        }
    //    }

    //    /*if (disabling.Get_Override())
    //    {
    //        rend.enabled = true;
    //    }*/
    //}
}
