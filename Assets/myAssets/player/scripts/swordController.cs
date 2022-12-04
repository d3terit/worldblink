using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class swordController : MonoBehaviour
{
    public int damageSword = 10;
    public float attackRange = 1.5f;

    void Start(){

    }

    void Update(){

    }

    void OnTriggerEnter(Collider other){
        if (other.gameObject.tag == "Gobling" || other.gameObject.tag == "Enemy"){
            other.gameObject.GetComponent<generalEnemyController>().takeDamage(damageSword);
        }
    }

}
