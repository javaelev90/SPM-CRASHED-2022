using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] float movementSpeed = 6f;
    [SerializeField] float rotationSpeed = 2f;
    [SerializeField] float targetPositionTolerance = 3f;
    private float minX;
    private float maxX;
    private float minZ;
    private float maxZ;
    Rigidbody objectBody;
    Vector3 velocity;

    FieldOfView fov;
    //Camera viewCamera;
    //Vector2 rawMousePos;

    //private void Start()
    //{
    //    objectBody = GetComponent<Rigidbody>();
    //    viewCamera = Camera.main;
    //}

    //private void Update()
    //{
    //    rawMousePos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
    //    Vector3 mousePos = viewCamera.ScreenToWorldPoint(new Vector3(rawMousePos.x, rawMousePos.y, viewCamera.transform.position.y));
    //    transform.LookAt(mousePos + Vector3.up * transform.position.y);
    //    Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    //    velocity = new Vector3(input.x, 0f, input.y).normalized * movementSpeed;
    //}

    //private void FixedUpdate()
    //{
    //    objectBody.MovePosition(objectBody.position + velocity * Time.deltaTime);
    //}

    private void Start()
    {
        minX = -45f;
        maxX = 45f;

        minZ = -45f;
        maxZ = 45f;
        fov = GetComponent<FieldOfView>();

        GetNextPosition();

    }

    private void Update()
    {
        
    }

    void GetNextPosition()
    {

    }
}
