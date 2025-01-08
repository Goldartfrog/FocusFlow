using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteScript : MonoBehaviour
{
    public InteractionEyeTracker EyePos;
    private Renderer rend;
    public float timeToInput = .7f;
    public KeyboardTextSystem keyboard;


    private float coolDownTimer = 0;
    private float coolDownDuration = 1f;
    private float timer = 0;
    private bool onCooldown = false;
    private float currentAlpha = 1f;
    private Material material;
    private bool pressed = false;
    private BoxCollider boxCollider;

    void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        rend = GetComponent<Renderer>();
        if (rend != null )
        {
            material = rend.material;
        }
    }

    // Update is called once per frame
    void Update()
    {
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
}
