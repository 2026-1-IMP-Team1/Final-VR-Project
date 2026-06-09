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
    public AudioClip successSound;  // 정답 사운드
    public AudioClip failSound;     // 오답 사운드
    private AudioSource audioSource;

    private List<SingleButton> selectedButtons = new List<SingleButton>();
    private bool isChecking = false;
    private bool isSolved = false;

    void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f; // 3D 사운드
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

            // 정답 사운드 재생
            if (successSound != null)
                audioSource.PlayOneShot(successSound);

            isSolved = true;
            Debug.Log("정답!");
        }
        else
        {
            // 오답 사운드 재생
            if (failSound != null)
                audioSource.PlayOneShot(failSound);

            Debug.Log("오답!");
            yield return new WaitForSeconds(resetDelay);
            ResetButtons();
        }

        isChecking = false;
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