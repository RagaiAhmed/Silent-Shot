using System;
using UnityEngine;

public class ChildCollider : MonoBehaviour
{
	public HelicopterController Main_Health;

	void OnCollision(Collision collision)
	{
		float otherMass;
		if (collision.rigidbody) {
			otherMass = collision.rigidbody.mass;
		} else {
			otherMass = 1000;
			float force = otherMass * (Mathf.Sqrt (Mathf.Pow (collision.relativeVelocity.x, 2) + Mathf.Pow (collision.relativeVelocity.y, 2) + Mathf.Pow (collision.relativeVelocity.z, 2)));
			if (force > 1) {
				Main_Health.decreaseHealth(Mathf.RoundToInt (force / 2500));
			}
		}
	}
}
