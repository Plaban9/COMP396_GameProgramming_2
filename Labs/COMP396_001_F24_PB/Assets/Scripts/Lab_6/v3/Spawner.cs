using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Lab_6.V3
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField] private int _count = 10;
        [SerializeField] Flock _flockPrefab;

        void Awake()
        {
            for (int i = 0; i < _count; i++)
            {
                Flock flock = Instantiate(_flockPrefab, transform.position, transform.rotation, this.transform) as Flock;
                flock.separationStrength = Random.Range(400f, 600f);
                flock.cohesionStrength = Random.Range(575f, 750f);
                flock.alignmentStrength = Random.Range(100f, 200f);
                flock.maxNeighbors = Random.Range(1, _count);
                flock.name = "FlockEntityV3";
            }
        }
    }
}
