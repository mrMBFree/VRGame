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
        InvokeRepeating("ChangeFishDirection", 3f, 3f);

        // Opóźnione włączenie generowania ryb o 10 sekund
        Invoke("EnableFishGeneration", 10f);
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
            float generateRadius = lakeRadius - 80f;
            Vector3 randomPos = new Vector3(transform.position.x + Random.Range(-generateRadius, generateRadius),
                transform.position.y - 10f,
                transform.position.z + Random.Range(-generateRadius, generateRadius));
            GameObject fish = Instantiate(fishPrefab, randomPos, Quaternion.identity);
            fish.GetComponent<Rigidbody>().velocity = Random.insideUnitSphere.normalized * swimSpeed;
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
            if (fish.transform.position.y  > transform.position.y - 5f || distance > lakeRadius)
            {
                fish.transform.position = new Vector3(transform.position.x + Random.Range(-100f, 100f),
                    transform.position.y - 10f,
                    transform.position.z + Random.Range(-100f, 100f));
            }
            Vector3 newDirection = new Vector3(Random.Range(-10f, 10f), 0f, Random.Range(-10f, 10f)).normalized;
            fish.GetComponent<Rigidbody>().velocity = newDirection * swimSpeed;
        }
    }
    void EnableFishGeneration()
    {
        fishGenerationAllowed = true; // Włącz generowanie ryb po 10 sekundach
    }
}
