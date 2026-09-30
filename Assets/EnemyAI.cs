using UnityEngine;
using UnityEngine.AI;
public enum EnemyState
{
    Patrol,
    chase,
    attack

}
public class EnemyAI : MonoBehaviour
{


    public Transform[] patrolPoints;
    public Transform player;
    public float detectionRange = 10f;
    public float attackRange = 2f;
    private NavMeshAgent agent;
    private int currentPoint;
    public float fieldofview=90f;
    public EnemyState currentState;
    public bool useLineOfSight = true;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        currentState = EnemyState.Patrol;
        GoToNextPoint();
    }
    void Update()
    {
      
        if (!agent.isOnNavMesh)
        {
           
            return;
        }

        if (CanSeePlayer())
        {
            float distanceFromPlayer = Vector3.Distance(
                transform.position,
                player.position
            );

            if (distanceFromPlayer <= attackRange)
            {
                currentState = EnemyState.attack;
            }
            else
            {
                currentState = EnemyState.chase;
            }
        }
        else
        {
            currentState = EnemyState.Patrol;
        }
        switch (currentState)
        {
            case EnemyState.Patrol:
                Patrol();
                break;
            case EnemyState.chase:
                Chase();
                break;
            case EnemyState.attack:
                Attack();
                break;
            
        }
    }
    void GoToNextPoint()
    {
        agent.SetDestination(patrolPoints[currentPoint].position);

        currentPoint++;

        if (currentPoint >= patrolPoints.Length)
        {
            currentPoint = 0;
        }
    }

    bool CanSeePlayer()
    {
       float distncefromPlayer=Vector3.Distance(transform.position,player.position);
       if(distncefromPlayer>detectionRange)
        {
            return false;
        }
       
        if(!useLineOfSight)
        {
            return true;
        }
        Vector3 directionToPlayer = player.position - transform.position;
        directionToPlayer.Normalize();
        RaycastHit hit;
        if (Physics.Raycast(transform.position, directionToPlayer, out hit, detectionRange))
        {
            if (hit.transform == player)
            {
                return true;
            }
        }

        return false;
   }
    void Patrol()
    {

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            GoToNextPoint();
        }
    }
    void Chase()
    {
        agent.isStopped = false;
        agent.SetDestination(player.position);
    }
    void Attack()
    {
        agent.isStopped = true;
    }
}
