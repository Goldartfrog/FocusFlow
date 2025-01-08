using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebuggingPositions : MonoBehaviour
{
    private TextMeshProUGUI temp;
    [SerializeField] private KeyboardExperimentManager keyboardExperimentManager;
    // Start is called before the first frame update
    void Start()
    {
        temp = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        temp.text = keyboardExperimentManager.GetGazePoint().ToString("F4");
    }
}
