using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeballView : MonoBehaviour
{
    [SerializeField] private Transform[] points;
    [SerializeField] private Transform _bodyTransform;

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotateSpeed = 360f;

    private int currentPoint = 0;

    private Transform _transform;

    private void Awake()
    {
        _transform = transform;
    }

    void Update()
    {
        if (points.Length == 0) return;

        Transform target = points[currentPoint];

        // Store old position
        Vector3 oldPosition = _transform.position;

        // Move toward target
        _transform.position = Vector3.MoveTowards(
            _transform.position,
            target.position,
            moveSpeed * Time.deltaTime
        );

        // Calculate movement direction
        Vector3 moveDir = _transform.position - oldPosition;

        // Rotate spike ball in movement direction
        if (moveDir.magnitude > 0.001f)
        {
            Vector3 rotationAxis = Vector3.Cross(Vector3.up, moveDir.normalized);

            float distance = moveDir.magnitude;

            _bodyTransform.Rotate(
                rotationAxis,
                distance * rotateSpeed,
                Space.World
            );
        }

        // Reached waypoint?
        if (Vector3.Distance(_transform.position, target.position) < 0.05f)
        {
            currentPoint++;

            if (currentPoint >= points.Length)
            {
                currentPoint = 0; // loop
            }
        }
    }
}
