using UnityEngine;

public class LabelBillboard : MonoBehaviour
{
    void Update()
    {
        Transform cam = Camera.main.transform;
        transform.LookAt(transform.position + cam.rotation * Vector3.forward,
                         cam.rotation * Vector3.up);
    }
}