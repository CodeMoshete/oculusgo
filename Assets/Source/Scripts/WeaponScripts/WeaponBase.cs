using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    protected Vector3 TargetPos;
    protected float Velocity;

    public virtual void Fire(Vector3 target, float velocity)
    {
        TargetPos = target;
        Velocity = velocity;
    }
}
