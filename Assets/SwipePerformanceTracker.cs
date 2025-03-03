using UnityEngine;

[System.Serializable]
public class SwipePerformanceData
{
    public float accuracy;
    public float duration;
    public int successfulSwipes;
    public int totalSwipes;
}

public class SwipePerformanceTracker : MonoBehaviour
{
    private int totalSwipes = 0;
    private int successfulSwipes = 0;
    private float totalTime = 0;
    
    public event System.Action<SwipePerformanceData> OnPerformanceUpdated;

    public void RecordSwipe(bool successful, float duration)
    {
        totalSwipes++;
        if (successful) successfulSwipes++;
        totalTime += duration;

        var performance = new SwipePerformanceData
        {
            accuracy = totalSwipes > 0 ? (float)successfulSwipes / totalSwipes : 0,
            duration = totalSwipes > 0 ? totalTime / totalSwipes : 0,
            successfulSwipes = successfulSwipes,
            totalSwipes = totalSwipes
        };

        OnPerformanceUpdated?.Invoke(performance);
    }

    public void Reset()
    {
        totalSwipes = 0;
        successfulSwipes = 0;
        totalTime = 0;
    }
} 