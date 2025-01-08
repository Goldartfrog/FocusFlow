using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class OutputData : MonoBehaviour
{
    private static float totalTime = 0f;
    public static void WriteGazePoints(List<Vector3> gazePoints, string word, string path, List<string> topwords)
    {
        using (StreamWriter stream = new FileInfo(path).AppendText())
        {
            for (int i = 0; i < gazePoints.Count; i++)
            {
                var point = gazePoints[i];
                stream.WriteLine(word + "," + point.x + "," + point.y + "," + point.z);
            }

            stream.WriteLine("Time: " + totalTime.ToString());
            string s = "";
            for (int i = 0; i < topwords.Count; i++)
            {
                s += topwords[i] + ", ";
            }
            stream.Write(s);
            stream.WriteLine("");
        }
    }
    public static void UpdateTime()
    {
        totalTime += Time.deltaTime;
    }

    public static void WriteSuggestionAccepted(string path, string word)
    {
        using (StreamWriter stream = new FileInfo(path).AppendText())
        {
            stream.WriteLine("---Suggestion accepted--- : " + word + " Time: " + totalTime.ToString());
        }
    }

    public static void WriteDeletePressed(string path)
    {
        using (StreamWriter stream = new FileInfo(path).AppendText())
        {
            stream.WriteLine("___Delete pressed___" + " Time: " + totalTime.ToString());
        }
    }

}
