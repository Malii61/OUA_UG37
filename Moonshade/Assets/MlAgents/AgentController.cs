using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using JetBrains.Annotations;


public class AgentController : Agent
{
   
   [SerializeField] private Transform hedef;
   public int potionCount;
   public GameObject food;
   [SerializeField] private List<GameObject> spawnedPotionList = new List<GameObject>();
   [SerializeField] private float moveSpeed =4f;
   [SerializeField] private Transform environmentLocation;


   private Rigidbody rb;

    public override void Initialize()
    {
        rb=GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(Random.Range(-29f,-19f), -2.327469f, Random.Range(-29f,-19f));
        CreatePotion();
        
        
    }

    private void CreatePotion()
    {

        if(spawnedPotionList.Count !=0)
        {
            RemovePotion(spawnedPotionList);
        }
        for (int i=0;i<potionCount;i++)
        {
            GameObject newPotion=Instantiate(food);
            newPotion.transform.parent=environmentLocation;
            Vector3 potionLocation =new Vector3(Random.Range(-29f,-19f), -2.327469f, Random.Range(-29f,-19f));
            newPotion.transform.localPosition=potionLocation;
            spawnedPotionList.Add(newPotion);

        }
    }

    private void RemovePotion(List<GameObject> DeletedGameObjectList)
    {
        foreach (GameObject item in DeletedGameObjectList)
        {
            Destroy(item.gameObject);
        }
        DeletedGameObjectList.Clear();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
       sensor.AddObservation(transform.localPosition);
       //sensor.AddObservation(hedef.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveRotate = actions.ContinuousActions[0];
        float moveForward = actions.ContinuousActions[1];

        rb.MovePosition(transform.position + transform.forward * moveForward * moveSpeed * Time.deltaTime);
        transform.Rotate(0f, moveRotate * moveSpeed, 0f, Space.Self);

        /*
        Vector3 velocity=new Vector3(moveX,0f,moveZ) ;
        velocity = velocity.normalized * Time.deltaTime * moveSpeed;

        transform.localPosition +=velocity;
        */
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousAction=actionsOut.ContinuousActions;
        continuousAction[0]=Input.GetAxisRaw("Horizontal");
        continuousAction[1]=Input.GetAxisRaw("Vertical");
    }

    private void OnTriggerEnter(Collider nesne)
 { 
    if(nesne.gameObject.tag =="Potion")
    {
        spawnedPotionList.Remove(nesne.gameObject);
        Destroy(nesne.gameObject);
        AddReward(5f);
        if(spawnedPotionList.Count==0)
        {
            RemovePotion(spawnedPotionList);
            AddReward(5f);
            EndEpisode();
        }
        
    }
    if (nesne.gameObject.tag == "Wall")
    {
        
        AddReward(-5f);
        EndEpisode();
    }
 }    
}
