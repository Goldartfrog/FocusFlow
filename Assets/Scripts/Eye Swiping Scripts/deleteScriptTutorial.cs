using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class deleteScriptTutorial : MonoBehaviour
{
    public InteractionEyeTracker EyePos;
    private Renderer rend;
    public float timeToInput = .7f;
    public KeyboardTextSystemIntroduction keyboard;
    public GameObject tmp;


    private float coolDownTimer = 0;
    private float coolDownDuration = 1f;
    private float timer = 0;
    private bool onCooldown = false;
    private float currentAlpha = 1f;
    private Material material;
    private bool pressed = false;
    private BoxCollider boxCollider;

    private bool on;

    void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        rend = GetComponent<Renderer>();
        if (rend != null)
        {
            material = rend.material;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("On is: " + on);

        if (on)
        {
            tmp.SetActive(true);
            rend.enabled = true;
        } else
        {
            tmp.SetActive(false);
            rend.enabled = false;
        }

        coolDownTimer += Time.deltaTime;
        //if (LookingAtBox())
        //{
        //    //Debug.Log("looking at delete");
        //}


        //if (LookingAtBox() || !on)
        //{
        //    rend.enabled = false;
        //    if (!onCooldown && on)
        //    {
        //        timer += Time.deltaTime;
        //        float t = Mathf.Clamp01(timer / timeToInput);
        //        currentAlpha = Mathf.Lerp(1f, 0f, t);
        //        SetAlpha(currentAlpha);
        //        if (timer > timeToInput)
        //        {
        //            TogglePressed();
        //            keyboard.RecieveDelete();
        //            timer = 0f;
        //            onCooldown = true;
        //            coolDownTimer = 0f;
        //        }
        //    }

        //}
        //else
        //{
        //    SetAlpha(1f);
        //    timer = 0f;
        //    rend.enabled = true;
        //}
        coolDownTimer += Time.deltaTime;

        if (LookingAtBox() && !onCooldown)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / timeToInput);
            currentAlpha = Mathf.Lerp(1f, 0f, t);
            SetAlpha(currentAlpha);
            if (timer > timeToInput)
            {
                TogglePressed();
                keyboard.RecieveDelete();
                timer = 0f;
                onCooldown = true;
                coolDownTimer = 0f;
            }
        }
        else
        {
            SetAlpha(1f);
            timer = 0f;
        }
        if (coolDownTimer > coolDownDuration)
        {
            onCooldown = false;
        }


        //CheckPos();
        //rend = gameObject.GetComponent<Renderer>();
        //boxCollider = gameObject.GetComponent<Collider>();

        //if (LookingAtBox())
        //{
        //    rend.enabled = false;
        //    timer += Time.deltaTime;

        //    if (timer > timeToInput)
        //    {
        //        timer = 0;
        //        keyboard.RecieveDelete();
        //    }
        //}
        //else
        //{
        //    rend.enabled = true;
        //    timer = 0;
        //}
    }

    private void SetAlpha(float alpha)
    {
        Color newColor = material.color;
        newColor.a = alpha;
        material.color = newColor;
    }

    private void TogglePressed()
    {
        if (pressed == false)
        {
            pressed = true;
        }
        else
        {
            pressed = false;
        }
    }

    bool LookingAtBox()
    {
        //Vector3 fixationPoint = EyePos.gazeLocation;
        //Vector3 userPosition = EyePos.worldPosition;


        ///*Vector3 userPosition = EyePos.worldPosition;
        //Vector3 fixationPoint = EyePos.gazeLocation;
        //Vector3 direction = (fixationPoint - userPosition).normalized;
        //float distance = Vector3.Distance(userPosition, fixationPoint);
        //Ray ray = new Ray(userPosition, direction);

        //RaycastHit hit;
        //if (boxCollider.Raycast(ray, out hit, Mathf.Infinity))
        //{
        //    return true;
        //}

        //return false;*/


        //Vector3 direction = (fixationPoint - userPosition).normalized;
        //float distance = Vector3.Distance(userPosition, fixationPoint);
        //Ray ray = new Ray(userPosition, direction);

        //if (rend.bounds.IntersectRay(ray))
        //{
        //    return true;
        //}

        //return false;
        Vector3 userPosition = EyePos.worldPosition;
        Vector3 fixationPoint = EyePos.gazeLocation;
        Vector3 direction = (fixationPoint - userPosition).normalized;
        float distance = Vector3.Distance(userPosition, fixationPoint);
        Ray ray = new Ray(userPosition, direction);
        if (direction != Vector3.zero)
        {
            RaycastHit hit;
            if (boxCollider.Raycast(ray, out hit, Mathf.Infinity))
            {
                return true;
            }
        }


        return false;
    }

    //void CheckPos()
    //{
    //    var newPos = keyboard.giveUpdatedPositions(gameObject.name);

    //    if (newPos == gameObject.transform.position)
    //    {
    //        return;
    //    }
    //    else
    //    {
    //        gameObject.transform.position = newPos;
    //    }
    //}

    public void enable()
    {
        on = true;
    }

    public void disable()
    {
        on = false;
    }
}
