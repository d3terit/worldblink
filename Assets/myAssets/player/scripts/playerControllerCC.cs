using UnityEngine;

public class playerControllerCC : MonoBehaviour
{
    private CharacterController controller;
    private Animator animator;
    public float playerVelocity = 5;
    public float factorRunVelocity = 3;
    public float gravity = 9.8f;
    public float velJump = 18;
    private float velGravity = 0;

    private bool corriendo = false;
    public bool pose = false;
    public Camera camara;

    private float horizontalM, verticalM;
    private Vector3 move;
    private Vector3 direction;
    private Vector3 camForward;
    private Vector3 camRight;
    private float velocidadRotacionSuave = 0.2f;
    private float rotacionSuave = 0.3f;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //estrada movimiento
        horizontalM = Input.GetAxis("Horizontal");
        verticalM = Input.GetAxis("Vertical");
        if(animator.GetBool("estaAtacando") || animator.GetBool("estaCubriendo") || pose){
            horizontalM = 0;
            verticalM = 0;
        }
        move = new Vector3(horizontalM, 0, verticalM);
        move = Vector3.ClampMagnitude(move, 1);

        //esta corriendo?
        corriendo = Input.GetKey(KeyCode.LeftShift);
        if (corriendo)
        {
            horizontalM = horizontalM * 2;
            verticalM = verticalM * 2;
        }

        animator.SetFloat("moveZ", verticalM);
        if (verticalM > 0.5 && horizontalM < -0 || verticalM < -0.5 && horizontalM > 0)
        {
            horizontalM = 0;
        }
        animator.SetFloat("moveX", horizontalM);
        playerSkills();
        if (move.magnitude >= 0.1f)
        {            
            //rotacion del personaje por la camara
            float anguloARotar = camara.transform.eulerAngles.y;
            float angulo = Mathf.SmoothDampAngle(transform.eulerAngles.y, anguloARotar, ref velocidadRotacionSuave, rotacionSuave);
            transform.rotation = Quaternion.Euler(0f, angulo, 0f);

            //movimiento del personaje
            Vector3 direccionDelMovimiento = Quaternion.Euler(0f, (Mathf.Atan2(move.x, move.z) * Mathf.Rad2Deg + anguloARotar), 0f) * Vector3.forward;
            var velocity = playerVelocity;
            if (corriendo)
            {
                velocity *= factorRunVelocity;
            }
            controller.Move(direccionDelMovimiento.normalized * velocity * Time.deltaTime);
        }
        setGravity();
        float rotationX = camara.transform.rotation.x;
        controller.Move(new Vector3(0, move.y * Time.deltaTime, 0));
        camara.transform.rotation = Quaternion.Euler(rotationX, camara.transform.rotation.y, camara.transform.rotation.z);

    }

    //habilidades del personaje
    public void playerSkills()
    {
        //Saltar
        if (controller.isGrounded && Input.GetButtonDown("Jump"))
        {
            animator.SetBool("estaSaltando", true);
        }
        //Atacar
        if (controller.isGrounded && Input.GetButton("Fire1"))
        {
            pose = true;
            animator.SetBool("estaAtacando", true);
            animator.SetBool("estaCubriendo", false);
        }
        //Cubrir
        if (controller.isGrounded && Input.GetButton("Fire2") && !animator.GetBool("estaCubriendo"))
        {
            pose = true;
            animator.SetBool("estaCubriendo", true);
            animator.SetBool("estaAtacando", false);
        }
        //Dejar de atacar
        if (Input.GetButtonUp("Fire1"))
        {
            animator.SetBool("estaAtacando", false);
        }
        //Dejar de cubrir
        if (Input.GetButtonUp("Fire2"))
        {
            animator.SetBool("estaCubriendo", false);
            posed();
        }
    }

    //salto del personaje
    public void Jump()
    {
        velGravity = velJump;
        if (corriendo && verticalM > 1)
        {
            velGravity *= 1.1f;
        }
        move.y = velGravity;
        controller.Move(new Vector3(0, move.y * Time.deltaTime, 0));
        animator.SetBool("estaSaltando", !animator.GetBool("estaSaltando"));
    }
    //activar movimiento
    public void posed(){
        pose = false;
    }

    //control de gravedad
    public void setGravity()
    {
        if (controller.isGrounded)
        {
            velGravity = -gravity;
        }
        else
        {
            velGravity -= gravity * Time.deltaTime;
        }
        move.y = velGravity;
    }
}