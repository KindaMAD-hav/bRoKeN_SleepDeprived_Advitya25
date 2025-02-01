using UnityEngine;

public class PickableObject : MonoBehaviour
{
    public int gearID; // Unique ID for each gear
    private bool isHeld = false;
    private Transform player;

    private void Start()
    {
        SpinObject spinner = GetComponent<SpinObject>();
        if (spinner != null) spinner.enabled = false;
    }

    private void Update()
    {
        if (isHeld && player != null)
        {
            transform.position = player.position + player.forward * 2;
        }
    }

    public void PickUpObject(Transform playerTransform)
    {
        isHeld = true;
        player = playerTransform;
        GetComponent<Rigidbody>().isKinematic = true;

        SpinObject spinner = GetComponent<SpinObject>();
        if (spinner != null) spinner.enabled = false;
    }

    public void PlaceObject(Transform attachPoint)
    {
        if (attachPoint == null)
        {
            Debug.LogError("AttachPoint is null!");
            return;
        }

        isHeld = false;
        player = null;
        transform.position = attachPoint.position;
        transform.rotation = attachPoint.rotation;
        transform.SetParent(attachPoint);

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        SpinObject spinner = GetComponent<SpinObject>();
        if (spinner != null)
        {
            spinner.enabled = true;
            spinner.StartSpinning();
        }
    }
}