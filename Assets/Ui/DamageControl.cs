using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageControl : MonoBehaviour
{
    [SerializeField] private float damage = 10f;

    [SerializeField] private HealthControler healthControler;

    //private void OnParticleTrigger(Collider collision)
    //{
        
    //}
    private void OnTriggerEnter(Collider collision)
    {
        if (gameObject.CompareTag("Player"))
        {
            healthControler.takeDamage(damage);
        }
    }
}
