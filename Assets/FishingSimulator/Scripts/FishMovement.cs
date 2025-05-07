using UnityEngine;

public class FishMovement : MonoBehaviour
{
    private Quaternion targetRotation;
    private float rotationSpeed = 2f; // Współczynnik płynności
    private Rigidbody rb;
    private bool isTurning = false;
    private Vector3 fishDirection;
    private float fishSpeed = 2f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void SetNewDirection(Vector3 newDirection, float swimSpeed)
    {
        targetRotation = Quaternion.LookRotation(newDirection);
        fishDirection = newDirection;
        fishSpeed = swimSpeed;
        isTurning = true;
        rb.velocity = Vector3.zero;
        // Wymuś rotację X i Z na 0
        Vector3 eulerRotation = transform.rotation.eulerAngles;
        eulerRotation.x = 0f;
        eulerRotation.z = 0f;
        transform.rotation = Quaternion.Euler(eulerRotation);
    }

    void FixedUpdate()
    {
        if (isTurning)
        {
            // Płynnie obracaj rybę w kierunku targetRotation
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                Time.deltaTime * rotationSpeed
            );


            if (Quaternion.Angle(transform.rotation, targetRotation) < 10f)
            {
                isTurning = false; // Obrót zakończony
                rb.velocity = fishDirection * fishSpeed; // Nadaj rybie prędkość w nowym kierunku
            }
        }
    }
}

