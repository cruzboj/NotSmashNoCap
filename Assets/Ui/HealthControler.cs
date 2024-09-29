using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthControler : MonoBehaviour
{
    [Header("health parameters")]
    [SerializeField]private float maxHealth = 999.0f;
    private float currenthealth;
    [SerializeField] private float smoothDecreaseDuration = 0.5f;

    [Header("UI parameters")]
    [SerializeField] private TMP_Text healthText;

    [Header("Damage Color Parameters")]
    [SerializeField] private Color originalHealthColor;
    [SerializeField] private Color damageHealthColor;
    
    private void Start()
    {
        currenthealth = 0;
        UpdateHealthText();
    }

    public void takeDamage(float damage)
    {
        StartCoroutine(smoothDecreaseHealth(damage));
    }
    
    private IEnumerator smoothDecreaseHealth(float damage)
    {
        healthText.color = damageHealthColor;

        float damgePerTick = damage / smoothDecreaseDuration;
        float elapsedTime = 0f;

        while(elapsedTime < smoothDecreaseDuration)
        {
            float currentDamage = damgePerTick * Time.deltaTime;
            currenthealth += currentDamage;
            elapsedTime += Time.deltaTime;

            UpdateHealthText();
            if(currenthealth >= maxHealth)
            {
                currenthealth = maxHealth;
                //player death
                break;
            }
            yield return null;
        }

        healthText.color = originalHealthColor;
    }

    void UpdateHealthText()
    {
        healthText.text = currenthealth.ToString("0");
    }
}
