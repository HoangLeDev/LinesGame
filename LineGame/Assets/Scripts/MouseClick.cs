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
        if(Ball.isGameOver==false)
        {
            /////Take the x,y position from name of Tile
            string name = this.gameObject.name;

            int from = name.IndexOf("e")+2; // take 3rd char from "e". Ex: Tile 5X3
            string x = name.Substring(from,1);
            Debug.Log("X: "+x);

            from = name.IndexOf("X")+1; // take 2nd char from "X"
            string y = name.Substring(from,1);
            Debug.Log("Y: "+y);



            if(transform.Find("Ball") != null) //if the chosen place have a ball
            {
                if(Ball.ballObject != null) //check if there is a chosen ball, reset it
                {
                    Ball.ballObject.GetComponent<SelectedBallAnimation>().enabled = false;
                    Ball.ballObject.transform.localScale = new Vector2(0.25f,0.25f); //reset the scale
                }
                //take the start position, preparing for moving
                Ball.startPosX= int.Parse(x);
                Ball.startPosY= int.Parse(y);
                // set up for new ball selection
                Ball.ballObject = transform.Find("Ball").gameObject;
                Ball.ballObject.GetComponent<SelectedBallAnimation>().enabled = true;
            }
            else //if player click to the empty place, then the ball will move to the destination
            {
                if(Ball.startPosX == -1) return; //There is no start point of ball -> return.
                
                int xPos = int.Parse(x);
                int yPos = int.Parse(y);
                Debug.Log("x: "+xPos +"\n" +"y: "+yPos);
                //Move the ball to the destination
                GameObject.Find("GameManager").GetComponent<GameManager>().InitializeBallMovement(Ball.ballObject, xPos,yPos);
            }
        }
    }
}
