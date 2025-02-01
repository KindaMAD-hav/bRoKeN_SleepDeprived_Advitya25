using UnityEngine;

public class WallAttachment : MonoBehaviour
{
    [System.Serializable]
    public class AttachPoint
    {
        public Transform point;
        public int requiredGearID; // Only gears with this ID can attach here
    }

    public AttachPoint[] attachPoints;
    private int attachedObjects = 0;

    private void Start()
    {
        if (attachPoints == null || attachPoints.Length == 0)
        {
            Debug.LogError("[WallAttachment] No attach points assigned!");
        }
    }

    public void AttachObject(PickableObject pickable)
    {
        if (pickable == null)
        {
            Debug.LogError("[WallAttachment] PickableObject is null!");
            return;
        }

        foreach (AttachPoint attachPoint in attachPoints)
        {
            if (attachPoint.point.childCount == 0 && pickable.gearID == attachPoint.requiredGearID)
            {
                Debug.Log($"[WallAttachment] Attaching {pickable.gameObject.name} to {attachPoint.point.name}");

                // Enable spinning
                SpinObject spinner = pickable.GetComponent<SpinObject>();
                if (spinner != null) spinner.enabled = true;

                pickable.PlaceObject(attachPoint.point);
                attachedObjects++;

                if (attachedObjects == attachPoints.Length)
                {
                    Debug.Log("[WallAttachment] All objects attached!");
                }
                return;
            }
        }

        Debug.Log($"[WallAttachment] {pickable.gameObject.name} does not match any attach point!");
    }
}
