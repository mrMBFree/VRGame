using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishGenerator : MonoBehaviour
{

    public GameObject fishPrefab;
    public int maxFishCount = 10;
    private int fishCount = 0;

    public float swimSpeed = 2f;
    private bool fishGenerationAllowed = false; // Flaga do kontroli generowania ryb

    void Start()
    {
        InvokeRepeating("ChangeFishDirection", 3f, 8f);

        // Opóźnione włączenie generowania ryb o 10 sekund
        Invoke("EnableFishGeneration", 5f);
    }


    // Update is called once per frame
    void Update()
    {
        if (!fishGenerationAllowed)
            return; // Jeśli generowanie ryb jest zablokowane, wyjdź z Update
        GameObject player = GameObject.Find("Main Camera");
        float distance = Vector3.Distance(player.transform.position, transform.position);
        MeshRenderer waterMeshRenderer = this.GetComponent<MeshRenderer>();
        float lakeRadius = waterMeshRenderer.bounds.size.x / 2f;
        if (distance < lakeRadius + 40f && fishCount < maxFishCount)
        {
            float generateRadius = lakeRadius - 20f;
            Vector3 randomPos = new Vector3(transform.position.x + Random.Range(-generateRadius, generateRadius),
                transform.position.y - 10f,
                transform.position.z + Random.Range(-generateRadius, generateRadius));

            Vector3 randomDirection = Random.insideUnitSphere.normalized; // Określamy kierunek ruchu
            randomDirection.y = 0; // Opcjonalnie ograniczamy ruch tylko w płaszczyźnie poziomej
            Quaternion targetRotation = Quaternion.LookRotation(randomDirection); // Tworzymy rotację zgodną z kierunkiem

            GameObject fish = Instantiate(fishPrefab, randomPos, targetRotation);
            fish.GetComponent<Rigidbody>().velocity = randomDirection * swimSpeed;
            fishCount++;
        }
    }

    void ChangeFishDirection()
    {
        MeshRenderer waterMeshRenderer = this.GetComponent<MeshRenderer>();
        float lakeRadius = waterMeshRenderer.bounds.size.x / 2f;
        GameObject[] fishes = GameObject.FindGameObjectsWithTag("Fish");
        foreach (GameObject fish in fishes)
        {
            float distance = Vector3.Distance(fish.transform.position, transform.position);
            if (fish.transform.position.y  > transform.position.y - 5f || distance > (lakeRadius-80))
            {
                fish.transform.position = new Vector3(transform.position.x + Random.Range(-150f, 150f),
                    transform.position.y - 10f,
                    transform.position.z + Random.Range(-150f, 150f));
            }
            //Vector3 randomDirection2 = Random.insideUnitSphere.normalized; // Określamy kierunek ruchu
            //randomDirection2.y = 0f; // Opcjonalnie ograniczamy ruch tylko w płaszczyźnie poziomej
                                     //Vector3 newDirection = new Vector3(Random.Range(-10f, 10f), 0f, Random.Range(-10f, 10f)).normalized;
                                     // Obecny kierunek ryby
            Vector3 currentDirection = fish.transform.forward;
            currentDirection.y = 0;

            // Losowy kąt w zakresie [-maxAngle, maxAngle]
            float randomAngle = Random.Range(-50, 50);

            // Obrót wokół osi Y (tylko w płaszczyźnie poziomej)
            Quaternion rotation = Quaternion.Euler(0, randomAngle, 0);
            //float rotationY = fish.GetComponent<Transform>().rotation.eulerAngles.y; 
            // Nowy kierunek jako wynik rotacji
            Vector3 newDirection = rotation * currentDirection;
            newDirection.y = 0f;
            // Ponownie normalizuj wektor, aby zachować stałą prędkość
            newDirection = newDirection.normalized;
            FishMovement fishMovement = fish.GetComponent<FishMovement>();
            if (fishMovement != null)
            {
                fishMovement.SetNewDirection(newDirection, swimSpeed);
            }
            //fish.GetComponent<Rigidbody>().velocity = newDirection * swimSpeed;

        }
    }
    void EnableFishGeneration()
    {
        fishGenerationAllowed = true; // Włącz generowanie ryb po 5 sekundach
    }
}
