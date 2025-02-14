using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightTrailManager : MonoBehaviour
{
    private List<GameObject> lightPool = new List<GameObject>();

    [SerializeField]
    private int numLights;
    [SerializeField]
    private GameObject prefab;

    public bool onKeyboard;
    
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < numLights; i++) {
            GameObject newLight = Instantiate(prefab, this.transform);
            lightPool.Add(newLight);
            newLight.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
