using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraServerControl : MonoBehaviour
{
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        var horizontal = Input.GetAxis("Mouse X") * 400f * Time.deltaTime;
        var vertical = Input.GetAxis("Mouse Y") * 400f * Time.deltaTime;

        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x - vertical, transform.rotation.eulerAngles.y + horizontal, 0f);

        transform.Translate(Vector3.forward * Input.GetAxis("Vertical") * 10f * Time.deltaTime);
    }
}
