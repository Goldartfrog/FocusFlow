using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class EyeReplay : MonoBehaviour
{
    public string filePath = "ball_data.csv";
    public GameObject ball;
    private List<Vector4> positions = new List<Vector4>();
    private bool isPlaying = false;

    void Start()
    {
        LoadCSV();

        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = Color.red;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) && !isPlaying)
        {
            StartCoroutine(ReplayMovement());
        }
    }

    void LoadCSV()
    {
        string fullPath = Path.Combine(Application.dataPath, filePath);
        if (File.Exists(fullPath))
        {
            string[] lines = File.ReadAllLines(fullPath);
            for (int i = 1; i < lines.Length; i++) // Skip header
            {
                string[] values = lines[i].Split(',');
                float x = float.Parse(values[0]);
                float y = float.Parse(values[1]);
                float z = float.Parse(values[2]);
                float time = float.Parse(values[3]);
                positions.Add(new Vector4(x, y, z, time));
            }
        }
        else
        {
            Debug.LogError("CSV file not found: " + fullPath);
        }
    }

    IEnumerator ReplayMovement()
    {
        isPlaying = true;
        for (int i = 0; i < positions.Count - 1; i++)
        {
            Vector3 startPos = new Vector3(positions[i].x, positions[i].y, positions[i].z);
            Vector3 endPos = new Vector3(positions[i + 1].x, positions[i + 1].y, positions[i + 1].z);
            float startTime = positions[i].w;
            float endTime = positions[i + 1].w;
            float duration = endTime - startTime;
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                ball.transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            ball.transform.position = endPos;
        }
        isPlaying = false;
    }
}
