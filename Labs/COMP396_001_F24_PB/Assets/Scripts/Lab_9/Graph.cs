using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace PathFinding.Lab_9
{
    public enum HeuristicStrategy { EuclideanDistance, ManhattanDistance, DictionaryDistance };

    public class Graph : MonoBehaviour
    {
        HeuristicStrategy strategy;

        public Graph(HeuristicStrategy strategy = HeuristicStrategy.EuclideanDistance)
        {
            this.strategy = strategy;
        }

        Dictionary<char, Dictionary<char, int>> vertices = new Dictionary<char, Dictionary<char, int>>();
        public void add_vertex(char vertex, Dictionary<char, int> edges)
        {
            vertices[vertex] = edges;
        }

        Dictionary<char, Vector3> verticesData = new Dictionary<char, Vector3>();
        public void add_vertex_for_AStar_with_position(char vertex, Vector3 pos, Dictionary<char, int> edges)
        {
            vertices[vertex] = edges;
            verticesData[vertex] = pos;
        }

        public void add_vertex_for_AStar_with_heuristic(char vertex, float heuristic, Dictionary<char, int> edges)
        {
            vertices[vertex] = edges;
            verticesData[vertex] = Vector3.zero;
            verticesData[vertex] = Vector3.right * heuristic;    //we use the 'x' component to save h[n]
        }

        public float EuclideanDistance(Vector3 v1, Vector3 v2)
        {
            return Vector3.Distance(v1, v2);
        }

        public float ManhatanDistance(Vector3 v1, Vector3 v2)
        {
            return Mathf.Abs(v1.x - v2.x) + Mathf.Abs(v1.y - v2.y) + Mathf.Abs(v1.z - v2.z);
        }

        public float GoalDistanceEstimate(char node, char finish)
        {
            float res = 0f;
            switch (strategy)
            {
                case HeuristicStrategy.EuclideanDistance:
                    res = EuclideanDistance(verticesData[node], verticesData[finish]);
                    break;
                case HeuristicStrategy.ManhattanDistance:
                    res = ManhatanDistance(verticesData[node], verticesData[finish]);
                    break;
                case HeuristicStrategy.DictionaryDistance:
                    res = verticesData[node].x;
                    break;
                default:
                    break;
            }
            return res;
        }

        public List<char> shortest_path_via_AStar_algo(char start, char finish)
        {
            List<char> path = null;
            var previous = new Dictionary<char, char>();
            var distances = new Dictionary<char, float>(); //try to put fScore (= gScore+hScore)
            var gScore = new Dictionary<char, float>();
            var Pending = new List<char>(); //Open priority queue
            var Closed = new List<char>();  //Closed list
                                            //Step 0
            gScore[start] = 0;
            float hScore = GoalDistanceEstimate(start, finish);
            distances[start] = gScore[start] + hScore;
            previous[start] = '-';
            Pending.Add(start);
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

        public OptimalPath shortest_path_via_Dijkstra(char start, char finish)
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
