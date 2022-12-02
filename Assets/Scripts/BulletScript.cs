using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    CircleCollider2D cc2d;
    Rigidbody2D rb2d;
    SpriteRenderer sprite;

    float destroyTime;

    public int damage = 1;

    [SerializeField] float bulletSpeed;
    [SerializeField] Vector2 bulletDirection;
    [SerializeField] float destroyDelay;

    [SerializeField] string[] collideWithTags = { "Enemy" };

    public enum BulletTypes { Default, MiniBlue, MiniGreen, MiniOrange, MiniPink, MiniRed };
    [SerializeField] BulletTypes bulletType = BulletTypes.Default;

    [System.Serializable]
    public struct BulletStruct
    {
        public Sprite sprite;
        public float radius;
        public Vector3 scale;
    }
    [SerializeField] BulletStruct[] bulletData;


    // Start is called before the first frame update
    void Awake()
    {
        // get handles to components
        cc2d = GetComponent<CircleCollider2D>();
        rb2d = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();

        // set default bullet sprite & radius
        SetBulletType(bulletType);
    }

    // Update is called once per frame
    void Update()
    {
        destroyTime -= Time.deltaTime;
        if (destroyTime < 0)
        {
            Destroy(gameObject);
        }
    }

    public void SetBulletType(BulletTypes type)
    {
        // set sprite image & collider radius
        sprite.sprite = bulletData[(int)type].sprite;
        cc2d.radius = bulletData[(int)type].radius;
        transform.localScale = bulletData[(int)type].scale;
    }

    public void SetBulletSpeed(float speed)
    {
        this.bulletSpeed = speed;
    }

    public void SetBulletDirection(Vector2 direction)
    {
        this.bulletDirection = direction;
    }

    public void SetDamageValue(int damage)
    {
        this.damage = damage;
    }

    public void SetDestroyDelay(float delay)
    {
        this.destroyDelay = delay;
    }

    public void SetCollideWithTags(params string[] tags)
    {
        // set game object tags bullet can collide with
        this.collideWithTags = tags;
    }

    public void Shoot()
    {
        sprite.flipX = (bulletDirection.x < 0);
        rb2d.velocity = bulletDirection * bulletSpeed;
        destroyTime = destroyDelay;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        foreach (string tag in collideWithTags)
        {
            // check for collision with this tag
            if (other.gameObject.CompareTag(tag))
            {
                Debug.Log(other.gameObject);
                switch (tag)
                {
                    case "Enemy":
                        // enemy controller will apply the damage player bullet can cause
                        EnemyController enemy = other.gameObject.GetComponent<EnemyController>();
                        if (enemy != null)
                        {
                            enemy.TakeDamage(this.damage);
                        }
                        break;
                    case "Player":
                        // player controller will apply the damage enemy bullet can cause
                        PlayerController player = other.gameObject.GetComponent<PlayerController>();
                        if (player != null)
                        {
                            player.HitSide(transform.position.x > player.transform.position.x);
                            player.TakeDamage(this.damage);
                        }
                        break;
                }
                // remove the bullet - just not immediately
                Destroy(gameObject, 0.01f);
            }
        }
    }
}
