using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class gameController : MonoBehaviour
{
    enum STATE {play, inventory, bestiary, config, end};
    STATE state = STATE.play;
    private bool isPaused = false;
    [Header("Game Objects to Pause")]
    public GameObject cameraObject;
    public GameObject inventoryMenu, inventoryButton;
    public GameObject bestiaryMenu, bestiaryButton;
    public GameObject configMenu, configButton;
    public Sprite buttonActive, buttonInactive;
    public GameObject optionsMenu;
    [Header("Sountracks")]
    public AudioClip mainSoundtrack;
    public Slider volumeSlider;
    [Header("Bestiary pages")]
    public GameObject[] bestiaryPages;
    public GameObject bestiaryNextPageButton, bestiaryPreviousPageButton;
    private int currentPage = 0;
    private int maxPages;
    void Start(){
        Cursor.lockState = CursorLockMode.Locked;
        GetComponent<AudioSource>().volume = volumeSlider.value;
        maxPages = bestiaryPages.Length;
        bestiaryPreviousPageButton.SetActive(false);
    }

    // Update is called once per frame
    void Update(){
        stateMachine();
        checkForStateChange();
        playSoundtrack();
    }

    void stateMachine(){
        switch(state){
            case STATE.play:
                playState();
                break;
            case STATE.inventory:
                inventoryState();
                break;
            case STATE.bestiary:
                bestiaryState();
                break;
            case STATE.config:
                configState();
                break;
            case STATE.end:
                break;
            default:
                break;
        }
    }

    void playState(){
        Cursor.lockState = CursorLockMode.Locked;
        if(isPaused){
            isPaused = false;
            Time.timeScale = 1;
            cameraObject.SetActive(true);
            optionsMenu.SetActive(false);
        }
    }

    void inventoryState(){
        if(!isPaused){
            isPaused = true;
            Time.timeScale = 0;
            cameraObject.SetActive(false);
            Cursor.lockState = CursorLockMode.None;
        }
    }

    void bestiaryState(){
        if(!isPaused){
            isPaused = true;
            Time.timeScale = 0;
            cameraObject.SetActive(false);
            Cursor.lockState = CursorLockMode.None;
        }
    }

    void configState(){
        if(!isPaused){
            isPaused = true;
            Time.timeScale = 0;
            cameraObject.SetActive(false);
            Cursor.lockState = CursorLockMode.None;
        }
    }

    void checkActiveButton(){
        switch(state){
            case STATE.play:
                inventoryButton.GetComponent<Image>().sprite = buttonInactive;
                inventoryMenu.SetActive(false);
                bestiaryButton.GetComponent<Image>().sprite = buttonInactive;
                bestiaryMenu.SetActive(false);
                configButton.GetComponent<Image>().sprite = buttonInactive;
                configMenu.SetActive(false);
                break;
            case STATE.inventory:
                inventoryButton.GetComponent<Image>().sprite = buttonActive;
                inventoryMenu.SetActive(true);
                bestiaryButton.GetComponent<Image>().sprite = buttonInactive;
                bestiaryMenu.SetActive(false);
                configButton.GetComponent<Image>().sprite = buttonInactive;
                configMenu.SetActive(false);
                break;
            case STATE.bestiary:
                inventoryButton.GetComponent<Image>().sprite = buttonInactive;
                inventoryMenu.SetActive(false);
                bestiaryButton.GetComponent<Image>().sprite = buttonActive;
                bestiaryMenu.SetActive(true);
                configButton.GetComponent<Image>().sprite = buttonInactive;
                configMenu.SetActive(false);
                break;
            case STATE.config:
                inventoryButton.GetComponent<Image>().sprite = buttonInactive;
                inventoryMenu.SetActive(false);
                bestiaryButton.GetComponent<Image>().sprite = buttonInactive;
                bestiaryMenu.SetActive(false);
                configButton.GetComponent<Image>().sprite = buttonActive;
                configMenu.SetActive(true);
                break;
            case STATE.end:
                break;
            default:
                break;
        }
    }

    public void resumeGame(){
        state = STATE.play;
    }

    public void navigateToInventory(){
        state = STATE.inventory;
        checkActiveButton();
    }

    public void navigateToBestiary(){
        state = STATE.bestiary;
        checkActiveButton();
    }

    public void navigateToConfig(){
        state = STATE.config;
        checkActiveButton();
    }

    void checkForStateChange(){
        bool pause = Input.GetKeyDown(KeyCode.Escape);
        bool inventory = Input.GetKeyDown(KeyCode.I);
        bool bestiary = Input.GetKeyDown(KeyCode.B);
        bool config = Input.GetKeyDown(KeyCode.P);
        switch(state){
            case STATE.play:
                if(pause || inventory) state = STATE.inventory;
                else if(bestiary) state = STATE.bestiary;
                else if(config) state = STATE.config;
                if(state != STATE.play){
                    optionsMenu.SetActive(true);
                    checkActiveButton();
                }
                break;
            case STATE.inventory:
                if(pause || inventory) state = STATE.play;
                else if(bestiary) navigateToBestiary();
                else if(config) navigateToConfig();
                break;
            case STATE.bestiary:
                if(pause || bestiary) state = STATE.play;
                else if(inventory) navigateToInventory();
                else if(config) navigateToConfig();
                break;
            case STATE.config:
                if(pause || config) state = STATE.play;
                else if(inventory) navigateToInventory();
                else if(bestiary) navigateToBestiary();
                break;
            case STATE.end:
                break;
            default:
                break;
        }
    }

    void playSoundtrack(){
        if(!GetComponent<AudioSource>().isPlaying){
            GetComponent<AudioSource>().clip = mainSoundtrack;
            GetComponent<AudioSource>().Play();
            GetComponent<AudioSource>().loop = true;
        }
    }

    public void changeMusicVolume(){
        GetComponent<AudioSource>().volume = volumeSlider.value;
    }

    public void nextPage(){
        if(currentPage < maxPages - 1){
            currentPage++;
            bestiaryPages[currentPage - 1].SetActive(false);
            bestiaryPages[currentPage].SetActive(true);
            if(currentPage == maxPages - 1){
                bestiaryNextPageButton.SetActive(false);
            }
            bestiaryPreviousPageButton.SetActive(true);
        }
    }

    public void previousPage(){
        if(currentPage > 0){
            currentPage--;
            bestiaryPages[currentPage + 1].SetActive(false);
            bestiaryPages[currentPage].SetActive(true);
            if(currentPage == 0){
                bestiaryPreviousPageButton.SetActive(false);
            }
            bestiaryNextPageButton.SetActive(true);
        }
    }
}
