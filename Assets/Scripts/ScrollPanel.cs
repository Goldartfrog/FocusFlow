using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollPanel : UIObject
{
    private float moving_speed;

    private void Up(float speed)
    {
        moving_speed = speed;
    }

    private void Down(float speed)
    {
        moving_speed = speed;
    }

    private void Stop(float depth_diff)
    {
        moving_speed = 0;
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

    // Start is called before the first frame update
    public new void Start()
    {
        moving_speed = 0;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(moving_speed);
        Vector3 currPos = this.transform.position, nextPos;
        nextPos.x = currPos.x;
        nextPos.y = currPos.y + moving_speed;
        nextPos.z = currPos.z;
        this.transform.Translate(nextPos - currPos);
    }
}
