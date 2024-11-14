using PathFinding.Lab_9;

using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace PathFinding.Lab_9
{
    public class TestAStarAlgorithm : MonoBehaviour
    {
        // Use this for initialization
        void Start()
        {
            //Testing with EuclideanDistance heuristic
            //Graph g = new Graph();
            //Graph g = new Graph(HeuristicStrategy.EuclideanDistance);
            //g.add_vertex_for_AStar('A', new Vector3(0, 4, 0), new Dictionary<char, int>() { { 'B', 4 }, { 'X', 20 } });
            //g.add_vertex_for_AStar('B', new Vector3(4, 4, 0), new Dictionary<char, int>() { { 'A', 4 }, { 'C', 4 } });
            //g.add_vertex_for_AStar('C', new Vector3(8, 4, 0), new Dictionary<char, int>() { { 'B', 4 }, { 'Z', 4 } });
            //g.add_vertex_for_AStar('X', new Vector3(0, 0, 0), new Dictionary<char, int>() { { 'A', 20 }, { 'W', 4 }, { 'Y', 4 } });
            //g.add_vertex_for_AStar('Y', new Vector3(4, 0, 0), new Dictionary<char, int>() { { 'X', 4 }, { 'Z', 6 } });
            //g.add_vertex_for_AStar('Z', new Vector3(8, 0, 0), new Dictionary<char, int>() { { 'C', 4 }, { 'Y', 4 } });
            //g.add_vertex_for_AStar('W', new Vector3(12, 0, 0), new Dictionary<char, int>() { { 'X', 4 } });
            //print("g:" + g);
            ////List<char> shortest_path = g.shortest_path_via_AStar_algo('A', 'X');

            ////Testing with ManhattanDistance heuristic
            ////Graph g2 = new Graph();
            //Graph g2 = new Graph(HeuristicStrategy.ManhattanDistance);
            //g2.add_vertex_for_AStar('A', new Vector3(0, 4, 0), new Dictionary<char, int>() { { 'B', 4 }, { 'X', 20 } });
            //g2.add_vertex_for_AStar('B', new Vector3(4, 4, 0), new Dictionary<char, int>() { { 'A', 4 }, { 'C', 4 } });
            //g2.add_vertex_for_AStar('C', new Vector3(8, 4, 0), new Dictionary<char, int>() { { 'B', 4 }, { 'Z', 4 } });
            //g2.add_vertex_for_AStar('X', new Vector3(0, 0, 0), new Dictionary<char, int>() { { 'A', 20 }, { 'W', 4 }, { 'Y', 4 } });
            //g2.add_vertex_for_AStar('Y', new Vector3(4, 0, 0), new Dictionary<char, int>() { { 'X', 4 }, { 'Z', 6 } });
            //g2.add_vertex_for_AStar('Z', new Vector3(8, 0, 0), new Dictionary<char, int>() { { 'C', 4 }, { 'Y', 4 } });
            //g2.add_vertex_for_AStar('W', new Vector3(12, 0, 0), new Dictionary<char, int>() { { 'X', 4 } });
            //print("g2:" + g2);
            ////List<char> shortest_path = g.shortest_path_via_AStar_algo('A', 'X');

            ////Testing with DictionaryDistance heuristic
            ////Graph g3 = new Graph();
            //Graph g3 = new Graph(HeuristicStrategy.DictionaryDistance);
            //g3.add_vertex_for_AStar('A', new Vector3(12, 0, 0), new Dictionary<char, int>() { { 'B', 4 }, { 'X', 20 } });
            //g3.add_vertex_for_AStar('B', new Vector3(8, 0, 0), new Dictionary<char, int>() { { 'A', 4 }, { 'C', 4 } });
            //g3.add_vertex_for_AStar('C', new Vector3(4, 0, 0), new Dictionary<char, int>() { { 'B', 4 }, { 'Z', 4 } });
            //g3.add_vertex_for_AStar('X', new Vector3(8, 0, 0), new Dictionary<char, int>() { { 'A', 20 }, { 'W', 4 }, { 'Y', 4 } });
            //g3.add_vertex_for_AStar('Y', new Vector3(4, 0, 0), new Dictionary<char, int>() { { 'X', 4 }, { 'Z', 6 } });
            //g3.add_vertex_for_AStar('Z', new Vector3(0, 0, 0), new Dictionary<char, int>() { { 'C', 4 }, { 'Y', 4 } });
            //g3.add_vertex_for_AStar('W', new Vector3(4, 0, 0), new Dictionary<char, int>() { { 'X', 4 } });
            //print("g3:" + g3);
            //List<char> shortest_path = g.shortest_path_via_AStar_algo('A', 'X');

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}