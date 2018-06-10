using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public enum CellType
{
	ROAD = 1,
	WALL = 2,
	CROSS = 3,
	WALL_CORNER = 4,
	NONE = 0,
}

public class MazeScaner : MonoBehaviour
{

    public float mazeGridSize;

	public Texture2D mazeAreaTexture;


	public int wantedWidth;

	public int wantedHeight;

	public int[,] GetAreaFromTexture()
	{
		var result = new int[wantedWidth,wantedHeight];

		Debug.Log("texture width : " + mazeAreaTexture.width);
		Debug.Log("texture height : " + mazeAreaTexture.height);
		
		var sizePerUnitX = (mazeAreaTexture.width / (mazeGridSize * wantedWidth))*mazeGridSize;
		var sizePerUnitY = (mazeAreaTexture.height / ( mazeGridSize * wantedHeight)) * mazeGridSize;

		for (var i = 0; i < wantedWidth; i++)
		{
			for (var j = 0; j < wantedHeight; j++)
			{
				var color = mazeAreaTexture.GetPixel((int) (sizePerUnitX* i),
					(int) (sizePerUnitY * j));

				Debug.Log((int) (sizePerUnitX * i) + "," + (int) (sizePerUnitY * j));

				if (color.r < 0.9f || color.g < 0.9f || color.b < 0.9f)
				{
					result[i, j] = 1;
				}
				else
				{
					result[i, j] = 0;
				}
			}
		}

		return result;
	}

	public CellType[,] GetOriginMaze()
	{
		return GetMazeWithWall(GetAreaFromTexture());
	}

	public CellType[,] GetMazeWithTwoWall()
	{
		return GetMazeWithTwoWall(GetAreaFromTexture());
	}

	private List<Vector2> Offsets = new List<Vector2>(8)
	{
		new Vector2(0,-1),
		new Vector2(1,-1),
		new Vector2(1,0),
		new Vector2(1,1),
		new Vector2(0,1),
		new Vector2(-1,1),
		new Vector2(-1,0),
		new Vector2(-1,-1),
	};

	public CellType[,] GetMazeWithTwoWall(int[,] maze)
	{
		var result = new CellType[maze.GetLength(0)*3, maze.GetLength(1)*3];

		for (var i = 0; i < maze.GetLength(0); i++)
		{
			for (var j = 0; j < maze.GetLength(1); j++)
			{
				if (maze[i, j] == 1)
				{
					result[i * 3, j * 3] = CellType.WALL;
					result[i * 3 + 1, j * 3] = CellType.WALL;
					result[i * 3 + 2, j * 3] = CellType.WALL;
					
					result[i * 3, j * 3 +1] = CellType.WALL;
					result[i * 3 + 1, j * 3 +1] = CellType.ROAD;
					result[i * 3 + 2, j * 3+1] = CellType.WALL;
					
					result[i * 3, j * 3 +2] = CellType.WALL;
					result[i * 3 + 1, j * 3 +2] = CellType.WALL;
					result[i * 3 + 2, j * 3+2] = CellType.WALL;
				}
				else
				{
					result[i * 3, j * 3] = CellType.NONE;
					result[i * 3 + 1, j * 3] = CellType.NONE;
					result[i * 3 + 2, j * 3] = CellType.NONE;
					
					result[i * 3, j * 3 +1] = CellType.NONE;
					result[i * 3 + 1, j * 3 +1] = CellType.NONE;
					result[i * 3 + 2, j * 3+1] = CellType.NONE;
					
					result[i * 3, j * 3 +2] = CellType.NONE;
					result[i * 3 + 1, j * 3 +2] = CellType.NONE;
					result[i * 3 + 2, j * 3+2] = CellType.NONE;
				}
			}
		}

		return result;
	}
	
	public CellType[,] GetMazeWithWall(int[,] maze)
	{
		var result = new CellType[maze.GetLength(0)*2+1, maze.GetLength(1)*2 +1];

		for (var i = 0; i < maze.GetLength(0); i++)
		{
			for (var j = 0; j < maze.GetLength(1); j++)
			{
				if (maze[i, j] == 1)
				{
					result[i * 2, j * 2] = CellType.WALL;
					result[i * 2 + 1, j * 2] = CellType.WALL;
					result[i * 2, j * 2 + 1] = CellType.WALL;
					result[i * 2 + 1, j * 2 + 1] = CellType.ROAD;
				}
				else
				{
					result[i * 2, j * 2] = CellType.NONE;
					result[i * 2 + 1, j * 2] = CellType.NONE;
					result[i * 2, j * 2 + 1] = CellType.NONE;
					result[i * 2 + 1, j * 2 + 1] = CellType.NONE;
				}
			}
		}

		for (var i = 0; i < result.GetLength(0); i++)
		{
			for (var j = 0; j < result.GetLength(1); j++)
			{
				if (result[i, j] == CellType.ROAD)
				{
					for (var k = 0; k < Offsets.Count; k++)
					{
						if (result[i + (int)Offsets[k].x, j + (int)Offsets[k].y] != CellType.WALL)
							result[i + (int)Offsets[k].x, j + (int)Offsets[k].y] = CellType.WALL;
					}
				}
			}
		}

		return result;
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.A))
		{
			var scanedArea = GetMazeWithWall(GetAreaFromTexture());

			for (var i = 0; i < scanedArea.GetLength(1); i++)
			{
				var line = new StringBuilder();
				for (var j = 0; j < scanedArea.GetLength(0); j++)
				{
					line.Append(" ").Append((int)scanedArea[j, i]);
				}
				Debug.Log(line.ToString());
			}
		}
	}
}