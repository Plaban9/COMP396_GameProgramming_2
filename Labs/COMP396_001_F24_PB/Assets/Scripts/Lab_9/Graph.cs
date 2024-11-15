using System.Collections;
using System.Collections.Generic;

using UnityEditor.Experimental.GraphView;

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

        public OptimalPath shortest_path_via_AStar_algo(char start, char finish)
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
            previous[start] = '\0';
            Pending.Add(start);
            //main loop
            while (Pending.Count > 0)
            {
                Pending.Sort((x, y) => distances[x].CompareTo(distances[y]));
                var u = Pending[0];

                Pending.RemoveAt(0);


                if (u == finish)
                {
                    path = new List<char>();
                    while (previous[u] != '\0')
                    {
                        path.Add(u);
                        u = previous[u];
                    }
                    path.Add(start);
                    //path.Reverse();
                    break;
                }

                Closed.Add(u);
                // Check each successor of node 'u'
                foreach (var neighbor in GetNeighbors(u))
                {

                    float tentative_gScore = gScore[u] + Cost(u, neighbor);

                    if ((Closed.Contains(neighbor) || Pending.Contains(neighbor)) && gScore[neighbor] <= tentative_gScore)
                        continue;

                    previous[neighbor] = u;
                    gScore[neighbor] = tentative_gScore;
                    hScore = GoalDistanceEstimate(neighbor, finish);
                    distances[neighbor] = gScore[neighbor] + hScore;

                    if (!Pending.Contains(neighbor))
                        Pending.Add(neighbor);
                }
            }

            if (strategy == HeuristicStrategy.DictionaryDistance)
            {
                distances[finish] = distances[finish] - GoalDistanceEstimate(finish, start);
            }

            var optimalPath = ConstructOptimalPathForAStar(distances, previous, start, finish);
            return optimalPath;
            //return path;
        }

        private List<char> GetNeighbors(char node)
        {
            var neighbours = vertices[node];
            var t = new List<char>();

            foreach (var neighbour in neighbours)
            {
                t.Add(neighbour.Key);
            }

            return t;
        }

        private float Cost(char fromNode, char toNode)
        {
            float res = 0f;
            switch (strategy)
            {
                case HeuristicStrategy.EuclideanDistance:
                    res = EuclideanDistance(verticesData[fromNode], verticesData[toNode]);
                    break;
                case HeuristicStrategy.ManhattanDistance:
                    res = ManhatanDistance(verticesData[fromNode], verticesData[toNode]);
                    break;
                case HeuristicStrategy.DictionaryDistance:
                    res = vertices[fromNode][toNode];
                    break;
                default:
                    break;
            }
            return res;
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

        OptimalPath ConstructOptimalPath(Dictionary<char, float> distances, Dictionary<char, char> previous, char start, char finish)
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

        OptimalPath ConstructOptimalPathForAStar(Dictionary<char, float> distances, Dictionary<char, char> previous, char start, char finish)
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
        private readonly float lengthF;

        public OptimalPath(List<char> path, int length)
        {
            this.path = path;
            this.length = length;
        }

        public OptimalPath(List<char> path, float lengthF)
        {
            this.path = path;
            this.lengthF = lengthF;
        }

        public List<char> GetPath()
        {
            return path;
        }

        public int GetLength()
        {
            return length;
        }

        public float GetLengthF()
        {
            return lengthF;
        }

        public string GetVisualizedPath()
        {
            var pathVisualizer = "";

            for (var i = 0; i < path.Count; i++)
            {
                pathVisualizer += path[i].ToString();

                if (i < path.Count - 1)
                {
                    pathVisualizer += " -> ";
                }
            }

            return pathVisualizer;
        }
    }
}
