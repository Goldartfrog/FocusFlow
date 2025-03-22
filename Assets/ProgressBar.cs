using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ProgressBar : MonoBehaviour
{
    public Image progressBar; // Assign in Inspector
    public Color color1 = Color.green;
    public Color color2 = Color.red;

    private float[] durations = { 480f, 120f, 480f, 120f, 480f, 120f, 480f, 120f, 480f, 120f, 480f, 120f }; // 8 min and 2 min alternating
    private int currentPhase = 0;
    private float elapsedTime = 0f;
    private float totalDuration;
    private bool canMoveOn = false;

    void Start()
    {
        totalDuration = GetTotalDuration();
        StartCoroutine(UpdateProgressBar());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            MoveOn();
        }
    }

    IEnumerator UpdateProgressBar()
    {
        while (currentPhase < durations.Length)
        {
            elapsedTime = 0f;
            float phaseDuration = durations[currentPhase];
            progressBar.color = (currentPhase % 2 == 0) ? color1 : color2;

            while (elapsedTime < phaseDuration)
            {
                elapsedTime += Time.deltaTime;
                progressBar.fillAmount = GetOverallProgress();
                yield return null;
            }

            yield return new WaitUntil(() => canMoveOn);
            canMoveOn = false;
            currentPhase++;
        }
    }

    public void MoveOn()
    {
        canMoveOn = true;
    }

    float GetTotalDuration()
    {
        float sum = 0;
        foreach (float time in durations)
        {
            sum += time;
        }
        return sum;
    }

    float GetOverallProgress()
    {
        float completedTime = 0;
        for (int i = 0; i < currentPhase; i++)
        {
            completedTime += durations[i];
        }
        completedTime += elapsedTime;
        return completedTime / totalDuration;
    }
}
