using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    public static PlayerStatus instance;

    [Header("Main stats")]
    public int hp;
    public int bombAmount;
    public int powerAmount;
    private bool isInvincible = false;
    public int score = 0;
    public Action onDead;


    [Header("Private var's")]
    [SerializeField]
    private float bombRadius = 3;
    [SerializeField]
    private int bombDamage = 3;
    [SerializeField]
    private GameObject attackedEffect;
    [SerializeField]
    private GameObject bombEffect;

    private void Awake() {
        instance = this;
    }

    public void TakeHP() {
        if(isInvincible) {
            return;
        }

        StartCoroutine(InvincibleSet(3));

        hp--;
        powerAmount = powerAmount * 5 / 6;

        if(hp <= 0) {
            onDead?.Invoke();
        }
    }

    public void UseBomb() {
        if(bombAmount <= 0) {
            return;
        }

        Destroy(Instantiate(bombEffect, transform.position, new Quaternion()), 1);

        List<Collider2D> objects = Physics2D.OverlapCircleAll(transform.position, bombRadius).ToList<Collider2D>();
        foreach(var obj in objects) {
            if(obj.CompareTag("Bullet")) {
                Destroy(obj.gameObject);
            }
            if(obj.CompareTag("Enemy"))
            {
                obj.GetComponent<EnemyStatus>().TakeHP(bombDamage);
            }
        }
 
        bombAmount--;
    }

    private IEnumerator InvincibleSet(float time) {
        isInvincible = true;
        yield return new WaitForSeconds(time);
        isInvincible = false;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Bullet")) {
            Debug.Log("Attacked!");
            Destroy(Instantiate(attackedEffect, other.transform.position, new Quaternion()), 1);
            Destroy(other.gameObject);
            TakeHP();
        }    
    }
}
