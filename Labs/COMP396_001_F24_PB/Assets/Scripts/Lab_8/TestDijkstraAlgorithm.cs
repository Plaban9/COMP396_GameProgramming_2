using System.Collections;
using System.Collections.Generic;

using Unity.VisualScripting;

using UnityEngine;

namespace PathFinding
{
    public class TestDijkstraAlgorithm : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            Graph g = new Graph();

            g.AddVertex('A', new Dictionary<char, int>()
            {
                {'B', 10 },
                {'C', 12 },
                {'D', 4 },
                {'E', 2 }
            });

            g.AddVertex('B', new Dictionary<char, int>()
            {
                {'C', 2 },
                {'D', 4 },
                {'E', 5 },
                {'F', 5 }
            });

            g.AddVertex('C', new Dictionary<char, int>()
            {
                {'B', 6 },
                {'F', 2 }
            });

            g.AddVertex('D', new Dictionary<char, int>()
            {
                {'B', 3 },
                {'E', 3 }
            });

            g.AddVertex('E', new Dictionary<char, int>()
            {
                {'B', 3 },
                {'D', 3 },
                {'F', 9 }
            });

            g.AddVertex('F', new Dictionary<char, int>());

            print("g:" + g);

            char start = 'A';
            char end = 'F';

            var shortest_path = g.ShortestPathViaDijkstra(start, end);         

            Debug.Log("The shortest path: " + GetVisualizedPath(shortest_path.GetPath(), end));
            Debug.Log("Length of the shortest path: " + shortest_path.GetLength());

            Debug.Log("----- DFS TEST START -----");
            g.TestDFS();
            Debug.Log("----- DFS TEST END -----");
        }

        private string GetVisualizedPath(List<char> path, char end)
        {
            var pathVisualizer = "";

            foreach (var node in path)
            {
                pathVisualizer += node.ToString();

                if (node != end)
                {
                    pathVisualizer += " -> ";
                }
            }

            return pathVisualizer;
        }
    }
}
