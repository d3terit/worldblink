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

    [Header("sliders")]
    public Slider healthSlider;
    public Slider staminaSlider;
    public Slider manaSlider;
    public GameObject uiGeneral;
    void Start() {
        healthSlider.maxValue = maxHealth;
        healthSlider.value = health;
        staminaSlider.maxValue = maxStamina;
        staminaSlider.value = stamina;
        manaSlider.maxValue = maxMana;
        manaSlider.value = mana;
    }

    void Update() {
        setSliderValues();
        if(health <=0) StartCoroutine("die");
    }

    void setSliderValues(){
        healthSlider.value = health;
        healthSlider.GetComponentInChildren<TextMeshProUGUI>().text = health.ToString() + " / " + maxHealth.ToString();
        staminaSlider.value = stamina;
        staminaSlider.GetComponentInChildren<TextMeshProUGUI>().text = stamina.ToString() + " / " + maxStamina.ToString();
        manaSlider.value = mana;
        manaSlider.GetComponentInChildren<TextMeshProUGUI>().text = mana.ToString() + " / " + maxMana.ToString();
    }

    IEnumerator die(){
        yield return new WaitForSeconds(1f);
        uiGeneral.SetActive(false);
    }
    
}
