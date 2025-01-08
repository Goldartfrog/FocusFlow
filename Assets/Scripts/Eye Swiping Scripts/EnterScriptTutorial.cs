using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ViveSR.anipal.Eye;
using UnityEngine.UI;


public class EnterScriptTutorial : MonoBehaviour
{
    public InteractionEyeTracker EyePos;
    private Renderer rend;
    private float timeToInput = 0.3f;
    private float timeout = 1f;
    private bool looked = false;
    public KeyboardTextSystemIntroduction keyboard;
    private bool allowInput = false;
    private bool visible = false;

    private MeshCollider meshCollider;

    private float currentAlpha = 1f;
    private Material material;
    private bool pressed = false;

    [SerializeField] private Image panel;

    private void Start()
    {
        rend = gameObject.GetComponent<Renderer>();
        meshCollider = GetComponent<MeshCollider>();
        if (rend != null)
        {
            material = rend.material;
        }
    }

    // Need button timer and cooldown timer
    // button timer starts when the gaze is on it
    // button presses when timer exceeds threshold
    //  Changes the color of the bar
    // then cooldown timer starts
    // can't push button while cooldown timer is going
    // once cooldown has exceeded can push again

    private float timer = 0f;
    private float coolDownTimer = 0f;
    private bool onCooldown = false;
    private float coolDownTime = 1f;
    void Update()
    {


        //timer += Time.deltaTime;
        coolDownTimer += Time.deltaTime;
        ChangeColor();
        if (Input.GetKeyDown("space"))
        {
            TogglePressed();
            keyboard.RecieveEnter();

        }

        if (LookingAtBox() && !onCooldown && allowInput)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / timeToInput);
            currentAlpha = Mathf.Lerp(1f, 0f, t);
            SetAlpha(currentAlpha);
            if (timer > timeToInput)
            {
                //Debug.Log("timer: " + timer);
                //Debug.Log("pressed: " + pressed);
                //Debug.Log("onCooldown: " + onCooldown);
                //Debug.Log("coolDownTimer: " + coolDownTimer);
                timer = 0f;
                onCooldown = true;
                coolDownTimer = 0f;
                TogglePressed();
                keyboard.RecieveEnter();
                //Debug.Log("timer agian: " + timer);
                //Debug.Log("coolDownTimer again: " + coolDownTimer);
            }
        }
        else if (!LookingAtBox() && allowInput)
        {
            SetAlpha(1f);
            timer = 0f;
        }


        if (coolDownTimer > coolDownTime && onCooldown)
        {
            onCooldown = false;
            timer = 0f;
        }

        //if (EyePos.worldPosition.magnitude > 0 && EyePos.gazeLocation.magnitude > 0)
        //{
        //    if (LookingAtBox(EyePos.worldPosition, EyePos.gazeLocation))
        //    {
        //        //rend.enabled = false;
        //        float t = Mathf.Clamp01(timer / timeToInput);
        //        currentAlpha = Mathf.Lerp(1f, 0f, t);
        //        SetAlpha(currentAlpha);
        //        if (!looked && timer > timeToInput)
        //        {
        //            TogglePressed();
        //            looked = true;
        //            timer = 0f;
        //            keyboard.RecieveEnter();
        //        }
        //    }
        //    else
        //    {
        //        //rend.enabled = true;

        //        if (!looked)
        //        {
        //            timer = 0;
        //            SetAlpha(1f);
        //        }
        //    }
        //}
        //if (looked && timer > timeout)
        //{
        //    timer = 0;
        //    looked = false;
        //    //SetAlpha(1f);
        //}
    }

    private void SetAlpha(float alpha)
    {
        Color newColor = material.color;
        newColor.a = alpha;
        material.color = newColor;
    }

    private void ChangeColor()
    { 
        //if (allowInput)
        //{
            if (pressed)
            {
                Color newColor = material.color;
                newColor = new Color(0.5f, 0.5f, 0.5f);
                material.color = newColor;
                panel.color = new Color(0.2f, 0.6f, 0.3f, 0.6f);
            }
            else
            {
                Color newColor = material.color;
                newColor = new Color(1f, 1f, 1f);
                material.color = newColor;
                panel.color = new Color(0.1f, 0.1f, 0.1f, 0.6f);
            }
        //}
        
    }

    public void TogglePressedAccessor() {
        TogglePressed();
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

    // Checks if the user is looking at the "spacebar"
    public bool LookingAtBox()
    {
        try
        {
            Vector3 userPosition = EyePos.worldPosition;
            Vector3 fixationPoint = EyePos.gazeLocation;
            Vector3 direction = (fixationPoint - userPosition);
            if (direction != Vector3.zero)
            {

                float distance = Vector3.Distance(userPosition, fixationPoint);
                Ray ray = new Ray(userPosition, direction.normalized);
                RaycastHit hit;
                if (meshCollider.Raycast(ray, out hit, Mathf.Infinity))
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


    public bool LookingAtBox(Vector3 userPosition, Vector3 fixationPoint)
    {
        /*
                Vector3 direction = (fixationPoint - userPosition).normalized;
                float distance = Vector3.Distance(userPosition, fixationPoint);
                Ray ray = new Ray(userPosition, direction);

                if (rend.bounds.IntersectRay(ray))
                {
                    return true;
                }

                return false;*/

        try
        {
            Vector3 direction = (fixationPoint - userPosition);
            float distance = Vector3.Distance(userPosition, fixationPoint);
            Ray ray = new Ray(userPosition, direction.normalized);
            RaycastHit hit;
            if (meshCollider.Raycast(ray, out hit, Mathf.Infinity))
            {
                return true;
            }
        }
        catch
        {

        }
        return false;
    }



    public void disable()
    {
        allowInput = false;
    }

    public void enable() { allowInput = true; }
    public void turnOff()
    {
        visible = false;
    }

    public void turnOn() { visible = true; }


    public void turnGreen(float g)
    {
        rend.material.color = new Color(g, 255, g);
    }

    public void turnWhite()
    {
        rend.material.color = Color.white;
    }

    public Vector3 getPosition()
    {
        return rend.transform.position;
    }
}
