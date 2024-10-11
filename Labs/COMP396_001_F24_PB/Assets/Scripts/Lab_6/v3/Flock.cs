
using UnityEngine;
namespace Lab_6.V3
{

    public class Flock : MonoBehaviour
    {
        public int maxNeighbors = 5; //maximum number of neighbors to consider for cohesion and alignment                                     
        [SerializeField] public float cohesionStrength = 1.0f; // how strong is the desire to stay close to others?
        [SerializeField] public float separationStrength = 1.0f; // how strong is the desire to avoid collisions?
        [SerializeField] public float alignmentStrength = 1.0f; // how strong is the desire to align with others?

        [SerializeField] private FlockBehavior cohesion;
        [SerializeField] private FlockBehavior separation;
        [SerializeField] private FlockBehavior alignment;

        void Start()
        {

            //cohesion = new Cohesion(maxNeighbors, cohesionStrength);
            //separation = new Separation(maxNeighbors, separationStrength);
            //alignment = new Alignment(maxNeighbors, alignmentStrength);

            // Initialize the flock's position and velocity
            transform.position = Random.insideUnitCircle * 10f;
            GetComponent<Rigidbody>().velocity = Vector3.zero;

            cohesion.SetStrength(cohesionStrength);
            separation.SetStrength(separationStrength);
            alignment.SetStrength(alignmentStrength);
        }

        void FixedUpdate()
        {
            cohesion.Update(transform.position);
            separation.Update(transform.position);
            alignment.Update(transform.position);

            // Combine the forces to get a new velocity
            Vector3 newVelocity = cohesion.GetForce() + separation.GetForce() + alignment.GetForce();

            // Normalize the velocity vector
            newVelocity.Normalize();


            // Apply the force to the entity's Rigidbody
            GetComponent<Rigidbody>().AddForce(newVelocity * 50f);
        }

        void Update()
        {
            // Keep the flock within a certain radius
            if (Vector3.Distance(transform.position, transform.parent.position) > 20f)
            {
                transform.position = Vector3.MoveTowards(transform.position, transform.parent.position, 1f);
            }
        }
    }
}