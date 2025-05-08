using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class PCFishingROD : MonoBehaviour
{
    public GameObject hook;
    public Slider powerBar;
    public float hookSpeedMultiplier = 14f;
    public float windUpSpeed = 3f;
    public float fishSwimSpeed = 1.5f;
    public float messageDisplayDuration = 5.0f;
    public float battleBarIncreaseSpeed = 0.3f;
    public float battleBarDecreaseSpeed = 0.15f;
    public float battleBarTimeThreshold = 3f;
    public float battleBarUpdateInterval = 3f;
    public GameObject waterPlane;
    public GameObject player_camera;
    public GameObject rodTip;
    public TextMeshProUGUI TMPDistance;
    public TextMeshProUGUI TMPMessage;
    public Image Tutorial;

    public Scrollbar battleBar;
    public GameObject targetBar;

    private bool isCasting = false;
    private bool hookFlying = false;

    private float initialFishDistance = -1;

    private float battleBarLastUpdateTime;
    float battleBarRandomWidth;
    float battleBarCenterOffset;
    private float battleBarTimer;

    private PlayerData playerData;
    public GameObject playerDataObj;

    public GameObject levelCanvas;
    public GameObject audioObj;

    public GameObject alertCanvas;
    private float waterLevelY;
    public Transform positionplayer;

    void Start()
    {
        battleBarLastUpdateTime = battleBarUpdateInterval;
        battleBarTimer = 0f;
        playerData = playerDataObj.GetComponent<PlayerData>();
        playerData.LoadData();
        UpdateLevelInfo();
        if (waterPlane != null)
        {
            waterLevelY = waterPlane.transform.position.y;
        }
    }

void Update()
{
    GameObject caughtFish = hook.GetComponent<Hook>().attachedFish;

    // Walka z rybą
    if (caughtFish != null)
    {
        ShowDistance();

        Vector3 directionFish = (caughtFish.transform.position - rodTip.transform.position).normalized;
        caughtFish.transform.LookAt(rodTip.transform.position);
        caughtFish.transform.Translate(directionFish * fishSwimSpeed * Time.deltaTime, Space.World);
        hook.transform.position = caughtFish.transform.position;

        float fishDistance = Vector3.Distance(hook.transform.position, rodTip.transform.position);

        if (initialFishDistance == -1)
        {
            initialFishDistance = fishDistance;
            ShowMessage("    Fish got hooked!", new Color(255, 165, 0), messageDisplayDuration);
            battleBar.gameObject.SetActive(true);
        }

        // Battle bar update
        if (battleBar.IsActive())
        {
            battleBarLastUpdateTime += Time.deltaTime;
            if (battleBarLastUpdateTime >= battleBarUpdateInterval)
            {
                battleBarRandomWidth = UnityEngine.Random.Range(0.2f, 0.4f);
                battleBarCenterOffset = UnityEngine.Random.Range(battleBarRandomWidth / 2, 1 - battleBarRandomWidth / 2) - 0.5f;

                RectTransform battleBarRect = battleBar.GetComponent<RectTransform>();
                RectTransform targetBarRect = targetBar.GetComponent<RectTransform>();

                Vector2 anchoredPosition = targetBarRect.anchoredPosition;
                anchoredPosition.x = battleBarRect.rect.width * battleBarCenterOffset;
                targetBarRect.anchoredPosition = anchoredPosition;

                Vector2 sizeDelta = targetBarRect.sizeDelta;
                sizeDelta.x = battleBarRect.rect.width * battleBarRandomWidth;
                targetBarRect.sizeDelta = sizeDelta;

                battleBarLastUpdateTime = 0f;
            }

            float minValue = battleBarCenterOffset + 0.5f - battleBarRandomWidth / 2;
            float maxValue = battleBarCenterOffset + 0.5f + battleBarRandomWidth / 2;

            if (battleBar.value < minValue || battleBar.value > maxValue)
            {
                battleBarTimer += Time.deltaTime;
                alertCanvas.gameObject.SetActive(true);
            }
            else
            {
                Vector3 targetPosition = rodTip.transform.position;
                targetPosition.y = waterPlane.transform.position.y;

                directionFish = (targetPosition - caughtFish.transform.position).normalized;
                caughtFish.transform.LookAt(targetPosition);
                caughtFish.transform.Translate(directionFish * fishSwimSpeed * 3 * Time.deltaTime, Space.World);
                hook.transform.position = caughtFish.transform.position;

                fishDistance = Vector3.Distance(hook.transform.position, rodTip.transform.position);
                alertCanvas.gameObject.SetActive(false);
            }
        }

        if (fishDistance < 20f)
        {
            string message = "Success!";
            int previousLevel = playerData.level;
            caughtFish.SetActive(false);
            hook.GetComponent<Hook>().attachedFish = null;
            EndFishing();

            playerData.AddExperience(10);
            UpdateLevelInfo();
            int currentLevel = playerData.level;

            message += currentLevel > previousLevel ? "\nLevel Up!" : "\n+10 Exp";
            ShowMessage(message, new Color(50, 205, 50), messageDisplayDuration);

            Transform catchFishObjTransform = audioObj.transform.Find("CatchFish");
            if (catchFishObjTransform != null)
            {
                catchFishObjTransform.GetComponent<AudioSource>().Play();
            }
        }
        else if (fishDistance > initialFishDistance + 200f || battleBarTimer > battleBarTimeThreshold)
        {
            ShowMessage("Failed!", new Color(255, 69, 0), messageDisplayDuration);
            caughtFish.SetActive(false);
            hook.GetComponent<Hook>().attachedFish = null;
            EndFishing();
        }
    }

    // Rzucanie wędki
    if (Input.GetMouseButton(0) && !battleBar.IsActive())
    {
        if (!isCasting)
        {
            isCasting = true;
            InitPowerBar();
        }
        Casting();
    }
    else if (Input.GetMouseButtonUp(0) && !battleBar.IsActive())
    {
        if (isCasting)
        {
            EndCasting();
        }

        ShowDistance();

        if (hookFlying && hook.transform.position.y <= waterPlane.transform.position.y - 10f)
        {
            hook.GetComponent<Rigidbody>().useGravity = false;
            hook.GetComponent<Rigidbody>().velocity = Vector3.zero;
            hookFlying = false;

            float distanceFromLakeCenter = Vector3.Distance(hook.transform.position, waterPlane.transform.position);
            float lakeRadius = waterPlane.GetComponent<MeshRenderer>().bounds.size.x / 2f;
            if (distanceFromLakeCenter > (lakeRadius - 70))
            {
                EndFishing();
                ShowMessage("  Missed the water! Try again.!", new Color(1f, 0f, 0f), messageDisplayDuration);
                return;
            }
        }
    }

    // Zwijanie (prawo)
    if (Input.GetMouseButton(1))
    {
        if (caughtFish != null)
        {
            windUpSpeed = playerData.level * 3f;
            Vector3 windUpDirection = (rodTip.transform.position - caughtFish.transform.position).normalized;
            if (caughtFish.transform.position.y > waterPlane.transform.position.y - 5f)
            {
                windUpDirection.y = 0;
            }
            caughtFish.transform.LookAt(rodTip.transform.position);
            caughtFish.transform.Translate(windUpDirection * windUpSpeed * Time.deltaTime, Space.World);
            hook.transform.position = caughtFish.transform.position;

            if (battleBar.value < 1)
            {
                battleBar.value += battleBarIncreaseSpeed * Time.deltaTime;
            }
        }
    }
    else
    {
        if (caughtFish != null && battleBar.value > 0)
        {
            battleBar.value -= battleBarDecreaseSpeed * Time.deltaTime;
        }
    }

    if (Input.GetKeyDown(KeyCode.E)) EndFishing();
    if (Input.GetKeyDown(KeyCode.T)) Tutorial.gameObject.SetActive(!Tutorial.gameObject.activeSelf);

    if (transform.position.y < waterLevelY) PlayerFellOffBoat();
}


    private void PlayerFellOffBoat()
    {
        Vector3 savedPosition = playerData.GetSavedPlayerPosition();
        positionplayer.position = savedPosition;
        ShowMessage("You fell off the boat!", new Color(1f, 0f, 0f), messageDisplayDuration);
    }

    void InitPowerBar()
    {
        powerBar.gameObject.SetActive(true);
        powerBar.value = 0f;
    }

    void Casting()
    {
        powerBar.value += Time.deltaTime;
    }

    void EndCasting()
    {
        isCasting = false;
        powerBar.gameObject.SetActive(false);
        hook.transform.position = rodTip.transform.position;
        hook.SetActive(true);

        FishingLine fishingLine = rodTip.GetComponent<FishingLine>();
        fishingLine.enabled = true;
        rodTip.GetComponent<LineRenderer>().enabled = true;

        float hookSpeed = (powerBar.value + 0.7f) * hookSpeedMultiplier;
        hook.GetComponent<Rigidbody>().velocity = player_camera.transform.forward * hookSpeed;
        hook.GetComponent<Rigidbody>().useGravity = true;
        hookFlying = true;

        TMPDistance.gameObject.SetActive(true);
    }

    void EndFishing()
    {
        hook.SetActive(false);
        FishingLine fishingLine = rodTip.GetComponent<FishingLine>();
        fishingLine.enabled = false;
        rodTip.GetComponent<LineRenderer>().enabled = false;
        TMPDistance.gameObject.SetActive(false);
        initialFishDistance = -1;
        battleBar.gameObject.SetActive(false);
        battleBarTimer = 0;
        battleBarLastUpdateTime = battleBarUpdateInterval;
        alertCanvas.gameObject.SetActive(false);
    }

    void ShowDistance()
    {
        float distance = Vector3.Distance(hook.transform.position, rodTip.transform.position);
        TMPDistance.text = "Distance: " + Math.Round(distance, 2);
    }

    public void ShowMessage(string message, Color color, float duration)
    {
        TMPMessage.color = color;
        TMPMessage.SetText(message);
        TMPMessage.gameObject.SetActive(true);
        StartCoroutine(HideMessage(duration));
    }

    private IEnumerator HideMessage(float duration)
    {
        yield return new WaitForSeconds(duration);
        TMPMessage.gameObject.SetActive(false);
    }

    public void UpdateLevelInfo()
    {
        TextMeshProUGUI levelInfo = levelCanvas.GetComponentInChildren<TextMeshProUGUI>();
        TextMeshProUGUI expInfo = levelCanvas.GetComponentsInChildren<TextMeshProUGUI>()[1];
        Slider expSlider = levelCanvas.GetComponentInChildren<Slider>();
        float maxExp = 100 + (playerData.level - 1) * 10;
        levelInfo.text = "LEVEL " + playerData.level;
        expInfo.text = playerData.experience + "/" + maxExp;
        expSlider.value = (float)playerData.experience / maxExp;
    }

    void OnApplicationQuit()
    {
        playerData.SaveData();
    }
}
