using System.Collections.Generic;
using UnityEngine;

namespace Components.MazeScaner.Scripts
{
    public class RandomQueue<T>
    {
        private LinkedList<T> _data = new LinkedList<T>();
        
        public void Enqueue(T element)
        {
            var random = Random.Range(0f, 1f);

            if (random < 0.5f)
            {
                _data.AddFirst(element);
            }
            else
            {
                _data.AddLast(element);
            }
        }

        public bool IsEmpty => _data.Count == 0;

        public T Dequeue()
        {
            var random = Random.Range(0f, 1f);
            T result;

            if (random < 0.5f)
            {
                result = _data.First.Value;
                _data.RemoveFirst();
                
                return result;
            }
            else
            {
                result = _data.Last.Value;
                _data.RemoveLast();
                
                return result;
            }
        }
    }
}