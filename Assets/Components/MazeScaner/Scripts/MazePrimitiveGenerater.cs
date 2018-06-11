using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Components.MazeScaner.Scripts
{
    public class MazePrimitiveGenerater:MonoBehaviour
    {

        public Texture2D heightMap;
        
        private CellType[,] _data;

        public CellType[,] Data
        {
            get { return _data;}
            set { _data = value; }
        }

        private int M => _data.GetLength(0);
        private int N => _data.GetLength(1);


        public float MaxHeight;
        public float MinHeight;
        
        public void Generate()
        {
            SetCross();
            SampleCross();
            GeneratePrimitives();
        }

        public List<MazeCorss> MazeCorsses => _mazeCorsses;

        private List<MazeCorss> _mazeCorsses = new List<MazeCorss>();

        public List<MazeConnect> MazeConnects => _mazeConnects;
        
        private List<MazeConnect> _mazeConnects = new List<MazeConnect>();
            
            
        private List<MazePrimitive> _mazePrimitives = new List<MazePrimitive>();

        private void SampleCross()
        {
            var textureWidth = heightMap.width;
            var widthSize = textureWidth / M;
            var textureHeight = heightMap.height;
            var heightSize = textureHeight / N;
            
            for (int i = 0; i < M; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    if (_data[i, j] == CellType.CROSS || _data[i, j] == CellType.WALL_CORNER)
                    {
                        var height = MinHeight + heightMap.GetPixel(widthSize*i+widthSize/2, heightSize*j + heightSize/2).r *(MaxHeight - MinHeight);

                        _mazeCorsses.Add(new MazeCorss
                        {
                            CellType = _data[i,j],
                            Point = new Vector2(i,j),
                            Height = height,
                        });
                    }
                }
            }

        }
        

        private void GeneratePrimitives()
        {
            _mazePrimitives.Clear();
            var visited = new bool[M, N];

            for (int i = 0; i < M; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    if (!visited[i,j] && (_data[i, j] == CellType.ROAD || _data[i, j] == CellType.WALL))
                    {
                        Stack<Vector2> stack = new Stack<Vector2>();

                        stack.Push(new Vector2(i, j));
                        visited[i, j] = true;
                        var pos = stack.Peek();
                        while (HaveUnvisitedAdjcent(_data[i, j], pos, visited))
                        {
                            var adjcent = GetUnvisitedAdjcent(_data[i, j], pos, visited);
                            stack.Push(adjcent);
                            visited[(int) adjcent.x, (int) adjcent.y] = true;
                            pos = stack.Peek();
                        }

                        _mazePrimitives.Add(new MazePrimitive
                        {
                            CellType = _data[i, j],
                            Points = stack.ToList(),
                        });
                    }
                }
            }

            foreach (var primitive in _mazePrimitives)
            {
                if (primitive.Points.Count == 1)
                {
                    var point = primitive.Points[0];
                    if (GetAdjcentAnchorCount(primitive.CellType, point) == 1)
                    {
                        var adjcentCross = GetAdjcentCrosses(primitive.CellType, point)[0];
                        var adjcentPoint = adjcentCross.Point;
                        var oppsitePoint = GetOppsiteAdjcent(adjcentPoint, point);
                        
                        _mazeConnects.Add(new MazeConnect
                        {
                            CellType = primitive.CellType,
                            PointA = adjcentPoint,
                            HeightA = adjcentCross.Height,
                            PointB = oppsitePoint,
                            HeightB = adjcentCross.Height,
                        });
                    }
                    else
                    {
                        var adjcentPoints = GetAdjcentCrosses(primitive.CellType, point);
                        
                        _mazeConnects.Add(new MazeConnect
                        {
                            CellType = primitive.CellType,
                            PointA = adjcentPoints[0].Point,
                            PointB = adjcentPoints[1].Point,
                            HeightA = adjcentPoints[0].Height,
                            HeightB = adjcentPoints[1].Height,
                        });
                    }
                }
                else
                {
                    var pointA = primitive.Points[0];
                    var pointB = primitive.Points[primitive.Points.Count - 1];

                    var adjcentCrossesA = GetAdjcentCrosses(primitive.CellType, pointA);
                    var adjcentCrossesB = GetAdjcentCrosses(primitive.CellType, pointB);
                    if (adjcentCrossesA.Count == 0 || adjcentCrossesB.Count == 0)
                    {
                        var noneAdjcentPoint = adjcentCrossesA.Count == 0 ? pointA : pointB;
                        var adjcentCross = adjcentCrossesA.Count == 0 ? adjcentCrossesB[0] : adjcentCrossesA[0];
                        
                        _mazeConnects.Add(new MazeConnect
                        {
                            CellType = primitive.CellType,
                            PointA = adjcentCross.Point,
                            HeightA = adjcentCross.Height,
                            PointB = GetOppsiteAdjcent(adjcentCross.Point, noneAdjcentPoint),
                            HeightB = adjcentCross.Height
                        });
                    }
                    else
                    {
                       _mazeConnects.Add(new MazeConnect
                       {
                           CellType = primitive.CellType,
                           PointA = adjcentCrossesA[0].Point,
                           HeightA = adjcentCrossesA[0].Height,
                           PointB = adjcentCrossesB[0].Point,
                           HeightB = adjcentCrossesB[0].Height,
                       }); 
                    }
                }
            }

            foreach (var connect in _mazeConnects)
            {
                connect.Sort();
            }
        }

        private Vector2 GetOppsiteAdjcent(Vector2 adjcentPoint, Vector2 point)
        {
            var offset = point - adjcentPoint;
            
            return point + new Vector2(Mathf.Clamp(offset.x,-1,1), Mathf.Clamp(offset.y, -1,1));
        }

        private List<MazeCorss> GetAdjcentCrosses(CellType cellType, Vector2 pos)
        {
            var result = new List<MazeCorss>();
            if (cellType == CellType.WALL)
            {
                foreach (var t in offset)
                {
                    var newPos = pos + t;
                    if (_data[(int) newPos.x, (int) newPos.y] == CellType.WALL_CORNER)
                        result.Add(_mazeCorsses.Single( x => Vector2.Distance(x.Point, newPos) <= 0.01f));
                }
            }else if (cellType == CellType.ROAD)
            {
                foreach (var t in offset)
                {
                    var newPos = pos + t;
                    if (_data[(int) newPos.x, (int) newPos.y] == CellType.CROSS)
                        result.Add(_mazeCorsses.Single( x => Vector2.Distance(x.Point, newPos) <= 0.01f));
                }
            }

            return result;
        }

        private int GetAdjcentAnchorCount(CellType cellType, Vector2 pos)
        {
            var result = 0;
            if (cellType == CellType.WALL)
            {
                foreach (var t in offset)
                {
                    var newPos = pos + t;
                    if (_data[(int) newPos.x, (int) newPos.y] == CellType.WALL_CORNER)
                        result++;
                }
            }else if (cellType == CellType.ROAD)
            {
                foreach (var t in offset)
                {
                    var newPos = pos + t;
                    if (_data[(int) newPos.x, (int) newPos.y] == CellType.CROSS)
                        result++;
                }
            }

            return result;
        }


        private Vector2 GetUnvisitedAdjcent(CellType cellType, Vector2 pos, bool[,] visited)
        {
            for (int i = 0; i < offset.Count; i++)
            {
                var newPos = pos + offset[i];
                if (_data[(int) newPos.x, (int) newPos.y] == cellType && !visited[(int) newPos.x, (int) newPos.y])
                    return newPos;
            }

            throw new Exception("univisted adjcent error : " + pos);
        }

        private bool HaveUnvisitedAdjcent(CellType cellType,Vector2 pos, bool[,] visited)
        {
            for (int i = 0; i < offset.Count; i++)
            {
                var newPos = pos + offset[i];
                if (_data[(int) newPos.x, (int) newPos.y] == cellType && !visited[(int) newPos.x, (int) newPos.y])
                    return true;
            }

            return false;
        }

        private void SetCross()
        {
            for (int i = 0; i < M; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    if (_data[i, j] == CellType.ROAD && IsCross(i,j, true))
                        _data[i, j] = CellType.CROSS;
                    else if (_data[i, j] == CellType.WALL && IsCross(i, j, false))
                        _data[i, j] = CellType.WALL_CORNER;
                }
            }
        }

        private bool IsCross(int i, int j, bool isRoad)
        {
            return AdjecentCount(new Vector2(i, j), isRoad) > 2 || 
                   (AdjecentCount(new Vector2(i, j), isRoad) == 2 && IsCorner(new Vector2(i,j), isRoad));
        }
        
        

        private bool IsCorner(Vector2 pos, bool isRoad)
        {
            for (var i = 0; i < offset.Count; i++)
            {
                var newPos1 = pos + offset[i];
                var newPos2 = pos + offset[(i + 1) % offset.Count];
                if (IsReachable(_data[(int) newPos1.x, (int) newPos1.y], isRoad) &&
                    IsReachable(_data[(int) newPos2.x, (int) newPos2.y], isRoad))
                    return true;
            }

            return false;
        }

        private bool IsReachable(CellType cellType, bool isRoad)
        {
            if(isRoad)
                return cellType == CellType.ROAD || cellType == CellType.CROSS;
            else
                return cellType == CellType.WALL || cellType == CellType.WALL_CORNER;
        }

        private List<Vector2> offset = new List<Vector2>
        {
            new Vector2(0,1),
            new Vector2(1,0),
            new Vector2(0,-1),
            new Vector2(-1,0),
        };

        private int AdjecentCount(Vector2 pos, bool isRoad)
        {
            var result = 0;
            
            offset.ForEach(x =>
            {
                var newPos = pos + x;
                if (isRoad && (_data[(int) newPos.x, (int) newPos.y] == CellType.ROAD
                    || _data[(int) newPos.x, (int) newPos.y] == CellType.CROSS))
                    result++;
                else if (!isRoad && (_data[(int) newPos.x, (int) newPos.y] == CellType.WALL
                         || _data[(int) newPos.x, (int) newPos.y] == CellType.WALL_CORNER))
                    result++;
            });

            return result;
        }
    }

    public class MazePrimitive
    {
        public CellType CellType;

        public List<Vector2> Points = new List<Vector2>();
        
    }

    public class MazeConnect
    {
        public CellType CellType;

        public Vector2 PointA;
        public Vector2 PointB;
        public float HeightA;
        public float HeightB;

        public bool IsHorizental => Mathf.Abs(PointA.y - PointB.y) < 0.001f;
        
        public void Sort()
        {
            if (IsHorizental)
            {
                if (PointA.x > PointB.x)
                {
                    Swap();
                }
            }
            else
            {
                if (PointA.y > PointB.y)
                {
                    Swap();
                }
            }
        }

        private void Swap()
        {
            Vector2 tmpPoint = PointA;
            float tmpHeight = HeightA;
            PointA = PointB;
            HeightA = HeightB;
            PointB = tmpPoint;
            HeightB = tmpHeight;
        }
    }

    public class MazeCorss
    {
        public CellType CellType;
        public Vector2 Point;
        public float Height;
    }
}