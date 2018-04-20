using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : Behaviour1D, IDamageable
{
    public static PlayerController Instance { get; private set; }

    [Tooltip("In unit per second")]
    public float speed = 0.5f;

    public int maxHealth = 5;

    public GameObject weapon;

    public int currentHealth
    {
        get { return _currentHealth; }
    }

    protected int _facing = 1;
    protected int _currentHealth;
    protected float _sinceLastAttack = -1.0f;

    protected override void OnEnable()
    {
        base.OnEnable();

        _currentHealth = maxHealth;
        Instance = this;
    }

    private void Update()
    {
        if (_sinceLastAttack > -1)
        {
            _sinceLastAttack -= Time.deltaTime;
            if (_sinceLastAttack <= 0)
            {
                _sinceLastAttack = -1;
                weapon.SetActive(false);
            }
        }

        float input = Input.GetAxis("Horizontal");

        if (input > 0.2f)
        {
            Translate(speed * Time.deltaTime);
            _facing = 1;
        }
        else if (input < -0.2f)
        {
            Translate(-speed * Time.deltaTime);
            _facing = -1;
        }
        else
            SnapFloatPosition();

        if (Input.GetButtonDown("Fire1"))
        {
            Attack();
        }

        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }
    }

    void Attack()
    {
        weapon.SetActive(true);
        weapon.transform.localPosition = new Vector3(_facing, 0, 0);
        _sinceLastAttack = 0.2f;

        var entries = Map.Instance.GetEntries(position + _facing);
        if (entries != null)
        {
            List<IDamageable> toDamage = new List<IDamageable>();

            foreach (var entry in entries)
            {
                var damageable = entry.content as IDamageable;
                if (damageable != null)
                {
                    toDamage.Add(damageable);
                }
            }

            foreach (var entry in toDamage)
            {
                entry.Damaged(this);
            }
        }
    }

    void Jump()
    {
        Translate(_facing * 2);
    }

    public void Damaged(Behaviour1D damager)
    {
        _currentHealth -= 1;

        if (_currentHealth == 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    protected override void Colliding(Behaviour1D other)
    {
        if (other is ChomperBehaviour)
        {
            Damaged(other);
        }
    }
}

public interface IDamageable
{
    void Damaged(Behaviour1D damager);
}