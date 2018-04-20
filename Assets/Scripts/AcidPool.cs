using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcidPool : Behaviour1D
{
    protected override void Colliding(Behaviour1D other)
    {
        var damageable = other as IDamageable;
        if (damageable != null)
        {
            damageable.Damaged(this);
        }
    }
}
