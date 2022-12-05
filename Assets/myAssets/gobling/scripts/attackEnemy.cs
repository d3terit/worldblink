using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class attackEnemy : MonoBehaviour
{   
    public int damage = 10;
    private void Start() {
        GetComponent<Collider>().enabled = false;
    }
    void OnTriggerEnter(Collider other) {
        if(other.gameObject.tag == "Shield"){
            GetComponent<Collider>().enabled = false;
            StartCoroutine("reactivateCollider");
        }
        else if(other.gameObject.tag == "Player"){
            other.gameObject.GetComponent<playerController>().takeDamage(damage);
        }
    }
    IEnumerator reactivateCollider(){
        yield return new WaitForSeconds(0.8f);
        GetComponent<Collider>().enabled = true;
    }
}
