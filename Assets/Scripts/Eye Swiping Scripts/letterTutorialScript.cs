using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;
using Valve.VR.InteractionSystem;
using ViveSR.anipal.Eye;

public class letterTutorialScript : MonoBehaviour
{
    // Start is called before the first frame update
    public InteractionEyeTracker EyePos;
    public float timeToInput = 1f;
    public KeyboardTextSystemIntroduction keyboard;
    public IntroductionManager manager;
    public GameObject soundSource;
    private AudioSource audioSource;

    private Renderer rend;
    private Material material;
    private BoxCollider boxCollider;
    private float lingerTime = 0.1f;
    private float timer = 0f;
    private float lastLookTime;
    private bool isBeingLooked;
    private float colorSpeed = 3f;
    private bool alwaysOff = false;

    private bool changing = true;

    void Start()
    {
        //keyboard.CollectPositions(gameObject.name, gameObject.transform.position);
        rend = gameObject.GetComponent<Renderer>();
        boxCollider = GetComponent<BoxCollider>();
        if (rend != null)
        {
            material = rend.material;
        }

        if (soundSource  != null)
        {
            audioSource = soundSource.GetComponent<AudioSource>();
        } else
        {
            //Debug.Log("No Sound object (Joseph's Scene)");
        }
    }

    private void Update()
    {
        ChangeColor();
    }

    private void ChangeColor()
    {
        if (LookingAtBox(EyePos.worldPosition, EyePos.gazeLocation))
        {
            isBeingLooked = true;
            lastLookTime = Time.time;
        }
        else
        {
            isBeingLooked = false;
        }
        

        if (changing && gameObject.name != "Center")
        {
            Color targetColor;
            if (isBeingLooked || Time.time - lastLookTime < lingerTime)
            {
                targetColor = new Color(0.65f, 0.9f, 0.9f);
            }
            else
            {
                targetColor = new Color(1f, 1f, 1f);
            }
            material.color = Color.Lerp(material.color, targetColor, Time.deltaTime * colorSpeed);
        }
        

        if (alwaysOff)
        {
            rend.enabled = false;
        }
    }

    public bool LookingAtBox(Vector3 userPosition, Vector3 fixationPoint)
    {

        Vector3 direction = (fixationPoint - userPosition).normalized;
        float distance = Vector3.Distance(userPosition, fixationPoint);
        Ray ray = new Ray(userPosition, direction);

        if (rend.bounds.IntersectRay(ray))
        {
            return true;
        }

        return false;
    }


    public void turnOn()
    {
        alwaysOff = false;
        rend.enabled = true;
    }

    public void turnOff()
    {
        alwaysOff = true;
    }

    public void turnGreen(float g)
    {
        rend.material.color = new Color(g, 1, g);

        if (audioSource != null)
        {
            audioSource.Play();
        } else
        {
            Debug.Log("Error in Playing Individual Key Sound (Specific to Joseph's Scene)");
        }
    }

    public void turnWhite()
    {
        rend.material.color = Color.white;
    }

    public void moveTo(Vector3 pos)
    {
        rend.transform.position = pos;
    }

    public void preventChange()
    {
        changing = false;
    }

    public void allowChange()
    {
        changing = true;
    }
}
