using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard : MonoBehaviour
{
    public float _knockbackStrength = 25; // KnockbackStrength
    public float _knockdownTime = 1; // Knockdown time in seconds
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        List<Collider2D> colliders = new List<Collider2D>();
        var filter = new ContactFilter2D();
        filter.useTriggers = true;
        filter.SetLayerMask(LayerMask.GetMask("PlayerHurtbox"));
        gameObject.GetComponent<Rigidbody2D>().OverlapCollider(filter, colliders);
        foreach (Collider2D trigger in colliders) {
            boi boi = trigger.gameObject.transform.parent.gameObject.GetComponent<boi>();
            if (boi != null) {
                Vector2 knockbackVector = trigger.gameObject.transform.position.x <   gameObject.transform.position.x ? Vector2.left : Vector2.right;
                boi.GetHit(knockbackVector * _knockbackStrength / 3 + Vector2.up * _knockbackStrength,
                    _knockdownTime);
            }
        }
    }
}
