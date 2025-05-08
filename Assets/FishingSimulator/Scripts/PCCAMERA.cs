using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCCAMERA : MonoBehaviour
{
    public Transform playerBody; // np. cały obiekt gracza (do obracania lewo-prawo)
    public float mouseSensitivity = 100f;

    private float xRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // Ukrywa i blokuje kursor na środku
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // ograniczenie góra–dół

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f); // obraca kamerę (góra–dół)
        playerBody.Rotate(Vector3.up * mouseX); // obraca gracza (prawo–lewo)
    }
}