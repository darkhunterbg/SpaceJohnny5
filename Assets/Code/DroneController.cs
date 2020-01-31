using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneController : MonoBehaviour
{
    [Header("Camera Settings")]
    public Camera Camera;
    public float Distance;
    public float VerticalShift;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LateUpdate()
    {
        Vector3 offset = Vector3.zero;
        offset += -transform.forward * Distance;
        offset += transform.up * VerticalShift;
        Camera.transform.position = transform.position + offset;
    }
}
