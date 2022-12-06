using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class statsController : MonoBehaviour
{
    [Header("Player Stats")]
    public float health = 350;
    public float maxHealth = 350;
    public float armor = 0;
    public float maxArmor = 100;
    public float stamina = 100;
    public float maxStamina = 100;
    public float mana = 100;
    public float maxMana = 100;
    public int experience = 0;
    public int maxExperience = 100;
    public int level = 1;

    [Header("sliders")]
    public Slider healthSlider;
    public TextMeshProUGUI healthText;
    public Slider staminaSlider;
    public TextMeshProUGUI staminaText;
    public Slider manaSlider;
    public TextMeshProUGUI manaText;
    public GameObject uiGeneral;
    public TextMeshProUGUI levelText;
    public Slider experienceSlider;
    public TextMeshProUGUI experienceText;
    public TextMeshProUGUI levelText2;
    private playerController playerController;
    void Start() {
        healthSlider.maxValue = (int)(maxHealth);
        healthSlider.value = (int)(health);
        staminaSlider.maxValue = (int)(maxStamina);
        staminaSlider.value = (int)(stamina);
        manaSlider.maxValue = (int)(maxMana);
        manaSlider.value = (int)(mana);
        playerController = GetComponent<playerController>();
        experienceSlider.maxValue = maxExperience;
        experienceSlider.value = experience;
    }

    void Update() {
        setSliderValues();
        if(health <=0) StartCoroutine("die");
        setExperienceValues();
    }

    void setSliderValues(){
        healthSlider.value = (int)(health);
        healthText.text = ((int)(health)).ToString() + " / " + ((int)(maxHealth)).ToString();
        healthSlider.GetComponentInChildren<TextMeshProUGUI>().text = ((int)(health)).ToString() + " / " + ((int)(maxHealth)).ToString();
        staminaSlider.value = (int)(stamina);
        staminaText.text = ((int)(stamina)).ToString() + " / " + ((int)(maxStamina)).ToString();
        staminaSlider.GetComponentInChildren<TextMeshProUGUI>().text = ((int)(stamina)).ToString() + " / " + ((int)(maxStamina)).ToString();
        manaSlider.value = (int)(mana);
        manaText.text = ((int)(mana)).ToString() + " / " + ((int)(maxMana)).ToString();
        manaSlider.GetComponentInChildren<TextMeshProUGUI>().text = ((int)(mana)).ToString() + " / " + ((int)(maxMana)).ToString();
    }

    IEnumerator die(){
        yield return new WaitForSeconds(1f);
        uiGeneral.SetActive(false);
    }

    void setExperienceValues(){
        if(experience >= maxExperience){
            level++;
            maxExperience = (int)(maxExperience * 2.2f);
            maxHealth = (int)(maxHealth * 1.05f);
            playerController.upgradeLevel();
        }
        experienceSlider.maxValue = maxExperience;
        experienceSlider.value = experience;
        experienceText.text = experience.ToString() + " / " + maxExperience.ToString();
        levelText.text = level.ToString();
        levelText2.text = level.ToString();
    }
    
    public void addExperience(int amount){
        experience += amount;
    }
}
