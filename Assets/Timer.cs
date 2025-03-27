using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI timerRef;
    [SerializeField]
    private ProgressionManager progressionManagerRef;
    
    private float startTime;
    private float currentTime;
    private bool isRunning;
    private int stageTime;

    void Update()
    {
        if (isRunning)
        {
            currentTime = Time.time - startTime;
            UpdateDisplay();
            if (currentTime >= stageTime) {
                StopTimer();
            }
        }
    }

    private void UpdateDisplay()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60f);
        int seconds = Mathf.FloorToInt(currentTime % 60f);
        timerRef.text = $"{minutes:00}:{seconds:00}";
    }

    public void StartTimer()
    {
        startTime = Time.time;
        isRunning = true;
        UpdateDisplay();
        stageTime = progressionManagerRef.GetStageTime();
    }

    public void StopTimer()
    {
        isRunning = false;
        currentTime = 0f;
        UpdateDisplay();
    }

    public void PauseTimer()
    {
        isRunning = false;
    }

    public float GetCurrentTime()
    {
        return currentTime;
    }
}
