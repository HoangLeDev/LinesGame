using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]private GameObject _tilePrefab;
    [SerializeField]private GameObject tilesContainer;
    [SerializeField]private int numOfRows;
    [SerializeField]private int numOfColumns;
    [SerializeField]private int numOfColors;
    private bool isFirstWave = true;
    // Start is called before the first frame update
    [SerializeField]private GameObject[] _ballPrefabs;
    void Start()
    {
        tilesContainer = GameObject.Find("TilesContainer");
        InitializeFields(numOfRows,numOfColumns);
        CreateTileMap();
        CreateNewBalls();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    // Initialize the field when start
    private void InitializeFields(int rows, int columns)
    {
        Ball.fields = new int[rows,columns];
        for (int i = 0; i < Ball.fields.GetLength(0); i++)
        {
            for (int j = 0; j < Ball.fields.GetLength(1); j++)
            {
                Ball.fields[i,j] = 0;
            }
        }
    }

    //Create the tile map
    private void CreateTileMap()
    {
        for (int i = 0; i < Ball.fields.GetLength(0); i++)
        {
            for (int j = 0; j < Ball.fields.GetLength(1); j++)
            {
                GameObject tile = Instantiate(_tilePrefab, new Vector2(i,j), Quaternion.identity);
                tile.name = $"Tile {i} {j}";
                tile.transform.parent = tilesContainer.transform;
            }
        }
        //Make camera focus on the map base on the position of tilesContainer
        Camera.main.transform.position = new Vector3(tilesContainer.transform.position.x + numOfRows/2, tilesContainer.transform.position.y + numOfColumns/2, -1 );

    }

    private void CreateNewBalls()
    {
        int numberOfBallsToCreate;
        if(isFirstWave)
        {
            numberOfBallsToCreate = 5;
            isFirstWave=false;

            while(numberOfBallsToCreate!=0)
            {
                int ballX = Random.Range(0,numOfRows);
                int ballY = Random.Range(0,numOfColumns);
                if(Ball.fields[ballX,ballY] !=0) continue; // check if array index =0, there is available ball in spawning place

                int ballColor = Random.Range(0, numOfColors); //maximum exclusive, then 0-6
                Ball.fields[ballX,ballY] = ballColor +1; //+1 prevent from the array index =0
                GameObject ball = (GameObject)Instantiate(_ballPrefabs[ballColor], new Vector3(ballX,ballY,-0.1f), Quaternion.identity);
                ball.name = "Ball";
                ball.transform.parent = GameObject.Find($"Tile {ballX} {ballY}").transform;
                numberOfBallsToCreate--;
                Debug.Log("Color: "+ballColor);
            }
        }
    }
}
