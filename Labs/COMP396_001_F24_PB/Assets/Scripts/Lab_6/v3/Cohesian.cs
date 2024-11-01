
using Lab_6.V3;

using Unity.VisualScripting;

using UnityEngine;
namespace Lab_6.V3
{
    public class Cohesion : FlockBehavior
    {
        //[SerializeField] float cohesionStrength;

        //public Cohesion(int maxNeighbors, float cohesionStrength)
        //{
        //    this.maxNeighbors = maxNeighbors;
        //    this.cohesionStrength = cohesionStrength;
        //}

        public override Vector3 GetForce()
        {
            // Calculate the average position of all neighbors
            float sumX = 0f;
            float sumY = 0f;
            float sumZ = 0f;

            for (int i = 0; i < maxNeighbors; i++)
            {
                if (neighbors[i] != Vector3.zero)
                {
                    sumX += neighbors[i].x;
                    sumY += neighbors[i].y;
                    sumZ += neighbors[i].z;
                }
            }

            float averageX = sumX / maxNeighbors;
            float averageY = sumY / maxNeighbors;
            float averageZ = sumZ / maxNeighbors;

            Vector3 averagePosition = new Vector3(averageX, averageY, averageZ);
            // Return a force vector pointing towards the average position
            return (averagePosition - transform.position).normalized * strength;
        }

        public override void UpdateBehaviour(Vector3 position)
        {
            // Find nearby entities within a certain radius
            float radius = 10f;
            int count = 0;
            for (int i = 0; i < maxNeighbors; i++)
            {
                Vector3 neighborPosition = position + (Random.onUnitSphere * radius);

                Flock entity = GameObject.Find("FlockEntityV3").GetComponent<Flock>();

                if (entity != null && Vector3.Distance(position, neighborPosition) <= radius)
                {
                    neighbors[count++] = neighborPosition;
                }

                if (count == maxNeighbors)
                    break;
            }
        }
    }
}
