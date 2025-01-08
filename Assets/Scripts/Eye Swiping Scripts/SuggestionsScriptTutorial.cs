using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Valve.VR.InteractionSystem;
using ViveSR.anipal.Eye;

public class SuggestionsScriptTutorial : MonoBehaviour
{
    public InteractionEyeTracker EyePos;
    private Renderer rend;
    private float timeToInput = .8f;
    public KeyboardTextSystemIntroduction keyboard;
    public TextMeshPro curr;

    private bool justEntered = false;

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
        rend = gameObject.GetComponent<Renderer>();
        if (rend != null)
        {
            material = rend.material;
        }
        boxCollider = gameObject.GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {

        coolDownTimer += Time.deltaTime;
        //ChangeColor();

        if (LookingAtSuggestion() && !onCooldown)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / timeToInput);
            currentAlpha = Mathf.Lerp(1f, 0f, t);
            SetAlpha(currentAlpha);
            if (timer > timeToInput)
            {
                TogglePressed();
                keyboard.RecieveSuggestion(curr.text);
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
            //timer = 0f;
        }
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

    bool LookingAtSuggestion()
    {

        Vector3 userPosition = EyePos.worldPosition;
        Vector3 fixationPoint = EyePos.gazeLocation;
        Vector3 direction = (fixationPoint - userPosition).normalized;
        float distance = Vector3.Distance(userPosition, fixationPoint);
        if (direction != Vector3.zero)
        {
            Ray ray = new Ray(userPosition, direction);

            RaycastHit hit;
            if (boxCollider.Raycast(ray, out hit, Mathf.Infinity))
            {
                return true;
            }
        }
        return false;
    }
}
