using UnityEngine;

public class TorpedoWeapon : WeaponBase
{
    public GameObject Explosion;
    private Vector3 movementDir;

    public override void Fire(Vector3 target, float velocity)
    {
        base.Fire(target, velocity);

        movementDir = Vector3.Normalize(target - transform.position) * velocity;
    }

    private void Update()
    {
        transform.Translate(movementDir);
    }

    public void OnTriggerEnter(Collider other)
    {
        Instantiate(Explosion, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
