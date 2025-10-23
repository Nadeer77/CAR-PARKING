using UnityEngine;
using UnityEngine.Events;

public class ParkingSpotChecker : MonoBehaviour
{
    [Header("Car-Tag")]
    public string carTag = "Player";

    [Header("Visual")]
    public Renderer spotRenderer;
    public Color redColor = Color.red;
    public Color greenColor = Color.green;

    [Header("Parking rules")]
    public float velocityThreshold = 0.5f;
    public float angleThreshold = 30f;
    public float requiredSecondsParked = 1.0f;

    public UnityEvent onParked;
    public UnityEvent onUnparked;

    private Material matInstance;
    private float parkedTimer = 0f;
    private bool isParked = false;
    private Collider spotCollider;

    // WheelController reference
    private WheelController wheelController;

    void Start()
    {
        if (spotRenderer == null) spotRenderer = GetComponentInChildren<Renderer>();
        if (spotRenderer == null) { Debug.LogError("Assign Spot Renderer"); return; }
        matInstance = spotRenderer.material;
        SetColor(redColor);

        spotCollider = GetComponent<Collider>();
        if (spotCollider == null) Debug.LogError("Parking spot must have a collider!");
    }

    void OnTriggerStay(Collider other)
    {
        GameObject root = other.transform.root.gameObject;
        if (!root.CompareTag(carTag)) return;

        Rigidbody carRb = root.GetComponent<Rigidbody>();
        if (carRb == null) return;

        // Cache wheel controller
        if (wheelController == null) wheelController = root.GetComponent<WheelController>();
        if (wheelController == null) { Debug.LogError("Car has no WheelController!"); return; }

        // 1) Velocity check
        if (carRb.velocity.magnitude > velocityThreshold)
        {
            ResetParking();
            return;
        }

        // 2) Orientation check
        Vector3 carForward = root.transform.forward;
        Vector3 spotForward = transform.forward;
        float angle = Vector3.Angle(carForward, spotForward);
        if (angle > angleThreshold)
        {
            ResetParking();
            return;
        }

        // 3) Wheel position check
        if (!AllWheelsInside())
        {
            ResetParking();
            return;
        }

        // Passed all checks
        parkedTimer += Time.deltaTime;
        if (!isParked && parkedTimer >= requiredSecondsParked)
        {
            isParked = true;
            SetColor(greenColor);
            onParked?.Invoke();
        }
    }

    void OnTriggerExit(Collider other)
    {
        GameObject root = other.transform.root.gameObject;
        if (!root.CompareTag(carTag)) return;
        ResetParking();
    }

    bool AllWheelsInside()
    {
        if (spotCollider == null || wheelController == null) return false;

        return spotCollider.bounds.Contains(wheelController.frontLeftTransform.position) &&
               spotCollider.bounds.Contains(wheelController.frontRightTransform.position) &&
               spotCollider.bounds.Contains(wheelController.rearLeftTransform.position) &&
               spotCollider.bounds.Contains(wheelController.rearRightTransform.position);
    }

    void ResetParking()
    {
        parkedTimer = 0f;
        if (isParked)
        {
            isParked = false;
            onUnparked?.Invoke();
        }
        SetColor(redColor);
    }

    void SetColor(Color c)
    {
        if (matInstance == null) return;
        if (matInstance.HasProperty("_BaseColor")) matInstance.SetColor("_BaseColor", c);
        else if (matInstance.HasProperty("_Color")) matInstance.SetColor("_Color", c);
        else matInstance.color = c;
    }

    public void PrintParked()
    {
        Debug.Log("Car successfully parked!");
        UIManager.Instance.GameWon();
    }

    public void PrintUnparked()
    {
        Debug.Log("Car left the spot.");
    }
}