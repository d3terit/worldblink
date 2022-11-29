using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
public class iaEnemyV2 : MonoBehaviour
{
    #region Estados
    public enum STATE { move, attack, pose, dead }
    public STATE state = STATE.move;
    #endregion
    #region target
    public Transform player;
    public float vewRange, attackRange;
    #endregion
    #region movement
    public NavMeshAgent agent;
    public LayerMask lGroud, lPlayer;
    private Animator animator;
    //moveFree
    private Vector3 walkPoint;
    [SerializeField]
    private bool moving = false;
    public float walkPointRange;

    //Attack
    public float intervalAttack = 0;
    private bool attacking = false;
    //state
    public bool playerVew, playerAttack;
    public float tempStatic = 0;
    #endregion
    #region stats
    public float health = 100;
    public float enterDamageSword = 30;
    public float enterDamageFeet = 10;
    #endregion
    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }
    void Start()
    {  
        Debug.Log(player);
    }
    void Update()
    {
        // playerVew = Physics.CheckSphere(transform.position, vewRange, lPlayer);
        // playerAttack = Physics.CheckSphere(transform.position, attackRange, lPlayer);
        // if (player.gameObject.layer == 0) playerVew = playerAttack = false;
        enemyStateMachine();
        setAnimationMove();
    }
    void enemyStateMachine()
    {
        switch (state)
        {
            case STATE.move:
                if (!attacking) moveTo();
                break;
            case STATE.attack:
                attack();
                break;
            default:
                break;
        }
        if (state != STATE.dead) changeState();
        if (moving) correct();
        else tempStatic = 0;
    }
    void changeState()
    {
        if (playerAttack)
        {
            state = STATE.attack;
            attacking = true;
        }
        else
        {
            state = STATE.move;
            animator.SetBool("attack", false);
        }
    }
    private void moveTo()
    {
        if (!playerVew)
        {
            if (!moving) Invoke(nameof(destination), 8);
            else agent.SetDestination(walkPoint);
            Vector3 distanceToWalkPoint = transform.position - walkPoint;
            //Walkpoint reached
            if (distanceToWalkPoint.magnitude < 1f) moving = false;
        }
        else
        {
            agent.SetDestination(player.position);
        }
    }
    private void correct(){
        if (agent.velocity.magnitude ==0) tempStatic ++;
        if(tempStatic >=100) moving = false; 
    }
    private void destination()
    {
        //Calculate random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, lGroud)) moving = true;

        if (!moving) destination();
    }
    private void attack()
    {
        agent.SetDestination(transform.position);
        transform.LookAt(player);
        animator.SetBool("attack", attacking);
    }
    private void setAnimationMove()
    {
        if (state == STATE.attack) animator.SetFloat("move", 0);
        else animator.SetFloat("move", agent.velocity.magnitude);
    }
    public void moveFree()
    {
        if (!playerAttack) attacking = false;
    }
}