using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Rendering;

using static UnityEditor.Searcher.SearcherWindow.Alignment;

namespace PathFinding
{
    public class Graph : MonoBehaviour
    {
        Dictionary<char, Dictionary<char, int>> vertices = new Dictionary<char, Dictionary<char, int>>();

        public void AddVertex(char vertex, Dictionary<char, int> edges)
        {
            vertices[vertex] = edges;
        }

        public List<char> ShortestPathViaDijkstra(char start, char finish)
        {
            //initialize
            List<char> path = new List<char>();
            var distances = new Dictionary<char, int>();
            var previous = new Dictionary<char, char>();
            var Pending = new List<char>();

            //step 0
            foreach (var v in vertices)
            {
                distances[v.Key] = int.MaxValue;
                previous[v.Key] = '\0';
                Pending.Add(v.Key);
            }
            distances[start] = 0;
            //main loop
            while (Pending.Count > 0)
            {
                Pending.Sort((x, y) => distances[x].CompareTo(distances[y]));
                var u = Pending[0];
                // TODO
                // ...
                // ...
            }

            return path;
        }
    }
}