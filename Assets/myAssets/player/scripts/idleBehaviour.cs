using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class idleBehaviour : StateMachineBehaviour
{
    [SerializeField]
    private float timeUntilNextState;

    [SerializeField]
    private int numAnimations;
    private float idleTime;
    private bool isBored;
    private int randomAnimation;


    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){
        resetIdleAnimation();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){
        if (!isBored)
        {
            idleTime += Time.deltaTime;
            if (idleTime >= timeUntilNextState && stateInfo.normalizedTime % 1 < 0.02f){
                isBored = true;
                randomAnimation = Random.Range(1, numAnimations+1);
                randomAnimation = randomAnimation *2 -1;
                animator.SetFloat("idle", randomAnimation-1);
            }
        }
        else if (stateInfo.normalizedTime % 1 >= 0.98f)
        {
            resetIdleAnimation();
        }
        animator.SetFloat("idle", randomAnimation, 0.2f, Time.deltaTime);
    }

    private void resetIdleAnimation(){
        if(isBored && randomAnimation > 0) randomAnimation --; 
        isBored = false;
        idleTime = 0;
    }
}
