using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using static UnityEditor.PlayerSettings;

public class FlockController : MonoBehaviour
{
    public Vector3 bound;
    public float speed = 100.0f;
    public float targetReachedRadius = 10.0f;

    private Vector3 initialPosition;
    private Vector3 nextMovementPoint;

    // Use this for initialization
    void Start()
    {
        initialPosition = transform.position;
        CalculateNextMovementPoint();
    }

    void CalculateNextMovementPoint()
    {
        //float posX = Random.Range(initialPosition.x - bound.x, initialPosition.x + bound.x);
        //float posY = Random.Range(initialPosition.y - bound.y, initialPosition.y + bound.y);
        //float posZ = Random.Range(initialPosition.z - bound.z, initialPosition.z + bound.z);

        float posX = Random.Range(-bound.x / 2, bound.x / 2);
        float posY = Random.Range(-bound.y / 2, bound.y / 2);
        float posZ = Random.Range(-bound.z / 2, bound.z / 2);

        nextMovementPoint = initialPosition + new Vector3(posX, posY, posZ);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(nextMovementPoint - transform.position), 1.0f * Time.deltaTime);

        if (Vector3.Distance(nextMovementPoint, transform.position) <= targetReachedRadius)
            CalculateNextMovementPoint();
    }

    public void OnDrawGizmos()
    {
        var drawColor = Color.cyan * new Color(1f, 1f, 1f, 0.25f);

        Gizmos.color = drawColor;
        Gizmos.DrawCube(initialPosition, bound);

        drawColor = Color.magenta * new Color(1f, 1f, 1f, 0.5f);
        Gizmos.color = drawColor;
        Gizmos.DrawSphere(nextMovementPoint, targetReachedRadius);

        drawColor = Color.green * new Color(1f, 1f, 1f, 1f);
        Gizmos.color = drawColor;
        Gizmos.DrawSphere(transform.position, 2f);
    }
}