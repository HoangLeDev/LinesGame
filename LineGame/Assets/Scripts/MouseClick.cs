using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseClick : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void OnMouseDown()
    {
        if(Ball.ballObject != null) //check if there is a chosen ball
        {
            Ball.ballObject.GetComponent<SelectedBallAnimation>().enabled = false;
            Ball.ballObject.transform.localScale = new Vector2(0.25f,0.25f); //reset the scale
        }
        Ball.ballObject = transform.Find("Ball").gameObject;
        Ball.ballObject.GetComponent<SelectedBallAnimation>().enabled = true;
        
    }
}
