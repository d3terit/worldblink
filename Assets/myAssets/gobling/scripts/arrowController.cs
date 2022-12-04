using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class arrowController : MonoBehaviour
{
    private Rigidbody rb;
    public float arrowSpeed;
    public int arrowDamage;
    public Vector3 destination;
    public float arrowLifeTime, lifeTime;
    private GameObject collisionOther;
    private float randomHeight;
    private bool finish = false;
    void Start() {
        randomHeight = Random.Range(1f, 1.6f);
    }
    void Update() {
        lifeTime += Time.deltaTime;
        if(!finish){
            float distance = Vector3.Distance(destination + randomHeight * Vector3.up, transform.position);
            if(distance > 0.1f){
                transform.position = Vector3.MoveTowards(transform.position, destination + randomHeight * Vector3.up, arrowSpeed * Time.deltaTime);
            }
            else Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.tag != "Gobling"){
            if(other.gameObject.tag == "Player") other.gameObject.GetComponent<playerController>().takeDamage(arrowDamage);
            transform.parent = other.transform;
            transform.position = other.ClosestPoint(transform.position);
            finish = true;
            GetComponent<Collider>().enabled = false;
            Destroy(gameObject, arrowLifeTime);
        }
    }
}
