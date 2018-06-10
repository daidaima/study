using Components.MazeScaner.Scripts;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(MazeScaner))]
public class MazeTextureOutputer:MonoBehaviour
{


    
    
/*    public void GenerateTwoWallMaze()
    {
        var mazeData = _mazeScaner.GetMazeWithTwoWall();
        var generateData = new MazeData(mazeData);
        generateData.Entrance = generateData.FirstRoad - new Vector2(3, 0);
        generateData.Exit = generateData.LastRoad + new Vector2(3, 0);
        generateData.GenerateTwoWall();
        data = generateData.Data;

        OutputToTexture(); 
    }

    private void GenerateOneWallMaze()
    {
        var mazeData = _mazeScaner.GetOriginMaze();
        var generateData = new MazeData(mazeData);
        generateData.Entrance = generateData.FirstRoad - new Vector2(2, 0);
        generateData.Exit = generateData.LastRoad + new Vector2(2, 0);
        generateData.GenerateOneWall();
        data = generateData.Data;

        OutputToTexture();
    }*/


    public float roadWidth = 5f;

    public CellType[,] data;

    public int M => data.GetLength(0);
    public int N => data.GetLength(1);


    public Texture2D outPutTexture;

    public void OutputToTexture()
    {
        var width = (int) (M * roadWidth);
        var height = (int) (N * roadWidth);

        outPutTexture = new Texture2D(width, height);

        for (var i = 0; i < M; i++)
        {
            for (var j = 0; j < N; j++)
            {
                switch (data[i, j])
                {
                    case CellType.NONE:
                        SetRect(i, j, Color.green);
                        break;
                    case CellType.ROAD:
                        SetRect(i, j, Color.cyan);
                        break;
                    case CellType.WALL:
                        SetRect(i, j, Color.gray);
                        break;
                }
            }
        }
		
        outPutTexture.Apply();
        AssetDatabase.Refresh();
    }

    private void SetRect(int i, int j, Color color)
    {
        for (var k = (int) (i * roadWidth); k < (i + 1) * (int) roadWidth; k++)
        {
            for (var l = (int) (j * roadWidth); l < (j + 1) * (int) roadWidth; l++)
            {
                outPutTexture.SetPixel(k, l, color);
            }
        }
    }
}