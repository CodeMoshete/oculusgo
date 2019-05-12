using UnityEngine;

public class TorpedoWeapon : WeaponBase
{
    private Vector3 movementDir;

    public override void Fire(FireAction fireAction, Transform sourceParent, float velocity)
    {
        base.Fire(fireAction, sourceParent, velocity);
        transform.position = SourceParent.position;
        movementDir = Vector3.Normalize(InitialPosition - transform.position) * Velocity;
    }

    private void Update()
    {
        transform.Translate(movementDir * Time.deltaTime);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (TargetCollider == null || other == TargetCollider)
        {
            if (OnHit != null)
            {
                OnHit.Invoke();
            }

            GameObject explosion = Instantiate(HitFX, transform.position, Quaternion.identity);
            explosion.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);
            Destroy(gameObject);
        }
    }
}
