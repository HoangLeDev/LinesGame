using System.Collections;
using System.Collections.Generic;
using static System.Math;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameManager : MonoBehaviour
{
    [SerializeField]private GameObject _tilePrefab;
    [SerializeField]private GameObject tilesContainer;
    [SerializeField]private int numOfRows;
    [SerializeField]private int numOfColumns;
    [SerializeField]private int numOfColors;
    private int iPath = 100;
    private int lastScore = 0;
    private int[,] placeholderBalls;
    private bool isFirstWave = true;
    private int numberOfConsecutiveBalls = 1;
	private int currentCellValue = 0;
	private int k1 = 0, k2 = 0;
	private bool flag = true;
    // Start is called before the first frame update
    void OnEnable()
    {
        isFirstWave = true;
		numberOfConsecutiveBalls = 1;
		placeholderBalls = null;
		lastScore = 0;
		currentCellValue = 0;
		k1 = 0;
		k2 = 0;
		flag = true;

        numOfRows = GameObject.Find("DataPersistence").GetComponent<DataPersistence>().ChosenNumOfRowsData;
        numOfColumns = GameObject.Find("DataPersistence").GetComponent<DataPersistence>().ChosenNumOfColsData;

        InitializeFields(numOfRows,numOfColumns);
        tilesContainer = GameObject.Find("TilesContainer");
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
                GameObject tile = (GameObject)Instantiate(Resources.Load("Tile", typeof(GameObject)));
                tile.name = $"Tile {i}X{j}";
                tile.transform.position = new Vector2(i, j);
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
                GameObject ball = (GameObject)Instantiate(Resources.Load("Ball"+(ballColor+1), typeof(GameObject)));
                ball.name = "Ball";
                ball.transform.parent = GameObject.Find($"Tile {ballX}X{ballY}").transform;
                ball.transform.localPosition = new Vector2(0, 0);
                numberOfBallsToCreate--;
                //Debug.Log("Color: "+ballColor);
            }
            CreatePlaceholderBalls(3);
        }
        else
        {
            numberOfBallsToCreate=3; //balls will be created for next waves except the 1st wave
            CheckForAvailableFields(numberOfBallsToCreate);

            ConvertPlaceholderBallsToRealBalls();
            CheckScore();
            CreatePlaceholderBalls(numberOfBallsToCreate);
            if(lastScore != Ball.score)
            {
                lastScore = Ball.score;
            }
        }
    }

    // Create single ball when player steps on a placeholder ball
    private void CreateNewBall(int placeholderXPos, int placeholderYPos)
    {
        int numOfBallToCreate=1;
        while(numOfBallToCreate!=0)
        {
            int ballX =Random.Range(0, numOfRows);
            int ballY =Random.Range(0, numOfColumns);
            if(Ball.fields[ballX,ballY] !=0) continue;

            int ballColor = placeholderBalls[placeholderXPos, placeholderYPos];
            placeholderBalls[placeholderXPos,placeholderYPos]=0;
            Ball.fields[ballX, ballY] = ballColor;
            GameObject ball = (GameObject)Instantiate(Resources.Load("Ball" + ballColor, typeof(GameObject)));
            ball.name = "Ball";
            ball.transform.parent = GameObject.Find($"Tile {ballX}X{ballY}").transform;
            ball.transform.localPosition = new Vector2(0, 0);
            numOfBallToCreate--;
        }
        CheckScore();
        if(lastScore != Ball.score)
        {
            lastScore = Ball.score;
        }
    }

    private void CheckScore()
    {
        CheckForScoreVertically();
		CheckForScoreHorizontally();
		CheckForScoreDiagonally();
		ResetConsecuteBallsSearchingVariables();
		CheckForScoreDiagonallyTraverse(Ball.fields, 0, 0, Ball.fields.GetLength(0), Ball.fields.GetLength(1));
		GetComponent<GameUI> ().UpdateScore();
    }

    private void ConvertPlaceholderBallsToRealBalls()
    {
        if(placeholderBalls == null) return;
		for (int i=0; i < placeholderBalls.GetLength(0); i++) {
			for (int j=0; j < placeholderBalls.GetLength(1); j++) {
				if(placeholderBalls[i, j] != 0) {
					if(GameObject.Find("Tile " + i + "X" + j).transform.Find("Ball") == null) {
						Ball.fields[i, j] = placeholderBalls[i, j];
						placeholderBalls[i, j] = 0;
					}
					
					GameObject ball = GameObject.Find("Tile " + i + "X" + j).transform.Find("BallPlaceholder").gameObject;
					ball.name = "Ball";
					ball.transform.localScale = new Vector2(0.25f, 0.25f);
                    ball.transform.position = new Vector3(i,j,-0.1f);
					ball.GetComponent<SpriteRenderer> ().sortingOrder = 0;
				}
			}
		}
    }

    private void CreatePlaceholderBalls(int numberOfBallsToCreate)
    {
        int availableFields = CheckForAvailableFields(numberOfBallsToCreate);
		if(numberOfBallsToCreate > availableFields) {
			numberOfBallsToCreate = availableFields;
		}
		placeholderBalls = new int[Ball.fields.GetLength(0), Ball.fields.GetLength(1)];
		while(numberOfBallsToCreate != 0) {
			int ballXPos = UnityEngine.Random.Range(0, numOfRows);
			int ballYPos = UnityEngine.Random.Range(0, numOfColumns);
			if(Ball.fields[ballXPos, ballYPos] != 0 || placeholderBalls[ballXPos, ballYPos] != 0) continue;

			int ballColor = UnityEngine.Random.Range(0, numOfColors);
			placeholderBalls[ballXPos, ballYPos] = (ballColor + 1);
			GameObject ball = (GameObject)Instantiate(Resources.Load("Ball" + (ballColor + 1), typeof(GameObject)));
			ball.name = "BallPlaceholder";
			ball.transform.localScale = new Vector2(0.1f, 0.1f);
			ball.transform.parent = GameObject.Find("Tile " + ballXPos + "X" + ballYPos).transform;
            ball.transform.localPosition = new Vector2(0, 0);
			ball.GetComponent<SpriteRenderer> ().sortingOrder = 0;
			numberOfBallsToCreate--;
		}
    }

    private int CheckForAvailableFields(int numberOfBallsToCreate)
    {
        int numberOfEmptyFields = 0;
		for(int i = 0; i < Ball.fields.GetLength(0); i++) 
        {
			for(int j = 0; j < Ball.fields.GetLength(1); j++)
            {
				if(Ball.fields[i,j] == 0) 
                {
					numberOfEmptyFields++;
				}
            }
		}
        if(numberOfEmptyFields == 0) {//When there is no available field game over menu will pop up
			GetComponent<GameUI> ().GameOverMenu();
			return 0;
		}
		return numberOfEmptyFields;
    }

    
    private void ResetConsecuteBallsSearchingVariables() {
		numberOfConsecutiveBalls = 1;
		currentCellValue = 0;
		k1 = 0;
		k2 = 0;
		flag = true;
	}

    private void DestroyBall(int x, int y) {//If 5 or more balls are matched, this method gets called to destory each ball
		GameObject ball = GameObject.Find("Tile " + x + "X" + y).transform.Find("Ball").gameObject;
		ball.GetComponent<BallExplosion> ().ActivateBallExplosion();
		Destroy(ball);
	}

    private bool CheckForScoreDiagonallyTraverse(int [,]m, int i, int j, int row, int col)
    {
        if (i >= row || j >= col) 
        {
			if (flag) 
            {
				int a = k1;
				k1 = k2;
				k2 = a;
				flag = !flag;
				k1++;
			} 
            else 
            {
				int a = k1;
				k1 = k2;
				k2 = a;
				flag = !flag;
			}
			numberOfConsecutiveBalls = 1;
			currentCellValue = 0;
			return false;
        }

        if(Ball.fields[i, j] != 0) 
        {
            if(Ball.fields[i, j] == currentCellValue) 
            {
                numberOfConsecutiveBalls++;
                if(numberOfConsecutiveBalls >= 5) 
                {
                    for(int y = 0; y < numberOfConsecutiveBalls; y++) 
                    {
                        Ball.score++;
                        Ball.fields[i - y, j - y] = 0;
                        DestroyBall((i - y), (j - y));
                    }	
                }
            }
            else 
            {
                currentCellValue =  Ball.fields[i, j];
                numberOfConsecutiveBalls = 1;
            }
        }
        else 
        {
            numberOfConsecutiveBalls = 1;
            currentCellValue = 0;
        }
    
        if (CheckForScoreDiagonallyTraverse(m, i + 1, j + 1, row, col)) {
            return true;
        }
        
        if (CheckForScoreDiagonallyTraverse(m, k1, k2, row, col)) {
            return true;
        }
        return true;
    }
    

    private void CheckForScoreVertically() {
		for(int i = 0; i < Ball.fields.GetLength(0); i++) {
			ResetConsecuteBallsSearchingVariables();
			for(int j = 0; j < Ball.fields.GetLength(1); j++) {
				if(Ball.fields[i,j] != 0) {
					if(Ball.fields[i,j] == currentCellValue) {
						numberOfConsecutiveBalls++;
						if(numberOfConsecutiveBalls >= 5) {
							for(int y = j - numberOfConsecutiveBalls + 1; y <= j; y++) {
								Ball.score++;
								Ball.fields[i,y] = 0;
								DestroyBall(i, y);
							}
						}
					}else {
						currentCellValue = Ball.fields[i,j];
						numberOfConsecutiveBalls = 1;
					}
				}else {
					ResetConsecuteBallsSearchingVariables();
				}
			}
		}
	}

	private void CheckForScoreHorizontally() {
		for(int i = 0; i < Ball.fields.GetLength(1); i++) {
			ResetConsecuteBallsSearchingVariables();
			for(int j = 0; j < Ball.fields.GetLength(0); j++) {
				
				if(Ball.fields[j,i] != 0) {
					if(Ball.fields[j,i] == currentCellValue) {
						numberOfConsecutiveBalls++;
						if(numberOfConsecutiveBalls >= 5) {
							for(int y = j - numberOfConsecutiveBalls + 1; y <= j; y++) {
								Ball.score++;
								Ball.fields[y,i] = 0;
								DestroyBall(y, i);
							}
						}
					}else {
						currentCellValue = Ball.fields[j,i];
						numberOfConsecutiveBalls = 1;
					}
				}else {
					ResetConsecuteBallsSearchingVariables();
				}
			}
		}
	}

	private void CheckForScoreDiagonally() {
		for (int line = 1; line <= (Ball.fields.GetLength(0) + Ball.fields.GetLength(1) - 1); line++) {
			ResetConsecuteBallsSearchingVariables();
			int start_col = Max(0, line - Ball.fields.GetLength(0));
			int count = Min(line, Min((Ball.fields.GetLength(1) - start_col), Ball.fields.GetLength(0)));

			for (int j = 0; j < count; j++) {
				if(Ball.fields[Min(Ball.fields.GetLength(0), line) - j - 1, start_col + j] != 0) {
					if(Ball.fields[Min(Ball.fields.GetLength(0), line) - j - 1, start_col + j] == currentCellValue) {
						numberOfConsecutiveBalls++;
						if(numberOfConsecutiveBalls >= 5) {
							for(int y = 0; y < numberOfConsecutiveBalls; y++) {
								Ball.score++;
								Ball.fields[Min(Ball.fields.GetLength(0), line) - j - 1 + y,start_col + j - y] = 0;
								DestroyBall((Min(Ball.fields.GetLength(0), line) - j - 1 + y), start_col + j - y);
							}
						}
					}else {
						currentCellValue =  Ball.fields[Min(Ball.fields.GetLength(0), line) - j - 1, start_col + j];
						numberOfConsecutiveBalls = 1;
					}
				}else {
					ResetConsecuteBallsSearchingVariables();
				}
			}
		}

	}

    public void InitializeBallMovement(GameObject ballObject, int xDestinationPos, int yDestinationPos)
    {
        //Get the ball color
        int ballColorType = Ball.fields[Ball.startPosX,Ball.startPosY]; 
        Ball.fields[Ball.startPosX, Ball.startPosY] = 0; //reset the field at that position

        int [,] path = FindAvailablePath(Ball.startPosX, Ball.startPosY, xDestinationPos, yDestinationPos);

        if(path!=null) //check the path is available or not
        {
            Ball.ballObject.GetComponent<SelectedBallAnimation>().enabled = false;
            Ball.ballObject.transform.localScale = new Vector2(0.25f, 0.25f);
            StartCoroutine(BallMovement(path, xDestinationPos, yDestinationPos, ballColorType));
            GameObject.Find("MovingSound").GetComponent<AudioSource>().Play();
        }
        else //if the path is not available
        {
            Ball.fields[Ball.startPosX, Ball.startPosY] = ballColorType +1;
            Ball.ballObject.transform.parent = GameObject.Find("Tile " + Ball.startPosX + "X" + Ball.startPosY).transform;
            Ball.ballObject.GetComponent<SelectedBallAnimation>().enabled = false;
            GameObject.Find("CancelMovingSound").GetComponent<AudioSource>().Play();

            Ball.ballObject.transform.localScale = new Vector2(0.25f, 0.25f);
            Ball.ballObject = null; //make that is none chosen ball
        }
    }

    private IEnumerator BallMovement(int[,] path, int xDesPos, int yDesPos, int ballColorType)
    {
        Ball.isMoving = true;
        while(new Vector2(Ball.ballObject.transform.position.x, Ball.ballObject.transform.position.y)!=new Vector2(xDesPos, yDesPos))
        {
            if((int)Ball.ballObject.transform.position.y>0 && path[(int)Ball.ballObject.transform.position.x,(int)Ball.ballObject.transform.position.y-1]==100)
            {
                path[(int)Ball.ballObject.transform.position.x, (int)Ball.ballObject.transform.position.y] = 1;
                path[(int)Ball.ballObject.transform.position.x, (int)Ball.ballObject.transform.position.y-1] = 1;
                Ball.ballObject.transform.position = new Vector2(Ball.ballObject.transform.position.x,Ball.ballObject.transform.position.y-1);
            }
            else if((int)Ball.ballObject.transform.position.y+1 < path.GetLength(1) && path[(int)Ball.ballObject.transform.position.x, (int)Ball.ballObject.transform.position.y+1]==100)
            {
                path[(int)Ball.ballObject.transform.position.x, (int)Ball.ballObject.transform.position.y] = 1;
                path[(int)Ball.ballObject.transform.position.x, (int)Ball.ballObject.transform.position.y+1] = 1;
                Ball.ballObject.transform.position = new Vector2(Ball.ballObject.transform.position.x,Ball.ballObject.transform.position.y+1);
            }
            else if((int)Ball.ballObject.transform.position.x-1 >=0 && path[(int)Ball.ballObject.transform.position.x-1, (int)Ball.ballObject.transform.position.y]==100)
            {
                path[(int)Ball.ballObject.transform.position.x, (int)Ball.ballObject.transform.position.y] = 1;
                path[(int)Ball.ballObject.transform.position.x-1, (int)Ball.ballObject.transform.position.y] = 1;
                Ball.ballObject.transform.position = new Vector2(Ball.ballObject.transform.position.x-1,Ball.ballObject.transform.position.y);
            }
            else if((int)Ball.ballObject.transform.position.x+1 < path.GetLength(0) && path[(int)Ball.ballObject.transform.position.x+1, (int)Ball.ballObject.transform.position.y]==100)
            {
                path[(int)Ball.ballObject.transform.position.x, (int)Ball.ballObject.transform.position.y] = 1;
                path[(int)Ball.ballObject.transform.position.x+1, (int)Ball.ballObject.transform.position.y] = 1;
                Ball.ballObject.transform.position = new Vector2(Ball.ballObject.transform.position.x+1,Ball.ballObject.transform.position.y);
            }
            yield return new WaitForSeconds(0.02f);
        }
        Ball.isMoving = false;
        Ball.fields[Ball.startPosX, Ball.startPosY]=0;
        Ball.fields[xDesPos,yDesPos] = ballColorType;
        Ball.startPosX = -1;
        Ball.ballObject.transform.parent = null;
        Ball.ballObject.transform.parent = GameObject.Find("Tile "+ xDesPos + "X" + yDesPos).transform;
        CheckScore();

        if(lastScore == Ball.score)
        {
            if(Ball.ballObject.transform.parent.transform.Find("BallPlaceholder")!=null)
            {
                Destroy(Ball.ballObject.transform.parent.transform.Find("BallPlaceholder").gameObject);
                CreateNewBall(xDesPos,yDesPos);
            }
            CreateNewBalls();
        }
        else
        {
            lastScore = Ball.score;
        }
    }

    private int[,] FindAvailablePath(int xPos, int yPos, int xDesPos, int yDesPos)
    {
        //because y counts up, then x*col will take all the elements of previous columns, then +y, we can take the Node position
        int BeginningNode = xPos*numOfColumns + yPos; 
        int EndingNode = xDesPos*numOfColumns + yDesPos;
        return (SearchPathEngine(BeginningNode,EndingNode)); //search path after detect the position of slot
    }

    private int GetNodeContents(int[,] iMaze, int iNodePos)
    {
        int iCols = iMaze.GetLength(1);
        return iMaze[iNodePos/iCols, iNodePos-iNodePos/iCols*iCols];
    }
    private void ChangeNodeContents(int[,] iMaze, int iNodePos, int iNewValue)
    {
        int iCols = iMaze.GetLength(1);
        iMaze[iNodePos/iCols, iNodePos-iNodePos/iCols*iCols] = iNewValue;
    }
    private enum Status{Ready, Waiting, Processed}
    private int[,] SearchPathEngine(int startNode, int stopNode)
    {
        const int empty = 0;

        int iRows = numOfRows;
        int iCols = numOfColumns;

        //Maximum Node in field
        int iMax = iRows*iCols;

        int[] Queue = new int[iMax];
        int[] Origin = new int[iMax];
        int iFront =0;
        int iRear =0;

        if(GetNodeContents(Ball.fields, startNode)!= empty || GetNodeContents(Ball.fields, stopNode) != empty) return null;

        int[,] iMazeStatus = new int[iRows,iCols];

        for(int i=0; i<iRows; i++)
        {
            for (int j = 0; j < iCols; j++)
            {
                iMazeStatus[i,j] = (int)Status.Ready;
            }
        }

        Queue[iRear]=startNode;
        Origin[iRear]=-1;
        iRear++;

        int iCurrent, iLeft, iRight, iTop, iDown;
        while(iFront!=iRear)
        {
            if(Queue[iFront]==stopNode) break;

            iCurrent = Queue[iFront];
            
            iLeft=iCurrent-1;
            if(iLeft>=0 && iLeft/iCols==iCurrent/iCols)
            {
                if (GetNodeContents(Ball.fields, iLeft)==empty)
                {
                    if(GetNodeContents(iMazeStatus,iLeft) == (int)Status.Ready)
                    {
                        Queue[iRear]=iLeft;
                        Origin[iRear]=iCurrent;
                        ChangeNodeContents(iMazeStatus, iLeft, (int)Status.Waiting);
                        iRear++;
                    }
                }
            }
            
            iRight=iCurrent+1;
            if(iRight<iMax && iRight/iCols==iCurrent/iCols)
            {
                if(GetNodeContents(Ball.fields, iRight)==empty)
                {
                    if (GetNodeContents(iMazeStatus, iRight) == (int)Status.Ready)
                    {
                        Queue[iRear]=iRight;
                        Origin[iRear]=iCurrent;
                        ChangeNodeContents(iMazeStatus, iRight, (int)Status.Waiting);
                        iRear++;
                    }
                }
            }

            iTop=iCurrent-iCols;
            if(iTop>=0)
            {
                if(GetNodeContents(Ball.fields, iTop)==empty)
                {
                    if(GetNodeContents(iMazeStatus, iTop)==(int)Status.Ready)
                    {
                        Queue[iRear]=iTop;
                        Origin[iRear]=iCurrent;
                        ChangeNodeContents(iMazeStatus, iTop, (int)Status.Waiting);
                        iRear++;
                    }
                }
            }
            iDown=iCurrent+iCols;
            if(iDown<iMax)
            {
                if(GetNodeContents(Ball.fields, iDown)==empty)
                {
                    if(GetNodeContents(iMazeStatus, iDown)==(int)Status.Ready)
                    {
                        Queue[iRear]=iDown;
                        Origin[iRear]=iCurrent;
                        ChangeNodeContents(iMazeStatus, iDown, (int)Status.Waiting);
                        iRear++;
                    }
                }
            }
            ChangeNodeContents(iMazeStatus, iCurrent, (int)Status.Processed);
            iFront++;
        }

        int[,] iMazeSolved = new int [iRows,iCols];
        for (int i = 0; i < iRows; i++)
        {
            for (int j = 0; j < iCols; j++)
            {
                iMazeSolved[i,j] = Ball.fields[i,j];
            }
        }

        iCurrent=stopNode;
        ChangeNodeContents(iMazeSolved, iCurrent, iPath);

        for (int i = iFront; i >= 0; i--)
        {
            if(Queue[i]== iCurrent)
            {
                iCurrent=Origin[i];
                if(iCurrent==-1) return(iMazeSolved);
                ChangeNodeContents(iMazeSolved, iCurrent, iPath);
            }
        }
        return null;
    } 

    public void RestartGame()
    {
        Ball.ResetAllVariables();
        SceneManager.LoadScene(0, LoadSceneMode.Single);

    }

    public void ExitGame()
    {
        #if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
        #else
            Application.Quit();
        #endif
    }

    public void ResetScore()//Reset best score
    {
        DataPersistence.instance.bestScore = 0;
        DataPersistence.instance.SaveData();
        FindObjectOfType<GameUI>().bestScoreDisplay.text = "Best Score: 0"; 
    }
}
