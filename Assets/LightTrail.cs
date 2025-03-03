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
        myLight = this.transform.gameObject.GetComponent<Light>();
    }

    void OnEnable() {
        opacity = 100f;
        isActive = true;
        myLight.range = 0.15f;
        myLight.intensity = opacity;
        Debug.Log("Enabled");
        StartCoroutine(FadeAndShrink());
    }

    public IEnumerator FadeAndShrink() {
        yield return new WaitForSeconds(tickLength);
        opacity -= 5;
        myLight.intensity = opacity;
        myLight.range -= 0.01f;
        if (opacity <= 0) {
            this.gameObject.SetActive(false);
            isActive = false;
        } else {
            StartCoroutine(FadeAndShrink());
        }
    }
}
