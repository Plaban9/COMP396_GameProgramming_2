using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using static UnityEngine.GraphicsBuffer;

public class TestVectors : MonoBehaviour
{
    public Vector3 v1, v2, v3;
    float k;
    private float _distanceShouldBeCoveredInTime = 0f;
    private float _startTime;
    private float _speed = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        _startTime = Time.time;

        v1 = new Vector3(1, 2, 3);
        v2 = new Vector3(2, -1, 1);
        k = 1.5f;
        Vector3 v1_plus_v2 = v1 + v2;
        Vector3 v2_plus_v1 = v2 + v1;
        print($"v1_plus_v2 = {v1_plus_v2}, v2_plus_v1 = {v2_plus_v1}, diff = {v1_plus_v2 - v2_plus_v1}");

        //TODO:
        //Calc: V1.k, k.v1, print results
        Vector3 v1_multiply_k = v1 * k;
        Vector3 k_multiply_v1 = k * v1;
        print($"v1_multiply_k = {v1_multiply_k}, k_multiply_v1 = {k_multiply_v1}, Equality Check = {v1_plus_v2 == v2_plus_v1}");

        //Calc: v1_hat (unit vector), v2_hat
        Vector3 v1_hat = v1.normalized;
        Vector3 v1_hat_calculated = v1 / v1.magnitude;
        Vector3 v2_hat = v2.normalized;
        Vector3 v2_hat_calculated = v2 / v2.magnitude;
        print($"v1_hat = {v1_hat}, v2_hat = {v2_hat}, v1_hat_calculated = {v1_hat_calculated}, v2_hat_calculated = {v2_hat_calculated}");

        //Calc: v1.v2 (dot product), v1_hat.v2_hat
        //Vector3.Dot(v1, v2);
        float v1_dot_v2_calculated = v1.x * v2.x + v1.y * v2.y + v1.z * v2.z;
        float v1_dot_v2 = Vector3.Dot(v1, v2);
        float v1_hat_dot_v2_hat = Vector3.Dot(v1_hat, v2_hat);
        print($"v1_dot_v2 = {v1_dot_v2}, v1_hat_dot_v2_hat = {v1_hat_dot_v2_hat}, v1_dot_v2_calculated = {v1_dot_v2_calculated}, Equality Check = {v1_dot_v2 == v1_hat_dot_v2_hat}");

        //Calc: v1 x v2 (Cross product), v2 x v1, compare them
        //Vector3.Cross(v1, v2); 
        Vector3 v1_cross_v2_calculated = new Vector3(v1.y * v2.z - v1.z * v2.y, v1.z * v2.x - v1.x * v2.z, v1.x * v2.y - v1.y * v2.x);
        Vector3 v1_cross_v2 = Vector3.Cross(v1, v2);
        Vector3 v2_cross_v1 = Vector3.Cross(v2, v1);
        print($"v1_cross_v2 = {v1_cross_v2}, v2_cross_v1 = {v2_cross_v1}, v1_cross_v2_calculated = {v1_cross_v2_calculated}, Equality Check = {v1_cross_v2 == v2_cross_v1}");

        //Calc. Magnitue, Distance, 1-2 Lerps
        float v1_magnitude = v1.magnitude;
        float v1_v2_distance_calculated = Mathf.Sqrt(Mathf.Pow(v2.x - v1.x, 2) + Mathf.Pow(v2.y - v1.y, 2) + Mathf.Pow(v2.z - v1.z, 2));
        print($"v1_magnitude = {v1_magnitude}, v1_v2_distance = {v1_v2_distance_calculated}");


        //Demo 4-6 of the following: Vector3.AngleBetween, Vector3.Angle, Vector3.Distance, Vector3.Normalize, Vector3.SqrMagnitude, Vector3.Scale, Vector3.RotateTowards, Vector3.ClampMagnitude
        float v1_v2_angle_between_radians = Vector3.AngleBetween(v1, v2);
        float v1_v2_angle = Vector3.Angle(v1, v2);
        float v1_v2_distance = Vector3.Distance(v1, v2);
        Vector3 v1_normalized = v1.normalized;
        float v1_sqr_magnitude = v1.sqrMagnitude;
        Vector3 v1_scale_v2 = Vector3.Scale(v1, v2);
        Vector3 v1_clamped_magnitude = Vector3.ClampMagnitude(v1, k);

        print($"v1_v2_angle_between = {v1_v2_angle_between_radians}, v1_v2_angle = {v1_v2_angle}");
        print($"v1_v2_distance = {v1_v2_distance}, v1_normalized = {v1_normalized}");
        print($"v1_sqr_magnitude = {v1_sqr_magnitude}, v1_scale_v2 = {v1_scale_v2}, v1_clamped_magnitude = {v1_clamped_magnitude}");
    }

    // Update is called once per frame
    void Update()
    {
        //Lerp
        LerpAction();

        //Rotate Action
        RotateTowardsAction();
    }

    void LerpAction()
    {
        float distCovered = (Time.time - _startTime) * _speed;
        float journeyDone = distCovered / Vector3.Distance(v1, v2);
        v3 = Vector3.Lerp(v1, v2, journeyDone);
        print($"Lerped v3 = {v3}");
    }

    void RotateTowardsAction()
    {
        Vector3 targetDirection = v1 - v2;

        // The step size is equal to speed times frame time.
        float singleStep = _speed * Time.deltaTime;

        // Rotate the forward vector towards the target direction by one step
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);

        // Draw a ray pointing at our target in
        Debug.DrawRay(transform.position, newDirection, Color.red);

        // Calculate a rotation a step closer to the target and applies rotation to this object
        transform.rotation = Quaternion.LookRotation(newDirection);
    }
}
