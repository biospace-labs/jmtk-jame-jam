using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ResourceType
{
    Brick,
    Metal,
}

public class ResourceItem : MonoBehaviour
{
    public ResourceType resourceType;
}
