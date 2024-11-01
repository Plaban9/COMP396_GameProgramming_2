
using UnityEngine;
namespace Lab_6.V3
{

    public abstract class FlockBehavior : MonoBehaviour
    {
        protected float strength; // how strong is this behavior?
        protected int maxNeighbors;
        protected Vector3[] neighbors;

        void Start()
        {
            // Initialize the neighbors array to store nearby entities
            maxNeighbors = 5;
            neighbors = new Vector3[maxNeighbors];
        }

        public void SetStrength(float strength)
        {
            this.strength = strength;
        }

        abstract public Vector3 GetForce(); // returns a vector representing the force of this behavior
        abstract public void UpdateBehaviour(Vector3 position); // updates the neighbors array and calculates the force for this behavior
    }
}