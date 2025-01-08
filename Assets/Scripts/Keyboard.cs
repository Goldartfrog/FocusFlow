using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Keyboard : MonoBehaviour
{

    [SerializeField] private string key = "";
    public void SetKey(string key)
    {
        this.key = key;
        Transform canvas = transform.GetChild(0);
        Transform tmp = canvas.GetChild(0);
        Debug.Log(tmp);
        TextMeshProUGUI textcomponent = tmp.gameObject.GetComponent<TextMeshProUGUI>();
        textcomponent.text = key + "";
        //tmp.GetComponent<TextMeshPro>().text = "k";
    }

    public string GetKey() { return this.key; }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
