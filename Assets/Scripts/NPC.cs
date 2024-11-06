using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class NPC : MonoBehaviour
{

    public bool findPath = false;
    public bool seek = false;
    public int pathToX = 0;
    public int pathToZ = 0;
        
    private const string GUARD = "guard";
    private const string PRISONER = "prisoner";
    private const string GUNNER = "gunner";

    [SerializeField] private Pathfinder pathfinder;
    [SerializeField] private GameObject targetGameObject = null;
    [SerializeField] private float maxSpeed = 8f;
    [SerializeField] private float targetRadius = 1.5f;
    [SerializeField] private float timeToTarget = 0.1f;
    [SerializeField] private string role = "";
    [SerializeField] float detectionRange = 10f;
    [SerializeField] float detectionAngle = 30f;
    [SerializeField] float distanceToFlee = 15f;

    private Vector3 target = Vector3.zero;
    private Vector3 direction = Vector3.zero;
    private Vector3 velocity = Vector3.zero;
    private float angular = 0f;
    private List<Node> path = null;
    private float distanceToNode;
    private int currentNodeIndex = 0;


    void Start()
    {
        target = targetGameObject.transform.position;
    }

    void Update()
    {

        if (role == GUARD)
        {
            DetectTarget();
        } else if (role == PRISONER)
        {
            if ((targetGameObject.transform.position - transform.position).magnitude > distanceToFlee)
            {
                findPath = true;
            }
        }

        if (findPath)
        {
            if (path == null)
            {
                path = this.pathfinder.FindPath(transform.position, new Vector3(pathToX, 0, pathToZ));
            }

            if (currentNodeIndex < path.Count)
            {
                direction = path[currentNodeIndex].Position - transform.position;
                distanceToNode = direction.magnitude;

                if (distanceToNode < 0.25f)
                {
                    currentNodeIndex++;
                }

                transform.forward = direction.normalized;
                transform.position += maxSpeed * Time.deltaTime * direction.normalized;

            }
            else
            {
                path = null;
                findPath = false;
                currentNodeIndex = 0;
            }
        } else if (seek) {

            KArrive();

        } else
        {
            transform.forward = (targetGameObject.transform.position - transform.position).normalized;
        }

        target = targetGameObject.transform.position;
    }

    private void KArrive()
    {
        velocity = target - transform.position;
        float distance = velocity.magnitude;

        if (distance < targetRadius)
        {
            velocity = Vector3.zero;
        }
        else
        {
            velocity /= timeToTarget;

            if (velocity.magnitude > maxSpeed)
            {
                velocity = velocity.normalized * maxSpeed;
            }

            transform.forward = velocity.normalized;
        }

        transform.position += Time.deltaTime * velocity;
    }


    private void DetectTarget()
    {
        if (targetGameObject == null)
            return;

        Vector3 directionToTarget = (targetGameObject.transform.position - transform.position).normalized;
        float distanceToTarget = Vector3.Distance(transform.position, targetGameObject.transform.position);

        if (distanceToTarget <= detectionRange)
        {
            float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);

            if (angleToTarget <= detectionAngle / 2)
            {

                if (Physics.Raycast(transform.position, directionToTarget, out RaycastHit hit, detectionRange))
                {
                    findPath = true;
                }
            }
        }
    }


    void OnDrawGizmos()
    {
        if (path != null)
        {
            foreach (Node node in path)
            {
                Gizmos.color = Color.yellow;
                Vector3 nodePosition = new Vector3(node.Position.x, 0, node.Position.z);
                Gizmos.DrawWireCube(nodePosition, Vector3.one * 0.9f);
            }
        }

        if (role == GUARD)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, detectionRange);
        }
    }

}
