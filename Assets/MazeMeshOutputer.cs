using System.Collections;
using System.Collections.Generic;
using Components.MazeScaner.Scripts;
using UnityEngine;

public class MazeMeshOutputer : MonoBehaviour
{
	private MazeScaner _mazeScaner;
	private MazePrimitiveGenerater _mazePrimitiveGenerater;

	[SerializeField] private MeshFilter _roadMeshFilter;
	[SerializeField] private MeshFilter _wallMeshFilter;

	private MazeTextureOutputer _mazeTextureOutputer;
	
	private void Awake()
	{
		_mazeScaner = GetComponent<MazeScaner>();
		_mazePrimitiveGenerater = GetComponent<MazePrimitiveGenerater>();

		_mazeTextureOutputer = GetComponent<MazeTextureOutputer>();
	}


	void Start () {
		var mazeData = _mazeScaner.GetOriginMaze();
		var generateData = new MazeData(mazeData);
		generateData.Entrance = generateData.FirstRoad - new Vector2(2, 0);
		generateData.Exit = generateData.LastRoad + new Vector2(2, 0);
		generateData.GenerateOneWall();
		_mazePrimitiveGenerater.Data = generateData.Data;
		
		_mazeTextureOutputer.data = generateData.Data;
		_mazeTextureOutputer.OutputToTexture();
		
		_mazePrimitiveGenerater.Generate();

		var  mazeMeshGenerator = new MazeMeshGenerator(_mazePrimitiveGenerater.MazeConnects, _mazePrimitiveGenerater.MazeCorsses);
		mazeMeshGenerator.Generate();

		_roadMeshFilter.mesh = mazeMeshGenerator.RoadMesh;
		_wallMeshFilter.mesh = mazeMeshGenerator.WallMesh;
	}

}
