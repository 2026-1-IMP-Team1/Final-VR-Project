using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ButtonManager : MonoBehaviour
{
    // Color applied to a button when the player selects it
    public Color selectedColor = Color.yellow;
    // Color applied to all selected buttons when the answer is correct
    public Color correctColor  = Color.green;
    // Maximum number of buttons the player can select at once
    public int maxSelect = 4;
    // Delay (seconds) before resetting buttons after a wrong answer
    public float resetDelay = 1.0f;
    // The correct button IDs that must be selected to solve the puzzle
    public int[] correctAnswer;

    public AudioClip successSound;
    public AudioClip failSound;
    private AudioSource audioSource;

    // World-space UI panel shown when the puzzle is cleared
    public GameObject clearUI;

    // Tracks which buttons the player has currently selected
    private List<SingleButton> selectedButtons = new List<SingleButton>();
    // Prevents new interactions while answer checking is in progress
    private bool isChecking = false;
    // Locks all input once the puzzle has been solved
    private bool isSolved = false;

    void Awake()
    {
        // Add AudioSource at runtime so no manual component setup is needed in the Inspector
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; // 2D sound — not positional

        if (clearUI != null)
            clearUI.SetActive(false);
    }

    // Called by SingleButton when the player interacts with it via XR ray
    public void OnButtonInteract(SingleButton button)
    {
        // Ignore input while checking answers or after the puzzle is already solved
        if (isChecking || isSolved) return;

        if (selectedButtons.Contains(button))
        {
            // Toggle off: player pressed the same button again to deselect it
            selectedButtons.Remove(button);
            button.SetSelected(false, selectedColor);
        }
        else
        {
            // Prevent selecting more than maxSelect buttons at once
            if (selectedButtons.Count >= maxSelect) return;

            selectedButtons.Add(button);
            button.SetSelected(true, selectedColor);

            // Automatically check the answer once the player has selected exactly maxSelect buttons
            if (selectedButtons.Count == maxSelect)
                StartCoroutine(CheckRoutine());
        }
    }

    IEnumerator CheckRoutine()
    {
        isChecking = true;

        if (CheckAnswers())
        {
            // Highlight all selected buttons green to indicate a correct answer
            foreach (SingleButton b in selectedButtons)
                b.SetSelected(true, correctColor);

            if (successSound != null)
                audioSource.PlayOneShot(successSound);

            isSolved = true;
            Debug.Log("Correct!");

            yield return new WaitForSeconds(1.0f);
            ShowClearUI();
        }
        else
        {
            // Flash all selected buttons red to indicate a wrong answer
            foreach (SingleButton b in selectedButtons)
                b.SetSelected(true, Color.red);

            if (failSound != null)
                audioSource.PlayOneShot(failSound);

            Debug.Log("Incorrect!");
            yield return new WaitForSeconds(resetDelay);
            ResetButtons();
        }

        isChecking = false;
    }

    void ShowClearUI()
    {
        if (clearUI == null) return;

        // Position the clear UI 2 metres in front of the player's camera and face it toward them
        Transform cam = Camera.main.transform;
        clearUI.transform.position = cam.position + cam.forward * 2f;
        clearUI.transform.LookAt(cam);
        clearUI.transform.Rotate(0, 180, 0); // Flip 180° so the UI faces the player correctly

        clearUI.SetActive(true);
    }

    // Returns true if the set of selected button IDs exactly matches the correct answer set
    bool CheckAnswers()
    {
        if (correctAnswer == null) return false;
        var selectedIds = new HashSet<int>();
        foreach (var b in selectedButtons) selectedIds.Add(b.buttonId);
        var answerIds = new HashSet<int>(correctAnswer);
        return selectedIds.SetEquals(answerIds); // Order-independent comparison
    }

    // Deselects all currently selected buttons and clears the selection list
    void ResetButtons()
    {
        foreach (SingleButton b in selectedButtons)
            b.SetSelected(false, selectedColor);
        selectedButtons.Clear();
    }
}