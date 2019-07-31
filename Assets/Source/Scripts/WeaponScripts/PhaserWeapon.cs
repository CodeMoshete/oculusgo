using UnityEngine;

public class PhaserWeapon : WeaponBase
{
    private readonly Vector3 SCALE_SPEED = new Vector3(0f, 0f, 5f);
    private readonly Vector3 DESTROY_SCALE_SPEED = new Vector3(0.5f, 0.5f, 0f);

    public Transform EndPoint;
    private float lifespan = 5f;
    private bool shouldDestroy = false;

    public override void Fire(FireAction fireAction, Transform sourceParent, float velocity)
    {
        base.Fire(fireAction, sourceParent, velocity);

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
                Debug.Log("Scale: " + transform.localScale);
                transform.localScale -= (DESTROY_SCALE_SPEED * Time.deltaTime);
            }
            else
            {
                Debug.Log("Destroy projectile");
                Destroy(gameObject);
            }
        }
        else
        {
            transform.localScale += SCALE_SPEED;

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

            GameObject explosion = Instantiate(HitFX, hitFxPoint, Quaternion.identity);
            explosion.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);
            shouldDestroy = true;
            Debug.Log("SHOULD Destroy projectile");
        }
    }
}
