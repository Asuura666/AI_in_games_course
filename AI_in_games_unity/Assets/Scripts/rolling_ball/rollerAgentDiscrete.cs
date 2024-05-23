using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using System.Threading;
using Unity.VisualScripting;

public class rollerAgentDiscrete : Agent
{
    private Rigidbody rBody;
    private float lastDistanceToTarget;
    //private float lastDistanceToObstacle;
    public Transform target;

    //Create obstacle
    public Transform obstacle;
    public Transform obstacle1;
    public Transform obstacle2;
    public Transform obstacle3;
    public Transform obstacle4;
    public Transform obstacle5;
    public Transform obstacle6;
    void Start () 
    {
        rBody = GetComponent<Rigidbody>();
        lastDistanceToTarget = 1000f;
        //lastDistanceToObstacle = 1000f;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        if(Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0)
        {
            discreteActionsOut[0] = 0;
        }
        else if(Input.GetAxis("Horizontal") > 0)
        {
            discreteActionsOut[0] = 1;
        }
        else if(Input.GetAxis("Horizontal") < 0)
        {
            discreteActionsOut[0] = 2;
        }
        else if(Input.GetAxis("Vertical") > 0)
        {
            discreteActionsOut[0] = 3;
        }
        else if(Input.GetAxis("Vertical") < 0)
        {
            discreteActionsOut[0] = 4;
        }
    }

    public override void OnEpisodeBegin()
    {
       // If the Agent fell, zero its momentum
        if (this.transform.localPosition.y < 0)
        {
        }

        this.rBody.angularVelocity = Vector3.zero;
        this.rBody.velocity = Vector3.zero;
        this.transform.localPosition = new Vector3( 0, 0.5f, 0);
        
        // Move the target to a new spot
        target.localPosition = new Vector3(Random.value * 16 - 8,
                                           1f,
                                           Random.value * 16 - 8);
        while((target.localPosition==obstacle.localPosition) && (target.localPosition==obstacle1.localPosition) && target.localPosition==obstacle2.localPosition && target.localPosition==obstacle3.localPosition && target.localPosition==obstacle4.localPosition && target.localPosition==obstacle5.localPosition && target.localPosition==obstacle6.localPosition)
        {
        target.localPosition = new Vector3(Random.value * 16 - 8,
                                           1f,
                                           Random.value * 16 - 8);
        }
    }

    protected Vector2 distanceVector(Transform object1, Transform object2)
    {
        float x, z;
        x = object1.transform.position.x - object2.transform.position.x;
        z = object1.transform.position.z - object2.transform.position.z;
        return new Vector2(x,  z);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Target and Agent positions
        //sensor.AddObservation(distanceVector(this.transform, target)); //2 observations
        sensor.AddObservation(this.transform.localPosition);
        sensor.AddObservation(target.localPosition);
        sensor.AddObservation(obstacle.localPosition);
        sensor.AddObservation(obstacle1.localPosition);
        sensor.AddObservation(obstacle2.localPosition);
        sensor.AddObservation(obstacle3.localPosition);
        sensor.AddObservation(obstacle4.localPosition);
        sensor.AddObservation(obstacle5.localPosition);
        sensor.AddObservation(obstacle6.localPosition);

        // Agent velocity
        sensor.AddObservation(rBody.velocity.x);
        sensor.AddObservation(rBody.velocity.z);

        // Step count
        sensor.AddObservation(this.StepCount);
    }

    public float forceMultiplier = 10;
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // Actions, size = 2
        Vector3 controlSignal = Vector3.zero;

        if(actionBuffers.DiscreteActions[0] == 0)
        {
            controlSignal.x = 0;
            controlSignal.z = 0;
        }
        else if(actionBuffers.DiscreteActions[0] == 1)
        {
            controlSignal.x = 1;
            controlSignal.z = 0;
        }
        else if(actionBuffers.DiscreteActions[0] == 2)
        {
            controlSignal.x = -1;
            controlSignal.z = 0;
        }

        if(actionBuffers.DiscreteActions[0] == 3)
        {
            controlSignal.x = 0;
            controlSignal.z = 1;
        }
        else if(actionBuffers.DiscreteActions[0] == 4)
        {
            controlSignal.x = 0;
            controlSignal.z = -1;
        }

        rBody.AddForce(controlSignal * forceMultiplier);

        // Rewards
        float distanceToTarget = Vector3.Distance(this.transform.localPosition, target.localPosition);
        float distanceToObstacle = Vector3.Distance(this.transform.localPosition, obstacle.localPosition);
        float distanceToObstacle1 = Vector3.Distance(this.transform.localPosition, obstacle1.localPosition);
        float distanceToObstacle2 = Vector3.Distance(this.transform.localPosition, obstacle2.localPosition);
        float distanceToObstacle3 = Vector3.Distance(this.transform.localPosition, obstacle3.localPosition);
        float distanceToObstacle4 = Vector3.Distance(this.transform.localPosition, obstacle4.localPosition);
        float distanceToObstacle5 = Vector3.Distance(this.transform.localPosition, obstacle5.localPosition);
        float distanceToObstacle6 = Vector3.Distance(this.transform.localPosition, obstacle6.localPosition);
            // Getting closer or further from target
        if(distanceToTarget < lastDistanceToTarget)
        {
            AddReward(0.5f);
        }
        else
        {
            AddReward(-0.5f);
        }
        lastDistanceToTarget = distanceToTarget;
    
            // Reached target
        if (distanceToTarget < 1.42f)
        {
            SetReward(1.0f);
            EndEpisode();
        }
        //Reache Obstacle
        if (distanceToObstacle< 1.44f)
        {
            //Debug.Log(distanceToObstacle);
            SetReward(-0.01f);
        }
        if (distanceToObstacle1< 1.44f)
        {
            //Debug.Log(distanceToObstacle1);
            SetReward(-0.01f);
        }
        if (distanceToObstacle2< 1.44f)
        {
            //Debug.Log(distanceToObstacle2);
            SetReward(-0.01f);
        }
        if (distanceToObstacle3< 1.44f)
        {
            //Debug.Log(distanceToObstacle3);
            SetReward(-0.01f);
        }
        if (distanceToObstacle4< 1.44f)
        {
            //Debug.Log(distanceToObstacle4);
            SetReward(-0.01f);
        }
        if (distanceToObstacle5< 1.44f)
        {
            //Debug.Log(distanceToObstacle5);
            SetReward(-0.01f);
        }
        if (distanceToObstacle6< 1.44f)
        {
            //Debug.Log(distanceToObstacle6);
            SetReward(-0.01f);
        }
            // Fell off platform
        if (this.transform.localPosition.y < 0)
        {
            SetReward(-1f);
            EndEpisode();
        }

    }

    private void FixedUpdate()
    {
        
        if(this.StepCount >= 1000)
        {
            SetReward(-0.5f);
            EndEpisode();
        }
    }

}