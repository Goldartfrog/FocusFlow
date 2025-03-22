using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using System;

public class EyeReplay : MonoBehaviour
{
    public GameObject projectedBall;
    public GameObject realPositionBall;
    public GameObject userBall;
    public TextMeshPro inputtedText;
    public TextMeshPro timer;

    private string filePath = "C:/Users/awefel2/Desktop/EyeTrackingData/2025-03-03-16-14TutorialSwiperProgression.txt";
    private float projectionConstant = 2; // Equal to Z value of keyboard, whatever that may be
    private float speedFactor = 2f;

    private List<(Vector4, Vector4, Vector4, string)> positions = new List<(Vector4, Vector4, Vector4, string)>();
    private bool isPlaying = false;
    private bool isPaused = false; 
    private float playbackSpeed = 1f;
    private int currentIndex = 0;
    private Coroutine playbackCoroutine = null;

    void Start()
    {
        LoadCSV();
        projectedBall.GetComponent<Renderer>().material.color = Color.red;
        realPositionBall.GetComponent<Renderer>().material.color = Color.cyan;
        userBall.GetComponent<Renderer>().material.color = Color.green;
    }

    void Update()
    {
        // Pause/unpause with P
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (isPlaying)
            {
                if (isPaused)
                {
                    isPaused = false;
                }
                else
                {
                    isPaused = true;
                }
            }
            else
            {
                playbackCoroutine = StartCoroutine(ReplayMovement());
            }
        }

        // Reset to beginning
        if (Input.GetKeyDown(KeyCode.R) && isPlaying)
        {
            StopCoroutine(playbackCoroutine);
            currentIndex = 0;
            isPaused = false;
            playbackCoroutine = StartCoroutine(ReplayMovement());
        }

        // Speed
        if (Input.GetKeyDown(KeyCode.UpArrow))
            playbackSpeed *= speedFactor;
        if (Input.GetKeyDown(KeyCode.DownArrow))
            playbackSpeed /= speedFactor;

        // Skip and rewind
        if (Input.GetKeyDown(KeyCode.RightArrow) && isPlaying)
            SkipTime(5f);
        if (Input.GetKeyDown(KeyCode.LeftArrow) && isPlaying)
            SkipTime(-5f);
    }

    void LoadCSV()
    {
        if (File.Exists(filePath))
        {
            string[] lines = File.ReadAllLines(filePath);
            for (int i = 10; i < lines.Length - 1; i++) // Skip header, arbitrarily picked 10 lines
            {
                string line = lines[i].Trim();
                // Checking if the line starts with a digit (0-9) or a '-' sign
                if (line.Length > 0 && (char.IsDigit(line[0]) || line[0] == '-'))
                {
                    string[] values = line.Split(',');
                    try
                    {
                        // Trim each value to remove any leading/trailing spaces
                        for (int j = 0; j < values.Length; j++)
                        {
                            values[j] = values[j].Trim();
                        }

                        float gazeDirectionX = float.Parse(values[0]);
                        float gazeDirectionY = float.Parse(values[1]);
                        float gazeDirectionZ = float.Parse(values[2]);
                        float gazeOriginX = float.Parse(values[3]);
                        float gazeOriginY = float.Parse(values[4]);
                        float gazeOriginZ = float.Parse(values[5]);
                        float depth = float.Parse(values[6]);
                        float time = float.Parse(values[7]);
                        string word = values[8];

                        Vector3 gazeDirectionCombined = new Vector3(gazeDirectionX, gazeDirectionY, gazeDirectionZ);
                        Vector3 gazeOriginCombined = new Vector3(gazeOriginX, gazeOriginY, gazeOriginZ);
                        Vector3 gazeLocationCombined = gazeOriginCombined + depth * gazeDirectionCombined;

                        Vector3 direction = gazeLocationCombined - gazeOriginCombined;
                        float t = (projectionConstant - gazeOriginCombined.z) / direction.z;
                        Vector3 intersectionPoint = gazeOriginCombined + t * direction;

                        positions.Add((new Vector4(gazeLocationCombined.x, gazeLocationCombined.y, gazeLocationCombined.z, time), new Vector4(intersectionPoint.x, intersectionPoint.y, intersectionPoint.z, time), new Vector4(gazeOriginCombined.x, gazeOriginCombined.y, gazeOriginCombined.z, time), word));
                    }
                    catch (System.FormatException e)
                    {
                        Debug.LogError("FormatException at line " + i + ": " + line);
                        Debug.LogError("Error detail: " + e.Message);
                    }
                }
            }
        }
        else
        {
            Debug.LogError("CSV file not found: " + filePath);
        }
    }


    IEnumerator ReplayMovement()
    {
        isPlaying = true;

        while (currentIndex < positions.Count - 1)
        {
            if (isPaused)
            {
                yield return null; // Wait to be unpaused
                continue;
            }

            Vector4 realPos = positions[currentIndex].Item1;
            Vector4 projectedPos = positions[currentIndex].Item2;
            Vector4 userPos = positions[currentIndex].Item3;
            string word = positions[currentIndex].Item4;

            Vector4 nextRealPos = positions[currentIndex + 1].Item1;
            Vector4 nextProjectedPos = positions[currentIndex + 1].Item2;
            Vector4 nextUserPos = positions[currentIndex + 1].Item3;

            float startTime = realPos.w;

            timer.text = startTime.ToString("F2");

            float endTime = nextRealPos.w;
            float duration = (endTime - startTime) / playbackSpeed;
            float elapsedTime = 0f;

            if (word == "None")
                inputtedText.text = "";
            else
                inputtedText.text = word;

            while (elapsedTime < duration)
            {
                if (isPaused)
                {
                    yield return null; // Wait until unpaused
                    continue;
                }

                Vector3 newRealPos = Vector3.Lerp(new Vector3(realPos.x, realPos.y, realPos.z), new Vector3(nextRealPos.x, nextRealPos.y, nextRealPos.z), elapsedTime / duration);
                Vector3 newProjectedPos = Vector3.Lerp(new Vector3(projectedPos.x, projectedPos.y, projectedPos.z), new Vector3(nextProjectedPos.x, nextProjectedPos.y, nextProjectedPos.z), elapsedTime / duration);
                Vector3 newUserPos = Vector3.Lerp(new Vector3(userPos.x, userPos.y, userPos.z), new Vector3(nextUserPos.x, nextUserPos.y, nextUserPos.z), elapsedTime / duration);

                if (!float.IsNaN(newRealPos.x) && !float.IsNaN(newRealPos.y) && !float.IsNaN(newRealPos.z))
                    realPositionBall.transform.position = newRealPos;

                if (!float.IsNaN(newProjectedPos.x) && !float.IsNaN(newProjectedPos.y) && !float.IsNaN(newProjectedPos.z))
                    projectedBall.transform.position = newProjectedPos;

                if (!float.IsNaN(newUserPos.x) && !float.IsNaN(newUserPos.y) && !float.IsNaN(newUserPos.z))
                    userBall.transform.position = newUserPos;

                elapsedTime += Time.deltaTime * playbackSpeed;
                yield return null;
            }

            currentIndex++;
        }

        isPlaying = false;
    }

    void SkipTime(float seconds)
    {
        float targetTime = positions[currentIndex].Item1.w + seconds;
        int newIndex = currentIndex;

        for (int i = 0; i < positions.Count; i++)
        {
            if (positions[i].Item1.w >= targetTime)
            {
                newIndex = i;
                break;
            }
        }

        // Check bounds
        newIndex = Mathf.Clamp(newIndex, 0, positions.Count - 1);
        currentIndex = newIndex;
    }
}
