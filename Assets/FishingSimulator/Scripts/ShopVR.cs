using UnityEngine;
using UnityEngine.XR;
using TMPro;
using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit;

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

      //  if (rightRayPointer != null)
       //     rightRayPointer.SetActive(false); // Ukryj promień na start
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
        bool isActive = shopPanel.activeSelf;
        isShopOpen = !isShopOpen;
        shopPanel.SetActive(!isActive);
        Rod.SetActive(!isShopOpen);
        Level.SetActive(!isShopOpen);
        EnableRayOnly();

        UpdateGoldText();

        //if (rightRayPointer != null)
        //    rightRayPointer.SetActive(isShopOpen);

        //Debug.Log("Shop toggled: " + (isShopOpen ? "ON" : "OFF"));
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
    void EnableRayOnly()
    {
        // Wyłącz wszystkie inne komponenty (jeśli trzeba) lub zostaw aktywne tylko potrzebne
        // Włącz XR Ray Interactor
        XRRayInteractor rayInteractor = rightRayPointer.GetComponent<XRRayInteractor>();
        if (rayInteractor != null)
        {
            rayInteractor.enabled = true;
        }

        // Włącz Line Renderer
        LineRenderer lineRenderer = rightRayPointer.GetComponent<LineRenderer>();
        if (lineRenderer != null)
        {
            lineRenderer.enabled = true;
        }

        // Możesz też opcjonalnie wyłączyć inne interaktory (np. Direct Interactor)
        XRDirectInteractor directInteractor = rightRayPointer.GetComponent<XRDirectInteractor>();
        if (directInteractor != null)
        {
            directInteractor.enabled = false;
        }
    }
}

