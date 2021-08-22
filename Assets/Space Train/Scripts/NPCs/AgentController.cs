using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AgentController : MonoBehaviour
{
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    private NavMeshAgent agent;

    public void StopMoving()
    { 
        agent.ResetPath();
    }
    
    public void RunToPosition(Vector3 _position)
    {
        agent.speed = runSpeed;
        agent.SetDestination(_position);
    }

    public void WalkToPosition(Vector3 _position)
    {
        agent.speed = walkSpeed;
        agent.SetDestination(_position);
    }

    public void TurnToFace(Vector3 _position)
    {
        StartCoroutine(TurnTowards(_position));
    }

    private IEnumerator TurnTowards(Vector3 _position)
    {
        while(agent.isStopped)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.FromToRotation(transform.forward, _position), 10);
            yield return null;
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!agent.pathPending && agent.velocity.magnitude == 0 && agent.remainingDistance <= agent.stoppingDistance)// has the agent reached it's position
        {
            StopMoving();
        }
    }
}
