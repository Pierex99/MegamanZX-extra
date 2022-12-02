using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordScript : MonoBehaviour
{
    CircleCollider2D cc2d;

    public int damage = 1;
    // Start is called before the first frame update
    void Start()
    {

    }
    void Awake()
    {
        cc2d = GetComponent<CircleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetDamageValue(int damage)
    {
        this.damage = damage;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(other.gameObject);
        // check for collision with this tag
        if (other.gameObject.CompareTag("Enemy"))
        {
            // enemy controller will apply the damage player bullet can cause
            EnemyController enemy = other.gameObject.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.TakeDamage(this.damage);
            }
        }
        gameObject.SetActive(false);
    }
}
