using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallTransparency : UIObject
{
    [SerializeField]
    private GameObject prefab;

    [SerializeField]
    private List<GameObject> walls = new List<GameObject>();

    [SerializeField]
    private int currentWall = 0;

    [SerializeField]
    private int numWalls = 100;

    private void Awake()
    {
        for (int i = 0; i < numWalls; i++)
        {
            Vector3 position = transform.position + new Vector3(0, 0, ((float) i) / numWalls);
            GameObject newWall = Instantiate(prefab, position, Quaternion.identity, transform);
            newWall.transform.localScale = new Vector3(1, 1, ((float)1) / numWalls);
            
            walls.Add(newWall);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            Debug.Log("pressing w");
            DisableCurrentWall();
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log("pressing s");
            EnableCurrentWall();
        }
    }

    public void SetCurrentWall(int currentWall)
    {
        this.currentWall = Mathf.Clamp(currentWall, 0, walls.Count - 1);
    }

    private void EnableCurrentWall()
    {
        walls[currentWall].SetActive(true);
        SetCurrentWall(currentWall - 1);
    }

    private void DisableCurrentWall()
    {
        walls[currentWall].SetActive(false);
        SetCurrentWall(currentWall + 1);
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
