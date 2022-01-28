using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    //Game Fields
    public static int[,] fields;
    //Ball is moving
    public static bool isMoving = false;
    public static GameObject ballObject;

    //Init for start position of ball
    public static int startPosX = -1;
    public static int startPosY = -1;

    public static int score =0;
}
