using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class TestControl : MonoBehaviour
{
    private string folderPath;
    
    public bool WriteFlag;
    public bool StartFlag;
    private string participantId;

    public GameObject Guidance_1;
    public GameObject Guidance_2;

    // Start is called before the first frame update
    void Start()
    {
        participantId = File.ReadAllText(System.IO.Path.Combine("Assets/Data", "participantID.txt"), Encoding.UTF8);
        string folderName = SceneManager.GetActiveScene().name;
        folderPath = System.IO.Path.Combine("Assets/Data", folderName, participantId);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            GameObject.Find("Interaction").GetComponent<Interaction>().UseGuidance_1 = !GameObject.Find("Interaction").GetComponent<Interaction>().UseGuidance_1;
            if (!GameObject.Find("Interaction").GetComponent<Interaction>().UseGuidance_1)
            {
                Guidance_1.SetActive(false);
            }
            else
            {
                Guidance_1.SetActive(true);
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            GameObject.Find("Interaction").GetComponent<Interaction>().UseGuidance_2 = !GameObject.Find("Interaction").GetComponent<Interaction>().UseGuidance_2;
            if (!GameObject.Find("Interaction").GetComponent<Interaction>().UseGuidance_2)
            {
                Guidance_2.SetActive(false);
            }
            else
            {
                Guidance_2.SetActive(true);
            }
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            GameObject.Find("Eye Tracker").GetComponent<ViveSR.anipal.Eye.EyeTracker>().useLineRenderer = !GameObject.Find("Eye Tracker").GetComponent<ViveSR.anipal.Eye.EyeTracker>().useLineRenderer;
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            Time.timeScale = 1;
            WriteFlag = true;
            StartFlag = true;
            Debug.Log("STARTED!!!!!");
        }

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            GameObject.Find("Interaction").SendMessage("GuidanceSwitch1");
        }

        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            GameObject.Find("Interaction").SendMessage("GuidanceSwitch2");
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameObject.Find("Interaction").SendMessage("GuidanceSwitch3");
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Time.timeScale = 0;
            Debug.Log("PAUSED!!!!!");
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            SceneManager.LoadScene("Test");
        }

    }
}
