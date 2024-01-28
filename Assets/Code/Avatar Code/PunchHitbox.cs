using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PunchHitbox : MonoBehaviour
{
    public float damage;
    public float impactForce;
    public float upwardsImpactForce;

    public PlayerManager owner;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerManager collisionPlayer = collision.GetComponent<PlayerManager>();

        if (collisionPlayer == null || collisionPlayer == owner)
            return;

        Vector2 playersDistance = owner.transform.position - collisionPlayer.transform.position;
        collisionPlayer.DealDamage(damage);

        owner.transform.rotation = Quaternion.identity;
        owner.rb.velocity = Vector3.zero;
        collisionPlayer.transform.rotation = Quaternion.identity;
        collisionPlayer.rb.velocity = Vector3.zero;


        owner.rb.AddForce(playersDistance.normalized * impactForce + Vector2.up * upwardsImpactForce);
        collisionPlayer.rb.AddForce(-playersDistance.normalized * impactForce + Vector2.up * upwardsImpactForce);
    }
}
