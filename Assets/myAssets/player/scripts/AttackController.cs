using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackController : MonoBehaviour
{
    public int damage = 10;
    public float attackRange = 1.5f;

    void OnTriggerEnter(Collider other){
        if (other.gameObject.tag == "Gobling" || other.gameObject.tag == "Warrok"){
            other.gameObject.GetComponent<generalEnemyController>().takeDamage(damage);
        }
    }

}
