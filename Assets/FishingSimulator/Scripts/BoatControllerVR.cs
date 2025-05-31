using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine;
using UnityEngine.XR;
using Unity.VisualScripting;
using System.Collections;

public class VROarRowing : MonoBehaviour
{
    [Header("Boat & Player")]
    public Transform boatTransform;
    public Transform seatTransform;
    public Transform playerTransform;

    [Header("XR Locomotion")]
    public ActionBasedContinuousMoveProvider moveProvider;

    [Header("Oar Setup")]
    public Transform leftOarPivot;
    public Transform rightOarPivot;
    public Transform leftController;
    public Transform rightController;

    [Header("Rowing Parameters")]
    public float movementForce = 5f;
    public float rotationStrength = 0.008f;
    public float detectionThreshold = 0.005f;
    public float elipseHorizontalRadius = 0.7f;
    public float elipseVerticalRadius = 0.55f;

    private Vector3 targetPosition;
    private Vector3 targetPosition2;
    private Quaternion targetRotation;
    private float accumulatedYaw = 0f;
    private bool shouldMove = false;
    private bool leftpivot = false;
    private bool wPrawe = false;
    private bool wLewe = false;
    private float pisLeft = 0f;
    private bool movedByLeft = false;
    private bool movedByRight = false;
    private bool korutyna = false;

    [Header("Pivot Settings")]
    public Vector3 pivotLeftRowingPos = new Vector3(-28f, -6f, 32f);
    public Vector3 pivotRightRowingPos = new Vector3(16f, -6f, 32f);
    public Vector3 pivotRotation = new Vector3(0f, 0f, 0f);
    public Vector3 pivotRestingPos = Vector3.zero;

    private Vector3 prevLeftLocalPos;
    private Vector3 prevRightLocalPos;

    private bool isRowingMode = false;
    private bool aButtonLastFrame = false;

    private GameObject fishingRod;

    void Start()
    {
        fishingRod = GameObject.FindGameObjectWithTag("FishingRod");
        prevLeftLocalPos = leftOarPivot.InverseTransformPoint(leftController.position);
        prevRightLocalPos = rightOarPivot.InverseTransformPoint(rightController.position);
        SetRowingMode(false);
        targetPosition = boatTransform.position;
        targetRotation = boatTransform.rotation;
        //targetPosition = Vector3.zero;
        //targetRotation = Quaternion.identity;
        // Vector3 currentEuler = leftOarPivot.rotation.eulerAngles;
        // leftOarPivot.rotation = Quaternion.Euler(currentEuler.x, currentEuler.y, 90f);
    }

    private IEnumerator ResetOarFlagsAfterDelay(float delay)
    {
        korutyna = true;
        yield return new WaitForSeconds(delay);
        movedByLeft = false;
        movedByRight = false;
        korutyna = false;
    }

    void Update()
    {
        bool aButtonPressed = false;
        var rightHand = new List<InputDevice>();
        InputDevices.GetDevicesAtXRNode(XRNode.RightHand, rightHand);
        if (rightHand.Count > 0 && rightHand[0].TryGetFeatureValue(CommonUsages.secondaryButton, out bool value))
        {
            aButtonPressed = value;
        }

        if (aButtonPressed && !aButtonLastFrame)
        {


            isRowingMode = !isRowingMode;
            SetRowingMode(isRowingMode);
        }
        aButtonLastFrame = aButtonPressed;

        if (!isRowingMode) return;
        leftpivot = true;
        HandleOar(leftController, leftOarPivot, ref prevLeftLocalPos, true);
        leftpivot = false;
        HandleOar(rightController, rightOarPivot, ref prevRightLocalPos, false);

        if (shouldMove)
        {
            //targetPosition = boatTransform.forward * movementForce * 4;
            //boatTransform.position = Vector3.Lerp(boatTransform.position, targetPosition, Time.deltaTime * 4f);
            // Jeśli oba wiosła poruszyły łódź w tej samej klatce – NIE obracaj
            if (!(movedByLeft && movedByRight))
            {

                boatTransform.rotation = Quaternion.Slerp(boatTransform.rotation, targetRotation, Time.deltaTime * 0.4f);
            }
            else
            {
                //targetPosition += boatTransform.forward * movementForce * 4;
                boatTransform.position = Vector3.Lerp(boatTransform.position, targetPosition, Time.deltaTime * 2f);
            }
            if (Vector3.Distance(boatTransform.position, targetPosition) < 0.005f)
                shouldMove = false;
            if (!korutyna)
            {
                StartCoroutine(ResetOarFlagsAfterDelay(3f)); // lub inna wartość
            }
        }
        // Resetujemy flagi na koniec klatki


    }

    void SetRowingMode(bool enable)
    {
        CharacterController cc = playerTransform.GetComponent<CharacterController>();
        if (enable)
        {
            targetPosition = boatTransform.position;
            targetRotation = boatTransform.rotation;

            if (cc != null)
            {
                cc.enabled = false;
                cc.detectCollisions = !enable; // <- to dodaj
            }


            // 2️ Potem ustaw dokładną pozycję i rotację gracza
            playerTransform.position = seatTransform.position;
            playerTransform.rotation = seatTransform.rotation;
            if (cc != null) cc.enabled = true;

            playerTransform.SetParent(boatTransform, true);

            leftOarPivot.localPosition = pivotLeftRowingPos;
            leftOarPivot.localRotation = Quaternion.Euler(pivotRotation);
            rightOarPivot.localPosition = pivotRightRowingPos;
            rightOarPivot.localRotation = Quaternion.Euler(pivotRotation);

            prevLeftLocalPos = leftOarPivot.InverseTransformPoint(leftController.position);
            prevRightLocalPos = rightOarPivot.InverseTransformPoint(rightController.position);
        }
        else
        {
            playerTransform.SetParent(null);
            if (cc != null) cc.enabled = true;
            leftOarPivot.localPosition = pivotRestingPos;
            leftOarPivot.localRotation = Quaternion.identity;
            rightOarPivot.localPosition = pivotRestingPos;
            rightOarPivot.localRotation = Quaternion.identity;

            playerTransform.SetParent(null);
        }

        if (moveProvider != null)
            moveProvider.enabled = !enable;
        // 🔁 Włącz/wyłącz wędkę
        if (fishingRod != null)
            fishingRod.SetActive(!enable);
    }

    void HandleOar(Transform controller, Transform oarPivot, ref Vector3 prevLocalPos, bool isLeft)
    {
        Vector3 currentLocalPos = oarPivot.InverseTransformPoint(controller.position);

        //Vector3 clampedLocal = new Vector3(
        //    Mathf.Clamp(currentLocalPos.x, -elipseHorizontalRadius, elipseHorizontalRadius),
        //    Mathf.Clamp(currentLocalPos.y, -elipseVerticalRadius, elipseVerticalRadius),
        //    0f
        //);
        //Vector3 worldPos = oarPivot.TransformPoint(clampedLocal);

       // if (oarPivot.childCount > 0)
       // {
       //     Transform oar = oarPivot.GetChild(0);
       //     oar.position = worldPos;

            // ROTACJA WIOSŁA DO KOŁA - WYZNACZ POZYCJĘ KONTROLERA W LOKALNEJ PRZESTRZENI PIVOTA
            Vector3 localToController = oarPivot.InverseTransformPoint(controller.position);
            float angleZ = Mathf.Atan2(localToController.y, localToController.x) * Mathf.Rad2Deg;
        if (leftpivot) angleZ += 100f;
        //oarPivot.rotation = Quaternion.Euler(0f, 0f, angleZ);
        //}
        // ✨ Nowość: obrót pivotu tak, aby jego niebieska strzałka (oś Z) patrzyła "tyłem" na kontroler
        Vector3 directionToController = controller.position - oarPivot.position;
        Quaternion lookRot = Quaternion.LookRotation(-directionToController, Vector3.up);

        // Tylko rotacja X i Y – Z zostaje bez zmian (zachowujemy efekt, który miałem wcześniej)
        Vector3 euler = lookRot.eulerAngles;
        Quaternion targetRotation = Quaternion.Euler(euler.x, euler.y, angleZ);
        // Quaternion targetRotation = Quaternion.Euler(euler.x, euler.y, oarPivot.rotation.eulerAngles.z);
         oarPivot.rotation = Quaternion.Slerp(oarPivot.rotation, targetRotation, Time.deltaTime * 5f);

        //Debug.DrawLine(oarPivot.position, oarPivot.position + oarPivot.forward * 0.5f, Color.blue);
        //Debug.DrawLine(oarPivot.position, oarPivot.position + oarPivot.right * 0.5f, Color.red);
        //Debug.DrawLine(oarPivot.position, oarPivot.position + oarPivot.up * 0.5f, Color.green);

        float localXDelta = prevLocalPos.x - currentLocalPos.x;
        if (localXDelta < -detectionThreshold)
        {
            ApplyRowingForce(isLeft);

        }

        prevLocalPos = currentLocalPos;
    }

    void ApplyRowingForce(bool isLeft)
    {
       // Vector3 forward = boatTransform.forward.normalized;
       // Vector3 newPos = boatTransform.position + forward * movementForce;

       // newPos.y = boatTransform.position.y;
       // targetPosition = boatTransform.position + boatTransform.forward * movementForce;
        targetPosition = boatTransform.position + boatTransform.forward * movementForce *10;
        float direction = isLeft ? 1f : -1f;
        accumulatedYaw = direction * rotationStrength;
        targetRotation = boatTransform.rotation * Quaternion.Euler(0f, accumulatedYaw, 0f);
        // Zachowaj oryginalne Y, by nie „opadało”

       // targetPosition = newPos;
        shouldMove = true;
        if (isLeft)
            movedByLeft = true;
        else
            movedByRight = true;

    }
}
