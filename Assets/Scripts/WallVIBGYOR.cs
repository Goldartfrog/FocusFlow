using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallVIBGYOR : UIObject
{
    [SerializeField]
    private List<GameObject> walls = new List<GameObject>();

    [SerializeField]
    private int minLayer = 8;

    private void Update()
    {
        //SetCurrentWall(Random.Range(minLayer, walls.Capacity + minLayer));
    }

    public void SetCurrentWall(int currentWall)
    {
        Debug.Log("Current wall: " + currentWall);
        for (int i = 0; i < walls.Capacity; i++)
        {
            MeshRenderer renderer = walls[i].GetComponent<MeshRenderer>();
            if (i < currentWall - minLayer)
            {
                renderer.enabled = false;
                //StartCoroutine(FadeOut(renderer));
            }
            else
            {
                renderer.enabled = true;
                //StartCoroutine(FadeIn(renderer));
            }
        }
    }

    IEnumerator FadeOut(MeshRenderer renderer)
    {
        Color c = renderer.material.color;
        for (float alpha = c.a; alpha >= 0; alpha -= 0.1f)
        {
            c.a = alpha;
            renderer.material.color = c;
            yield return new WaitForSeconds(.1f);
        }
    }

    IEnumerator FadeIn(MeshRenderer renderer)
    {
        Color c = renderer.material.color;
        for (float alpha = c.a; alpha <= 0.5f; alpha += 0.1f)
        {
            c.a = alpha;
            renderer.material.color = c;
            yield return new WaitForSeconds(.1f);
        }
    }

    public override void Hover()
    {

    }

    public override void Activate(string name)
    {

    }

    public override void Deactivate()
    {

    }
}
