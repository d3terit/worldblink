using UnityEngine;
using UnityEngine.UI;

public class uiController : MonoBehaviour
{
    public Image indicadorCentral;

    void Start()
    {
        
    }

    
    void Update()
    {
        inputs();
    }

    void inputs(){
        if (Input.GetKey(KeyCode.Mouse0))
        {
            indicadorCentral.color = Color.red;
            indicadorCentral.rectTransform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        }
        if (Input.GetKey(KeyCode.Mouse1))
        {
            indicadorCentral.color = Color.green;
            indicadorCentral.rectTransform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        }
        if (Input.GetKeyUp(KeyCode.Mouse0) || Input.GetKeyUp(KeyCode.Mouse1))
        {
            indicadorCentral.color = Color.white;
            indicadorCentral.rectTransform.localScale = new Vector3(0.15f, 0.15f, 0.15f);
        }
    }
}
