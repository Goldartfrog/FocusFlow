using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightTrail : MonoBehaviour
{
    private float opacity;
    public float tickLength;
    private bool isActive;
    [SerializeField]
    private Light myLight;
    
    // Start is called before the first frame update
    void Start()
    {
        opacity = 100f;
        isActive = false;
    }

    void OnEnable() {
        opacity = 100f;
        isActive = true;

        StartCoroutine(Fade());
    }

    public IEnumerator Fade() {
        yield return new WaitForSeconds(tickLength);
        opacity -= 1;
        myLight.intensity = opacity;
        if (opacity <= 0) {
            this.gameObject.SetActive(false);
            isActive = false;
        }
        StartCoroutine(Fade());
    }
}
