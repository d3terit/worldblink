using UnityEngine;

public class playSoundWalk : MonoBehaviour
{
    public AudioSource pie;

    void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.tag);
        if(other.CompareTag("Terrain")){
            pie.Play();
        }
    }

}
