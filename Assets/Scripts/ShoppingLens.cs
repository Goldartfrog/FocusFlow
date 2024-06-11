using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShoppingLens : UIObject
{
    public override void Hover()
    {
        
    }

    public override void Activate(string ProductName)
    {
        isLocked = true;

        /* YOUR CODE HERE */


        status = 2;
        isLocked = false;
    }

    public override void Deactivate()
    {
        isLocked = true;

        /* YOUR CODE HERE */
        

        status = 0;
        isLocked = false;
    }

    // Start is called before the first frame update
    public new void Start()
    {
        // Base Start()
        status = 0;
        stepTime = 0.02f;
        defaultColor = this.GetComponent<Image>().color;
        isLocked = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
