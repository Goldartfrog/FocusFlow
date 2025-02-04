using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine.SceneManagement;
using System.Text;
using UnityEngine.Profiling.Memory.Experimental;

[Serializable]
public class EyeTrackingFrame
{
    public float timestamp;
    public int frameNumber;
    public Vector3Serializable gazeDirection;
    public Vector3Serializable gazeOrigin;
    public float gazeDepth;
}

[Serializable]
public class Vector3Serializable
{
    public float x;
    public float y;
    public float z;

    public Vector3Serializable(Vector3 v)
    {
        x = v.x;
        y = v.y;
        z = v.z;
    }
}

[Serializable]
public class EventEntry
{
    public float timestamp;
    public string eventType;
    public string eventData;
    public Dictionary<string, object> additionalData;
}

[Serializable]
public class SessionMetadata
{
    public string participantId;
    public string sceneName;
    public string sessionStart;
    public HardwareInfo hardwareInfo;
}

[Serializable]
public class HardwareInfo
{
    public string headset;
    public int refreshRate;
    public string unityVersion;
}

public class EyeTrackingLogger : MonoBehaviour
{
    private string logDirectory;
    private string sessionId;
    private float startTime;
    private InteractionEyeTracker eyeData;
    private KeyboardTextSystem keyboardSystem;
    private List<EyeTrackingFrame> frameBuffer;
    private StreamWriter continuousWriter;
    private StreamWriter eventWriter;
    private SessionMetadata metadata;

    private const int FRAMES_BEFORE_WRITE = 300; // Write every 300 frames
    private string lastWord = "";
    private char lastHighlightedLetter = ' ';

    void Awake()
    {
        sessionId = DateTime.Now.ToString("yyyy-MM-dd-HH-mm");
        logDirectory = Path.Combine(Application.dataPath, "..", "EyeTrackingData", sessionId);
        Directory.CreateDirectory(logDirectory);

        frameBuffer = new List<EyeTrackingFrame>();
        startTime = Time.time;

        InitializeMetadata();
        InitializeStreams();
    }

    void InitializeStreams()
    {
        string continuousPath = Path.Combine(logDirectory, "continuous.json");
        string eventsPath = Path.Combine(logDirectory, "events.json");

        File.WriteAllText(continuousPath, "{\n\"frames\": [\n");
        continuousWriter = new StreamWriter(continuousPath, true);

        File.WriteAllText(eventsPath, "{\n\"events\": [\n");
        eventWriter = new StreamWriter(eventsPath, true);
    }

    void Start()
    {
        eyeData = FindObjectOfType<InteractionEyeTracker>();
        keyboardSystem = FindObjectOfType<KeyboardTextSystem>();

        WriteMetadata();
    }

    void InitializeMetadata()
    {
        metadata = new SessionMetadata
        {
            participantId = SystemInfo.deviceUniqueIdentifier,
            sceneName = SceneManager.GetActiveScene().name,
            sessionStart = DateTime.Now.ToString("o"),
            hardwareInfo = new HardwareInfo
            {
                headset = "Vive Pro Eye",
                refreshRate = Screen.currentResolution.refreshRate,
                unityVersion = Application.unityVersion
            }
        };
    }

    void Update()
    {
        float currentTime = Time.time - startTime;

        // Log eye tracking frame
        var frame = new EyeTrackingFrame
        {
            timestamp = currentTime,
            frameNumber = Time.frameCount,
            gazeDirection = new Vector3Serializable(eyeData.gazeDirection),
            gazeOrigin = new Vector3Serializable(eyeData.gazeOrigin),
            gazeDepth = eyeData.gazeDepth
        };

        frameBuffer.Add(frame);

        // Check for UI state changes and log as events
        if (keyboardSystem != null)
        {
            string currentWord = keyboardSystem.GetCurrentWord();
            if (currentWord != lastWord)
            {
                LogEvent("current_word_changed", currentWord);
                lastWord = currentWord;
            }

            // NOTE: SAM COMMENTED THIS OUT BECAUSE keybaordSystem has no method GetHighlightedLetter().
            
            // char currentHighlight = keyboardSystem.GetHighlightedLetter(); // You'll need to add this method
            // if (currentHighlight != lastHighlightedLetter)
            // {
            //     LogEvent("letter_highlight_changed", currentHighlight.ToString());
            //     lastHighlightedLetter = currentHighlight;
            // }
        }

        if (frameBuffer.Count >= FRAMES_BEFORE_WRITE)
        {
            WriteFrameBuffer();
        }
    }

    void WriteFrameBuffer()
    {
        if (frameBuffer.Count == 0) return;

        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < frameBuffer.Count; i++)
        {
            string json = JsonUtility.ToJson(frameBuffer[i]);
            sb.Append(json);
            if (i < frameBuffer.Count - 1)
            {
                sb.Append(",\n");
            }
        }

        continuousWriter.Write(sb.ToString() + ",\n");
        continuousWriter.Flush();
        frameBuffer.Clear();
    }

    public void LogEvent(string eventType, string eventData, Dictionary<string, object> additionalData = null)
    {
        var evt = new EventEntry
        {
            timestamp = Time.time - startTime,
            eventType = eventType,
            eventData = eventData,
            additionalData = additionalData ?? new Dictionary<string, object>()
        };

        string json = JsonUtility.ToJson(evt);
        eventWriter.WriteLine(json + ",");
        eventWriter.Flush();
    }

    void WriteMetadata()
    {
        string filename = Path.Combine(logDirectory, "metadata.json");
        string json = JsonUtility.ToJson(metadata, true);
        File.WriteAllText(filename, json);
    }

    // Typing-specific event logging methods
    public void LogTypingEvent(char letter, float time)
    {
        var data = new Dictionary<string, object>
        {
            {"duration", time}
        };
        LogEvent("letter_typed", letter.ToString(), data);
    }

    public void LogWordCompletion(string word, float duration)
    {
        var data = new Dictionary<string, object>
        {
            {"duration", duration}
        };
        LogEvent("word_completed", word, data);
    }

    public void LogPhraseStart(string phrase)
    {
        LogEvent("phrase_start", phrase);
    }

    public void LogPhraseEnd(string phrase, float totalTime)
    {
        var data = new Dictionary<string, object>
        {
            {"duration", totalTime}
        };
        LogEvent("phrase_end", phrase, data);
    }

    void OnApplicationQuit()
    {
        WriteFrameBuffer();
        continuousWriter.WriteLine("]}");
        eventWriter.WriteLine("]}");
        continuousWriter.Close();
        eventWriter.Close();
    }

    void OnDestroy()
    {
        if (continuousWriter != null)
        {
            continuousWriter.Close();
        }
        if (eventWriter != null)
        {
            eventWriter.Close();
        }
    }
}