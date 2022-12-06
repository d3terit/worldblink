using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class statsController : MonoBehaviour
{
    [Header("Player Stats")]
    public int health = 350;
    public int maxHealth = 350;
    public int armor = 0;
    public int maxArmor = 100;
    public int stamina = 100;
    public int maxStamina = 100;
    public int mana = 100;
    public int maxMana = 100;
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
        healthSlider.maxValue = maxHealth;
        healthSlider.value = health;
        staminaSlider.maxValue = maxStamina;
        staminaSlider.value = stamina;
        manaSlider.maxValue = maxMana;
        manaSlider.value = mana;
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
        healthSlider.value = health;
        healthText.text = health.ToString() + " / " + maxHealth.ToString();
        healthSlider.GetComponentInChildren<TextMeshProUGUI>().text = health.ToString() + " / " + maxHealth.ToString();
        staminaSlider.value = stamina;
        staminaText.text = stamina.ToString() + " / " + maxStamina.ToString();
        staminaSlider.GetComponentInChildren<TextMeshProUGUI>().text = stamina.ToString() + " / " + maxStamina.ToString();
        manaSlider.value = mana;
        manaText.text = mana.ToString() + " / " + maxMana.ToString();
        manaSlider.GetComponentInChildren<TextMeshProUGUI>().text = mana.ToString() + " / " + maxMana.ToString();
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
