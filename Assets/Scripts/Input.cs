using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class CameraMovement : MonoBehaviour
{
     public Transform player;
    public float rotationSpeed = 45f;

    void Update()
    {
        // Pobieranie osi od kontrolerów VR (np. od joysticka)
        Vector2 primaryAxis = InputTracking.GetLocalPosition(XRNode.RightHand);
        
        // Obracanie gracza
        if (primaryAxis.x != 0)
        {
            player.Rotate(Vector3.up, primaryAxis.x * rotationSpeed * Time.deltaTime);
        }

        // Obsługa klikania (przykład dla triggera)
        if (Input.GetButtonDown("Fire1"))
        {
            Debug.Log("Przycisk naciśnięty!");
            // Dodaj tutaj logikę dla kliknięcia
        }
    }
}
