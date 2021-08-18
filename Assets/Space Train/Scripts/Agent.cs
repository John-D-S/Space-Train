using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Agent : MonoBehaviour
{
    private NavMeshAgent agent;
    private WayPoint[] waypoints;
    
    [SerializeField] 

    //will give us a random waypoint in the array as a variable every time i access it
    private WayPoint RandomPoint => waypoints[Random.Range(0, waypoints.Length)];

    // Start is called before the first frame update
    void Start()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();
        // FindObjectsOfType gets every instanc of this component in the scene.
        waypoints = FindObjectsOfType<WayPoint>();

        // Tell the agent to move ot a random position in the scen waypoints
        agent.SetDestination(RandomPoint.Position);
    }

    // Update is called once per frame
    void Update()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.25f)// has the agent reached it's position
        {
            // Tell the agent to move ot a random position in the scen waypoints
            agent.SetDestination(RandomPoint.Position);
        }
    }
}
