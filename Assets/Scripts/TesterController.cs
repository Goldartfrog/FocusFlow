using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class TesterController : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI TypingField;
    [SerializeField] private TextMeshProUGUI TargetWordField;
    [SerializeField] private List<string> words;
    [SerializeField] private AudioSource successSound;
    private int completedWords = 0;
    private bool typedYet = false;
    private float timer = 0f;
    // Start is called before the first frame update
    void Start()
    {
        TargetWordField.text = words[completedWords];
    }

    // Update is called once per frame

    public void WordProgress(string k)
    {
        if (completedWords < 10)
        {
            WriteToFile(k);
            if (TypingField.text == words[completedWords])
            {
                completedWords++;
                TypingField.text = "";
                successSound.Play();

                Debug.Log($"Total time elapsed: {timer}");
                Debug.Log("wpm: " + (completedWords) / (timer / 60));

                if (completedWords != 10)
                {
                    TargetWordField.text = words[completedWords];
                }
            }
        }
    }
    void Update()
    {
        if (typedYet == false)
        {
            if (TypingField.text != "") 
            {
                typedYet = true;

            }
        }
        else
        {
            timer += Time.deltaTime;
            //if (completedWords < 10)
            //{
            //    timer += Time.deltaTime;
            //    if (TypingField.text == words[completedWords])
            //    {
            //        completedWords++;
            //        TypingField.text = "";
            //        successSound.Play();

            //        Debug.Log(timer);
            //        Debug.Log("wpm: " + (10) / (timer / 60));
                    
            //        if (completedWords != 10)
            //        {
            //            TargetWordField.text = words[completedWords];
            //        }
            //    }
            //}
        }
        
    }

    void WriteToFile(string typedKey)
    {
        //string folderName = "AidanPushKeybardTestData";
        //string participantId = "0";
        //var folderPath = System.IO.Path.Combine("Assets/Data", folderName, participantId);
        //using (StreamWriter sw = new StreamWriter(System.IO.Path.Combine(folderPath, "typingInfo.txt")))
        //{
        //    sw.WriteLine("{0}, {1}, {2}", words[completedWords], typedKey, timer);
        //}
    }
}
