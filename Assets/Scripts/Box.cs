using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : Behaviour1D, IDamageable
{
    public void Damaged(Behaviour1D damager)
    {
        Destroy(gameObject);
    }
}
