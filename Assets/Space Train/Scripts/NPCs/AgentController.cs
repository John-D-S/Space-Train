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
        StartCoroutine(TurnTowards(_position, -1));
    }
    public void TurnToFace(Vector3 _position, float _timeToKeepTurning)
    {
        StartCoroutine(TurnTowards(_position, _timeToKeepTurning));
    }
    
    /// <summary>
    /// Will cause the agent to turn towards a point for as long as they are still for timeToKeepTurning
    /// </summary>
    /// <param name="_position">The point to turn towards.</param>
    /// <param name="_timeToKeepTurning">How long to keep turning towards that point. Will be forever if value is negative</param>
    private IEnumerator TurnTowards(Vector3 _position, float _timeToKeepTurning)
    {
        float remainingTime = _timeToKeepTurning;
        bool timeLeftToTurn = true;
        while(agent.isStopped && timeLeftToTurn)
        {
            if(_timeToKeepTurning > 0)
            {
                remainingTime -= Time.fixedDeltaTime;
                if(remainingTime <= 0)
                    timeLeftToTurn = false;
            }
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.FromToRotation(transform.forward, _position), 360 * Time.fixedDeltaTime);
            yield return new WaitForFixedUpdate();
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
