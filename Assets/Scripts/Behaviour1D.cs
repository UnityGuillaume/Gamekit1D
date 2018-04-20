using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public abstract class Behaviour1D : MonoBehaviour
{
    public int position
    {
        get { return _position; }
        set
        {
            if (Map.Instance != null)
            {
                var entries = Map.Instance.GetEntries(value);

                if (entries != null)
                {
                    foreach (var entry in entries)
                    {
                        //we can't go on that square, it's blocking.
                        if (entry.content != null && entry.content.isBlocking)
                        {
                            _internalFloatPosition = _position;
                            return;
                        }
                    }
                }
            }

            _position = value;
            Vector3 pos = transform.position;
            pos.x = _position;
            transform.position = pos;

            UpdateEntry();
        }
    }

    public bool isBlocking = false;

    private int _position = 0;
    private Map.MapEntry _mapEntry = null;
    private float _internalFloatPosition = 0.0f;

    protected virtual void OnEnable()
    {
        _internalFloatPosition = transform.position.x;
        _position = Mathf.RoundToInt(_internalFloatPosition);

        if (Map.Instance != null)
            _mapEntry = Map.Instance.Register(_position, this);
    }

    protected virtual void OnDisable()
    {
        if (Map.Instance != null && _mapEntry != null)
            Map.Instance.Unregister(_mapEntry);
    }

    // will translate the internal floating point position, that will take care of moving the round position when reaching it
    public void Translate(float amount)
    {
        _internalFloatPosition += amount;
        position = Mathf.RoundToInt(_internalFloatPosition);
    }

    //will set the float position to the nearest integer, allow to not leave the object in a "in between" state that the user can't see
    //used e.g. by character when they stop moving, so the next movmeent restart from the position and not the in between state, creating
    //a strange behaviour where they seem to take different time to move between position
    public void SnapFloatPosition()
    {
        _internalFloatPosition = Mathf.RoundToInt(_internalFloatPosition);
    }

    public void EnsurePosition()
    {
        Vector3 pos = transform.position;

        var resultingPos = Mathf.RoundToInt(transform.position.x);

        if (Map.Instance != null)
        {
            var entries = Map.Instance.GetEntries(resultingPos);

            if (entries != null)
            {
                foreach (var entry in entries)
                {
                    //we can't go on that square, it's blocking.
                    if (entry.content != null && entry.content.isBlocking)
                    {
                        resultingPos = _mapEntry.position;
                        _internalFloatPosition = resultingPos;
                        break;
                    }
                }
            }
        }

        _position = resultingPos;

        pos.x = _position;
        pos.y = 0;

        transform.position = pos;
        transform.rotation = Quaternion.identity;

        UpdateEntry();
    }

    void UpdateEntry()
    {
        if (Map.Instance != null && _mapEntry != null && _mapEntry.position != _position)
        {
            var entries = Map.Instance.GetEntries(position);

            if (entries != null)
            {
                foreach (var entry in entries)
                {
                    entry.content.Colliding(this);
                    this.Colliding(entry.content);
                }
            }

            Map.Instance.MoveEntry(_mapEntry, position);
        }
    }

    void LateUpdate()
    {
        EnsurePosition();
    }

    protected virtual void Colliding(Behaviour1D other)
    {

    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(Behaviour1D), true)]
public class Behaviour1DEditor : Editor
{
    private Behaviour1D _target;

    private void OnEnable()
    {
        _target = target as Behaviour1D;
    }

    private void OnSceneGUI()
    {
        _target.EnsurePosition();
    }
}
#endif