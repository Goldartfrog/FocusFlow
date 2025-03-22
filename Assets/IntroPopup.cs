using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class IntroPopup : MonoBehaviour
{
    public List<Material> materials;
    private int currStage;
    private ProgressionManager progressionManager;
    // Start is called before the first frame update
    void Start()
    {
        currStage = 0;
        progressionManager = GameObject.Find("ProgressionManager").GetComponent<ProgressionManager>();
    }

    public void OnActivate() {
        // this.GetComponent<Renderer>().material = materials[currStage];
        this.transform.GetChild(1).transform.GetChild(0).GetComponent<TextMeshPro>().text = progressionManager.GetCurrentStageInfo().prescreen;
    }
    public void Deactivate() {
        currStage += 1;
        this.gameObject.SetActive(false);
    }



}
