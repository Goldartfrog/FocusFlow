using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MoveText : UIObject
{
    public RectTransform rectTransform;
    public Scrollbar scrollbar;
    public float lerpingSpeed = 2f;
    public float lerpingScale = 1f;

    public override void Hover()
    {

    }

    public override void Activate(string name)
    {

    }

    public override void Deactivate()
    {

    }

    void Update()
    {
        // Move text upwards when 'W' key is pressed
        if (Input.GetKeyDown(KeyCode.W))
        {
            Debug.Log("pressing w");
            LerpText(lerpingScale);
        }

        // Move text downwards when 'S' key is pressed
        if (Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log("pressing s");
            LerpText(-lerpingScale);
        }
    }

    private void Up(float speed)
    {
        Debug.Log("pressing w");
        //LerpText(lerpingScale);
        scrollbar.value += 0.01f;
    }

    private void Down(float speed)
    {
        Debug.Log("pressing s");
        //LerpText(-lerpingScale);
        scrollbar.value -= 0.01f;
    }

    private void Stop(float depth_diff)
    {
    }

    void LerpText(float direction)
    {
        // Calculate target position based on direction
        Vector3 targetPosition = rectTransform.anchoredPosition3D + Vector3.up * direction;

        // Lerping the position
        StartCoroutine(LerpCoroutine(targetPosition));
    }

    IEnumerator LerpCoroutine(Vector3 targetPosition)
    {
        float elapsedTime = 0f;
        Vector3 startPosition = rectTransform.anchoredPosition3D;

        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime * lerpingSpeed;
            rectTransform.anchoredPosition3D = Vector3.Lerp(startPosition, targetPosition, elapsedTime);
            yield return null;
        }
    }
}
