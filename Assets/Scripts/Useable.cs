using System;
using System.Collections;
using UnityEngine;

public abstract class Useable : MonoBehaviour
{
    public abstract void BeginUse();
    public virtual void EndUse() {}
}