using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatController : MonoBehaviour
{
    public Transform player;
    public Transform boatSeat;
    public float maxSpeed = 6f;
    public float acceleration = 2f;
    public float deceleration = 2f;
    public float turnSpeed = 90f; // stopnie na sekundę
    public bool isPlayerOnBoat = false;

    private float currentSpeed = 0f;
    private CharacterController playerController;
    private Camera playerCamera;

    void Start()
    {
        playerController = player.GetComponent<CharacterController>();
        playerCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            if (!isPlayerOnBoat)
                EnterBoat();
            else
                ExitBoat();
        }

        if (isPlayerOnBoat)
        {
            HandleBoatMovement();

            // Trzymanie gracza na siedzeniu
            player.position = boatSeat.position;
            player.rotation = boatSeat.rotation;
        }
    }

    void HandleBoatMovement()
    {
        float input = Input.GetAxis("Vertical");

        // Przyspieszanie
        if (Mathf.Abs(input) > 0.1f)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, input * maxSpeed, acceleration * Time.deltaTime);

            // Obracanie w kierunku kamery
            Vector3 cameraForward = playerCamera.transform.forward;
            cameraForward.y = 0f;
            if (cameraForward != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(cameraForward);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
            }
        }
        else
        {
            // Zwalnianie do zera
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, deceleration * Time.deltaTime);
        }

        // Przesunięcie łódki
        transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);
    }

    void EnterBoat()
    {
        isPlayerOnBoat = true;
        player.position = boatSeat.position;
        player.rotation = boatSeat.rotation;
        if (playerController != null) playerController.enabled = false;
    }

    void ExitBoat()
    {
        isPlayerOnBoat = false;
        player.position = boatSeat.position;
        player.rotation = boatSeat.rotation;
        if (playerController != null) playerController.enabled = true;
        currentSpeed = 0f;
    }
}