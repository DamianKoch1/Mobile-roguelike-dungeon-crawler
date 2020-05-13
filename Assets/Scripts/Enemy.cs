using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable, ILockOnTarget
{
    [SerializeField]
    private SpriteRenderer lockOnIndicator;

    [SerializeField]
    private int maxHp;

    private int hp;

    private void Start()
    {
        hp = maxHp;
    }

    public void OnDeath()
    {
        Destroy(gameObject);
    }


    public void OnLockedOn(Player player)
    {
        if (!lockOnIndicator) return;
        lockOnIndicator.gameObject.SetActive(true);
    }

    public void OnLockedOff(Player player)
    {
        if (!lockOnIndicator) return;
        lockOnIndicator.gameObject.SetActive(false);
    }

    public void TakeDamage(int amount)
    {
        hp = Mathf.Max(0, hp - amount);
        if (hp == 0)
        {
            OnDeath();
        }
    }
}
