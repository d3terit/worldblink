using UnityEngine;
using UnityEngine.SceneManagement;
public class combatController : MonoBehaviour
{
    public BoxCollider sword, feet;
    private playerController playerController;
    public float enterPunchVampireDamage = 30;
    private Animator animator;
    private AudioSource audioSource;
    public AudioClip swordAttack;
    public AudioClip deadSound;
    public Canvas stats;
    void Awake()
    {
        playerController = GetComponent<playerController>();    
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        audioSource.clip = swordAttack;
        disableColliders();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void disableColliders(){
        sword.enabled = feet.enabled = false;
    }
    public void eneableCollidersSword(){
        sword.enabled = true;
    }
    public void eneableCollidersFeet(){
        feet.enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (playerController.state != playerController.STATE.dead && playerController.valLife>0)
        {
            if (other.CompareTag("punchImpactV"))
            {   
                if(playerController.state == playerController.STATE.move ||playerController.state == playerController.STATE.cover){
                    animator.SetBool("impact", true);
                    Invoke("impactFree",0.4f);
                }
                playerController.valLife -= enterPunchVampireDamage;
            }
            /* else if (other.CompareTag("feetImpact"))
            {
                //reproducir animacion
                playerController.valLife -= enterDamageFeet;
            } */
            if (playerController.valLife <= 0)
            {
                playerController.state = playerController.STATE.dead;
                animator.Play("dead"+Random.Range(2,3),0);
                audioSource.clip = deadSound;
                stats.enabled = false;
                audioSource.Play();
                Invoke("restar",4);
            }
        }
    }
    private void restar(){
        SceneManager.LoadScene(1);
    }

    private void impactFree(){
        animator.SetBool("impact",false);
    }
    
    public void playAttackSwordSound(){
        audioSource.Play();
    }
}
