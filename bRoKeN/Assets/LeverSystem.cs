using UnityEngine;
using System.Collections.Generic;

public class LeverSystem : MonoBehaviour
{
    public PowerBox powerBox;
    private Dictionary<int, bool> leverStates = new Dictionary<int, bool>();

    [Header("Correct Lever Sequence")]
    [Tooltip("Set the correct sequence of lever numbers that need to be activated")]
    [SerializeField]
    private int[] correctSequence = new int[] { 1, 3, 2, 4 }; // Set this in Inspector

    public void LeverActivated(int leverID, bool isOn)
    {
        leverStates[leverID] = isOn;

        if (isOn)
        {
            CheckProgress();
        }
    }

    private void CheckProgress()
    {
        List<int> currentSequence = new List<int>();
        foreach (var lever in leverStates)
        {
            if (lever.Value)  // if lever is on
            {
                currentSequence.Add(lever.Key);
            }
        }

        // Enable test button only when the same number of levers are activated
        powerBox.EnableTestButton(currentSequence.Count == correctSequence.Length);
    }

    public bool ValidateSequence()
    {
        List<int> currentSequence = new List<int>();
        foreach (var lever in leverStates)
        {
            if (lever.Value)
            {
                currentSequence.Add(lever.Key);
            }
        }

        if (currentSequence.Count != correctSequence.Length)
        {
            return false;
        }

        for (int i = 0; i < correctSequence.Length; i++)
        {
            if (currentSequence[i] != correctSequence[i])
            {
                return false;
            }
        }

        return true;
    }

    public void ResetLevers()
    {
        leverStates.Clear();

        // Find and reset all lever objects
        Lever[] allLevers = FindObjectsOfType<Lever>();
        foreach (Lever lever in allLevers)
        {
            lever.ResetLever();
        }
    }
}