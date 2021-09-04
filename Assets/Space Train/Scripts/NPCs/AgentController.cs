using SpaceTrain.Player;


using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(Animator))]
public class AgentController : MonoBehaviour
{
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    private NavMeshAgent agent;
    private Animator animator;
    private bool hasArrived = true;
    public bool HasArrived => hasArrived;
    
    private void SetMovementAnimation(MovementState _movementState)
    {
        switch(_movementState)
        {
            case MovementState.Idle:
                animator.SetBool("Idle", true);
                animator.SetBool("Walking", false);
                animator.SetBool("Running", false);
                break;
            case MovementState.Walking:
                animator.SetBool("Idle", false);
                animator.SetBool("Walking", true);
                animator.SetBool("Running", false);
                break;
            case MovementState.Running:
                animator.SetBool("Idle", false);
                animator.SetBool("Walking", false);
                animator.SetBool("Running", true);
                break;
            case MovementState.Dead :
                break;
        }
    }
    
    public void StopMoving()
    { 
        agent.ResetPath();
        hasArrived = true;
    }
    
    public bool TryRunToPosition(Vector3 _position)
    {
        agent.speed = runSpeed;
        hasArrived = false;
        if(agent.SetDestination(_position))
        {
            SetMovementAnimation(MovementState.Running);
        }
        return agent.SetDestination(_position);
    }

    public bool TryWalkToPosition(Vector3 _position)
    {
        agent.speed = walkSpeed;
        hasArrived = false;
        return agent.SetDestination(_position);
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
        animator = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!agent.pathPending && agent.velocity.magnitude == 0 && agent.remainingDistance <= agent.stoppingDistance)// has the agent reached it's position
        {
            StopMoving();
        }
        if(agent.isStopped)
        {
            hasArrived = true;
        }
    }
}
