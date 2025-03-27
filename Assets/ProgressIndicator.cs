using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressIndicator : MonoBehaviour
{
    private List<GameObject> indicators = new List<GameObject>();
    [SerializeField]
    private int sentencesPerRound = 5;

    [SerializeField]
    private GameObject indicatorPrefab;
    
    public Material unfulfilledMat;
    public Material fulfilledMat;

    public int currProgress;
    public GameObject popup;
    
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < sentencesPerRound; i++) {
            GameObject currIndicator = Instantiate(indicatorPrefab, this.transform.position, this.transform.rotation);
            currIndicator.transform.eulerAngles += new Vector3(90, 0, 0);
            currIndicator.GetComponent<Renderer>().material = unfulfilledMat;
            indicators.Add(currIndicator);
        }
        //Hopefully five at most.
        if (sentencesPerRound % 2 == 0) {
            int half = sentencesPerRound / 2;
            float initialX = -1 * ((half - 1) * 0.5f + 0.25f);
            for (int j = 0; j < sentencesPerRound; j++) {
                indicators[j].transform.position = this.transform.position + new Vector3((initialX + ((float) j * .5f)), 0, 0);
            }
        } else {
            int half = (sentencesPerRound - 1) / 2;
            float initialX = -1 * (half) * 0.5f;
            for (int k = 0; k < sentencesPerRound; k++) {
                indicators[k].transform.position = this.transform.position + new Vector3((initialX + ((float) k * 0.5f)), 0, 0);
            }
        }
        currProgress = 0;
    }

    // // Update is called once per frame
    // void Update()
    // {
        
    // }

    public void ReceiveUpdate() {
        indicators[currProgress].GetComponent<Renderer>().material = fulfilledMat;
        currProgress += 1;
        if (currProgress >= sentencesPerRound) {
            //Call popup
            Popup();
        }
    }

    public void NextStage() {
        currProgress = 0;
        popup.GetComponent<IntroPopup>().Deactivate();
        for (int i = 0; i < sentencesPerRound; i++) {
            indicators[i].GetComponent<Renderer>().material = unfulfilledMat;
        }
    }

    public void Popup() {
        popup.SetActive(true);
        popup.GetComponent<IntroPopup>().OnActivate();
    }
}
