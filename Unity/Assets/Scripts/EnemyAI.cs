using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    private enum State
    {
        Idle,
        Tracking,
        Attacking
    }

    [SerializeField] private float detectionRadius = 10.0f;
    [SerializeField] private float attackRange = 2.0f;
    [SerializeField] private float idleWanderRadius = 5.0f;
    [SerializeField] private float idleWanderDelay = 3.0f;
    [SerializeField] private float idleSpeeed = 2f;
    [SerializeField] private float trackingSpeed = 5f;

    private State currentState = State.Idle;
    private Transform player;
    private NavMeshAgent navMeshAgent;
    private Animator animator;

    private float idleTimer = 0f;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponentInChildren<Animator>();
        
        navMeshAgent.speed = idleSpeeed;
    }

    private void Update()
    {
        switch (currentState)
        {
            case State.Idle:
                HandleIdleState();
                break;

            case State.Tracking:
                HandleTrackingState();
                break;

            case State.Attacking:
                HandleAttackingState();
                break;
        }

        CheckForPlayer();
        HandleAnimations();
    }

    private void HandleIdleState()
    {
        idleTimer += Time.deltaTime;

        navMeshAgent.speed = idleSpeeed;

        if (idleTimer >= idleWanderDelay)
        {
            // Pick a random point within the idle wander radius
            Vector3 randomDirection = Random.insideUnitSphere * idleWanderRadius;
            randomDirection += transform.position;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDirection, out hit, idleWanderRadius, NavMesh.AllAreas))
            {
                navMeshAgent.SetDestination(hit.position);
            }

            idleTimer = 0f; // Reset the timer
        }

        navMeshAgent.isStopped = false; // Ensure the agent is moving
    }

    private void HandleTrackingState()
    {
        if (player == null) return;
        
        navMeshAgent.speed = trackingSpeed;

        // Move towards the player
        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(player.position);

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            navMeshAgent.isStopped = true;
            ChangeState(State.Attacking);
        }
    }

    private void HandleAttackingState()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        navMeshAgent.isStopped = true;

        if (distanceToPlayer > attackRange)
        {
            ChangeState(State.Tracking);
        }
    }

    private void CheckForPlayer()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        
        if (currentState == State.Idle && distanceToPlayer <= detectionRadius)
        {
            ChangeState(State.Tracking);
        }
        else if (currentState == State.Tracking && distanceToPlayer > detectionRadius)
        {
            ChangeState(State.Idle);
        }
    }

    private void ChangeState(State newState)
    {
        currentState = newState;

        if (newState == State.Idle)
        {
            idleTimer = 0f; // Reset idle timer on entering Idle state
        }
    }
    
    private string currentAnimation;

    private void HandleAnimations()
    {
        string targetAnimation = null;

        //animator.SetLayerWeight(1, currentState == State.Idle ? 1f : 0f);
        
        if (currentState == State.Idle)
        {
            if (navMeshAgent.velocity.magnitude > 0.1f)
            {
                // Set target animation to "walk" if moving
                targetAnimation = "Walk";
            }
            else
            {
                // Set target animation to "idle" if stationary
                targetAnimation = "Idle";
            }
        }
        else if (currentState == State.Tracking)
        {
            // Set target animation to "run" when tracking
            targetAnimation = "Run";
        }
        else if (currentState == State.Attacking)
        {
            // Set target animation to "attack" when attacking
            targetAnimation = "Attack";
        }

        // Crossfade to the target animation only if it differs from the current animation
        if (targetAnimation != null && targetAnimation != currentAnimation)
        {
            animator.CrossFade(targetAnimation, 0.2f);
            currentAnimation = targetAnimation;
        }
    }

}
