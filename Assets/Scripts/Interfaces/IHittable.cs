using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHittable
{
	void OnHit(Vector2 direction, float damage);
}
