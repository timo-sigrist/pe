using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using UnityEditor;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Assertions.Must;

/*
    Accelerates the cube to which it is attached, modelling an harmonic oscillator.
    Writes the position, velocity and acceleration of the cube to a CSV file.
    
    Remark: For use in "Physics Engines" module at ZHAW, part of physics lab
    Author: kemf
    Version: 1.0
*/
public class CubeRightController : MonoBehaviour
{
    private Rigidbody rigidBodyCubeRight;
    private Rigidbody rigidBodyCubeLeft;

    public int springRightForce;
    public int springLeftForce;
    public GameObject springRight;
    public GameObject springLeft;
    public GameObject cubeLeft;

    public GameObject cubeInnerUpper;
    public GameObject cubeInnerLower;
    public GameObject cubeOuterUpper;
    public GameObject invisibleCube;

    public float windConstant;
    public bool oscillate;
    public Phase phase = Phase.oscillator;

    public GameObject centerOfMass;

    private float currentTimeStep;
    private List<List<float>> timeSeries;
    private float cubeRightForceX;
    private float cubeLeftForceX;
    private float springLeftFurthestPointRight;

    public enum Phase
    { oscillator, wind, collision, inelasticCollision, none }

    // Start is called before the first frame update
    void Start()
    {
        GameObject[] objects = new GameObject[] { cubeInnerUpper, cubeInnerLower, cubeOuterUpper, invisibleCube };
        centerOfMass.transform.position = CalculateCenterOfMass(objects);

        rigidBodyCubeRight = GetComponent<Rigidbody>();
        rigidBodyCubeLeft = cubeLeft.GetComponent<Rigidbody>();
        timeSeries = new List<List<float>>();
        springLeftFurthestPointRight = springLeft.transform.position.x + GetSpringLeftWidth() / 2f;
    }

    // FixedUpdate can be called multiple times per frame
    void FixedUpdate()
    {
        currentTimeStep += Time.deltaTime;
        var angular_momentum = 0.0f;
        var angularMomentumGlideVOR = Vector3.zero;
        var speed = 0f;

        switch(phase)
        {
            case Phase.oscillator:
                var deltaL = rigidBodyCubeRight.position.x - springRight.transform.position.x;
                cubeRightForceX = -deltaL * springRightForce;
                
                AddForceToRigidBody(cubeRightForceX);

                //Cube comes back from spring and now only the wind carries the cube
                if (rigidBodyCubeRight.position.x < springRight.transform.position.x && rigidBodyCubeRight.velocity.x < 0 && !oscillate) {
                    cubeRightForceX = 0;
                    phase = Phase.wind;
                }

                break;

            case Phase.wind:
                cubeRightForceX = windConstant;
                AddForceToRigidBody(cubeRightForceX);

                if(rigidBodyCubeRight.position.x <= springLeftFurthestPointRight)
                {
                    phase = Phase.collision;
                }

                break;

            case Phase.collision:
                cubeRightForceX = springLeftForce;
                AddForceToRigidBody(cubeRightForceX);

                cubeLeftForceX = -cubeRightForceX;
                cubeLeft.GetComponent<Rigidbody>().AddForce(new Vector3(cubeLeftForceX, 0f, 0f));

                speed = -6f;
                if (rigidBodyCubeRight.position.x >= springLeftFurthestPointRight)
                {
                    phase = Phase.none;
                }
                break;

            case Phase.inelasticCollision:
                GameObject[] objects = new GameObject[] { cubeInnerUpper, cubeInnerLower, cubeOuterUpper, cubeLeft };
                centerOfMass.transform.position = CalculateCenterOfMass(objects);

                angular_momentum = AngularMomentumRotate(cubeLeft.GetComponent<Rigidbody>());
                speed = -1.5f;
                break;

            case Phase.none:
                angularMomentumGlideVOR = AngularMomentumGlide(cubeLeft.GetComponent<Rigidbody>(), centerOfMass.transform.position);
                speed = -6f;
                break;

        }
        timeSeries.Add(new List<float>() { currentTimeStep, cubeLeft.GetComponent<Rigidbody>().position.x, cubeLeftForceX, rigidBodyCubeRight.position.x, rigidBodyCubeRight.velocity.x, cubeRightForceX, angular_momentum, cubeLeft.GetComponent<Rigidbody>().angularVelocity.y, cubeLeft.GetComponent<Rigidbody>().velocity.x});
    }

    private void AddForceToRigidBody(float cubeRightForceX) {
        rigidBodyCubeRight.AddForce(new Vector3(cubeRightForceX, 0f, 0f));
    }

    // Helper function to get the width of springLeft
    private float GetSpringLeftWidth()
    {
        Renderer renderer = springLeft.GetComponent<Renderer>();
        if (renderer != null)
        {
            return renderer.bounds.size.x;
        }
        else
        {
            Debug.LogError("Renderer component not found on the object.");
            return 0f;
        }
    }

    // Calculate the angular momentum L of a body at point Q with respect to the pivot point P
    private Vector3 AngularMomentumGlide(Rigidbody rb, Vector3 centerOfMass) {
        // get vector between pivot point and center of mass
        Vector3 R = rb.worldCenterOfMass - centerOfMass;

        // calculate momentum of the body
        Vector3 p = rb.velocity * rb.mass;

        // calculate and return angular momentum
        Vector3 angular_momentum = Vector3.Cross(R, p);
        return angular_momentum;
    }


    private float AngularMomentumRotate(Rigidbody rb) {
            var w = rb.angularVelocity;
            Debug.Log("w: " + w);

            float d_cubeLeft = Vector3.Magnitude(cubeLeft.transform.position - centerOfMass.transform.position);
            var J_cubeLeft = CalculateMomentOfInertiaOuter(2f, 1, d_cubeLeft);
            var angularMomentumRotate_cubeLeft = w * J_cubeLeft;

            float d_cubeOuterUpper = Vector3.Magnitude(cubeOuterUpper.transform.position - centerOfMass.transform.position);
            var J_cubeOuterUpper = CalculateMomentOfInertiaOuter(2f, 1, d_cubeOuterUpper);
            var angularMomentumRotate_cubeOuterUpper = w * J_cubeOuterUpper;

            float d_cubeInnerUpper = Vector3.Magnitude(cubeInnerUpper.transform.position - centerOfMass.transform.position);
            var J_cubeInnerUpper = CalculateMomentOfInertiaInner(2f, 1, d_cubeInnerUpper);
            var angularMomentumRotate_cubeInnerUpper = w * J_cubeInnerUpper;

            float d_cubeInnerLower = Vector3.Magnitude(cubeInnerLower.transform.position - centerOfMass.transform.position);
            var J_cubeInnerLower = CalculateMomentOfInertiaInner(2f, 1, d_cubeInnerLower);
            var angularMomentumRotate_cubeInnerLower = w * J_cubeInnerLower;

            var sumRotate = CalculateMagnitude(angularMomentumRotate_cubeLeft) + CalculateMagnitude(angularMomentumRotate_cubeOuterUpper) + CalculateMagnitude(angularMomentumRotate_cubeInnerUpper) + CalculateMagnitude(angularMomentumRotate_cubeInnerLower);
            return sumRotate;
    }
  


    private float CalculateMomentOfInertiaInner(float m, float a, float d)
    {
        // Moment of inertia formula for a cube
        float momentOfInertia = ((1.0f / 6.0f) * m * Mathf.Pow(a, 2)) + (m * Mathf.Pow(d, 2));
        return momentOfInertia;
    }

    private float CalculateMomentOfInertiaOuter(float m, float a, float d)
    {
        // Moment of inertia formula for a cube
        float momentOfInertia = ((1.0f / 6.0f) * m * Mathf.Pow(a, 2)) + (m * (Mathf.Pow(a, 2) + Mathf.Pow(d, 2)));
        return momentOfInertia;
    }

    void OnApplicationQuit() {
        WriteTimeSeriesToCSV();
    }

    void WriteTimeSeriesToCSV() {
        using (var streamWriter = new StreamWriter("data.csv")) {
            streamWriter.WriteLine("t,x(t) cubeL,F(t) cubeL,x(t) cubeR,v(t) cubeR,F(t) cubeR,L(y-axis) yellowL,w(t) cubeL,v(t) cubeL");
            
            foreach (List<float> timeStep in timeSeries) {
                streamWriter.WriteLine(string.Join(",", timeStep));
                streamWriter.Flush();
            }
        }
    }

    Vector3 CalculateCenterOfMass(GameObject[] objects)
    {
        Vector3 sumPositions = Vector3.zero;

        foreach (GameObject obj in objects)
        {
            sumPositions += obj.transform.position;
        }

        Vector3 centerOfMass = sumPositions / objects.Length;
        return centerOfMass;
    }

    // Methode zur Berechnung des Betrags eines Vector3
    public static float CalculateMagnitude(Vector3 vector)
    {
        return Mathf.Sqrt(vector.x * vector.x + vector.y * vector.y + vector.z * vector.z);
    }
}