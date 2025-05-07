using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FishingTrigger : MonoBehaviour
{
    public Transform player;               // Transform gracza
    public float minLeftRotation = -120f;  // Minimalny kąt dla lewej strony
    public float maxLeftRotation = -55f;   // Maksymalny kąt dla lewej strony
    public float minRightRotation = 40f;   // Minimalny kąt dla prawej strony
    public float maxRightRotation = 120f;  // Maksymalny kąt dla prawej strony
    public List<GameObject> fishIcons;     // Lista GameObjectów reprezentujących ryby
    public float fishingDuration = 10f;    // Czas, po jakim zostanie wybrana ryba

    private bool canFishLeft = false;      // Czy można łowić z lewej strony
    private bool canFishRight = false;     // Czy można łowić z prawej strony
    private bool isFishing = false;        // Czy gracz jest w trybie łowienia
    private Vector3 originalPosition;      // Pozycja gracza przed rozpoczęciem łowienia
    private Coroutine fishingCoroutine;    // Przechowuje referencję do Coroutine łowienia
    private GameObject currentFishIcon;    // Aktualnie wybrana ikona ryby

    void Start()
    {
        // Zachowaj pierwotną pozycję gracza
        originalPosition = player.position;

        // Ukryj wszystkie ikony ryb na początku
        foreach (GameObject fishIcon in fishIcons)
        {
            fishIcon.SetActive(false);
        }
    }

    void Update()
    {
        // Sprawdź, czy gracz nie jest już w trybie łowienia
        if (!isFishing)
        {
            // Odczytaj bieżącą rotację Y gracza
            float currentYRotation = player.eulerAngles.y;

            // Znormalizuj wartość Y do zakresu (-180, 180) dla ułatwienia sprawdzania warunków
            if (currentYRotation > 180)
                currentYRotation -= 360;

            // Sprawdź, czy gracz patrzy w lewo
            if (currentYRotation >= minLeftRotation && currentYRotation <= maxLeftRotation)
            {
                if (!canFishLeft)
                {
                    Debug.Log("Patrzysz w lewo. Możesz zacząć łowić.");
                    canFishLeft = true;
                    canFishRight = false;
                }

                // Po naciśnięciu przycisku Z zmień pozycję gracza i rozpocznij łowienie
                if (Input.GetKeyDown(KeyCode.Z))
                {
                    player.position = new Vector3(208f, player.position.y, player.position.z);
                    Debug.Log("Zmieniono pozycję na 208 (lewa strona). Rozpoczęto łowienie.");
                    isFishing = true;
                    StartFishing();
                }
            }
            // Sprawdź, czy gracz patrzy w prawo
            else if (currentYRotation >= minRightRotation && currentYRotation <= maxRightRotation)
            {
                if (!canFishRight)
                {
                    Debug.Log("Patrzysz w prawo. Możesz zacząć łowić.");
                    canFishRight = true;
                    canFishLeft = false;
                }

                // Po naciśnięciu przycisku Z zmień pozycję gracza i rozpocznij łowienie
                if (Input.GetKeyDown(KeyCode.Z))
                {
                    player.position = new Vector3(212f, player.position.y, player.position.z);
                    Debug.Log("Zmieniono pozycję na 212 (prawa strona). Rozpoczęto łowienie.");
                    isFishing = true;
                    StartFishing();
                }
            }
            else
            {
                // Resetuj flagi, gdy gracz nie patrzy w lewo ani w prawo
                canFishLeft = false;
                canFishRight = false;
            }
        }
        else
        {
            // Jeżeli gracz jest w trybie łowienia i naciśnie klawisz X
            if (Input.GetKeyDown(KeyCode.X))
            {
                // Zatrzymaj łowienie i wróć na oryginalną pozycję
                StopFishing();
                player.position = new Vector3(210f, player.position.y, player.position.z);
                Debug.Log("Wrócono na pozycję 210. Zakończono łowienie.");
                isFishing = false;
            }
        }
    }

    void StartFishing()
    {
        // Rozpocznij coroutine łowienia ryby
        fishingCoroutine = StartCoroutine(FishingProcess());
    }

    void StopFishing()
    {
        // Zatrzymaj proces łowienia, jeśli trwa
        if (fishingCoroutine != null)
        {
            StopCoroutine(fishingCoroutine);
        }

        // Ukryj aktualną ikonę ryby, jeśli istnieje
        if (currentFishIcon != null)
        {
            currentFishIcon.SetActive(false);
            currentFishIcon = null;
        }
    }

    IEnumerator FishingProcess()
    {
        Debug.Log("Rozpoczęto proces łowienia...");

        // Czekaj określony czas na złowienie ryby
        yield return new WaitForSeconds(fishingDuration);

        // Losuj rybę z listy
        int randomFishIndex = Random.Range(0, fishIcons.Count);
        currentFishIcon = fishIcons[randomFishIndex];

        // Pokaż wylosowaną rybę (GameObject)
        currentFishIcon.SetActive(true);

        Debug.Log("Złowiłeś rybę: " + currentFishIcon.name);
    }
}