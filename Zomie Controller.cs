using UnityEngine;
using UnityEngine.AI;

public class ZomieController : MonoBehaviour
{
    Animator animator;
    NavMeshAgent agent;

    public float walkingSpeed;
    public GameObject attackHitBox;

    public float attackDistance;

    public int zomieHP = 2;

    public AudioSource zombieVoice;

    public AudioSource zombieDeadVoice;

    enum STATE { IDLE, WANDER, ATTACK, CHASE, DEAD };
    STATE state = STATE.IDLE;

    GameObject target;
    public float runSpeed;
    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        if(target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player");
        }

        zombieVoice.Play();
    }

    public void TurnOffTrigger()
    {
        animator.SetBool("Walk", false);
        animator.SetBool("Run", false);
        animator.SetBool("Death", false);
        animator.SetBool("Attack", false);
    }

    public void AttackStart()
    {
        attackHitBox.SetActive(true);
    }

    public void AttackEnd()
    {
        attackHitBox.SetActive(false);
    }

    float DistanceToPlayer()
    {
        return Vector3.Distance(target.transform.position, transform.position);



    }

    bool CanSeePlayer()
    {
        if (DistanceToPlayer() < 15)
        { return true; }

        return false;

    }

    bool ForGetPlayer()
    {
        if (DistanceToPlayer() > 20)
        { return true; }

        return false;        
                }
    
    void Update()
    {
        switch (state)
        {
            case STATE.IDLE:
                TurnOffTrigger();

                if(CanSeePlayer())
                {
                    state = STATE.CHASE;
                }

                else if (Random.Range(0, 5000) < 5)
                {
                    state = STATE.WANDER;
                }

                
                break;

            case STATE.WANDER:

                if(!agent.hasPath)
                {
                    float newX = transform.position.x + Random.Range(-5, 5);
                    float newZ = transform.position.z + Random.Range(-5, 5);

                    Vector3 NextPos = new Vector3(newX, transform.position.y, newZ);

                    agent.SetDestination(NextPos);
                    agent.stoppingDistance = 0;

                    TurnOffTrigger();

                    agent.speed = walkingSpeed;
                    animator.SetBool("Walk", true);

                }

                if(Random.Range(0, 5000) < 5)
                {
                    state = STATE.IDLE;
                    agent.ResetPath();
                }

                if (CanSeePlayer())
                {
                    state = STATE.CHASE;
                }

                break;

            case STATE.CHASE:
                agent.SetDestination(target.transform.position);
                agent.stoppingDistance = 3f;

                TurnOffTrigger();

                agent.speed = runSpeed;
                animator.SetBool("Run", true);

                if (DistanceToPlayer() <= agent.stoppingDistance)
                {
                    state = STATE.ATTACK;
                }



                if (ForGetPlayer())
                {
                    
                    agent.ResetPath();
                    state = STATE.WANDER;
                }

                break;

            case STATE ATTACK:
                agent.ResetPath();

                TurnOffTrigger();
                animator.SetBool("Attack", true);

                if (DistanceToPlayer() > attackDistance)
                    { 
                    state = STATE.CHASE;
                }

                /*Vector3 direction = target.transform.position - transform.position;
                direction.y = 0;
                if(direction != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    
                }*/
                
                break;

        }
            
                 
    }

    public void zombieTakeDamage(int damage)
    {   Debug.Log("ゾンビがダメージを受けた;");
        zomieHP -= damage;

        Debug.Log(zomieHP);
        if (zomieHP <= 0)
        {
            Debug.Log("死亡したよ");
            state = STATE.DEAD;
            agent.ResetPath();
            zombieVoice.Stop();
            zombieDeadVoice.Play();
            TurnOffTrigger();
            animator.SetBool("Death", true);
           
        }
    }
}
