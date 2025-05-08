using UnityEngine;
using TMPro;

public class Shop : MonoBehaviour
{
    public GameObject shopPanel;            // Panel sklepu
    public PlayerData playerData;           // PlayerData
    public TextMeshProUGUI goldText;        // Tekst aktualnego gold
    public GameObject Rod;                  // Obiekt Rod (wędka)
    public GameObject Level;                // Obiekt Level (poziom)

    public int betterRodPrice = 200;        // Cena lepszej wędki

    void Start()
    {
        shopPanel.SetActive(false);         // Ukryj panel sklepu na starcie
        Cursor.lockState = CursorLockMode.Locked; // Zablokuj kursor
        Cursor.visible = false;             // Ukryj kursor
    }

    void Update()
    {
        // Jeśli naciśniemy G, przełączamy stan sklepu
        if (Input.GetKeyDown(KeyCode.G))
        {
            Debug.Log("Naciśnięto G!");

            bool isActive = shopPanel.activeSelf;
            shopPanel.SetActive(!isActive); // Pokaż/ukryj sklep
            UpdateGoldText();               // Zaktualizuj gold

            // Ukrywamy/wyświetlamy Rod i Level
            Rod.SetActive(!shopPanel.activeSelf);
            Level.SetActive(!shopPanel.activeSelf);

            // Sterowanie widocznością i aktywnością kursora
            if (shopPanel.activeSelf)
            {
                Cursor.lockState = CursorLockMode.None; // Odblokuj kursor
                Cursor.visible = true; // Pokaż kursor
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked; // Zablokuj kursor
                Cursor.visible = false; // Ukryj kursor
            }
        }
    }

    // Funkcja do zakupu lepszej wędki
    public void BuyBetterRod()
    {
        if (playerData.hasBetterRod)  // Sprawdzenie, czy już mamy lepszą wędkę
        {
            Debug.Log("Już masz lepszą wędkę!");
            return;
        }

        if (playerData.money >= betterRodPrice) // Jeśli gracz ma wystarczająco pieniędzy
        {
            playerData.SpendMoney(betterRodPrice); // Odejmujemy pieniądze
            playerData.hasBetterRod = true; // Ustawiamy, że gracz ma lepszą wędkę

            Debug.Log("Kupiono lepszą wędkę!");
            UpdateGoldText(); // Zaktualizowanie UI z nową ilością pieniędzy

            // Możesz tutaj dodać jakąś animację lub efekt dźwiękowy, jeśli chcesz
        }
        else
        {
            Debug.Log("Nie masz wystarczająco pieniędzy!");
        }
    }

    // Funkcja do aktualizacji tekstu gold
    void UpdateGoldText()
    {
        goldText.text = "Money: $" + playerData.money;
    }
}
