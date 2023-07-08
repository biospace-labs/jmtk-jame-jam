using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildPoint : MonoBehaviour
{
    public GameObject prefabToBuild;

    void Build()
    {
        Instantiate(prefabToBuild, transform.position, transform.rotation);
    }
}
