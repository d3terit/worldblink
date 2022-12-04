using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameController : MonoBehaviour
{
    enum STATE {play, pause, end};
    STATE state = STATE.play;
    private bool isPaused = false;
    [Header("Game Objects to Pause")]
    public GameObject cameraObject;
    public GameObject pauseMenu;
    void Start(){
        
    }

    // Update is called once per frame
    void Update(){
        stateMachine();
        checkForStateChange();
    }

    void stateMachine(){
        switch(state){
            case STATE.play:
                playState();
                break;
            case STATE.pause:
                pauseState();
                break;
            case STATE.end:
                break;
            default:
                break;
        }
    }

    void playState(){
        if(isPaused){
            isPaused = false;
            Time.timeScale = 1;
            cameraObject.SetActive(true);
            pauseMenu.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    void pauseState(){
        if(!isPaused){
            isPaused = true;
            Time.timeScale = 0;
            cameraObject.SetActive(false);
            pauseMenu.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void pauseSwitch(){
        if(state == STATE.play){
            state = STATE.pause;
        }else{
            state = STATE.play;
        }
    }

    void checkForStateChange(){
        bool pause = Input.GetKeyDown(KeyCode.Escape);
        switch(state){
            case STATE.play:
                if(pause) state = STATE.pause;
                break;
            case STATE.pause:
                if(pause) state = STATE.play;
                break;
            case STATE.end:
                break;
            default:
                break;
        }
    }
}
