using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class platerState : MonoBehaviour
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
    public bool roll = false;
    private bool cover = false;
    private bool impact = false;
    private bool crouched = false;
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
    public float velocidadRotacionSuave = 0.2f;
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
    private float velGravity = 0;
    #endregion
    
    
    // Start is called before the first frame update
    void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        playerStateMachine();
        setAnimations();
        setGravity();
    }

    void playerStateMachine(){
        isJumping = !controller.isGrounded;
        switch(state){
            case STATE.Free:
                movePlayer();
                break;
            case STATE.Attack:
                attackState();
                break;
            case STATE.Cover:
                coverState();
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
            controller.Move(move * velocity * Time.deltaTime);
        }
        else{
            moveX = 0;
            moveZ = 0;
        }
    }

    void attackState(){

    }

    void coverState(){

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

    }

    void bloquedState(){

    }
    
    void checkForStateChange(){
        bool inJump = Input.GetKey(KeyCode.Space);
        bool inRoll = Input.GetKey(KeyCode.C);
        bool inAttack = Input.GetMouseButton(0);
        bool inCover = Input.GetMouseButton(1);
        bool inCrouched = Input.GetKey(KeyCode.LeftControl);

        switch (state)
        {
            case STATE.Free:
                if(!isJumping && inJump){
                    jump = true;
                }else if (inRoll){
                    roll = true;
                    state = STATE.Roll;
                }
                else if(inCrouched){
                    crouched = true;          
                    canMove = false;
                    state = STATE.Crouched;
                }
                break;  
            case STATE.Crouched:
                crouched = inCrouched;
                if(inCrouched){
                    canMove = false;
                }
                attack = inAttack;
                break;  
            default:
                break;
        }
    }

    Vector3 getInputMove()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        float zDirection = verticalInput;
        if(state == STATE.Roll){
            zDirection = 1;
        }
        runInput = Input.GetKey(KeyCode.LeftShift);
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
        if (runInput && verticalInput > 1) velGravity *= 1.1f;
        move.y = velGravity;
        controller.Move(new Vector3(0, move.y * Time.deltaTime, 0));
        jump = false;
    }

    public void setGravity()
    {
        if (controller.isGrounded) velGravity = -gravity;
        else velGravity -= gravity * Time.deltaTime;
        move.y = velGravity;
        float rotationX = camara.transform.rotation.x;
        controller.Move(new Vector3(0, move.y * Time.deltaTime, 0));
        camara.transform.rotation = Quaternion.Euler(rotationX, camara.transform.rotation.y, camara.transform.rotation.z);
    }

    public void moveFree(){
        if(state == STATE.Roll){
            roll = false;
        }else if(state == STATE.Crouched){
            crouched = false;
        }
        canMove = true;
        state = STATE.Free;
    }

    void setAnimations()
    {
        animator.SetBool("Jump", jump);
        animator.SetBool("Attack", attack);
        animator.SetBool("Roll", roll);
        animator.SetBool("Cover", cover);
        animator.SetBool("Crouched", crouched);
        animator.SetBool("Impact", impact);
        animator.SetFloat("MoveX", moveX);
        animator.SetFloat("MoveZ", moveZ);
    }
}
