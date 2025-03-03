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
    [SerializeField]
    private float spawnInterval;

    [SerializeField]
    private FollowGazeIntercept gazerRef;
    private int curIdx = 0; //index of next object to activate in pool

    public bool onKeyboard;
    
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < numLights; i++) {
            GameObject newLight = Instantiate(prefab, this.transform);
            lightPool.Add(newLight);
            newLight.SetActive(false);
        }
        onKeyboard = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (gazerRef.seeingKeyboard) {
            Debug.Log("Seeing board");
            if (onKeyboard == false) {
                Debug.Log("Ever get here?");
                onKeyboard = true;

                StartCoroutine(SpawnTrail());
            }
            onKeyboard = true;
        } else {
            Debug.Log("not seeing board");
            onKeyboard = false;
        }
    }

    public IEnumerator SpawnTrail() {
        if (onKeyboard) {
            yield return new WaitForSeconds(spawnInterval);
            lightPool[curIdx].transform.position = this.transform.position;
            lightPool[curIdx].SetActive(true);
            curIdx = (curIdx + 1) % lightPool.Count;
            Debug.Log("curidx: " + curIdx);
            StartCoroutine(SpawnTrail());
        }
        
    }
}
