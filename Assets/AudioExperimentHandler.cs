using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class AudioExperimentHandler : MonoBehaviour
{
    private List<AudioSource> zConstantHints = new List<AudioSource>();
    private List<AudioSource> yConstantHints = new List<AudioSource>();

    private List<AudioSource> currentZSequence = new List<AudioSource>();
    private List<AudioSource> currentYSequence = new List<AudioSource>();

    private int currentSoundIndex = 0;
    private bool sequencePlaying = false;
    void Start()
    {
        if (transform.childCount >= 2)
        {
            foreach (Transform child in transform.GetChild(0)) //ZConstantHints or vertical
            {
                AudioSource source = child.GetComponent<AudioSource>();
                if (source != null) zConstantHints.Add(source);
            }

            foreach (Transform child in transform.GetChild(1)) //YConstantHints or horizontal
            {
                AudioSource source = child.GetComponent<AudioSource>();
                if (source != null) yConstantHints.Add(source);
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && !sequencePlaying)
        {
            GenerateNewSequences();
            PlayCurrentSound();
        }
        else if (Input.GetKeyDown(KeyCode.F) && sequencePlaying) //replay sound
        {
            PlayCurrentSound();
        }
        else if (Input.GetKeyDown(KeyCode.J) && sequencePlaying) //next sound
        {
            NextSound();
        }
    }

    void GenerateNewSequences()
    {
        currentZSequence = zConstantHints.OrderBy(x => Random.value).Take(5).ToList();
        currentYSequence = yConstantHints.OrderBy(x => Random.value).Take(5).ToList();

        currentSoundIndex = 0;
        sequencePlaying = true;
    }

    void PlayCurrentSound()
    {
        if (currentSoundIndex < 5)
        {
            currentZSequence[currentSoundIndex].Play();
        } else if (currentSoundIndex < 10)
        {
            currentYSequence[currentSoundIndex - 5].Play();
        }
    }

    void NextSound()
    {
        currentSoundIndex++;

        if (currentSoundIndex < 10)
        {
            PlayCurrentSound();
        }
        else
        {
            sequencePlaying = false;
        }
    }
}
