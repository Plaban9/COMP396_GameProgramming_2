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
            Graph g = new Graph(HeuristicStrategy.EuclideanDistance);
            g.add_vertex_for_AStar_with_position('A', new Vector3(0, 4, 0), new Dictionary<char, int>() { { 'B', 4 }, { 'X', 20 } });
            g.add_vertex_for_AStar_with_position('B', new Vector3(4, 4, 0), new Dictionary<char, int>() { { 'A', 4 }, { 'C', 4 } });
            g.add_vertex_for_AStar_with_position('C', new Vector3(8, 4, 0), new Dictionary<char, int>() { { 'B', 4 }, { 'Z', 4 } });
            g.add_vertex_for_AStar_with_position('X', new Vector3(8, 0, 0), new Dictionary<char, int>() { { 'A', 20 }, { 'W', 4 }, { 'Y', 4 } });
            g.add_vertex_for_AStar_with_position('Y', new Vector3(4, 0, 0), new Dictionary<char, int>() { { 'X', 4 }, { 'Z', 6 } });
            g.add_vertex_for_AStar_with_position('Z', new Vector3(8, 0, 0), new Dictionary<char, int>() { { 'C', 4 }, { 'Y', 4 } });
            g.add_vertex_for_AStar_with_position('W', new Vector3(12, 0, 0), new Dictionary<char, int>() { { 'X', 4 } });
            print("----------------------------- G1:" + g);
            var shortest_path_euclidean = g.shortest_path_via_AStar_algo('A', 'W');
            Debug.Log($"For Euclidean: {shortest_path_euclidean.GetVisualizedPath()}, Length = {shortest_path_euclidean.GetLengthF()}");

            //Testing with ManhattanDistance heuristic
            //Graph g2 = new Graph();
            Graph g2 = new Graph(HeuristicStrategy.ManhattanDistance);
            g2.add_vertex_for_AStar_with_position('A', new Vector3(0, 4, 0), new Dictionary<char, int>() { { 'B', 4 }, { 'X', 20 } });
            g2.add_vertex_for_AStar_with_position('B', new Vector3(4, 4, 0), new Dictionary<char, int>() { { 'A', 4 }, { 'C', 4 } });
            g2.add_vertex_for_AStar_with_position('C', new Vector3(8, 4, 0), new Dictionary<char, int>() { { 'B', 4 }, { 'Z', 4 } });
            g2.add_vertex_for_AStar_with_position('X', new Vector3(8, 0, 0), new Dictionary<char, int>() { { 'A', 20 }, { 'W', 4 }, { 'Y', 4 } });
            g2.add_vertex_for_AStar_with_position('Y', new Vector3(4, 0, 0), new Dictionary<char, int>() { { 'X', 4 }, { 'Z', 6 } });
            g2.add_vertex_for_AStar_with_position('Z', new Vector3(8, 0, 0), new Dictionary<char, int>() { { 'C', 4 }, { 'Y', 4 } });
            g2.add_vertex_for_AStar_with_position('W', new Vector3(12, 0, 0), new Dictionary<char, int>() { { 'X', 4 } });
            print(" ----------------------------- G2:" + g2);
            var shortest_path_Manhattan = g2.shortest_path_via_AStar_algo('A', 'W');
            Debug.Log($"For Manhattan: {shortest_path_Manhattan.GetVisualizedPath()}, Length = {shortest_path_Manhattan.GetLengthF()}");

            //Testing with DictionaryDistance heuristic
            //Graph g3 = new Graph();
            Graph g3 = new Graph(HeuristicStrategy.DictionaryDistance);
            g3.add_vertex_for_AStar_with_heuristic('A', 12, new Dictionary<char, int>() { { 'B', 4 }, { 'X', 20 } });
            g3.add_vertex_for_AStar_with_heuristic('B', 8, new Dictionary<char, int>() { { 'A', 4 }, { 'C', 4 } });
            g3.add_vertex_for_AStar_with_heuristic('C', 4, new Dictionary<char, int>() { { 'B', 4 }, { 'Z', 4 } });
            g3.add_vertex_for_AStar_with_heuristic('X', 8, new Dictionary<char, int>() { { 'A', 20 }, { 'W', 4 }, { 'Y', 4 } });
            g3.add_vertex_for_AStar_with_heuristic('Y', 4, new Dictionary<char, int>() { { 'X', 4 }, { 'Z', 6 } });
            g3.add_vertex_for_AStar_with_heuristic('Z', 0, new Dictionary<char, int>() { { 'C', 4 }, { 'Y', 4 } });
            g3.add_vertex_for_AStar_with_heuristic('W', 4, new Dictionary<char, int>() { { 'X', 4 } });
            print(" ----------------------------- G3:" + g3);
            var shortest_path_dictionary = g3.shortest_path_via_AStar_algo('A', 'W');
            Debug.Log($"For Dictionary: {shortest_path_dictionary.GetVisualizedPath()}, Length = {shortest_path_dictionary.GetLengthF()}");


            Graph g4 = new Graph(HeuristicStrategy.DictionaryDistance);
            g4.add_vertex_for_AStar_with_heuristic('A', 0, new Dictionary<char, int>() { { 'B', 4 }, { 'X', 4 } });
            g4.add_vertex_for_AStar_with_heuristic('B', 0, new Dictionary<char, int>() { { 'A', 4 }, { 'C', 4 } });
            g4.add_vertex_for_AStar_with_heuristic('C', 0, new Dictionary<char, int>() { { 'B', 4 }, { 'Z', 8 } });
            g4.add_vertex_for_AStar_with_heuristic('X', 0, new Dictionary<char, int>() { { 'A', 4 }, { 'Y', 4 } });
            g4.add_vertex_for_AStar_with_heuristic('Y', 0, new Dictionary<char, int>() { { 'X', 4 }, { 'Z', 6 } });
            g4.add_vertex_for_AStar_with_heuristic('Z', 0, new Dictionary<char, int>() { { 'C', 8 }, { 'Y', 6 }, { 'W', 4 } });
            g4.add_vertex_for_AStar_with_heuristic('W', 0, new Dictionary<char, int>() { { 'Z', 4 } });
            print(" ----------------------------- G4:" + g4);
            var shortest_path_dictionary_g4 = g4.shortest_path_via_AStar_algo('A', 'Z');
            Debug.Log($"For Graph - 4 Dictionary: {shortest_path_dictionary_g4.GetVisualizedPath()}, Length = {shortest_path_dictionary_g4.GetLengthF()}");

        }
    }
}