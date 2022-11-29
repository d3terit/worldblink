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

    public LayerMask lGroud, lPlayer;

    [SerializeField]
    public bool playerView = false;
    private float normVelocity;
    public float runVelocity = 2f;
    private float currentVelocity;
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
        agent.SetDestination(transform.position);
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
        Vector3 direction = player.position - transform.position;
        float angle = Vector3.Angle(direction, transform.forward);
        float distance = Vector3.Distance(player.position, transform.position);
        playerView = distance <= playerDetectRange || distance <= playerLargeViewRange && angle <= viewAngleRange;
        state = distance > playerAttackRange ? STATE.move : STATE.attack;
    }

    void setAnimations(){
        animator.SetFloat("speed", agent.velocity.magnitude);
    }

}
