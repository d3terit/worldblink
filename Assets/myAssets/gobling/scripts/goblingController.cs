using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class goblingController : MonoBehaviour
{
    public enum STATE { move, attack, pose, dead }
    public STATE state = STATE.move;
    public enum ATTACKSTATE { none, attackNear, attackFar }
    public ATTACKSTATE attackState = ATTACKSTATE.none;
    private Transform player;
    private Vector3 walkPoint;
    [SerializeField]
    public float walkPointRange;

    private Animator animator;
    private NavMeshAgent agent;

    public float playerLargeViewRange, playerDetectRange;
    public float minLargeAttackRange, maxLargeAttackRange, rangeShortAttack;
    public float intervalFarAttack, intervalNearAttack;
    public float viewAngleRange;
    public float maxIdleTime = 20;
    private bool moving = false;
    [SerializeField]
    private float idleTime = 0;
    [SerializeField]
    private float attackTime = 0;

    [SerializeField]
    public bool playerView = false;
    private float normVelocity;
    public float runVelocity = 2f;
    private float currentVelocity;
    private float distanceToPlayer;
    private bool canMove = true;
    //crear una variable para rotar el gobling en direccion al player
    private Vector3 directionToPlayer;
    public float angleToPlayer;
    public GameObject arrowPrefab;
    public Transform arrowSpawn;
    public float arrowSpeed = 10f;
    public float arrowDamage = 10f;
    public float arrowLifeTime = 5f;
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
        else{
            agent.SetDestination(player.position);
            currentVelocity = runVelocity;
            agent.speed = currentVelocity;
        }
    }

    void attack(){
        if(distanceToPlayer >= minLargeAttackRange && distanceToPlayer <= maxLargeAttackRange){
            attackState = ATTACKSTATE.attackFar;
            agent.SetDestination(transform.position);
            executeAttack();
        }
        else if(distanceToPlayer <= rangeShortAttack){
            attackState = ATTACKSTATE.attackNear;
            agent.SetDestination(transform.position);
            executeAttack();
        }
        else{
            attackState = ATTACKSTATE.none;
            if(canMove) agent.SetDestination(player.position);
        }
    }

    void executeAttack(){
        bool attackFar = attackTime >= intervalFarAttack;
        bool attackNear = attackTime >= intervalNearAttack;
        if(attackFar && attackState == ATTACKSTATE.attackFar){
            attackTime = 0;
            animator.SetBool("attackFar", true);
            animator.SetBool("attackNear", false);
        }
        else if(attackNear && attackState == ATTACKSTATE.attackNear){
            attackTime = 0;
            animator.SetBool("attackNear", true);
            animator.SetBool("attackFar", false);
        }
        else{
            animator.SetBool("attackNear", false);
            animator.SetBool("attackFar", false);
            attackTime += Time.deltaTime;
        }
        checkRotation();
    }

    void checkRotation(){
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(directionToPlayer), 0.5f);
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
        angleToPlayer = Vector3.Angle(directionToPlayer, transform.forward);
        distanceToPlayer = Vector3.Distance(player.position, transform.position);
        playerView = distanceToPlayer <= playerDetectRange || distanceToPlayer <= playerLargeViewRange && angleToPlayer <= viewAngleRange;
        switch (state){
            case STATE.move:
                if((distanceToPlayer <= maxLargeAttackRange-2f && playerView) || distanceToPlayer <= playerDetectRange) state = STATE.attack;
                break;
            case STATE.attack:
                if(distanceToPlayer > maxLargeAttackRange + 0.5f){
                    state = STATE.move;
                    attackState = ATTACKSTATE.none;
                    animator.SetBool("attackFar", false);
                    animator.SetBool("attackNear", false);   
                }
                break;
            default:
                break;
        }
    }
    void shootArrow(){
        Vector3 arrowDirection = player.position - transform.position;
        arrowDirection.y += 115f;
        Vector3 pos = arrowSpawn.position + new Vector3(-0.5f,0.6f,1.18f);
        GameObject arrow = Instantiate(arrowPrefab, arrowSpawn.position, Quaternion.LookRotation(arrowDirection));
        arrow.GetComponent<arrowController>().arrowDamage = arrowDamage;
        arrow.GetComponent<arrowController>().arrowSpeed = arrowSpeed;
        arrow.GetComponent<arrowController>().arrowLifeTime = arrowLifeTime;
        arrow.GetComponent<arrowController>().destination = player.position;
    }

    void blockMove(){
        canMove = false;
    }

    void unblockMove(){
        canMove = true;
    }

    void setAnimations(){
        animator.SetFloat("speed", agent.velocity.magnitude);
        animator.SetBool("posFar", attackState == ATTACKSTATE.attackFar);
        animator.SetBool("posNear", attackState == ATTACKSTATE.attackNear);
    }

}
