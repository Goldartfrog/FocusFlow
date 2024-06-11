using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipPanel : UIObject
{
    [SerializeField]
    private AutoFlip book;

    private void Right()
    {
        book.FlipRightPage();
    }

    private void Left()
    {
        book.FlipLeftPage();
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

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(moving_speed);
        //Vector3 currPos = this.transform.position, nextPos;
        //nextPos.x = currPos.x;
        //nextPos.y = currPos.y + moving_speed;
        //nextPos.z = currPos.z;
        //this.transform.Translate(nextPos - currPos);
    }
}
