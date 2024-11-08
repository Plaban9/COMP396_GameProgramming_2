using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Rendering;

namespace PathFinding
{
    public class Graph : MonoBehaviour
    {
        Dictionary<char, Dictionary<char, int>> vertices = new Dictionary<char, Dictionary<char, int>>();

        public void AddVertex(char vertex, Dictionary<char, int> edges)
        {
            vertices[vertex] = edges;
        }

        public void TestDFS()
        {
            var graph = new Dictionary<string, List<string>>
            {
                { "A", new List<string> { "B", "C" } },
                { "B", new List<string> { "A", "D", "E" } },
                { "C", new List<string> { "A", "F" } },
                { "D", new List<string> { "B" } },
                { "E", new List<string> { "B", "F" } },
                { "F", new List<string> { "C", "E" } }
            };

            var discovered = new HashSet<string>();
            DepthFirstSearch(graph, "A", discovered);
        }

        void DepthFirstSearch(Dictionary<string, List<string>> graph, string start, HashSet<string> discovered)
        {
            // Label the start vertex as discovered
            discovered.Add(start);
            Debug.Log(start);

            // Explore each adjacent vertex
            foreach (var neighbor in graph[start])
            {
                if (!discovered.Contains(neighbor))
                {
                    DepthFirstSearch(graph, neighbor, discovered);
                }
            }
        }


        public OptimalPath ShortestPathViaDijkstra(char start, char finish)
        {
            //initialize
            List<char> path = new List<char>();
            var distances = new Dictionary<char, int>();
            var previous = new Dictionary<char, char>(); // Parents
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
                // Priority Queue Sort
                Pending.Sort((x, y) => distances[x].CompareTo(distances[y]));
                var currentVisited = Pending[0];
                Pending.RemoveAt(0);

                foreach (var adjacentVertex in vertices[currentVisited])
                {
                    var weight = adjacentVertex.Value;
                    var node = adjacentVertex.Key;

                    if (distances[node] > distances[currentVisited] + weight)
                    {
                        distances[node] = weight + distances[currentVisited];
                        previous[node] = currentVisited;
                    }
                }
            }

            return ConstructOptimalPath(distances, previous, start, finish);
        }

        OptimalPath ConstructOptimalPath(Dictionary<char, int> distances, Dictionary<char, char> previous, char start, char finish)
        {
            var optimalDistance = distances[finish];

            var pathList = new List<char>();

            var currentNode = finish;

            while (currentNode != '\0')
            {
                pathList.Add(currentNode);
                var tempNode = previous[currentNode];
                currentNode = tempNode;
            }

            pathList.Reverse();

            return new OptimalPath(pathList, optimalDistance);
        }

    }

    public class OptimalPath
    {
        private readonly List<char> path;
        private readonly int length;

        public OptimalPath(List<char> path, int length)
        {
            this.path = path;
            this.length = length;
        }

        public List<char> GetPath()
        {
            return path;
        }

        public int GetLength()
        {
            return length;
        }
    }
}