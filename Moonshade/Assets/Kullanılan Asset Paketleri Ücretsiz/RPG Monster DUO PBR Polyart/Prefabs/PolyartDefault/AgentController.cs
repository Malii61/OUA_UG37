using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class AgentController : Agent
{
   
   [SerializeField] private Transform hedef;

    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(-4f,-2.165f,-14f);

        int randm=Random.Range(0,2);
        if(randm==0)
        {
            hedef.localPosition=new Vector3(-6f,-2.323f,-14f);
        }
        if( randm ==1)
        {
            hedef.localPosition=new Vector3(-2f,-2.323f,-14f);
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
       sensor.AddObservation(transform.localPosition);
       sensor.AddObservation(hedef.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float move = actions.ContinuousActions[0];
        float moveSpeed=2f;

        transform.localPosition +=new Vector3(move,0f) * Time.deltaTime*moveSpeed;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousAction=actionsOut.ContinuousActions;
        continuousAction[0]=Input.GetAxisRaw("Horizontal");
    }

    private void OnTriggerEnter(Collider nesne)
 { 
    if(nesne.gameObject.tag =="Potion")
    {
        AddReward(2f);
        EndEpisode();
    }
    if (nesne.gameObject.tag == "Wall")
    {
        AddReward(-1f);
        EndEpisode();
    }
 }    
}
