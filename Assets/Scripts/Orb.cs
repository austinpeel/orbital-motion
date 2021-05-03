using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orb : MonoBehaviour
{
    [Min(0)]
    public float radius;
    [Range(0.1f, 40)]
    public float density = 1;
    public float mass { get; private set; }

    [HideInInspector]
    public bool needToDestroy;

    private Vector3 velocity;

    public void UpdateVelocity(Vector3 acceleration, float timeStep)
    {
        velocity += acceleration * timeStep;
    }

    public void UpdatePosition(float timeStep)
    {
        transform.Translate(velocity * timeStep, Space.World);
    }

    public void SetInitialVelocity(Vector3 initialVelocity)
    {
        velocity = initialVelocity;
    }

    public void UpdateProperties()
    {
        OnValidate();
    }

    private void OnValidate()
    {
        transform.localScale = radius * Vector3.one;
        mass = 4 * Mathf.PI * radius * radius * radius * density / 3;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name != "Sun")
        {
            other.GetComponent<Orb>().needToDestroy = true;
        }
    }
}
