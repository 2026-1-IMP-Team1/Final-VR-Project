using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ButtonManager : MonoBehaviour
{
    public Color selectedColor = Color.yellow;   // 선택 중 표시 색
    public Color correctColor  = Color.green;    // 정답일 때 색
    public int maxSelect = 4;
    public float resetDelay = 1.0f;              // 오답 시 초기화까지 시간
    public int[] correctAnswer;                  // 정답 버튼 id 순서, 예: [2, 5, 7, 1]

    private List<SingleButton> selectedButtons = new List<SingleButton>();
    private bool isChecking = false;
    private bool isSolved = false;

    public void OnButtonInteract(SingleButton button)
    {
        if (isChecking || isSolved) return;      // 확인 중이거나 이미 풀렸으면 입력 무시

        if (selectedButtons.Contains(button))
        {
            // 이미 선택됨 → 취소
            selectedButtons.Remove(button);
            button.SetSelected(false, selectedColor);
        }
        else
        {
            // 새로 선택
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
            // 정답 → 4개 버튼을 정답 색(초록)으로 바꾸고 그대로 유지
            foreach (SingleButton b in selectedButtons)
                b.SetSelected(true, correctColor);

            isSolved = true;
            Debug.Log("정답!");
        }
        else
        {
            // 오답 → 잠깐 보여준 뒤 초기화
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