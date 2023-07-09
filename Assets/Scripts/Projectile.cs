using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float lifespan = 5;
    public float force = 10;
    public float time = 0;
    public float blastRadius = 1;
    private Rigidbody2D _rigidbody;
    private SpriteRenderer _spriteRenderer;
    // Start is called before the first frame update
    void Start()
    {   
        _rigidbody = gameObject.GetComponent<Rigidbody2D>();
        Vector2 forceDirection = gameObject.transform.up;
            //new Vector2(Mathf.Cos(gameObject.transform.eulerAngles.z * Mathf.Deg2Rad),
            //Mathf.Sin(gameObject.transform.eulerAngles.z * Mathf.Deg2Rad));
        _rigidbody.AddForce(forceDirection * force, ForceMode2D.Impulse);
        _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if (time >= lifespan) {
            Destroy(gameObject.transform.parent.gameObject);
        }
        _spriteRenderer.transform.up = _rigidbody.velocity.normalized;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground") ||
            other.gameObject.layer == LayerMask.NameToLayer("PlayerHurtbox"))
        {
            foreach (ParticleSystem psystem in gameObject.GetComponentsInChildren<ParticleSystem>())
            {
                psystem.Play();
            }
            gameObject.GetComponent<CircleCollider2D>().radius = blastRadius;
            StartCoroutine(selfDelete());
        }
    }

    private IEnumerator selfDelete() {
        _spriteRenderer.enabled = false;
        yield return null;
        gameObject.GetComponent<CircleCollider2D>().enabled = false;
        yield return new WaitForSeconds(1);
        Destroy(gameObject.transform.parent.gameObject);
    }
}
