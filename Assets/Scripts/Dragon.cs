﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dragon : Unit
{

    [SerializeField]
    uint reward = 100;

    List<Soldier> seenSoldiers = new List<Soldier>();
    Soldier ClosestSoldier
    {
        get
        {
            if (seenSoldiers == null || seenSoldiers.Count <= 0) return null;
            float minDistance = float.MaxValue;
            Soldier closestSoldier = null;
            foreach (Soldier soldier in seenSoldiers)
            {
                if (!soldier || !soldier.IsAlive) continue;
                float distance = Vector3.Magnitude(soldier.transform.position - transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestSoldier = soldier;
                }
            }
            return closestSoldier;

        }
    }

    [SerializeField]
    float indlingCooldown = 2;
    [SerializeField]
    float chasingSpeed = 5;
    [SerializeField]
    float patrolRadius = 5;
    float normalSpeed;

    Vector3 startPoint;
    float indlingTimer;


    protected override void Awake()
    {
        base.Awake();
        normalSpeed = nav.speed;
        startPoint = transform.position;
        GameController.DragonList.Add(this);
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        var soldier = other.gameObject.GetComponent<Soldier>();
        if (soldier && seenSoldiers.Contains(soldier))
        {
            seenSoldiers.Add(soldier);
        }
    }

    protected override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);
        var soldier = other.gameObject.GetComponent<Soldier>();
        if (soldier && seenSoldiers.Contains(soldier))
        {
            seenSoldiers.Remove(soldier);
        }
    }

    protected override void Idling()
    {
        base.Idling();
        UpdateSight();
        if ((indlingTimer -= Time.deltaTime) <= 0)
        {
            indlingTimer = indlingCooldown;
            task = Task.move;
            SetRandomRoamingPosition();
        }
    }

    protected override void Moving()
    {
        base.Moving();
        nav.speed = normalSpeed;
        UpdateSight();
    }

    protected override void Chasing()
    {
        base.Chasing();
        nav.speed = chasingSpeed;
    }

    void UpdateSight()
    {
        var soldier = ClosestSoldier;
        if (soldier)
        {
            target = soldier.transform;
            task = Task.chase;
        }
    }

    void SetRandomRoamingPosition()
    {
        Vector3 destination;

        Vector3 delta = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
        delta.Normalize();
        delta *= patrolRadius;

        nav.SetDestination(startPoint + delta);
    }

    public override void ReceiveDamage(float damage, Vector3 damageDealerPosition)
    {
        base.ReceiveDamage(damage, damageDealerPosition);

        if (IsAlive)
        {
            if (!target)
            {
                task = Task.move;
                nav.SetDestination(damageDealerPosition);
            }
            if (HealthPercent > .5f)
            {
                animator.SetTrigger("Get Hit");
                nav.velocity = Vector3.zero;
            }

        }
        else
        {
            if (!IsAlive && Money.TryAddMoney(reward) && reward > 0)
            {
                MoneyEarner.ShowMoneyText(transform.position, (int)reward);
                reward = 0;
            }

        }


    }

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();
        if (Application.isPlaying) startPoint = transform.position;
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(startPoint, patrolRadius);
    }

}
