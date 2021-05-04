using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NBodySimulation : MonoBehaviour
{
    public GameObject planetPrefab;
    public GameObject planetMarkerPrefab;
    public GameObject sunMarkerPrefab;
    public DrawVector vector;
    public Graph graph;
    public float maxDistance = 20;

    private List<Orb> orbs;
    private List<Transform> orbMarkers;
    private Transform newOrb;
    private Transform newOrbMarker;
    private Vector2 graphSize;

    private void Awake()
    {
        // Get all orbs in the scene as a List (to be able to add more later)
        orbs = new List<Orb>(FindObjectsOfType<Orb>());
        orbMarkers = new List<Transform>();
        Debug.Log("Setting fixed delta time to " + Params.physicsTimeStep + " (from " + Time.fixedDeltaTime + ")");
        Time.fixedDeltaTime = Params.physicsTimeStep;

        graphSize = graph.transform.GetComponent<RectTransform>().rect.size;

        // Should only have the sun at index 0
        if (orbs.Count > 0)
        {
            Transform sunMarker = Instantiate(sunMarkerPrefab, graph.transform).transform;
            sunMarker.GetComponent<RectTransform>().anchoredPosition = new Vector2(graph.x0 * graphSize.x, graph.y0 * graphSize.y);
            orbMarkers.Add(sunMarker);
        }
    }

    private void Start()
    {
        // Try to fix WebGL build bug where planets don't respond to the sun initially
        UpdateSunMass(0.5f);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            if (orbs.Count > 10)
            {
                return;
            }

            Cursor.visible = false;

            // Get clicked position
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = -Camera.main.transform.position.z;
            Vector3 position = Camera.main.ScreenToWorldPoint(mousePosition);
            newOrb = Instantiate(planetPrefab, position, Quaternion.identity).transform;

            // Anchor the tail of the velocity vector
            vector.SetTailPosition(position);

            // Plot distance to sun on graph x-axis (assuming the sun stays at the origin)
            newOrbMarker = Instantiate(planetMarkerPrefab, graph.transform).transform;
            newOrbMarker.GetComponent<RectTransform>().anchoredPosition = new Vector2(graph.x0 * graphSize.x + position.magnitude * 10, graph.y0 * graphSize.y);

            LineRenderer lr = newOrbMarker.GetChild(0).GetComponent<LineRenderer>();
            lr.positionCount = 2;
            lr.SetPosition(0, Vector3.zero);
            lr.SetPosition(1, new Vector3(0, graph.ComputePotential(position.magnitude * 10), 0));
        }

        if (newOrb != null)
        {
            // Draw initial velocity vector
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = -Camera.main.transform.position.z;
            Vector3 position = Camera.main.ScreenToWorldPoint(mousePosition);
            vector.SetHeadPosition(position);

            // Include the new orb in the gravitational system
            if (Input.GetMouseButtonUp(0))
            {
                Orb orb = newOrb.GetComponent<Orb>();
                orb.SetInitialVelocity(vector.GetValue());
                orbs.Add(orb);
                newOrb = null;
                vector.Reset();

                orbMarkers.Add(newOrbMarker);
                newOrbMarker = null;

                Cursor.visible = true;
            }
        }
    }

    private void FixedUpdate()
    {
        // First compute accelerations on each orb based on current positions
        foreach (Orb orb in orbs)
        {
            Vector3 acceleration = ComputeAcceleration(orb.transform.position, orb);
            orb.UpdateVelocity(acceleration, Params.physicsTimeStep);
        }

        // Then move each orb and its graph marker
        List<Orb> orbsToDestroy = new List<Orb>();
        // Sun should be at index 0
        foreach (Orb orb in orbs)
        {
            orb.UpdatePosition(Params.physicsTimeStep);

            if (orb.name != "Sun")
            {
                if (orb.transform.position.magnitude > maxDistance || orb.needToDestroy)
                {
                    orbsToDestroy.Add(orb);
                }

                // Update planet marker on graph
                Transform orbMarker = orbMarkers[orbs.IndexOf(orb)];
                float r = orb.transform.position.magnitude;
                float x = Mathf.Min(graphSize.x, graph.x0 * graphSize.x + r * 10);
                orbMarker.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, graph.y0 * graphSize.y);

                LineRenderer lr = orbMarker.GetChild(0).GetComponent<LineRenderer>();
                lr.positionCount = 2;
                lr.SetPosition(0, Vector3.zero);
                lr.SetPosition(1, new Vector3(0, graph.ComputePotential(x - graph.x0 * graphSize.x), 0));
            }
        }

        foreach (Orb orb in orbsToDestroy)
        {
            int index = orbsToDestroy.IndexOf(orb);
            Transform orbMarker = orbMarkers[index + 1];  // Avoid the sun
            orbMarkers.Remove(orbMarker);
            Destroy(orbMarker.gameObject);
            orbs.Remove(orb);
            Destroy(orb.gameObject);
        }
    }

    private Vector3 ComputeAcceleration(Vector3 x, Orb ignoredOrb = null)
    {
        Vector3 acceleration = Vector3.zero;

        foreach (Orb orb in orbs)
        {
            if (orb != ignoredOrb)
            {
                // Squared distance between this orb and the current other
                float r2 = (orb.transform.position - x).sqrMagnitude;
                // Unit vector pointing towards the other orb
                Vector3 rhat = (orb.transform.position - x).normalized;
                // Acceleration this orb experiences due to the other
                acceleration += rhat * Params.newtonG * orb.mass / r2;
            }
        }

        return acceleration;
    }

    public void UpdateSunMass(float radius)
    {
        foreach (Orb orb in orbs)
        {
            if (orb.name == "Sun")
            {
                orb.radius = 2 * radius;
                orb.UpdateProperties();  // TODO Need to be cleaner than this ?!?!
                break;
            }
        }
    }
}
