using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard : MonoBehaviour
{
    public float _knockbackStrength = 25; // KnockbackStrength
    public float _knockdownTime = 1; // Knockdown time in seconds

    private void OnCollisionStay2D(Collision2D other)
    {
        boi boi = other.gameObject.GetComponentInParent<boi>();
        if (boi)
        {
            BonkThat(boi);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        boi boi = other.gameObject.GetComponentInParent<boi>();
        if (boi)
        {
            BonkThat(boi);
        }
    }

    void BonkThat(boi boi)
    {
        Vector2 knockbackVector = boi.transform.position.x < transform.position.x ? Vector2.left : Vector2.right;
        boi.GetHit(knockbackVector * _knockbackStrength / 3 + Vector2.up * _knockbackStrength,
            _knockdownTime);
    }
}
