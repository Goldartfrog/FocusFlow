using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class Timer : MonoBehaviour
{
    private int time = 0;
    [SerializeField]
    private TextMeshProUGUI timerRef;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(RunTimer());
    }

    // Update is called once per frame

    public String DetermineText() {
        int minutes = (int) Math.Floor((double) time / 60);
        int seconds = time % 60;
        String minString = minutes.ToString();
        if (minutes < 10) {
            minString = "0" + minString;
        }
        String secString = seconds.ToString();
        if (seconds < 10) {
            secString = "0" + secString;
        }

        String ret = minString + ":" + secString;
        return ret;
    }

    public IEnumerator RunTimer() {
        yield return new WaitForSeconds(1.0f);
        time += 1;
        timerRef.text = DetermineText();
        StartCoroutine(RunTimer());
    }
}
