using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class playerController : MonoBehaviour
{
    #region Estados
    public enum STATE { move, attack, roll, cover, pose,dead }
    public STATE state = STATE.move;
    #endregion
    #region Componentes
    private CharacterController controller;
    private Animator animator;
    #endregion
    #region Movimiento
    //variables de movimiento
    public float playerVelocity = 1.4f;
    public float factorRunVelocity = 3;
    public float gravity = 6;
    public float velJump = 2;
    private float velGravity = 0;
    private Vector3 move;
    private Vector3 direction;
    private Vector3 camForward;
    private Vector3 camRight;
    //variables rotacion jugador
    private float velocidadRotacionSuave = 0.2f;
    private float rotacionSuave = 0.3f;
    private float anguloARotar;
    private bool saltando = false;
    public bool canMove = true;
    #region inputMovimiento
    private bool run;
    private float horizontalInput;
    private float verticalInput;
    #endregion
    #endregion
    #region Control de cámara
    public Camera camara;
    #endregion
    #region Control de UI
    public Slider energyBar;
    public Slider lifeBar;
    public Slider manaBar;
    #endregion
    #region Estadisticas y consumo
    public float valEnergy = 100;
    public float valERoll = 40;
    public float valEJump = 20;
    public float valEHeal = 70;
    public float valERun = 0.2f;
    public float valResEnergy = 0.15f;
    public float valLifeMax = 750;
    public float valLife = 750;
    public float valResLife = 0.4f;
    public float valLHeal = 100;
    public float valManaMax = 150;
    public float valMana = 150;
    public float valResMana = 0.5f;
    public float valMHability1 = 100;
    #endregion
    #region particles
    [Header("Particles")]
    public ParticleSystem particleHeal;
    public ParticleSystem particleIncrement;
    #endregion
    void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }
    void Start()
    {
        StartCoroutine("resEnergy");
        StartCoroutine("resLife");
        StartCoroutine("resMana");
    }
    void Update()
    {
        playerStateMachine();
        setGravity();
        updateBarStats();
    }
    void playerStateMachine()
    {
        switch (state)
        {
            case STATE.move:
                if (controller.isGrounded) saltando = false;
                movePlayer();
                break;
            case STATE.attack:
                if (controller.isGrounded) saltando = false;
                movePlayer();
                attack();
                break;
            case STATE.cover:
                movePlayer();
                cover();
                break;
            case STATE.roll:
                movePlayer();
                roll();
                break;
            case STATE.dead:
                gameObject.layer = 0;
                break;
            default:
                break;
        }
        changeState();
    }
    void changeState()
    {
        switch (state)
        {
            case STATE.move:
                if (!saltando)
                {
                    if (Input.GetKey(KeyCode.Mouse0))
                    {
                        switchStateAnimation();
                        animator.SetBool("estaAtacando", true);
                        state = STATE.attack;
                    }
                    else if (Input.GetKey(KeyCode.Mouse1))
                    {
                        switchStateAnimation();
                        state = STATE.cover;
                    }
                    else if (Input.GetKey(KeyCode.Alpha1) && valMana >= valMHability1) hability1();
                    else if (Input.GetKey(KeyCode.Q) && valEnergy >= valEHeal) heal();
                    else if (Input.GetButtonDown("Jump") && valEnergy >= valEJump){
                        animator.SetBool("estaSaltando", true);
                    }
                }
                if (Input.GetKey(KeyCode.C) && valEnergy >= valERoll)
                {
                    animator.SetBool("estaRodando", true);
                    valEnergy -= valERoll;
                    switchStateAnimation();
                    state = STATE.roll;
                }
                break;
            case STATE.attack:
                if (Input.GetKey(KeyCode.Mouse1))
                {
                    animator.SetBool("estaAtacando", false);
                    state = STATE.cover;
                }
                else if (Input.GetKey(KeyCode.C) && valEnergy >= valERoll)
                {   
                    animator.SetBool("estaAtacando", false);
                    switchStateAnimation();
                    valEnergy -= valERoll;
                    animator.SetBool("estaRodando", true);
                    state = STATE.roll;
                }
                break;
            case STATE.cover:
                if (Input.GetKeyUp(KeyCode.Mouse1))
                {
                    animator.SetBool("estaCubriendo", false);
                    moveFree();
                }
                else if (Input.GetKey(KeyCode.C) && valEnergy >= valERoll)
                {
                    animator.SetBool("estaCubriendo", false);
                    switchStateAnimation();
                    valEnergy -= valERoll;
                    animator.SetBool("estaRodando", true);
                    state = STATE.roll;
                }
                break;
            default:
                break;
        }
    }
    void switchStateAnimation()
    {
        setAnimationMove();
        transform.rotation = Quaternion.Euler(0f, camara.transform.eulerAngles.y, 0f);
    }
    void movePlayer()
    {
        move = getInputMove();
        if (move.magnitude >= 0.1f && canMove)
        {
            move = calcDirectionMove();
            //esta corriendo?
            if (valEnergy >= valERun) run = Input.GetKey(KeyCode.LeftShift);
            else run = false;
            var velocity = playerVelocity;
            if (run)
            {
                valEnergy -= valERun;
                velocity *= factorRunVelocity;
            }
            move *= velocity;
            controller.Move(move * Time.deltaTime);
        }
        setAnimationMove();
    }
    Vector3 getInputMove()
    {
        switch (state)
        {
            case STATE.move:
                horizontalInput = Input.GetAxis("Horizontal");
                verticalInput = Input.GetAxis("Vertical");
                break;
            case STATE.cover:
                horizontalInput = Input.GetAxis("Horizontal");
                verticalInput = Input.GetAxis("Vertical");
                break;
            case STATE.roll:
                verticalInput = 1;
                break;
            case STATE.attack:
                if (valEnergy >= valEJump || saltando){
                    verticalInput = Input.GetAxis("Vertical");
                    if(verticalInput<0) verticalInput =0;
                }else verticalInput = 0;
                horizontalInput = 0;
                break;
            default:
                break;
        }
        Vector3 direccion = new Vector3(horizontalInput, 0, verticalInput);
        direccion = Vector3.ClampMagnitude(direccion, 1);
        return direccion;
    }
    Vector3 calcDirectionMove()
    {
        if(state != STATE.attack){
            //calculo rotación
            anguloARotar = camara.transform.eulerAngles.y;
        }
        //determinar la dirección
        Vector3 direccionDelMovimiento = Quaternion.Euler(0f, (Mathf.Atan2(move.x, move.z) * Mathf.Rad2Deg + anguloARotar), 0f) * Vector3.forward;
        //rotación del personaje por la camara
        if ((verticalInput >= 0.1 && horizontalInput <= -0.1) || (verticalInput <= -0.1 && horizontalInput >= 0.1))
        {
            anguloARotar -= 45;
        }
        float angulo = Mathf.SmoothDampAngle(transform.eulerAngles.y, anguloARotar, ref velocidadRotacionSuave, rotacionSuave);
        if(state !=STATE.attack) transform.rotation = Quaternion.Euler(0f, angulo, 0f);
        return direccionDelMovimiento.normalized;
    }
    void setAnimationMove()
    {
        if (run)
        {
            horizontalInput = horizontalInput * 2;
            verticalInput = verticalInput * 2;
        }
        animator.SetFloat("moveZ", verticalInput);
        if ((verticalInput > 0.1 && horizontalInput < -0.1) || (verticalInput < -0.1 && horizontalInput > 0.1))
        {
            horizontalInput = 0;
        }
        animator.SetFloat("moveX", horizontalInput);
    }
    //salto del personaje
    public void Jump()
    {
        saltando = true;
        valEnergy -= valEJump;
        velGravity = velJump;
        if (run && verticalInput > 1) velGravity *= 1.1f;
        move.y = velGravity;
        controller.Move(new Vector3(0, move.y * Time.deltaTime, 0));
        animator.SetBool("estaSaltando", false);
    }
    //control de gravedad
    public void setGravity()
    {
        if (controller.isGrounded) velGravity = -gravity;
        else velGravity -= gravity * Time.deltaTime;
        move.y = velGravity;
        float rotationX = camara.transform.rotation.x;
        controller.Move(new Vector3(0, move.y * Time.deltaTime, 0));
        camara.transform.rotation = Quaternion.Euler(rotationX, camara.transform.rotation.y, camara.transform.rotation.z);
    }
    void cover()
    {
        animator.SetBool("estaCubriendo", true);
    }
    void attack()
    {
        if (Input.GetKey(KeyCode.Mouse0)) animator.SetBool("estaAtacando", true);
        else animator.SetBool("estaAtacando", false);
    }
    void roll()
    {
        if (Input.GetKey(KeyCode.C) && valEnergy >= valERoll) animator.SetBool("estaRodando", true);
        else animator.SetBool("estaRodando", false);
    }
    void heal()
    {
        state = STATE.pose;
        animator.Play("heal");
        valEnergy -= valEHeal;
    }
    void healPlay(){
        verticalInput = horizontalInput = 0;
        setAnimationMove();
        particleIncrement.Play();
        particleHeal.Play();
        if(valLife + valLHeal >valLifeMax) valLife = valLifeMax;
        else valLife += valLHeal;
    }
    void hability1()
    {
        state = STATE.pose;
        animator.Play("hability1");
        valMana -= valMHability1;
    }
    void moveFree()
    {
        if (state == STATE.roll && animator.GetBool("estaRodando")){
            if(valEnergy <valERoll){
                state = STATE.move;
                animator.SetBool("estaRodando", false);
            }
            else valEnergy -= valERoll;
        }
        else state = STATE.move;
        canMove = true;
    }
    void blockMove(){
        canMove = !canMove;
    }
    void updateBarStats(){
        energyBar.value = valEnergy;
        lifeBar.value = valLife;
        manaBar.value = valMana;
    }
    IEnumerator resEnergy(){
        while (state != STATE.dead){
            if(valEnergy <= valERun){
                yield return new WaitForSeconds(3f);
                valEnergy += 10;
            }
            else yield return null;
            if(valEnergy < 100){
                valEnergy += valResEnergy;
            }
        }
    }
    IEnumerator resLife(){
        while (state != STATE.dead){
            if(valLife <= valLifeMax/30){
                yield return new WaitForSeconds(5f);
                if (state != STATE.dead) valLife += 100;
            }
            else yield return new WaitForSeconds(0.5f);
            if(valLife < valLifeMax*7/10){
                if(valLife + valResLife > valLifeMax*7/10) valLife = valLifeMax*7/10;
                else valLife += valResLife;
            }
            else if(valLife + valResLife/5 > valLifeMax) valLife = valLifeMax;
            else valLife += valResLife/5;
        }
    }
    IEnumerator resMana(){
        while (state != STATE.dead){
            if(valMana <= valManaMax/20){
                yield return new WaitForSeconds(10f);
                valMana += 20;
            }
            else yield return new WaitForSeconds(0.5f);
            if(valMana < valManaMax){
                if(valMana + valResMana >valManaMax) valMana = valManaMax;
                else valMana += valResMana;
            }
        }
    }
    IEnumerator changeStateTemp(){
        while (state != STATE.move)
        {
            yield return new WaitForSeconds(0.6f);
            AnimatorStateInfo aim = animator.GetCurrentAnimatorStateInfo(0);
            if(aim.IsName("movimiento")) state = STATE.move;
        }
    }
}