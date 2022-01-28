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

    public static bool isGameOver=false;

    public static void ResetAllVariables() {
        startPosX = -1;
        startPosY = -1;
        isMoving = false;
        score = 0;
        isGameOver=false;
    }
}
