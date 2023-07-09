using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    public GameObject toShoot;
    public float timeToShoot = 2;
    
    private float time = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if (time >= timeToShoot) {
            ShootProjectile();
            time -= timeToShoot;
        }
    }

    public void ShootProjectile()
    {
        Instantiate(toShoot, gameObject.transform.position + gameObject.transform.up * 0.5f, gameObject.transform.rotation);
    }
}
