using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IDamageable
{
    private Rigidbody2D rb;

    private Weapon weapon;

    [SerializeField]
    private float maxHP;

    private float hp;

    [SerializeField]
    private float speed;

    [HideInInspector]
    public Team team;

    private void Start()
    {
        team = Team.player;
        rb = GetComponent<Rigidbody2D>();
        hp = maxHP;
        SetWeapon(GetComponentInChildren<Weapon>());
    }

    private void Update()
    {
        if (MobileInput.Action)
        {
            weapon?.TryFire();
        }
    }

    private void FixedUpdate()
    {
        if (MobileInput.Axes.magnitude == 0) return;
        rb.MovePosition(rb.position + MobileInput.Axes * speed * Time.fixedDeltaTime);
        weapon.transform.up = MobileInput.Axes;
    }

    public void SetWeapon(Weapon _weapon)
    {
        if (weapon)
        {
            Destroy(weapon.gameObject);
        }
        weapon = _weapon;
        weapon.transform.position = transform.position;
        weapon.transform.SetParent(transform);
        weapon.team = team;
    }

    public void TakeDamage(float amount)
    {
        hp = Mathf.Max(0, hp - amount);
        if (hp == 0)
        {
            OnDeath();
        }
    }

    public void OnDeath()
    {
    }
}

public enum Team
{
    player,
    enemy
}