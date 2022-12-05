using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class generalEnemyController : MonoBehaviour
{
    [Header("Enemy Stats")]
    public int maxHealth = 100;
    public int health = 100;
    public float timeToDie = 2f;

    public Slider healthBar;
    public float coldownDamage = .5f;
    public float timeToDamage = 0f;

    void Start(){
        healthBar.maxValue = maxHealth;
        healthBar.value = health;
        healthBar.GetComponentInChildren<TextMeshProUGUI>().text = health.ToString() + " / " + maxHealth.ToString();
    }

    void Update(){
        timeToDamage -= Time.deltaTime;
    }

    void killEnemy(){
        GetComponent<Collider>().enabled = false;
        if(GetComponent<goblingController>() != null){
            GetComponent<goblingController>().state = goblingController.STATE.dead;
            healthBar.gameObject.SetActive(false);
        }
        else if(GetComponent<warrokController>() != null){
            GetComponent<warrokController>().state = warrokController.STATE.dead;
            healthBar.gameObject.SetActive(false);
        }
        StartCoroutine("destroyEnemy");
    }

    public void takeDamage(int damage){
        if(timeToDamage <= 0){
            health -= damage;
            healthBar.value = health;
            if(health < 0) health = 0;
            healthBar.GetComponentInChildren<TextMeshProUGUI>().text = health.ToString() + " / " + maxHealth.ToString();
            timeToDamage = coldownDamage;
            if(health == 0) killEnemy();
        }
    }

    IEnumerator destroyEnemy(){
        yield return new WaitForSeconds(timeToDie);
        Destroy(gameObject);
    }
    
}
