using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveController : MonoBehaviour
{
    [System.Serializable]
    public class Anim {
        public AnimationClip animationClip;
        public string response;
    }

    #region 
    public List<Anim> anims = new List<Anim>();
    #endregion

    public List<string> textToAnimate = new List<string>();
    public string currentAnimation = "";

    private Animator animator;

    //Arreglo de posiciones
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(currentAnimation == "" && textToAnimate.Count > 0)
        {
            currentAnimation = textToAnimate[0];
            textToAnimate.RemoveAt(0);
            foreach(Anim anim in anims)
            {
                if(anim.response == currentAnimation)
                {
                    float duration = anim.animationClip.length;
                    animator.Play(anim.animationClip.name);
                    Invoke("resetCurrentAnimation", duration);
                    break;
                }
            }
        }
    }

    void resetCurrentAnimation()
    {
        currentAnimation = "";
    }
}
