using UnityEngine;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

public class CarController : MonoBehaviour
{
    internal enum driver
    {
        AI,
        keyboard
    }
    [SerializeField] driver driverController;
    public enum Axel
    {
        Front,
        Rear
    }

    [Serializable]
    public struct Wheel
    {
        public GameObject wheelModel;
        public WheelCollider wheelCollider;
        public Axel axel;
    }

    public float maxAcceleration = 30.0f;
    public float brakeAcceleration = 50.0f;

    public float offRoadAccelerationFactor = 0.7f; 
    private bool onRoad = true; 

    public float turnSensitivity = 1.0f;
    public float maxSteerAngle = 30.0f;

    public Vector3 _centerOfMass;

    public List<Wheel> wheels;

    float moveInput;
    float steerInput;

    private Rigidbody carRb;
    private bool halfRace;

    public string carName;
    public int currentNode;
    public trackWaypoints waypoints;
    public Transform currentWaypoint;
    public List<Transform> nodes = new List<Transform>();
    [Range(0, 10)] public int distanceOffset;
    [Range(0, 5)] public float sterrForce;
    [Range(0, 5)] public float speedForce;
    void Start()
    {
        carRb = GetComponent<Rigidbody>();
        carRb.centerOfMass = _centerOfMass;
        halfRace = false;
        carName = gameObject.name;
    }

    public void Awake()
    {
        waypoints = GameObject.FindGameObjectWithTag("Path").GetComponent<trackWaypoints>();
        nodes = waypoints.nodes;
    }

    void FixedUpdate()
    {
        calculateDistanceOfWayPoints();
        switch (driverController)
        {
            case driver.AI:
                AIDrive();
                break;
            case driver.keyboard:
                {
                    calculateDistanceOfWayPoints();
                    keyboardDrive();
                    break;
                }

        }
    }

    void LateUpdate()
    {
        Move();
        Steer();
        Brake();
    }

    void GetInputs()
    {
        moveInput = Input.GetAxis("Vertical");
        steerInput = Input.GetAxis("Horizontal");
    }

    void Move()
    {
       // Reduce acceleration if not on the road
        float currentAcceleration = onRoad ? maxAcceleration : maxAcceleration * offRoadAccelerationFactor;

        foreach (var wheel in wheels)
        {
            wheel.wheelCollider.motorTorque = -moveInput * 600 * currentAcceleration * Time.deltaTime;
        }
    }

    void Steer()
    {
        foreach (var wheel in wheels)
        {
            if (wheel.axel == Axel.Front)
            {
                var _steerAngle = steerInput * turnSensitivity * maxSteerAngle;
                wheel.wheelCollider.steerAngle = Mathf.Lerp(wheel.wheelCollider.steerAngle, _steerAngle, 0.6f);
            }
        }
    }

    void Brake()
    {
        if (Input.GetKey(KeyCode.Space) || moveInput == 0)
        {
            foreach (var wheel in wheels)
            {
                wheel.wheelCollider.brakeTorque = 300 * brakeAcceleration * Time.deltaTime;
            }
        }
        else
        {
            foreach (var wheel in wheels)
            {
                wheel.wheelCollider.brakeTorque = 0;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the car enters a road collider
        if (collision.gameObject.CompareTag("road"))
        {
            onRoad = true;
            Debug.Log($"{carName} is on the road!");
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        // Check if the car exits a road collider
        if (collision.gameObject.CompareTag("road"))
        {
            onRoad = false;
            Debug.Log($"{carName} is off the road!");
        }
    }

    void AnimateWheels()
    {
        foreach (var wheel in wheels)
        {
            Quaternion rot;
            Vector3 pos;
            wheel.wheelCollider.GetWorldPose(out pos, out rot);
            wheel.wheelModel.transform.position = pos;
            wheel.wheelModel.transform.rotation = rot;
        }
    }
    private void AIDrive()
    {
        moveInput = speedForce;
        AISteer();
    }
    private void keyboardDrive()
    {
        GetInputs();
        AnimateWheels();
    }
    private void calculateDistanceOfWayPoints()
    {
        Vector3 position = gameObject.transform.position;
        float distance = Mathf.Infinity;
        //Debug.Log(nodes.Count);

        for (int i = 0; i < nodes.Count; i++)
        {
            Vector3 difference = nodes[i].transform.position - position;
            float currentDistance = difference.magnitude;

            if (currentDistance < distance)
            {
                if ((i + distanceOffset) >= nodes.Count)
                {
                    currentWaypoint = nodes[1];
                    distance = currentDistance;
                }
                else
                {
                    currentWaypoint = nodes[i + distanceOffset];
                    distance = currentDistance;
                }
            }

            currentNode = i;
            //string message = carName.ToString() + " " + currentNode.ToString();
            //Debug.Log(message);
        }
    }

    private void AISteer()
    {
        Vector3 relative = transform.InverseTransformPoint(currentWaypoint.transform.position);
        relative /= relative.magnitude;

        steerInput = (relative.x / relative.magnitude) * -sterrForce;
    }
    public void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(currentWaypoint.transform.position, 3);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("CheckPoint"))
        {
            halfRace = true;
        }
        if (other.gameObject.CompareTag("FinishRace") && halfRace)
        {
            SceneManager.LoadSceneAsync(2);
        }
    }

}
