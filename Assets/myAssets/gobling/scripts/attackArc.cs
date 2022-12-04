using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class attackArc : MonoBehaviour
{   
    public int damage = 10;
    private void Start() {
        //desactivar collider de arc
        GetComponent<Collider>().enabled = false;
    }
    void OnTriggerEnter(Collider other) {
        if(other.gameObject.tag == "Player"){
            other.gameObject.GetComponent<playerController>().takeDamage(damage);
        }
    }
}
