using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour, IDamageable
{
    private Rigidbody2D rb;

    private Weapon weapon;

    [SerializeField]
    private int maxHP;

    private int hp;

    [SerializeField]
    private float speed;

    [HideInInspector]
    public Team team;

    private ILockOnTarget currTarget;

    private ILockOnTarget CurrTarget
    {
        set
        {
            if (currTarget != value)
            {
                currTarget?.OnLockedOff(this);
                value?.OnLockedOn(this);
            }
            currTarget = value;
        }
        get => currTarget;
    }
    private List<ILockOnTarget> lockOnTargets;

    private void Start()
    {
        lockOnTargets = new List<ILockOnTarget>();
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
        CurrTarget = GetClosestTarget();
        if (CurrTarget != null)
        {
            weapon.transform.up = ((MonoBehaviour)CurrTarget).transform.position - transform.position;
        }
        else
        {
            weapon.transform.up = MobileInput.Axes;
        }
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

    public void TakeDamage(int amount)
    {
        hp = Mathf.Max(0, hp - amount);
        if (hp == 0)
        {
            OnDeath();
        }
    }

    public void OnDeath()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.isTrigger) return;
        var lockOnTarget = collision.GetComponent<ILockOnTarget>();
        if (lockOnTarget == null) return;
        if (lockOnTargets.Contains(lockOnTarget)) return;
        lockOnTargets.Add(lockOnTarget);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.isTrigger) return;
        var lockOnTarget = collision.GetComponent<ILockOnTarget>();
        if (lockOnTarget == null) return;
        if (!lockOnTargets.Contains(lockOnTarget)) return;
        lockOnTargets.Remove(lockOnTarget);
    }

    private ILockOnTarget GetClosestTarget()
    {
        float bestDistance = 10000;
        ILockOnTarget bestTarget = null;
        foreach (var target in lockOnTargets)
        {
            var targetPos = ((MonoBehaviour)target).transform.position;
            var distance = Vector2.Distance(transform.position, targetPos);
            if (distance >= bestDistance) continue;
            if (Physics2D.Raycast(transform.position, targetPos - transform.position, 2, 10)) continue;
            bestDistance = distance;
            bestTarget = target;
        }
        return bestTarget;
    }
}

public enum Team
{
    player,
    enemy
}