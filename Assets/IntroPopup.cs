using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroPopup : MonoBehaviour
{
    public List<Material> materials;
    private int currStage;
    // Start is called before the first frame update
    void Start()
    {
        currStage = 0;
    }

    public void OnActivate() {
        this.GetComponent<Renderer>().material = materials[currStage];
    }
    public void Deactivate() {
        currStage += 1;
        this.gameObject.SetActive(false);
    }

}
