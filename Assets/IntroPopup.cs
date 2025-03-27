using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class IntroPopup : MonoBehaviour
{
    public List<Material> materials;
    private int currStage;
    [SerializeField]
    private ProgressionManager progressionManager;
    [SerializeField]
    private TextMeshProUGUI popupText;
    [SerializeField]
    private Timer timer;
    // Start is called before the first frame update
    void Start()
    {
        currStage = 0;

        if (progressionManager == null) {
            progressionManager = GameObject.Find("ProgressionManager").GetComponent<ProgressionManager>();
        }
    }

    public void OnActivate() {
        // this.GetComponent<Renderer>().material = materials[currStage];
        popupText.text = progressionManager.GetCurrentStageInfo().prescreen;
        timer.StopTimer();
    }
    public void Deactivate() {
        currStage += 1;
        this.gameObject.SetActive(false);
    }



}
