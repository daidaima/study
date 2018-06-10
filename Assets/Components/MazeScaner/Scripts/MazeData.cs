using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Components.MazeScaner.Scripts
{
    public class MazeData
    {
        private CellType[,] _data;

        public CellType[,] Data => _data;

        public MazeData(CellType[,] data)
        {
            this._data = data;
        }

        public MazeData(CellType[,] data, Vector2 entrance, Vector2 exit)
        {
            _data = data;
            Entrance = entrance;
            Exit = exit;
        }

        public int M => _data.GetLength(0);
        public int N => _data.GetLength(1);

        public bool InArea(Vector2 pos)
        {
            return pos.x >= 0 && pos.x < M && pos.y >= 0 && pos.y < N && _data[(int)pos.x, (int)pos.y] != CellType.NONE;
        }

        private Vector2 _entrance = Vector2.zero;

        public Vector2 FirstRoad
        {
            get{
                for (int i = 0; i < M; i++)
                {
                    for (int j = 0; j < N; j++)
                    {
                        if(_data[i,j] == CellType.ROAD)
                            return new Vector2(i,j);
                    }
                }
                
                return Vector2.zero;
            }
        }
        
        public Vector2 LastRoad
        {
            get{
                for (int i = M-1; i > -1; i--)
                {
                    for (int j = N-1; j >-1; j--)
                    {
                        if(_data[i,j] == CellType.ROAD)
                            return new Vector2(i,j);
                    }
                }
                
                return Vector2.zero;
            }
        }


        public Vector2 Entrance
        {
            get
            {
                if(InArea(_entrance))
                    return _entrance;
                
                Debug.LogError("entrance is not in area : " + _entrance);
                return Vector2.zero;
            }
            set
            {
                if (_entrance != Vector2.zero)
                {
                    Debug.LogError("Duplicate set,entrance has been set : " + _entrance);
                    return;
                }
                
                _entrance = value;
                _data[(int) value.x, (int) value.y] = CellType.ROAD;
            }
        }
        
        
        private Vector2 _exit = Vector2.zero;

        public Vector2 Exit
        {
            get
            {
                if(InArea(_exit))
                    return _exit;
                
                Debug.LogError("entrance is not in area : " + _exit);
                return Vector2.zero;
            }
            set
            {
                if (_exit != Vector2.zero)
                {
                    Debug.LogError("Duplicate set,entrance has been set : " + _exit);
                    return;
                }

                _exit = value;
                _data[(int) value.x, (int) value.y] = CellType.ROAD;
            }
        }

        private List<Vector2> Offsets = new List<Vector2>(8)
        {
            new Vector2(0,1),
            new Vector2(-1, 0),
            new Vector2(0,-1),
            new Vector2(1,0)
            
/*            new Vector2(0,-1),
            new Vector2(1,-1),
            new Vector2(1,0),
            new Vector2(1,1),
            new Vector2(0,1),
            new Vector2(-1,1),
            new Vector2(-1,0),
            new Vector2(-1,-1),*/
        };

        private bool[,] visited;
        public void GenerateOneWall()
        {
            visited = new bool[M,N];

            RandomQueue<Vector2> queue = new RandomQueue<Vector2>();
            queue.Enqueue(Entrance);
            visited[(int) Entrance.x, (int) Entrance.y] = true;

            while (!queue.IsEmpty)
            {
                var curPos = queue.Dequeue();

                for (int i = 0; i < Offsets.Count; i++)
                {
                    var newPos = curPos + Offsets[i] * 2;

                    if (InArea(newPos)
                        && !visited[(int) newPos.x, (int) newPos.y]
                        && _data[(int) newPos.x, (int) newPos.y] == CellType.ROAD)
                    {
                        queue.Enqueue(newPos);
                        visited[(int) newPos.x, (int) newPos.y] = true;
                        MakeRoad(curPos + Offsets[i]);
                    }
                }
            }
        }
        
        public void GenerateTwoWall()
        {
            visited = new bool[M,N];

            RandomQueue<Vector2> queue = new RandomQueue<Vector2>();
            queue.Enqueue(Entrance);
            visited[(int) Entrance.x, (int) Entrance.y] = true;

            while (!queue.IsEmpty)
            {
                var curPos = queue.Dequeue();

                for (int i = 0; i < Offsets.Count; i++)
                {
                    var newPos = curPos + Offsets[i] * 3;

                    if (InArea(newPos)
                        && !visited[(int) newPos.x, (int) newPos.y]
                        && _data[(int) newPos.x, (int) newPos.y] == CellType.ROAD)
                    {
                        queue.Enqueue(newPos);
                        visited[(int) newPos.x, (int) newPos.y] = true;
                        MakeRoad(curPos + Offsets[i]);
                        MakeRoad(curPos + Offsets[i]*2);
                    }
                }
            }
        }

        private void MakeRoad(Vector2 pos)
        {
            if (InArea(pos))
                _data[(int) pos.x, (int) pos.y] = CellType.ROAD;
        }
    }
}