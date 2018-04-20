using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChomperBehaviour : Behaviour1D, IDamageable
{
    public enum ChomperState
    {
        WALKING,
        RUNNING,
        ATTACKING
    }


    [Tooltip("In unit per second")]
    public float speed = 0.5f;

    public int visionRange = 1;
    public float attackDelay = 2.0f;
    public float attackCoolDown = 2.0f;

    private ChomperState _currentState;

    private float _nextDirectionChange;
    private int _walkingDirection = 1;

    private float _toAttack;
    private float _postAttackCooldown;

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    void Start()
    {
        _currentState = ChomperState.WALKING;
    }

    void Update()
    {
        switch (_currentState)
        {
            case ChomperState.WALKING:
                Walk();
                break;
            case ChomperState.RUNNING:
                Run();
                break;
            case ChomperState.ATTACKING:
                Attack();
                break;
            default:
                break;
        }
    }

    bool CanSeePlayer()
    {
        int dist = Mathf.Abs(PlayerController.Instance.position - position);

        if (dist <= visionRange)
        {
            return true;
        }

        return false;
    }

    public void Damaged(Behaviour1D damager)
    {
        Destroy(gameObject);
    }

    //=== State Update Functions

    void Walk()
    {
        if (CanSeePlayer())
        {
            StartPursuit();
            return;
        }

        _nextDirectionChange -= Time.deltaTime;

        if (_nextDirectionChange <= 0)
            StartWalking();

        switch (_walkingDirection)
        {
            case -1:
                Translate(-speed * Time.deltaTime);
                break;
            case 1:
                Translate(speed * Time.deltaTime);
                break;
            default:
                SnapFloatPosition();
                break;
        }
    }

    void Run()
    {
        if (!CanSeePlayer())
        {
            StartWalking();
            return;
        }

        int playerDist = PlayerController.Instance.position - position;

        if (Mathf.Abs(playerDist) == 1)
        {
            StartAttack();
            return;
        }

        int playerDirection =  playerDist < 0 ? -1 : 1;

        switch (playerDirection)
        {
            case -1:
                Translate(-(speed + 0.5f) * Time.deltaTime);
                break;
            case 1:
                Translate((speed + 0.5f) * Time.deltaTime);
                break;
            default:
                SnapFloatPosition();
                break;
        }
    }

    void Attack()
    {
        if (_postAttackCooldown < 0)
        {
            _toAttack -= Time.deltaTime;

            if (_toAttack < 0)
            {
                int playerDist = PlayerController.Instance.position - position;
                int playerDirection = playerDist < 0 ? -1 : 1;

                position += playerDirection;

                _postAttackCooldown = attackCoolDown;
            }
        }

        else
        {
            _postAttackCooldown -= Time.deltaTime;

            if (_postAttackCooldown < 0)
            {

                _currentState = ChomperState.WALKING;
            }
        }
    }

    //=== State Change Functions

    void StartWalking()
    {
        _nextDirectionChange = Random.Range(2.0f, 5.0f);
        _walkingDirection = Random.Range(0, 3) - 1;
        _currentState = ChomperState.WALKING;
    }

    void StartPursuit()
    {
        _currentState = ChomperState.RUNNING;
    }

    void StartAttack()
    {
        _currentState = ChomperState.ATTACKING;
        _toAttack = attackDelay;
        _postAttackCooldown = -1;
    }

    // ====

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1.0f, 0.0f, 0.0f, 0.5f);
        Gizmos.DrawCube(transform.position, new Vector3(1.0f + visionRange * 2, 1.0f, 1.0f));
    }
}
