using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using NUnit;
using Unity.VisualScripting;
using Unity.MLAgents.Policies;
public class DodgeballAgent : Agent
{
    public GameObject area;
    public float playerSpeed = 3f;
    public float rotationSpeed = 200f; 
    [SerializeField] List<GameObject> balls;
    [SerializeField] float distanceToPickUp = 2f;

    public int NumberOfTimesPlayerCanBeHit = 2;
    public int HitPointsRemaining;
    public BehaviorParameters behaviorParameters;
    public int teamID;
    private Vector3 startingPos;
    private Quaternion startingRot;
    public Rigidbody agentRb;
    private DodgeballEnvController envController;
    private bool firstInitialize = true;
    public override void Initialize() {
        agentRb = GetComponent<Rigidbody>();
        envController =  GetComponentInParent<DodgeballEnvController>();
        behaviorParameters = gameObject.GetComponent<BehaviorParameters>();
        teamID = behaviorParameters.TeamId;
        if (firstInitialize)
        {
            startingPos = transform.position;
            startingRot = transform.rotation;
            firstInitialize = false;
            
        }
        
    }
    public override void CollectObservations(VectorSensor sensor) {
        sensor.AddObservation(transform.position);
        sensor.AddObservation(transform.rotation);
    }

    public override void OnActionReceived(ActionBuffers actions) {
        //Debug.Log("Action Recieved");

        int ball_action = actions.DiscreteActions[0];

        if (ball_action == 1) {
            pickupBall();
        } else if (ball_action == 2) {
            throwBall();
        } else {

        }

        float moveInput = actions.DiscreteActions[1]; // Move forward/backward
        float rotateInput = actions.ContinuousActions[0]; // Rotation (left/right)

        Vector3 move = transform.forward * moveInput * playerSpeed * Time.deltaTime;
        transform.position += move;

        float rotation = rotateInput * rotationSpeed * Time.deltaTime;
        transform.Rotate(0, rotation, 0);
    }

    private void pickupBall() {
        foreach (GameObject ball in balls) {
            if (Vector3.Distance(ball.transform.position, transform.position) < distanceToPickUp) {
                DodgeballController script = ball.GetComponent<DodgeballController>();
                script.pickUpBall(transform, this);
                // this.SetReward(1.0f);
                break;
            }
        }
    }

    private void throwBall() {
        foreach (GameObject ball in balls) {
            if (Vector3.Distance(ball.transform.position, transform.position) < distanceToPickUp) {
                DodgeballController script = ball.GetComponent<DodgeballController>();
                script.throwBall(transform, this);
                break;
            }
        }
    }

     public void ResetAgent()
    {
        transform.position = startingPos;
        transform.rotation = Quaternion.Euler(new Vector3(0f, (1 - teamID) * 180, 0f));
        agentRb.linearVelocity = Vector3.zero;
        agentRb.angularVelocity = Vector3.zero;
        agentRb.linearDamping = 4;
        agentRb.angularDamping = 1;
    }

    
    private void OnCollisionEnter(Collision other) {
        DodgeballController ballController = other.gameObject.GetComponent<DodgeballController>();
        if (other.gameObject.CompareTag("Wall")) {
            //Debug.Log("Wall hit");

            this.AddReward(-0.5f);

        } //else if (other.gameObject.CompareTag("Ball")) {
            //Debug.Log("Ball hit");
            //envController.PlayerHit(this, ballController.thrownBy);
        //} 
        
    }


}