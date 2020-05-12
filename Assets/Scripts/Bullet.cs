using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float damage;

    private float speed;

    private float range;

    private float lifespan;

    private float age;

    private Vector2 startPos;

    private Rigidbody2D rb;

    public void Initialize(Team ownerTeam, float _damage, float _speed, float _range = -1, float _lifespan = -1)
    {
        rb = GetComponent<Rigidbody2D>();
        startPos = rb.position;
        damage = _damage;
        speed = _speed;
        range = _range;
        lifespan = _lifespan;
        age = 0;
        switch (ownerTeam)
        {
            case Team.player:
                gameObject.layer = 8;
                break;
            case Team.enemy:
                gameObject.layer = 9;
                break;
        }
    }

    private void FixedUpdate()
    {
        if (!rb) return;
        rb.MovePosition(rb.position + (Vector2)transform.up * speed * Time.fixedDeltaTime);
        CheckDeathConditions();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.isTrigger) return;
        var damageable = collision.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(damage);
        }
        Destroy(gameObject);
    }

    private void CheckDeathConditions()
    {
        if (range > 0)
        {
            if (Vector2.Distance(startPos, rb.position) >= range)
            {
                Destroy(gameObject);
            }
        }
        if (lifespan > 0)
        {
            age += Time.fixedDeltaTime;
            if (age >= lifespan)
            {
                Destroy(gameObject);
            }
        }
    }
}
