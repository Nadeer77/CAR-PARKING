using UnityEngine;
using UnityEngine.AI;

public class NpcController : MonoBehaviour
{
    public NavMeshAgent agent;       // NPC navigation
    public Animator animator;        // NPC animations
    public GameObject wayPoints;          // Parent object holding waypoints
    public Transform[] points;   // All waypoint positions
    public float minDistance = 1f;   // How close to waypoint before switching
    private int index = 0;           // Current waypoint

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        // Load waypoints from PATH parent
        points = new Transform[wayPoints.transform.childCount];
        for (int i = 0; i < points.Length; i++)
        {
            points[i] = wayPoints.transform.GetChild(i);
        }

        // Start walking to first waypoint
        if (points.Length > 0)
            agent.SetDestination(points[index].position);
    }

    void Update()
    {
        Roam();
    }

    void Roam()
    {
        if (points.Length == 0) return;

        // Check if NPC is close to the current waypoint
        if (Vector3.Distance(transform.position, points[index].position) < minDistance)
        {
            // Move to next waypoint (loops back to 0 after last)
            index = (index + 1) % points.Length;
            agent.SetDestination(points[index].position);
        }

        // Update animation
        animator.SetBool("Idle", false);
        animator.SetBool("Walking", !agent.isStopped);
    }
}