using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UIElements;

public class Kinematic : MonoBehaviour, Target
{

    public Vector3 Velocity
    {
        get { return velocity; }
    }

    public Vector3 Position
    {
        get { return position; }
    }

    public float Orientation
    {
        get { return orientation; }
    }
    private const string WANDER = "wander";
    private const string SEEK = "seek";
    private const string FLEE = "flee";
    private const string FACE = "face";
    private const string ARRIVE = "arrive";
    private const string ALIGN = "align";
    private const string MATCH = "match";
    private const string PURSUE = "pursue";
    private const string EVADE = "evade";
    private const string SEPARATION = "separation";
    private const string AVOID = "avoid";

    [SerializeField] private Kinematic[] separationTargets;
    [SerializeField] private GameObject targetGameObject = null;
    [SerializeField] private Target targetObject = null;
    [SerializeField] private float timeToTarget = 0.1f;
    [SerializeField] private float targetRadius = 1.5f;
    [SerializeField] private float slowRadius = 5f;
    [SerializeField] private float maxSpeed = 8f;
    [SerializeField] private float maxRotationSpeed = 8f;
    [SerializeField] private bool dynamic = false;
    [SerializeField] private bool kinematic = false;
    [SerializeField] private bool move = true;
    [SerializeField] private string action = "wander";
    [SerializeField] private float distanceToFlee = 5f;
    [SerializeField] private float maxAcceleration = 3f;
    [SerializeField] private float maxAngularAcceleration = 3f;
    [SerializeField] private float maxPrediction = 3f;
    [SerializeField] private float threshold = 6f;
    [SerializeField] private float decayCoefficient = 2f;
    [SerializeField] private float lookahead = 2f;
    [SerializeField] private float avoidDistance = 2f;

    [SerializeField] private float wanderOffset = 1f;
    [SerializeField] private float wanderRadius = 3f;
    [SerializeField] private float wanderRate = 1f;
    
    private float wanderOrientation = 0f;

    private Vector3 target = Vector3.zero;
    private float targetOrientation = 0f;
    private Vector3 direction = Vector3.zero;
    public Vector3 velocity = Vector3.zero;
    public Vector3 position = Vector3.zero;
    public float orientation = 0f;
    private Vector3 targetVelocity = Vector3.zero;
    private float rotation = 0f;
    private float targetSpeed = 0f;
    private float targetRotation = 0f;
    private float angularAccel = 0f;

    private Vector3 linear = Vector3.zero;
    private float angular = 0f;

    

    void Start()
    {
        targetObject = targetGameObject.GetComponent<Target>();
    }

    void Update()
    {
        if (dynamic)
        {
            switch (action)
            {
                case SEEK:
                    DSeek();
                    break;
                case FLEE:
                    DFlee();
                    break;
                case ARRIVE:
                    timeToTarget = 0.1f;
                    DArrive();
                    break;
                case ALIGN:
                    Align();
                    break;
                case FACE:
                    Face();
                    break;
                case MATCH:
                    VelocityMatch();
                    break;
                case PURSUE:
                    Pursue();
                    break;
                case EVADE:
                    Evade();
                    break;
                case WANDER:
                    DWander();
                    break;
                case SEPARATION:
                    Separation();
                    break;
                case AVOID:
                    ObstacleAvoidance();
                    break;
                default:
                    break;
            }
            DynamicUpdate();
        }
        else if (kinematic)
        {

            switch (action)
            {
                case ARRIVE:
                    timeToTarget = 0.25f;
                    KArrive();
                    break;
                case FLEE:
                    timeToTarget = 0.25f;
                    KFlee();
                    break;
                case WANDER:
                    KWander();
                    break;
                default:
                    break;
            }
            Move();
        }
        
        position = transform.position;
        orientation = transform.eulerAngles.y;
        target = targetGameObject.transform.position;
        targetOrientation = targetObject.Orientation;
    }

    private void Move()
    {
        if (move) transform.position += Time.deltaTime * velocity;
    }

    private void DynamicUpdate()
    {
        transform.position += Time.deltaTime * velocity;
        transform.eulerAngles += rotation * Vector3.up;

        velocity += Time.deltaTime * linear;
        rotation += Time.deltaTime * angular;

        if (velocity.magnitude > maxSpeed)
        {
            velocity = velocity.normalized * maxSpeed;
        }
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
    }

    private void KFlee()
    {
        velocity = transform.position - target;
        float distance = velocity.magnitude;

        if (distance < distanceToFlee)
        {
            velocity = maxSpeed * velocity.normalized;

            transform.forward = velocity.normalized;
        }
        else
        {
            transform.forward = -1 * velocity.normalized;
            velocity = Vector3.zero;
        }

    }

    private float RandomBinomial()
    {
        return UnityEngine.Random.Range(0.0f, 1.0f) - UnityEngine.Random.Range(0.0f, 1.0f); ;
    }

    private void KWander()
    {

        velocity = maxSpeed * transform.forward;
        transform.forward += maxRotationSpeed * new Vector3(RandomBinomial(), 0f, RandomBinomial());

        //direction = direction + new Vector3(RandomBinomial(), 0f, RandomBinomial());
        //velocity = maxSpeed * direction.normalized;
    }

    private void DSeek()
    {
        linear = target - transform.position;
        linear.Normalize();
        linear *= maxAcceleration;
        
        angular = 0f;
    }

    private void DFlee()
    {
        linear = target - transform.position;
        
        if ( linear.magnitude < distanceToFlee)
        {
            linear.Normalize();
            linear *= -1 * maxAcceleration;

        }
        else
        {
            linear = Vector3.zero;
            velocity = Vector3.zero;
        }

        angular = 0f;
    }

    private void DArrive()
    {
        direction = target - transform.position;
        float distance = direction.magnitude;

        if (distance < targetRadius)
        {
            velocity = Vector3.zero;
        }
        else
        {
            if (distance > slowRadius)
            {
                targetSpeed = maxSpeed;
            }
            else
            {
                targetSpeed = (maxSpeed * distance) / slowRadius;
            }

            targetVelocity = targetSpeed * direction.normalized;

            linear = targetVelocity - velocity;
            linear /= timeToTarget;

            if (linear.magnitude > maxAcceleration)
            {
                linear.Normalize();
                linear *= maxAcceleration;
            }

            angular = 0;
        }
    }

    public float mapToRange(float angle)
    {
        return (angle + Mathf.PI) % (2 * Mathf.PI) - Mathf.PI;
    }

    private void Align()
    {
        float newRotation = (targetOrientation * (Mathf.PI / 180)) - (transform.eulerAngles.y * (Mathf.PI / 180));
        newRotation = mapToRange(newRotation);
        float rotationSize = Mathf.Abs(newRotation);


        if (rotationSize < targetRadius)
        {
            angular = 0;
            rotation = 0;
        }
        else 
        {
            if (rotationSize > slowRadius)
            {
                targetRotation = maxRotationSpeed;
            }
            else
            {
                targetRotation = (maxRotationSpeed * rotationSize) / slowRadius;
            }

            targetRotation *= newRotation / rotationSize;

            angular = targetRotation - (rotation * (Mathf.PI / 180));
            angular /= timeToTarget;

            angularAccel = Mathf.Abs(angular);
            if (angularAccel > maxAngularAcceleration)
            {
                angular /= angularAccel;
                angular *= maxAngularAcceleration;
            }
            
            angular *= (180 / Mathf.PI);

            linear = Vector3.zero;
        }
    }

    private void Face()
    {
        direction = target - transform.position;

        if (direction.magnitude != 0)
        {
            targetOrientation = Mathf.Atan2(direction.x, direction.z);
            targetOrientation *= (180 / Mathf.PI);
            Align();
        }
    }

    private void VelocityMatch()
    {
        linear = targetObject.Velocity - velocity;
        linear /= timeToTarget;

        if (linear.magnitude > maxAcceleration)
        {
            linear.Normalize();
            linear *= maxAcceleration;
        }

        angular = 0;
    }

    private void Pursue()
    {
        direction = target - transform.position;
        float distance = direction.magnitude;

        float speed = velocity.magnitude;

        float prediction = 0f;
        
        if (speed <= (distance / maxPrediction))
        {
            prediction = maxPrediction;
        }
        else
        {
            prediction = distance / speed;
        }

        target += targetObject.Velocity * prediction;
        DArrive();
    }

    private void Evade()
    {
        direction = target - transform.position;
        float distance = direction.magnitude;

        float speed = velocity.magnitude;

        float prediction = 0f;

        if (speed <= (distance / maxPrediction))
        {
            prediction = maxPrediction;
        }
        else
        {
            prediction = distance / speed;
        }

        target += targetObject.Velocity * prediction;
        DFlee();
    }

    private void DWander()
    {
        wanderOrientation += RandomBinomial() * wanderRate;
        float newOrientation = wanderOrientation + (orientation * (Mathf.PI / 180));

        target = position + (wanderOffset * transform.forward);
        target += wanderRadius * (new Vector3(Mathf.Cos(newOrientation), 0f, Mathf.Sin(newOrientation)));
        
        Face();
        
        linear = maxAcceleration * transform.forward;
    }

    private void Separation()
    {
        VelocityMatch();
     
        foreach ( Kinematic separationTarget in separationTargets)
        {
            Vector3 sepDirection = position - separationTarget.Position;
            float distance = sepDirection.magnitude;

            if (distance < threshold)
            {
                float strength = maxAcceleration * (threshold - distance) / threshold;

                direction.Normalize();
                linear += strength * sepDirection;
            }
        }

    }

    private void LookWhereYoureGoing()
    {
        if (velocity.magnitude > 0)
        {
            targetOrientation = Mathf.Atan2(velocity.x, velocity.z) * (180 / Mathf.PI);
            Align();
        }
    }
     
    private void ObstacleAvoidance()
    {
        
        bool hit = Physics.CapsuleCast(transform.position, transform.position, 0.7f, (target - transform.position).normalized, out RaycastHit hitInfo, lookahead);

        if (hit)
        {
            target += avoidDistance * hitInfo.normal;
            Debug.Log(target);
        } else
        {
            target = targetObject.Position;
        }
        DArrive();
    }
}
