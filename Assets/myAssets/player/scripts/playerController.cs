using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
public class playerController : MonoBehaviour
{
    
    #region Control de cámara
    public Camera camara;
    #endregion
    #region Estados
    public enum STATE {Free, Attack, Cover, Roll, Crouched, Dead, Bloqued};
    public STATE state = STATE.Free;
    #endregion

    #region Direcion del movimiento
    public enum Direction {None= 0, Left, Right, Forward, Backward};
    private Vector3 move; 
    #endregion

    #region states to animation
    private bool jump = false;
    private bool attack = false;
    private bool roll = false;
    private bool cover = false;
    private bool impact = false;
    private bool crouched = false;
    private bool isMoving = false;
    private float moveX = 0;
    private float moveZ = 0;    
    #endregion

    private CharacterController controller;
    private Animator animator;

    #region Variables de movimiento
    public float walkSpeed = 1.4f;
    public float runSpeed   = 1.5f;
    public float jumpSpeed  = 1.5f;
    public float gravity = 6;
    #endregion

    #region Variables de rotacion
    private float velocidadRotacionSuave = 0.2f;
    private float rotacionSuave = 0.3f;
    #endregion

    #region inputs de movimiento
    private float horizontalInput;
    private float verticalInput;
    private bool runInput;
    #endregion

    #region movimiento
    private bool isJumping = false;
    private bool canMove = true;
    private bool noBack = false;
    private bool canRunningToAttack = true;
    private float velGravity = 0;
    #endregion

    #region particulas
    public ParticleSystem heal1;
    public ParticleSystem heal2;
    #endregion
    private statsController stats;

    private bool isDead = false;

    [Header("Ataque")]
    public GameObject sword;
    public GameObject shield;
    public GameObject kick;
    public int damageSword = 10;
    public int damageKick = 20;
    public int damageChrounced = 15;
    public float attackRange = 1.5f;
    private int currentAttackDamage;
    public TextMeshProUGUI textDamage;
    public int costCuration = 60;
    private bool canResetMana = true;
    public float intervalResetMana = 8f;
    public float timeToResetMana = 8f;
    public int costPower1 = 70;
    public int power1Damage = 80;
    public int costJumpAttack = 30;
    public int costRoll = 20;
    public bool canResetStamina = true;
    public float intervalResetStamina = 6f;
    public float timeToResetStamina = 6f;
    private bool canResetHealth = true;
    public float intervalResetHealth = 5f;
    public float timeToResetHealth = 5f;
    public int resetHealt = 50;
    public Slider volumeSlider;
    public AudioClip swordSound, deathSound;
    
    // Start is called before the first frame update
    void Awake(){
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        stats = GetComponent<statsController>();
    }

    void Start(){
        sword.GetComponent<Collider>().enabled = false;
        shield.GetComponent<Collider>().enabled = false;
        kick.GetComponent<Collider>().enabled = false;
        currentAttackDamage = damageSword;
        GetComponent<AudioSource>().volume = volumeSlider.value;
    }

    // Update is called once per frame
    void Update(){
        if(!isDead){
            playerStateMachine();
            setAnimations();
            setGravity();
            setAttackProperties();
            setStats();
            resetStats();
        }
    }

    void playerStateMachine(){
        isJumping = !controller.isGrounded;
        switch(state){
            case STATE.Free:
            case STATE.Cover:
            case STATE.Attack:
                movePlayer();
                break;
            case STATE.Roll:
                rollState();
                break;
            case STATE.Crouched:
                crouchedState();
                break;
            case STATE.Dead:
                deadState();
                break;
            case STATE.Bloqued:
                bloquedState();
                break;
            default:
                break;
        }
        checkForStateChange();
    }
    void movePlayer(){
        move = getInputMove();
        if(move.magnitude > 0 && !crouched){
            isMoving = true;
            if(canMove){
                move = calcDirectionMove();
                float velocity = 1;
                moveZ = verticalInput;
                moveX = horizontalInput;
                if(verticalInput != 0){
                    moveX = 0;
                }
                if(verticalInput > 0){
                    velocity = walkSpeed;
                }
                if(runInput){
                    velocity = runSpeed;
                    moveZ *= 2;
                    moveX *= 2;
                }
                if(isJumping) velocity *= 1.6f;
                controller.Move(move * velocity * Time.deltaTime);
            }
        }
        else{
            moveX = 0;
            moveZ = 0;
            isMoving = false;
        }
    }
    
    void rollState(){
        move = getInputMove();
        move = calcDirectionMove();
        float velocity = walkSpeed;
        if(runInput && verticalInput > 0){
            velocity = runSpeed;
        }
        controller.Move(move * velocity * 2 * Time.deltaTime);
    }

    void crouchedState(){
        if(canMove){
            state = STATE.Free;
        }
    }

    void deadState(){
        isDead = true;
        int random = Random.Range(1, 3);
        if(random == 1) animator.Play("death-1");
        else animator.Play("death-2");
        controller.enabled = false;
        GetComponent<AudioSource>().PlayOneShot(deathSound);
        StartCoroutine(restartLevel());
    }

    void bloquedState(){

    }
    
    void checkForStateChange(){
        bool inJump = Input.GetKey(KeyCode.Space);
        bool inRoll = Input.GetKey(KeyCode.C);
        bool inAttack = Input.GetMouseButtonDown(0);
        bool inCover = Input.GetMouseButton(1);
        bool inCrouched = Input.GetKey(KeyCode.LeftControl);
        bool interaction = Input.GetKeyDown(KeyCode.X);
        bool inRage = Input.GetKeyDown(KeyCode.Q);
        bool inPower1 = Input.GetKeyDown(KeyCode.Alpha1);
        switch (state)
        {
            case STATE.Free:
                if(!isJumping && inJump){
                    jump = true;
                }else if (inRoll && stats.stamina >= costRoll){
                    stats.stamina -= costRoll;
                    timeToResetStamina = intervalResetStamina;
                    canResetStamina = false;
                    StartCoroutine(resetStamina());
                    roll = true;
                    state = STATE.Roll;
                }
                else if(inCrouched){
                    crouched = true;          
                    canMove = false;
                    state = STATE.Crouched;
                }else if(inCover){
                    cover = true;
                    state = STATE.Cover;
                }else if(inAttack){
                    attack = true;
                    noBack = true;
                    state = STATE.Attack;
                }else if(interaction && !isJumping){
                    animator.Play("attack-m-1");
                    state = STATE.Bloqued;
                }else if(inRage && !isJumping && stats.mana >= costCuration){
                    stats.mana -= costCuration;
                    stats.health += resetHealt;
                    canResetMana = false;
                    timeToResetMana = intervalResetMana;
                    StartCoroutine(resetMana());
                    animator.Play("rage");
                    heal1.Play();
                    heal2.Play();
                    state = STATE.Bloqued;
                }else if(inPower1 && !isJumping && stats.mana >= costPower1){
                    stats.mana -= costPower1;
                    canResetMana = false;
                    timeToResetMana = intervalResetMana;
                    StartCoroutine(resetMana());
                    animator.Play("power-1");
                    currentAttackDamage = power1Damage;
                    state = STATE.Bloqued;
                }
                if(!inAttack){
                    attack = false;
                }
                break;  
            case STATE.Crouched:
                crouched = inCrouched;
                attack = inAttack;
                cover = inCover;
                break;  
            case STATE.Cover:
                cover = inCover;
                if(!cover){
                    state = STATE.Free;
                }
                break;
            case STATE.Attack:
                if(!attack) attack = inAttack;
                if(attack) noBack = true;
                break;
            default:
                break;
        }
    }

    Vector3 getInputMove()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        runInput = Input.GetKey(KeyCode.LeftShift);
        float zDirection = verticalInput;
        switch (state){
            case STATE.Attack:
                horizontalInput = 0;
                if(verticalInput < 0) zDirection = 0;
                break;
            case STATE.Roll:
                zDirection = 1;
                break;
            default:
                break;
        }
        if(noBack){
            horizontalInput = 0;
            if(canMove){
                zDirection = 1;
                runInput = canRunningToAttack;
            }
        }
        Vector3 direccion = new Vector3(horizontalInput, 0, zDirection);
        direccion = Vector3.ClampMagnitude(direccion, 1);
        return direccion;
    }

    Vector3 calcDirectionMove(){
        float anguloARotar = camara.transform.eulerAngles.y;
        Vector3 direccionDelMovimiento = Quaternion.Euler(0f, (Mathf.Atan2(move.x, move.z) * Mathf.Rad2Deg + anguloARotar), 0f) * Vector3.forward;
        anguloARotar += 45 * move.x * move.z;
        anguloARotar = Mathf.SmoothDampAngle(transform.eulerAngles.y, anguloARotar, ref velocidadRotacionSuave, rotacionSuave);
        controller.transform.rotation = Quaternion.Euler(0f, anguloARotar, 0f);
        return direccionDelMovimiento.normalized;
    }

    public void Jump()
    {
        isJumping = true;
        velGravity = jumpSpeed;
        if (runInput && verticalInput > 1) velGravity *= 4f;
        move.y = velGravity;
        controller.Move(new Vector3(0, move.y * Time.deltaTime, 0));
        jump = false;
        if(state == STATE.Attack){
            stats.stamina -= costJumpAttack;
            canResetStamina = false;
            timeToResetStamina = intervalResetStamina;
            StartCoroutine(resetStamina());
        }
    }

    public void setGravity()
    {
        if (controller.isGrounded) velGravity = -gravity;
        else velGravity -= gravity * Time.deltaTime;
        move.y = velGravity;
        controller.Move(new Vector3(0, move.y * Time.deltaTime, 0));
    }

    public void resetAttack(){
        attack = false;
    }

    public void blockMove(){
        canMove = false;
    }

    public void setRunnigToAttack(){
        canRunningToAttack = true;
    }

    public void moveFree(){
        roll = false;
        crouched = false;
        canMove = true;
        noBack = false;
        canRunningToAttack = false;
        state = STATE.Free;
        currentAttackDamage = damageSword;
    }

    public void takeDamage(int damage){
        if(state != STATE.Dead){
            stats.health -= damage;
            canResetHealth = false;
            timeToResetHealth = intervalResetHealth;
            StartCoroutine(resetHealth());
            if(stats.health <= 0f) state = STATE.Dead;
        }
    }

    public void setAttackProperties(){
        sword.GetComponent<AttackController>().damage = currentAttackDamage;
        sword.GetComponent<AttackController>().attackRange = attackRange;
    }

    public void enableSwordCollider(){
        sword.GetComponent<Collider>().enabled = true;
    }

    public void disableSwordCollider(){
        sword.GetComponent<Collider>().enabled = false;
    }

    public void enableKickCollider(){
        kick.GetComponent<Collider>().enabled = true;
        currentAttackDamage = damageKick;
    }

    public void disableKickCollider(){
        kick.GetComponent<Collider>().enabled = false;
        currentAttackDamage = damageSword;
    }

    void setAnimations()
    {
        animator.SetBool("Jump", jump);
        animator.SetBool("Attack", attack);
        animator.SetBool("Roll", roll);
        animator.SetBool("Cover", cover);
        animator.SetBool("Crouched", crouched);
        animator.SetBool("Impact", impact);
        animator.SetBool("Move", isMoving);
        animator.SetFloat("MoveX", moveX);
        animator.SetFloat("MoveZ", moveZ);
    }

    void setStats(){
        textDamage.text = damageSword.ToString();
    }

    public void upgradeLevel(){
        damageSword += 2;
        damageKick += 2;
        resetHealt += 2;
        setStats();
    }

    void resetStats(){
        if(canResetHealth && Time.timeScale != 0 && stats.health < stats.maxHealth*0.7) stats.health +=  .1f;
        if(canResetMana && Time.timeScale != 0 && stats.mana < stats.maxMana) stats.mana += .05f;
        if(canResetStamina && Time.timeScale != 0 && stats.stamina < stats.maxStamina) stats.stamina += 0.1f;
    }

    IEnumerator resetHealth(){
        yield return new WaitForSeconds(timeToResetHealth);
        canResetHealth = true;
    }

    IEnumerator resetMana(){
        yield return new WaitForSeconds(timeToResetMana);
        canResetMana = true;
    }

    IEnumerator resetStamina(){
        yield return new WaitForSeconds(timeToResetStamina);
        canResetStamina = true;
    }

    public void playSwordSound(){
        GetComponent<AudioSource>().PlayOneShot(swordSound);
    }
    
    public void changeMusicVolume(){
        GetComponent<AudioSource>().volume = volumeSlider.value;
    }

    IEnumerator restartLevel(){
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(0);
    }
}
