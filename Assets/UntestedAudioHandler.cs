using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UntestedAudioHandler : MonoBehaviour
{
    private List<AudioSource> audioSources = new List<AudioSource>();

    void Start()
    {
        foreach (Transform child in transform)
        {
            foreach (Transform grandchild in child)
            {
                AudioSource source = grandchild.GetComponent<AudioSource>();
                int count = 0;
                if (source != null)
                {
                    audioSources.Add(source);
                    count++;
                    print(count + " audio sources added");
                }
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            PlayRandomSound();
        }
    }

    void PlayRandomSound()
    {
        if (audioSources.Count > 0)
        {
            int randomIndex = Random.Range(0, audioSources.Count);
            audioSources[randomIndex].Play();
        }
    }
}