using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class iaEnemy : MonoBehaviour
{
    private int rutina;
    private CharacterController controller;
    private float cronometro;
    private Animator animator;

    public float gravity = 9.8f;
    private float velGravity = 0;
    private Vector3 move;

    private float rotacionSuave = 0.1f;
    private float velocidadRotacionSuave = 0.2f;

    public bool seguir = true;

    public GameObject target;
    public float loockDistance = 12;
    public Quaternion angle;
    public float grade;
    public float velocity = 1.8f;

    public float distancia;

    public int modo = 0;

    public int attack = 0;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        target = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        comportamiento();
    }

    public void comportamiento()
    {
        distancia = Vector3.Distance(transform.position, target.transform.position);
        if (distancia > loockDistance)
        {
            seguir = true;
            modo = 0;
            animator.SetBool("agresive", false);
            cronometro += 1 * Time.deltaTime;
            if (cronometro >= 5)
            {
                rutina = Random.Range(0, 3);
                cronometro = 0;
            }
            switch (rutina)
            {
                case 0:
                    animator.SetBool("walk", false);
                    break;
                case 1:
                    grade = Random.Range(0, 360);
                    angle = Quaternion.Euler(0, grade, 0);
                    rutina++;
                    break;
                case 2:
                    float angulo = Mathf.SmoothDampAngle(transform.eulerAngles.y, grade, ref velocidadRotacionSuave, rotacionSuave);
                    transform.rotation = Quaternion.Euler(0f, angulo, 0f);
                    Vector3 direccionDelMovimiento = Quaternion.Euler(0f, grade, 0f) * Vector3.forward;
                    controller.Move(direccionDelMovimiento.normalized * velocity * Time.deltaTime);
                    animator.SetBool("walk", true);
                    break;
                default:
                    animator.SetBool("walk", false);
                    break;
            }
        }
        else
        {
            if (distancia > 4)
            {
                if (modo == 0)
                {
                    animator.SetBool("agresive", true);
                    seguir = false;
                    modo = 1;
                }
                else if (seguir)
                {
                    animator.SetBool("agresive", false);
                }
                var lookPos = target.transform.position - transform.position;
                lookPos.y = 0;
                var rotation = Quaternion.LookRotation(lookPos);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, 3);
                grade = transform.eulerAngles.y;
                Vector3 direccionDelMovimiento = Quaternion.Euler(0f, grade, 0f) * Vector3.forward;
                if (seguir)
                {
                    controller.Move(direccionDelMovimiento.normalized * velocity * 1.2f * Time.deltaTime);
                }
                animator.SetBool("walk", true);
                animator.SetBool("attack", false);
            }
            else
            {
                attack = Random.Range(1, 60);
                animator.SetInteger("step", attack);
                if (modo <= 1)
                {
                    animator.SetBool("agresive", true);
                    seguir = false;
                    modo = 2;
                }
                else if (seguir)
                {
                    var lookPos = target.transform.position - transform.position;
                    lookPos.y = 0;
                    var rotation = Quaternion.LookRotation(lookPos);
                    grade = transform.eulerAngles.y - rotation.eulerAngles.y;
                    if(grade > 20 || grade <-20){
                        animator.SetBool("walk", true);
                        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, 10);                
                    }else{
                        animator.SetBool("walk", false);
                    }
                    animator.SetBool("agresive", false);
                    animator.SetBool("attack", true);
                }
            }
        }
        setGravity();
    }

    public void seguimiento()
    {
        seguir = true;
    }

    public void finalAttack()
    {
        animator.SetBool("attack", false);
    }

    //control de gravedad
    public void setGravity()
    {
        move = new Vector3(0, 0, 0);
        if (controller.isGrounded)
        {
            velGravity = -gravity;
        }
        else
        {
            velGravity -= gravity * Time.deltaTime;
        }
        move.y = velGravity;
        controller.Move(new Vector3(0, move.y * Time.deltaTime, 0));
    }

}
