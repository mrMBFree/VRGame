using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemporaryPlayeScript : MonoBehaviour
{
    public float mouseSensitivity = 2f;
    public GameObject boat; // przypiszesz tutaj swoją łódkę
    public float movementSpeed = 5f;

    private bool isControllingBoat = false;
    private Transform originalParent;
    private float xRotation = 0f;

    private void Start()
    {
        originalParent = transform.parent; // zapamiętaj oryginalnego rodzica
    }

    private void Update()
    {
        Look();
        HandleBoatControlToggle();
        if (!isControllingBoat)
        {
            Move();
        }
    }

    private void Look()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        if (transform.parent != null)
            transform.parent.Rotate(Vector3.up * mouseX);
        else
            transform.Rotate(Vector3.up * mouseX);
    }

    private void Move()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        transform.position += move * movementSpeed * Time.deltaTime;
    }

    private void HandleBoatControlToggle()
    {
        if (Input.GetKeyDown(KeyCode.Return)) // Enter
        {
            isControllingBoat = !isControllingBoat;

            if (isControllingBoat)
            {
                transform.SetParent(boat.transform); // dziecko łódki
            }
            else
            {
                transform.SetParent(originalParent); // wracasz do pierwotnego rodzica
            }
        }
    }
}