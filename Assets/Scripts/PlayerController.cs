using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Behaviour1D
{
    public static PlayerController Instance { get; private set; }

    [Tooltip("In unit per second")]
    public float speed = 0.5f;

    protected override void OnEnable()
    {
        base.OnEnable();

        Instance = this;
    }

    private void Update()
    {
        float input = Input.GetAxis("Horizontal");

        if (input > 0.2f)
            Translate(speed * Time.deltaTime);
        else if (input < -0.2f)
            Translate(-speed * Time.deltaTime);
        else
            SnapFloatPosition();
    }

    protected override void Colliding(Behaviour1D other)
    {
        if (other is ChomperBehaviour)
        {
            Debug.Log("Damaged");
        }
    }
}
