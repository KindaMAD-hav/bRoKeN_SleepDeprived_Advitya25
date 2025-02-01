using UnityEngine;

public class LeverInteraction : MonoBehaviour
{
    public float interactionDistance = 3f;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, interactionDistance))
            {
                Lever lever = hit.collider.GetComponent<Lever>();
                if (lever != null)
                {
                    lever.ToggleLever();
                }
            }
        }
    }
}