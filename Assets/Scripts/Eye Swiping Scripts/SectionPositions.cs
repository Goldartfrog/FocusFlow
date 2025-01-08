using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SectionPositions : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        PrintPositions();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PrintPositions()
    {
        string st = "";
        foreach (Transform child in gameObject.transform.GetComponentInChildren<Transform>())
        {
            string name = child.GetComponentInChildren<TextMeshPro>().text;
            Transform pos = child.GetComponentInChildren<Transform>();
            st += name + " (" + pos.position.x.ToString("F4") + "," + pos.position.y.ToString("F4") + ")\n";
        }
        Debug.Log(st);
        
    }
    public string GetPositions2D()
    {
        string st = "";
        foreach (Transform child in gameObject.transform.GetComponentInChildren<Transform>())
        {
            string letters = child.GetComponentInChildren<TextMeshPro>().text;
            letters = letters.Replace(" ", "");
            letters = letters.Replace("\n", "");
            Transform pos = child.GetComponentInChildren<Transform>();
            st += letters + " (" + pos.position.x.ToString("F4") + "," + pos.position.y.ToString("F4") + ")\n";
        }
        st = st.Substring(0, st.Length - 2);
        return st;
    }
    public string GetPositions3D()
    {
        string st = "";
        foreach (Transform child in gameObject.transform.GetComponentInChildren<Transform>())
        {
            string letters = child.GetComponentInChildren<TextMeshPro>().text;
            letters = letters.Replace(" ", "");
            letters = letters.Replace("\n", "");
            Transform pos = child.GetComponentInChildren<Transform>();
            st += letters + " (" + pos.position.x.ToString("F4") + "," + pos.position.y.ToString("F4") + "," + pos.position.z.ToString("F4") + ")\n";
        }
        st = st.Substring(0, st.Length - 2);
        return st;
    }
}
