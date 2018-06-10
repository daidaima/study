using System.Collections.Generic;
using ProceduralToolkit;
using UnityEngine;

namespace Components.MazeScaner.Scripts
{
    public class MazeMeshGenerator
    {
        private List<MazeConnect> _mazeConnects;
        private List<MazeCorss> _mazeCorsses;

        private MeshFilter _meshFilterA;
        private MeshFilter _meshFilterB;

        public float WallHeight = 5f;

        public float GridSize = 5f;

        public MazeMeshGenerator(List<MazeConnect> mazeConnects, List<MazeCorss> mazeCorsses)
        {
            this._mazeConnects = mazeConnects;
            this._mazeCorsses = mazeCorsses;
        }


        private MeshDraft _roadDraft = new MeshDraft();

        private MeshDraft _wallDraft = new MeshDraft();

        public void Generate()
        {
            foreach (var cross in _mazeCorsses)
            {
                var origin = new Vector3((cross.Point.x + 0.5f) * GridSize, cross.Height * GridSize,
                    (cross.Point.y + 0.5f) * GridSize);
                if (cross.CellType == CellType.CROSS)
                {
                    _roadDraft.AddHexaheron(origin, new Vector3(GridSize, 0, 0), new Vector3(0, 0, GridSize),
                        new Vector3(0, GridSize, 0));
                }
                else if (cross.CellType == CellType.WALL_CORNER)
                {
                    origin = new Vector3(origin.x, origin.y + WallHeight/2, origin.z);
                    _wallDraft.AddHexaheron(origin, new Vector3(GridSize, 0, 0), new Vector3(0,0, GridSize), 
                        new Vector3(0, GridSize + WallHeight, 0));
                }
            }

            foreach (var connect in _mazeConnects)
            {
                Vector3 pointA, pointB;

                if (connect.IsHorizental)
                {
                    pointA = new Vector3((connect.PointA.x + 1f) * GridSize , connect.HeightA*GridSize, (connect.PointA.y + 0.5f) * GridSize);
                    pointB = new Vector3(connect.PointB.x * GridSize , connect.HeightB*GridSize, (connect.PointB.y + 0.5f) * GridSize);
                }
                else
                {
                    pointA = new Vector3((connect.PointA.x + 0.5f) * GridSize , connect.HeightA*GridSize, (connect.PointA.y + 1f) * GridSize);
                    pointB = new Vector3((connect.PointB.x  + 0.5f) * GridSize , connect.HeightB*GridSize, connect.PointB.y * GridSize);
                }


                if (connect.CellType == CellType.ROAD)
                {
                    _roadDraft.AddSkewBrige(pointA, pointB, GridSize, GridSize);
                }else if (connect.CellType == CellType.WALL)
                {
                    pointA = new Vector3(pointA.x, pointA.y + WallHeight/2, pointA.z);
                    pointB = new Vector3(pointB.x, pointB.y + WallHeight/2, pointB.z);
                    _wallDraft.AddSkewBrige(pointA, pointB, GridSize, GridSize + WallHeight);
                }
            }
        }

        public Mesh RoadMesh => _roadDraft.ToMesh();
        public Mesh WallMesh => _wallDraft.ToMesh();

    }
}