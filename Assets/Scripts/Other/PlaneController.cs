using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneController : MonoBehaviour
{

    public float RotateSpeed = 1; 
    public float Radius = 5f;

    [SerializeField]
    private Vector3 centre;
    private float _angle;

    private void Start()
    {
        centre = transform.position;
    }

    private void Update()
    {
        _angle += RotateSpeed * Time.deltaTime; // opposite -=

        var offset = new Vector3(Mathf.Sin(_angle), Mathf.Cos(_angle)) * Radius;

        transform.position = centre - offset;
        UpdateRotation();
    }

    private void UpdateRotation()
    {
        Vector3 diff = centre - transform.position;
        diff.Normalize();

        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0.0f, 0.0f, rot_z + 90);
    }
}
