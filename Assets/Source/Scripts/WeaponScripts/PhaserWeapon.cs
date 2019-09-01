using UnityEngine;

public class PhaserWeapon : WeaponBase
{
    private readonly Vector3 DESTROY_SCALE_SPEED = new Vector3(0.5f, 0.5f, 0f);
    private const float DEFAULT_SCALE_SPEED = 5f;

    public Transform EndPoint;
    private float lifespan = 5f;
    private Vector3 scaleSpeed;
    private bool shouldDestroy = false;

    public override void Fire(FireAction fireAction, Transform sourceParent, float velocity)
    {
        base.Fire(fireAction, sourceParent, velocity);

        float zSpeed = Velocity > 0f ? Velocity : DEFAULT_SCALE_SPEED;
        scaleSpeed = new Vector3(0f, 0f, zSpeed);
        transform.parent = SourceParent;
        transform.localPosition = Vector3.zero;
    }

    private void Update()
    {
        transform.LookAt(InitialPosition);

        if (shouldDestroy)
        {
            if (transform.localScale.x > 0)
            {
                transform.localScale -= (DESTROY_SCALE_SPEED * Time.deltaTime);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        else
        {
            transform.localScale += scaleSpeed;

            lifespan -= Time.deltaTime;
            shouldDestroy = lifespan <= 0f;
        }

    }

    public void OnTriggerEnter(Collider other)
    {
        if (TargetCollider == null || other == TargetCollider)
        {
            if (OnHit != null)
            {
                OnHit.Invoke();
            }

            Vector3 hitFxPoint = EndPoint.position;
            Ray ray = new Ray(transform.position, EndPoint.position - transform.position);
            RaycastHit[] hits = Physics.RaycastAll(ray);
            for (int i = 0, count = hits.Length; i < count; ++i)
            {
                RaycastHit hit = hits[i];
                if (hit.collider == TargetCollider)
                {
                    hitFxPoint = hit.point;
                }
            }

            float distToPoint = Vector3.Distance(hitFxPoint, transform.position);
            Vector3 scale = transform.localScale;
            scale.z = distToPoint;
            transform.localScale = scale;

            GameObject explosion = Instantiate(HitFX, hitFxPoint, Quaternion.identity);
            explosion.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);
            shouldDestroy = true;
        }
    }
}
