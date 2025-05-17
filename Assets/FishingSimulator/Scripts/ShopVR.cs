using UnityEngine;
using UnityEngine.XR;
using TMPro;
using System.Collections.Generic;

public class ShopVR : MonoBehaviour
{
    [Header("Shop Settings")]
    public GameObject shopPanel;
    public PlayerData playerData;
    public TextMeshProUGUI goldText;
    public GameObject Rod;
    public GameObject Level;

    [Header("VR Ray Pointer")]
    public GameObject rightRayPointer; // <- przypisz tu obiekt XR Ray Interactor lub jego dziecko z LineRendererem

    [Header("Shop Values")]
    public int betterRodPrice = 50;

    private bool isShopOpen = false;
    private bool yButtonLastFrame = false;

    void Start()
    {
        shopPanel.SetActive(false);

        if (rightRayPointer != null)
            rightRayPointer.SetActive(false); // Ukryj promień na start
    }

    void Update()
    {
        // Pobierz lewy kontroler
        var leftHandDevices = new List<InputDevice>();
        InputDevices.GetDevicesAtXRNode(XRNode.LeftHand, leftHandDevices);

        if (leftHandDevices.Count > 0 && leftHandDevices[0].TryGetFeatureValue(CommonUsages.secondaryButton, out bool yPressed))
        {
            if (yPressed && !yButtonLastFrame)
            {
                ToggleShop(); // Przełącz sklep i promień
            }

            yButtonLastFrame = yPressed;
        }
    }

    void ToggleShop()
    {
        isShopOpen = !isShopOpen;
        shopPanel.SetActive(isShopOpen);
        Rod.SetActive(!isShopOpen);
        Level.SetActive(!isShopOpen);

        UpdateGoldText();

        if (rightRayPointer != null)
            rightRayPointer.SetActive(isShopOpen);

        Debug.Log("Shop toggled: " + (isShopOpen ? "ON" : "OFF"));
    }

    public void BuyBetterRod()
    {
        if (playerData.hasBetterRod)
        {
            Debug.Log("Już masz lepszą wędkę!");
            return;
        }

        if (playerData.money >= betterRodPrice)
        {
            playerData.SpendMoney(betterRodPrice);
            playerData.hasBetterRod = true;

            Debug.Log("Kupiono lepszą wędkę!");
            UpdateGoldText();
        }
        else
        {
            Debug.Log("Nie masz wystarczająco pieniędzy!");
        }
    }

    void UpdateGoldText()
    {
        goldText.text = "Money: $" + playerData.money;
    }
}

