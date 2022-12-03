using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class enemyController : MonoBehaviour
{
    public enum STATE { move, attack, pose, dead }
    public STATE state = STATE.move;
    private Transform player;
    private Vector3 walkPoint;
    [SerializeField]
    public float walkPointRange;

    private Animator animator;
    private NavMeshAgent agent;

    public float playerLargeViewRange, playerDetectRange, playerAttackRange;
    public float viewAngleRange;
    public float maxIdleTime = 20;
    private bool moving = false;
    [SerializeField]
    private float idleTime = 0;
    [SerializeField]
    public bool playerView = false;
    private float normVelocity;
    public float runVelocity = 2f;
    private float currentVelocity;
    private Vector3 directionToPlayer;
    private bool canMove = true;
    private float attackType = -1;

    [Header("Estadisticas")]
    public float vida = 100;
    public float damage = 10;
    public float coldownAttack = 1;
    private float coldownAttackTime = 0;

    void Start(){
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        walkPoint = transform.position;
        normVelocity = agent.speed;
    }

    // Update is called once per frame
    void Update(){
        switch (state){
            case STATE.move:
                moveTo();
                break;
            case STATE.attack:
                attack();
                break;
            default:
                break;
        }
        changeStates();
        setAnimations();
    }

    void moveTo(){
        if (!playerView){
            if(!moving && idleTime >= maxIdleTime){
                walkPoint = getRandomPoint();
                moving = true;
                idleTime = 0;
            }
            if (moving){
                agent.SetDestination(walkPoint);
                if (Vector3.Distance(transform.position, walkPoint) <= 1f) moving = false;
                StartCoroutine(testMove());
            }
            else{
                idleTime += Time.deltaTime;
            }
            currentVelocity = normVelocity;
            agent.speed = currentVelocity;
        }
        else if(canMove){
            agent.SetDestination(player.position);
            currentVelocity = runVelocity;
            agent.speed = currentVelocity;
        }
    }

    void attack(){
        agent.SetDestination(transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(directionToPlayer), 0.5f);
        if (coldownAttackTime >= coldownAttack){
            coldownAttackTime = 0;
            if(attackType == -1) attackType = Random.Range(1, 3);
        }
        else{
            coldownAttackTime += Time.deltaTime;
        }
    }

    Vector3 getRandomPoint(){
        float x = Random.Range(-walkPointRange, walkPointRange);
        float z = Random.Range(-walkPointRange, walkPointRange);
        return new Vector3(transform.position.x + x, transform.position.y, transform.position.z + z);
    }

    IEnumerator testMove(){
        yield return new WaitForSeconds(3f);
        if(agent.velocity.magnitude <= 0.1f){
            moving = false;
        }
    }

    void changeStates(){
        directionToPlayer = player.position - transform.position;
        float angle = Vector3.Angle(directionToPlayer, transform.forward);
        float distance = Vector3.Distance(player.position, transform.position);
        playerView = distance <= playerDetectRange || distance <= playerLargeViewRange && angle <= viewAngleRange;
        switch (state){
            case STATE.move:
                if(distance <= playerAttackRange){
                    state = STATE.attack;
                    coldownAttackTime = coldownAttack-0.1f;
                }
                break;
            case STATE.attack:
                if(distance > playerAttackRange + 0.5f) state = STATE.move;
                break;
            default:
                break;
        }
    }

    void blockMove(){
        canMove = false;
    }

    void unblockMove(){
        canMove = true;
        attackType = -1;
        coldownAttackTime = 0;
    }

    void setAnimations(){
        animator.SetFloat("speed", agent.velocity.magnitude);
        animator.SetFloat("attack", attackType);
    }

}
