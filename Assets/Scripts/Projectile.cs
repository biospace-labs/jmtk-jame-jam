using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float lifespan = 5;
    public float force = 10;
    public float time = 0;
    private Rigidbody2D _rigidbody;
    // Start is called before the first frame update
    void Start()
    {   
        _rigidbody = gameObject.GetComponent<Rigidbody2D>();
        Vector2 forceDirection = new Vector2(Mathf.Cos(gameObject.transform.eulerAngles.z * Mathf.Deg2Rad),
            Mathf.Sin(gameObject.transform.eulerAngles.z * Mathf.Deg2Rad));
        _rigidbody.AddForce(forceDirection * force, ForceMode2D.Impulse);
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if (time >= lifespan) {
            Destroy(gameObject.transform.parent.gameObject);
        }
    }
}
