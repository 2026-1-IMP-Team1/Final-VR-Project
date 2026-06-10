using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ButtonManager : MonoBehaviour
{
    public Color selectedColor = Color.yellow;
    public Color correctColor  = Color.green;
    public int maxSelect = 4;
    public float resetDelay = 1.0f;
    public int[] correctAnswer;

    [Header("사운드")]
    public AudioClip successSound;
    public AudioClip failSound;
    private AudioSource audioSource;

    [Header("클리어 UI")]
    public GameObject clearUI;

    private List<SingleButton> selectedButtons = new List<SingleButton>();
    private bool isChecking = false;
    private bool isSolved = false;

    void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f;

        if (clearUI != null)
            clearUI.SetActive(false);
    }

    public void OnButtonInteract(SingleButton button)
    {
        if (isChecking || isSolved) return;

        if (selectedButtons.Contains(button))
        {
            selectedButtons.Remove(button);
            button.SetSelected(false, selectedColor);
        }
        else
        {
            if (selectedButtons.Count >= maxSelect) return;

            selectedButtons.Add(button);
            button.SetSelected(true, selectedColor);

            if (selectedButtons.Count == maxSelect)
                StartCoroutine(CheckRoutine());
        }
    }

    IEnumerator CheckRoutine()
    {
        isChecking = true;

        if (CheckAnswers())
        {
            foreach (SingleButton b in selectedButtons)
                b.SetSelected(true, correctColor);

            if (successSound != null)
                audioSource.PlayOneShot(successSound);

            isSolved = true;
            Debug.Log("정답!");

            yield return new WaitForSeconds(1.0f);
            ShowClearUI();
        }
        else
        {
            foreach (SingleButton b in selectedButtons)
                b.SetSelected(true, Color.red);

            if (failSound != null)
                audioSource.PlayOneShot(failSound);

            Debug.Log("오답!");
            yield return new WaitForSeconds(resetDelay);
            ResetButtons();
        }

        isChecking = false;
    }

    void ShowClearUI()
    {
        if (clearUI == null) return;

        Transform cam = Camera.main.transform;
        clearUI.transform.position = cam.position + cam.forward * 2f;
        clearUI.transform.LookAt(cam);
        clearUI.transform.Rotate(0, 180, 0);

        clearUI.SetActive(true);
    }

    bool CheckAnswers()
    {
        if (correctAnswer == null) return false;
        var selectedIds = new HashSet<int>();
        foreach (var b in selectedButtons) selectedIds.Add(b.buttonId);
        var answerIds = new HashSet<int>(correctAnswer);
        return selectedIds.SetEquals(answerIds);
    }

    void ResetButtons()
    {
        foreach (SingleButton b in selectedButtons)
            b.SetSelected(false, selectedColor);
        selectedButtons.Clear();
    }
}