﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct WeaponPoint
{
    public GameObject ProjectilePrefab;
    public Transform SourceParent;
    public float WeaponVelocity;
    public float ShotsPerMinute;

    private WeaponBase weapon;
    [HideInInspector]
    public WeaponBase Weapon
    {
        get
        {
            if (weapon == null)
            {
                weapon = ProjectilePrefab.GetComponent<WeaponBase>();
            }
            return weapon;
        }
    }
}

[System.Serializable]
public struct ProjectileTarget
{
    public Transform TargetPoint;
    public Collider TargetCollider;
}

public class FireAtWillAction : FireWeaponBaseAction
{
    public List<WeaponPoint> WeaponPoints;
    public List<ProjectileTarget> TargetPoints;
    public float StartDelay;
    public float Duration;

    public override void Initiate()
    {
        if (StartDelay > 0f)
        {
            Service.TimerManager.CreateTimer(StartDelay, StartFiring, null);
        }
        else
        {
            StartFiring(null);
        }
    }

    private void StartFiring(object cookie)
    {
        Service.UpdateManager.AddObserver(OnUpdate);
        if (Duration > 0f)
        {
            Service.TimerManager.CreateTimer(Duration, OnDurationExpired, null);
        }
    }

    private void OnDurationExpired(object cookie)
    {
        CeaseFire();
    }

    private void OnUpdate(float dt)
    {
        for (int i = 0, count = WeaponPoints.Count; i < count; ++i)
        {
            float shotsPerSecond = WeaponPoints[i].ShotsPerMinute / 60f;
            float shotsPerDeltaTime = dt * shotsPerSecond;
            float rand = Random.Range(0f, 1f);
            if (rand <= shotsPerDeltaTime)
            {
                ProjectileTarget randomTarget = TargetPoints[Random.Range(0, TargetPoints.Count - 1)];
                FireAction fireAction = new FireAction();
                fireAction.TargetPosition = randomTarget.TargetPoint;
                fireAction.TargetCollider = randomTarget.TargetCollider;
                WeaponPoints[i].Weapon.Fire(fireAction, WeaponPoints[i].SourceParent, WeaponPoints[i].WeaponVelocity);
            }
        }
    }

    public override void CeaseFire()
    {
        Service.UpdateManager.RemoveObserver(OnUpdate);
    }
}
