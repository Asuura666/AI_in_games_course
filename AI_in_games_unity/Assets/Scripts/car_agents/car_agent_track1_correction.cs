using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;


public class car_agent_track1_correction : car_agent
{
    // To set in inspector
    [SerializeField] private Transform toSet_training_positions;
    

    /// <summary>
    /// Call back of the mlagent OnEpisodeBegin() function.
    /// In this project, the base OnEpisodeBegin() function is used to initialise car agent 
    /// and target positions.
    /// </summary>
    protected override void _onEpisodeBegin()
    {
        // We randomly select one starting point at the beginning of the episode
        for(int i=0; i<toSet_training_positions.transform.childCount; i++)
        {
            toSet_training_positions.transform.GetChild(i).gameObject.SetActive(false);
        }
        positionStep = Random.Range(0, toSet_training_positions.transform.childCount);
        toSet_training_positions.transform.GetChild(positionStep).gameObject.SetActive(true);
    }


    /// <summary>
    /// Find and return the target to be used at the start of an episode.
    /// It is called in the base OnEpisodeBegin() function.
    /// </summary>
    /// <returns>Return the transform of the wanted target.</returns>
    protected override Transform _getTarget()
    {
        return toSet_training_positions.transform.GetChild(positionStep).GetChild(1);
    }

    /// <summary>
    /// Find and return the "start" object to be used to initialise the car agent position
    /// at the start of an episode.
    /// It is called in the base OnEpisodeBegin() function.
    /// </summary>
    /// <returns>Return the transform of the wanted "start" object.</returns>
    protected override Transform _getStart()
    {
        return toSet_training_positions.transform.GetChild(positionStep).GetChild(0);  
    }

    /// <summary>
    /// Regroup all observations used by the car agent during training.
    /// Use "sensor.AddObservation(obs)" to add an observation.
    /// </summary>
    /// <param name="sensor">Vector listing all the agent observations</param>
    public override void CollectObservations(VectorSensor sensor)
    {
        // Target and Agent positions
        sensor.AddObservation(distanceVector(this.transform, target)); //3 observations

        // Agent velocity
        sensor.AddObservation(rBody.velocity.x);
        sensor.AddObservation(rBody.velocity.z);
    }

    private float lastDistance;
    /// <summary>
    /// Set the rewards given to the agent during training.
    /// Use "AddReward(float)" or "SetReward(float)" to add reward goal.
    /// Note that rewards depending on collision are to be set in "_collisionRewards" or 
    /// "_triggerRewards"
    /// </summary>
    protected override void _fixRewards()
    {
        // Approached target
        float distanceToTarget = Vector3.Distance(this.transform.position, target.position);
        if(distanceToTarget < lastDistance)
        {
            AddReward(0.00001f);
        }
        else
        {
            AddReward(-0.00005f);
        }
        lastDistance = distanceToTarget;

    }

    /// <summary>
    /// Set the rewards resulting to a collision with an external hitbox.
    /// Use "AddReward(float)" or "SetReward(float)" to add reward goal.
    /// </summary>
    /// <param name="tag">Tag of the object colliding with the agent</param>
    protected override void _collisionRewards(string tag)
    {
        if(tag == "Death")
        {
            SetReward(-1.0f);
            EndEpisode();
        }
    }

    /// <summary>
    /// Set the rewards resulting to a collision with an external trigger hitbox.
    /// Use "AddReward(float)" or "SetReward(float)" to add reward goal.
    /// You can set different behaviour whether the agent is inside or leaving the hitbox.
    /// Note that when an agent is inside a trigger hitbox, this function is called every frame.
    /// </summary>
    /// <param name="tag">Tag of the object colliding with the agent</param>
    /// <param name="is_inside">Set to true when the agent is inside the trigger hitbox, and to false when it is leaving.</param>
    protected override void _triggerRewards(string tag, bool is_inside)
    {
        if(is_inside)
        {
            if(tag == "Target")
            {
                SetReward(1.0f);

                // When the target is reached, we switch to the next target.
                toSet_training_positions.transform.GetChild(positionStep).gameObject.SetActive(false);
                if(positionStep + 1 >= toSet_training_positions.transform.childCount)
                {
                    positionStep = 0;
                }
                else
                {
                    positionStep++;
                }
                
                toSet_training_positions.transform.GetChild(positionStep).gameObject.SetActive(true);
                target = toSet_training_positions.transform.GetChild(positionStep).GetChild(1);
                target.position = toSet_training_positions.transform.GetChild(positionStep).GetChild(1).position;
            }
        }
    }

    /// <summary>
    /// Set the action to be executed by the agent when receiving the model outputs.
    /// You can interact with the car_controller script to link the model output to actions in the environment.
    /// The model output are stored in the actionbuffers.
    /// Outputs can be retrieved using "actionBuffers.DiscreteActions[int]" or "actionBuffers.ContinuousActions[int]
    /// /// </summary>
    /// <param name="actionBuffers">Output of the neural network model.</param>
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        //Break
        if(actionBuffers.DiscreteActions[0] == 1)
        {
            car_script.isBreaking = true;
        }
        else if(actionBuffers.DiscreteActions[0] == 0)
        {
            car_script.isBreaking = false;
        }

        //Horizontal
        if(actionBuffers.DiscreteActions[1] == 0)
        {
            car_script.horizontalInput = 0;
        }
        else if(actionBuffers.DiscreteActions[1] == 1)
        {
            car_script.horizontalInput = 1;
        }
        else if(actionBuffers.DiscreteActions[1] == 2)
        {
            car_script.horizontalInput = -1;
        }

        //Vertical
        if(actionBuffers.DiscreteActions[2] == 0)
        {
            car_script.verticalInput = 0;
        }
        else if(actionBuffers.DiscreteActions[2] == 1)
        {
            car_script.verticalInput = 1;
        }
        else if(actionBuffers.DiscreteActions[2] == 2)
        {
            car_script.verticalInput = -1;
        }

       _fixRewards();
    }

    
}

