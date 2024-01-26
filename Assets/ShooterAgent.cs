using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents; 
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class ShooterAgent : Agent
{
    // Transform agent_transform;
    Rigidbody2D agent_rb;
    public Transform target_transform;
    public GameObject bullet;
    Bullet bullet_script;
    Rigidbody2D bullet_rb;
    Transform bullet_transform;
    bool death_flag = false;
    bool reset_target_pos = true;
    // Start is called before the first frame update
    void Start()
    {
        // get Rigidbody2D
        agent_rb = GetComponent<Rigidbody2D>();
        // Get bullet script | Rigidbody2D | transform
        bullet_script = bullet.GetComponent<Bullet>();
        bullet_rb = bullet.GetComponent<Rigidbody2D>();
        bullet_transform = bullet.GetComponent<Transform>();
    }

    public override void OnEpisodeBegin()
    {
        if (death_flag)
        { // if death_flag is set, reset position of agent
            death_flag = false;
            agent_rb.velocity = Vector2.zero;
            this.transform.localPosition = new Vector2(0, -3.5f);
        }
        if (reset_target_pos)
        { // if reset_target_pos flag is set, move target box to a new random position
            target_transform.localPosition = new Vector3(Random.value * 18 - 9, Random.value * 5 - 1, 0);
            reset_target_pos = false;
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(target_transform.localPosition); // Target position
        sensor.AddObservation(bullet_transform.localPosition); // Bullet position
        sensor.AddObservation(bullet_rb.velocity.y); // Bullet velocity
        sensor.AddObservation(this.transform.localPosition); // Agent position
        sensor.AddObservation(agent_rb.velocity.x); // Agent velocity x
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // Agent Actions
        // Give x-axis movement to Agent
        Vector2 x_control = Vector2.zero;
        x_control.x = actionBuffers.ContinuousActions[0];
        agent_rb.AddForce(x_control * 10);

        // Give "shooting" action to Agent
        int fire_control = 0;
        fire_control = actionBuffers.DiscreteActions[0];
        if (fire_control > 0 && bullet_script.can_be_fired)
        { // Can only fire if bullet is inactive (aka only after bullet hit the target or went off screen)
            Vector2 bullet_start_position = new Vector2(this.transform.localPosition.x, this.transform.localPosition.y+1);
            bullet_script.Fire_Bullet(bullet_start_position);
        }

        float bulletToTarget = Vector2.Distance(bullet_transform.localPosition, target_transform.localPosition);
        if (bulletToTarget < 1.0f)
        { // if bullet is close enough to target award agent 
            SetReward(1.0f);
            bullet_script.HitObstacle();
            reset_target_pos = true;
            EndEpisode();
        }

        if (this.transform.localPosition.x < -10 || this.transform.localPosition.x > 10)
        { // if agent tries to go out of bounds, remove reward a bit and reset episode
            SetReward(-0.1f);
            EndEpisode();
            death_flag = true;
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // Allow user to move Agent with keyboard
        // Move with "ad" or the left and right arrows
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        // Shoot with the space bar
        var discreteActionsOut = actionsOut.DiscreteActions;
        if (Input.GetButton("Jump")) discreteActionsOut[0] = 1;
        else discreteActionsOut[0] = 0;
    }
}
