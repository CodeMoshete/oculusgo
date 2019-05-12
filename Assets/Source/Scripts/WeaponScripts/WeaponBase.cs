using System;
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    public GameObject HitFX;

    [HideInInspector]
    public Action OnHit;

    protected Vector3 InitialPosition;
    protected Transform Target;
    protected float Velocity;
    protected Collider TargetCollider;
    protected Transform SourceParent;

    public virtual void Fire(FireAction fireAction, Transform sourceParent, float velocity)
    {
        InitialPosition = fireAction.TargetPosition.position;
        Target = fireAction.TargetPosition;
        Velocity = velocity;
        TargetCollider = fireAction.TargetCollider;
        SourceParent = sourceParent;
    }
}
