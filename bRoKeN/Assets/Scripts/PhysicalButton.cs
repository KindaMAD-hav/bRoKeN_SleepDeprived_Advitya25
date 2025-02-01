using UnityEngine;
using System.Collections;

public class PhysicalButton : MonoBehaviour
{
    [Tooltip("The index of the column this button will trigger (0-based).")]
    public int columnIndex = 0;

    [Tooltip("Reference to your Connect4Manager script.")]
    public Connect4Manager connect4Manager;

    [Tooltip("How far the button moves when pressed.")]
    public float pressDepth = 0.1f;

    [Tooltip("Duration of the press animation (seconds).")]
    public float pressDuration = 0.1f;

    // Stores the initial local position of the button.
    private Vector3 initialPosition;

    // Store the initial position on start.
    void Start()
    {
        initialPosition = transform.localPosition;
    }

    // This method is called when the user clicks on the object.
    void OnMouseDown()
    {
        Debug.Log("buttonPressed");
        if (connect4Manager != null)
        {
            connect4Manager.OnColumnButtonPressed(columnIndex);
        }
        else
        {
            Debug.LogError("Connect4Manager reference is missing on PhysicalButton.");
        }

        // Animate the button press.
        StartCoroutine(AnimateButtonPress());
    }

    // Coroutine to animate the button moving down and then back up.
    IEnumerator AnimateButtonPress()
    {
        Vector3 pressedPosition = initialPosition - new Vector3(0, pressDepth, 0);

        // Animate button press downward.
        float elapsedTime = 0;
        while (elapsedTime < pressDuration)
        {
            transform.localPosition = Vector3.Lerp(initialPosition, pressedPosition, elapsedTime / pressDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = pressedPosition;

        // Optionally, wait a brief moment while the button is in the pressed state.
        yield return new WaitForSeconds(0.1f);

        // Animate button release back to its initial position.
        elapsedTime = 0;
        while (elapsedTime < pressDuration)
        {
            transform.localPosition = Vector3.Lerp(pressedPosition, initialPosition, elapsedTime / pressDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = initialPosition;
    }
}
