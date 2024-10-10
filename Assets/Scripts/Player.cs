using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour, Target
{

    public static Player Instance { get; private set; }
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private GameInput gameInput;

    float rotationSpeed = 20f;
    private bool isWalking;

    public Vector3 velocity = Vector3.zero;
    public Vector3 position = Vector3.zero;
    public float orientation = 0f;

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


    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("More than 1 player");
        }

        Instance = this;
    }

    private void Update() {

        HandleMovement();
    }

    public bool IsWalking()
    {
        return isWalking;
    }

    private void HandleMovement()
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);
        float playerRadius = 0.7f;
        float playerHeight = 2f;
        float moveDistance = Time.deltaTime * moveSpeed;
        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + (Vector3.up * playerHeight), playerRadius, moveDir, moveDistance);
        velocity = moveDir * moveSpeed;
        orientation = transform.eulerAngles.y;

        if (!canMove)
        {
            Vector3 moveX = new Vector3(moveDir.x, 0f, 0f).normalized;
            canMove = moveDir.x != 0 && !Physics.CapsuleCast(transform.position, transform.position + (Vector3.up * playerHeight), playerRadius, moveX, moveDistance);

            if (canMove) moveDir = moveX;
            else
            {
                Vector3 moveZ = new Vector3(0f, 0f, moveDir.z).normalized;
                canMove = moveDir.z != 0 && !Physics.CapsuleCast(transform.position, transform.position + (Vector3.up * playerHeight), playerRadius, moveZ, moveDistance);
                if (canMove) moveDir = moveZ;
            }
        }

        if (canMove)
        {
            transform.position += moveDistance * moveDir;
            position = transform.position;
        }

        isWalking = moveDir != Vector3.zero;

        transform.forward = Vector3.Slerp(transform.forward, moveDir, rotationSpeed * Time.deltaTime);
    }
}
