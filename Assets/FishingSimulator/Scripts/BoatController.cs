using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatController : MonoBehaviour
{
    public Transform player;                // Referencja do gracza
    public Transform boatSeat;              // Miejsce, gdzie gracz „siada” na łódce
    public float moveSpeed = 5f;
    public float turnSpeed = 50f;
    public bool isPlayerOnBoat = false;

    private CharacterController playerController;


    void Start()
    {
        playerController = player.GetComponent<CharacterController>();

    }

    void Update()
    {
        // Wejście/wyjście z łódki
        if (Input.GetKeyDown(KeyCode.L))
        {
            if (!isPlayerOnBoat)
            {
                EnterBoat();
            }
            else
            {
                ExitBoat();
            }
        }

        // Sterowanie łódką
        if (isPlayerOnBoat)
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            // Ruch do przodu/tyłu
            transform.Translate(Vector3.forward * vertical * moveSpeed * Time.deltaTime);

            // Obrót
            transform.Rotate(Vector3.up * horizontal * turnSpeed * Time.deltaTime);
        }
    }

    void EnterBoat()
    {
        isPlayerOnBoat = true;

        // Przenieś gracza na siedzenie
        player.position = boatSeat.position;
        player.rotation = boatSeat.rotation;

        player.SetParent(transform); // gracz staje się dzieckiem łódki

        // Wyłącz kontrolę gracza
        if (playerController != null) playerController.enabled = false;

    }

    void ExitBoat()
    {
        isPlayerOnBoat = false;

        player.SetParent(null); // ← odłączenie

        // Odsuń gracza obok łódki
        player.position = transform.position + transform.right * 2f;

        // Włącz kontrolę gracza
        if (playerController != null) playerController.enabled = true;

    }
}